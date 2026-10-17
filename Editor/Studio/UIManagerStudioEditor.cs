using Veneer;
using UnityEditor;
using UnityEngine.UIElements;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of <see cref="UIManager"/>. Tab Views: the views outside any panel by kind, with a "Collect from children"
    /// button; tab Controllers: the controller list. The header pill counts what is listed.
    /// </summary>
    [CustomEditor(typeof(UIManager))]
    [CanEditMultipleObjects]
    internal sealed class UIManagerStudioEditor : VeneerInspector<UIManager>
    {
        // The pill text is rebuilt only when a count changed, so the 200 ms tick allocates nothing while the lists stay as they are.
        private int shownViews = -1;
        private int shownControllers = -1;
        private string shownText = string.Empty;

        protected override VeneerBrand Brand => UniMVCBrand.Value;

        protected override void Build(VeneerInspectorBuilder b)
        {
            b.Header("UI Manager", "Initializes the views, then the controllers");
            b.StatePill(Summary);

            VisualElement[] pages = b.Tabs(("Views", "layers"), ("Controllers", "bolt"));

            using (b.Into(pages[0]))
            {
                UniMVCLayout.AddCollectButton(b, pages[0], true);
                UniMVCLayout.AddViewLists(b, "views");
            }

            using (b.Into(pages[1]))
            {
                UniMVCLayout.AddCollectButton(b, pages[1], false);
                b.Card("Controllers", "bolt", () => b.List("controllers", "Controllers"));
            }
        }

        private (string text, string kind) Summary()
        {
            if (target == null)
            {
                return (string.Empty, "idle");
            }

            var manager = (UIManager)target;
            ViewCollection views = manager.Views;
            int viewCount = views.Panels.Count + views.Popups.Count + views.Buttons.Count + views.Toggles.Count + views.Sliders.Count
                + views.Dropdowns.Count + views.Texts.Count + views.Images.Count;
            int controllerCount = manager.Controllers.Count;
            if (viewCount != shownViews || controllerCount != shownControllers)
            {
                shownViews = viewCount;
                shownControllers = controllerCount;
                shownText = viewCount + " views, " + controllerCount + " controllers";
            }

            return (shownText, viewCount == 0 && controllerCount == 0 ? "warn" : "ok");
        }
    }
}
