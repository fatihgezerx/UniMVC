using System.Collections.Generic;
using Veneer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of every panel and popup. Tab Panel: how it opens and closes (animations, duration, ease) and its two
    /// behaviour switches; tab Settings (only when the panel class adds visible fields of its own); tab Views: the views inside it by
    /// kind with a "Collect from children" button. The CanvasGroup the animations fade is added as before.
    /// </summary>
    [CustomEditor(typeof(PanelViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class PanelViewStudioEditor : VeneerInspector<PanelViewBase>
    {
        // Fields of PanelViewBase / PopupViewBase that have their own place in the layout.
        private static readonly HashSet<string> BaseFields = new()
        {
            "openAnimation", "closeAnimation", "popupOpenAnimation", "popupCloseAnimation", "animationDuration", "animationEase",
            "rebuildLayoutOnShow", "blocksGameplay", "childViews"
        };

        protected override VeneerBrand Brand => UniMVCBrand.Value;

        private void OnEnable() => PanelCanvasGroupFix.EnsureOnNextTick(targets);

        protected override void Build(VeneerInspectorBuilder b)
        {
            bool isPopup = Target is PopupViewBase;
            string kind = isPopup ? "Popup" : "Panel";
            b.Header(ObjectNames.NicifyVariableName(target.GetType().Name), kind + " view", isPopup ? "overlay" : "layers");
            b.StatePill(() =>
            {
                if (Application.isPlaying && target != null)
                {
                    return ((PanelViewBase)target).IsVisible ? ("Open", "ok") : ("Hidden", "idle");
                }

                return (kind, "info");
            });

            List<string> ownPaths = UniMVCLayout.OwnPaths(serializedObject, BaseFields);
            var labels = new List<(string label, string icon)>(3) { ("Panel", isPopup ? "overlay" : "layers") };
            if (ownPaths.Count > 0)
            {
                labels.Add(("Settings", "settings"));
            }

            labels.Add(("Views", "list"));
            VisualElement[] pages = b.Tabs(labels.ToArray());

            int page = 0;
            using (b.Into(pages[page++]))
            {
                BuildAnimation(b, isPopup);
                b.Card("Behaviour", "settings", () => b.Chips(
                    ("blocksGameplay", "Blocks gameplay", "lock"),
                    ("rebuildLayoutOnShow", "Rebuild layout on show", "refresh")));
            }

            if (ownPaths.Count > 0)
            {
                using (b.Into(pages[page++]))
                {
                    b.Card(ObjectNames.NicifyVariableName(target.GetType().Name), "settings", () => UniMVCLayout.AddFields(b, ownPaths));
                }
            }

            VisualElement viewsPage = pages[page];
            using (b.Into(viewsPage))
            {
                UniMVCLayout.AddCollectButton(b, viewsPage, true);
                UniMVCLayout.AddViewLists(b, "childViews");
            }
        }

        private void BuildAnimation(VeneerInspectorBuilder b, bool isPopup)
        {
            string openPath = isPopup ? "popupOpenAnimation" : "openAnimation";
            string closePath = isPopup ? "popupCloseAnimation" : "closeAnimation";

            b.Card("Animation", "play", () =>
            {
                b.Field(openPath, "Open animation");
                b.Field(closePath, "Close animation");

                // Both None: nothing animates, so duration and ease do not apply.
                var timing = new VisualElement();
                b.Add(timing);
                using (b.Into(timing))
                {
                    b.Slider("animationDuration", "Duration", "Fast", "Slow", unit: "s");
                    b.Field("animationEase", "Ease");
                }

                SerializedProperty open = serializedObject.FindProperty(openPath);
                SerializedProperty close = serializedObject.FindProperty(closePath);
                b.VisibleWhen(timing, () => open.hasMultipleDifferentValues || close.hasMultipleDifferentValues
                    || open.enumValueIndex != 0 || close.enumValueIndex != 0);
#if !HAS_DOTWEEN
                b.Hint("These animations need DOTween, which isn't in this project. Until it's installed, the panel opens and closes " +
                       "at once; the settings are kept.", "info");
#endif
            });
        }
    }
}
