using UnityEditor;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// For editor code that builds UI into a scene - e.g. a system setting up the panel Easy UI just built: the
    /// canvas's <see cref="UIManager"/> and a controller on it, and every new view listed where it belongs. All of
    /// it is recorded for undo, and none of it adds anything twice, so several systems can set up one UI.
    /// </summary>
    public static class UISetup
    {
        /// <summary>The <see cref="UIManager"/> on <paramref name="canvas"/>'s object, added if it has none.</summary>
        public static UIManager EnsureUIManager(Canvas canvas)
        {
            var go = canvas.gameObject;
            if (go.TryGetComponent<UIManager>(out var ui))
            {
                return ui;
            }

            ui = Undo.AddComponent<UIManager>(go);
            Debug.Log($"[UniMVC] Added a UIManager to '{canvas.name}'.", canvas);
            return ui;
        }

        /// <summary>
        /// The <typeparamref name="T"/> on <paramref name="ui"/>'s always active object, added if it has none, and
        /// listed in its Controllers.
        /// </summary>
        public static T EnsureController<T>(UIManager ui) where T : ControllerBase
        {
            if (!ui.TryGetComponent<T>(out var controller))
            {
                controller = Undo.AddComponent<T>(ui.gameObject);
            }

            Undo.RecordObject(ui, "Add Controller");
            ui.AddController(controller);
            return controller;
        }

        /// <summary>
        /// Lists every view under <paramref name="root"/> (itself included) where it belongs: in the closest panel
        /// or popup above it, or in <paramref name="ui"/> when it is in none - as "Collect From Children" would,
        /// without touching views listed before. Entries of views since destroyed are dropped from
        /// <paramref name="ui"/>. Views on (or under) <paramref name="templates"/> - objects copied at runtime, e.g.
        /// a list's slot template - are left out: each copy is set up by whoever makes it.
        /// </summary>
        public static void ListViews(UIManager ui, Transform root, params Component[] templates)
        {
            Undo.RecordObject(ui, "List Views");
            ui.Views.RemoveMissing();

            foreach (var view in root.GetComponentsInChildren<ViewBase>(true))
            {
                if (IsTemplate(view, templates))
                {
                    continue;
                }

                var owner = ViewCollection.OwnerOf(view);
                if (owner != null)
                {
                    Undo.RecordObject(owner, "List Views");
                    owner.ChildViews.Add(view);
                    EditorUtility.SetDirty(owner);
                }
                else
                {
                    ui.Views.Add(view);
                }
            }

            EditorUtility.SetDirty(ui);
        }

        private static bool IsTemplate(ViewBase view, Component[] templates)
        {
            foreach (var template in templates)
            {
                if (template != null && view.transform.IsChildOf(template.transform))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
