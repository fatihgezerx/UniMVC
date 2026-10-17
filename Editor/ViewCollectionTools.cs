using UnityEditor;
using UnityEngine;

namespace UniMVC
{
    /// <summary>The "Collect From Children" action without any UI, shared by every Inspector that shows a <see cref="ViewCollection"/>.</summary>
    internal static class ViewCollectionTools
    {
        /// <summary>Tooltip of the action's button.</summary>
        public const string Tooltip =
            "Refills every list with the views below this object that aren't inside another panel or popup (and, on a UIManager, its controllers).";

        /// <summary>
        /// Refills the view lists of every <see cref="UIManager"/> and <see cref="PanelViewBase"/> in <paramref name="targets"/> from their
        /// hierarchy (and the controller list of a UIManager). Undoable; prefab-instance overrides are recorded.
        /// </summary>
        /// <param name="targets">The inspected components.</param>
        public static void Collect(Object[] targets)
        {
            foreach (var target in targets)
            {
                ViewCollection collection;
                PanelViewBase owner = null;
                UIManager manager = null;
                switch (target)
                {
                    case UIManager ui:
                        manager = ui;
                        collection = ui.Views;
                        break;
                    case PanelViewBase panel:
                        owner = panel;
                        collection = panel.ChildViews;
                        break;
                    default:
                        continue;
                }

                var component = (Component)target;
                Undo.RecordObject(target, "Collect Views");
                collection.CollectFrom(component.transform, owner);
                if (manager != null)
                {
                    manager.CollectControllers();
                }

                EditorUtility.SetDirty(target);
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
        }
    }
}
