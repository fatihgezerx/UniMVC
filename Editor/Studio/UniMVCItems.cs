using System;
using Veneer;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniMVC
{
    /// <summary>
    /// The pieces of the UniMVC Inspectors that are not plain fields, registered with Veneer so the default layouts (and a user's copy of
    /// them) can place them: the state pills of panels, sliders and the UIManager, the "Collect from children" buttons and the widgets
    /// of the panel Duration and Ease rows (they hide while both animations are None). They can be moved or hidden in the Inspector
    /// Designer, not changed.
    /// </summary>
    internal static class UniMVCItems
    {
        /// <summary>Id of the panel / popup state pill (header badge).</summary>
        public const string PanelStateId = "unimvc.panel-state";

        /// <summary>Id of the slider state pill (header badge).</summary>
        public const string SliderStateId = "unimvc.slider-state";

        /// <summary>Id of the UIManager counts pill (header badge).</summary>
        public const string ManagerStateId = "unimvc.manager-state";

        /// <summary>Id of the primary "Collect from children" button (card item).</summary>
        public const string CollectId = "unimvc.collect";

        /// <summary>Id of the plain (not accent-filled) "Collect from children" button (card item).</summary>
        public const string CollectSecondaryId = "unimvc.collect-secondary";

        private const string DurationPath = "animationDuration";
        private const string EasePath = "animationEase";

        /// <summary>Registers the items and widgets of <see cref="PanelViewBase"/> (popups use them too). Safe to call again.</summary>
        public static void RegisterPanel()
        {
            Type type = typeof(PanelViewBase);
            VeneerCustomItems.Register(type, new VeneerCustomItem
            {
                Id = PanelStateId,
                Label = "State",
                Icon = "check-circle",
                Description = "A pill in the header: Open or Hidden in Play Mode, otherwise Panel or Popup.",
                Slot = VeneerCustomItemSlot.HeaderBadge,
                Build = BuildPanelState,
            });

            RegisterCollect(type, CollectId, "Collect button", true);
            VeneerCustomItems.RegisterWidget(type, DurationPath, (context, property) => BuildTiming(context, property, true));
            VeneerCustomItems.RegisterWidget(type, EasePath, (context, property) => BuildTiming(context, property, false));
        }

        /// <summary>Registers the state pill of <see cref="SliderViewBase"/>. Safe to call again.</summary>
        public static void RegisterSlider()
        {
            VeneerCustomItems.Register(typeof(SliderViewBase), new VeneerCustomItem
            {
                Id = SliderStateId,
                Label = "State",
                Icon = "check-circle",
                Description = "A pill in the header: Trail, Instant, or Mixed when the selected sliders differ.",
                Slot = VeneerCustomItemSlot.HeaderBadge,
                Build = BuildSliderState,
            });
        }

        /// <summary>Registers the items of <see cref="UIManager"/>. Safe to call again.</summary>
        public static void RegisterManager()
        {
            Type type = typeof(UIManager);
            VeneerCustomItems.Register(type, new VeneerCustomItem
            {
                Id = ManagerStateId,
                Label = "Counts",
                Icon = "check-circle",
                Description = "A pill in the header: how many views and controllers are listed (a warning while both are empty).",
                Slot = VeneerCustomItemSlot.HeaderBadge,
                Build = BuildManagerState,
            });

            RegisterCollect(type, CollectId, "Collect button", true);
            RegisterCollect(type, CollectSecondaryId, "Collect button (plain)", false);
        }

        private static void RegisterCollect(Type type, string id, string label, bool primary)
        {
            VeneerCustomItems.Register(type, new VeneerCustomItem
            {
                Id = id,
                Label = label,
                Icon = "refresh",
                Description = "Fills the lists below it from the children of this object (Undo is recorded).",
                Slot = VeneerCustomItemSlot.Item,
                Build = context => BuildCollect(context, primary),
            });
        }

        // ------------------------------------------------------------------ pills

        private static VisualElement BuildPanelState(VeneerCustomItemContext context)
        {
            var panel = context.SerializedObject.targetObject as PanelViewBase;
            string idle = panel is PopupViewBase ? "Popup" : "Panel";
            Label pill = VeneerStyle.Pill(string.Empty, "info");
            Action refresh = () =>
            {
                if (Application.isPlaying && panel != null)
                {
                    if (panel.IsVisible)
                    {
                        VeneerStyle.SetPill(pill, "ok", "Open");
                    }
                    else
                    {
                        VeneerStyle.SetPill(pill, "idle", "Hidden");
                    }
                }
                else
                {
                    VeneerStyle.SetPill(pill, "info", idle);
                }
            };

            // The first run of Poll is on the first tick: show the right state at once.
            refresh();
            context.Poll(refresh);
            return pill;
        }

        private static VisualElement BuildSliderState(VeneerCustomItemContext context)
        {
            SerializedObject serializedObject = context.SerializedObject;
            SerializedProperty animation = serializedObject.FindProperty("valueAnimation");
            if (animation == null)
            {
                return null;
            }

            Label pill = VeneerStyle.Pill(string.Empty, "idle");
            Action refresh = () =>
            {
                serializedObject.UpdateIfRequiredOrScript();
                if (animation.hasMultipleDifferentValues)
                {
                    VeneerStyle.SetPill(pill, "idle", "Mixed");
                }
                else if (animation.enumValueIndex == (int)SliderAnimation.Trail)
                {
                    VeneerStyle.SetPill(pill, "ok", "Trail");
                }
                else
                {
                    VeneerStyle.SetPill(pill, "idle", "Instant");
                }
            };

            refresh();
            context.Poll(refresh);
            return pill;
        }

        private static VisualElement BuildManagerState(VeneerCustomItemContext context)
        {
            var manager = context.SerializedObject.targetObject as UIManager;
            Label pill = VeneerStyle.Pill(string.Empty, "idle");

            // The text is rebuilt only when a count changed, so the 200 ms tick allocates nothing while the lists stay as they are.
            int shownViews = -1;
            int shownControllers = -1;
            string shownText = string.Empty;
            Action refresh = () =>
            {
                if (manager == null)
                {
                    VeneerStyle.SetPill(pill, "idle", string.Empty);
                    return;
                }

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

                VeneerStyle.SetPill(pill, viewCount == 0 && controllerCount == 0 ? "warn" : "ok", shownText);
            };

            refresh();
            context.Poll(refresh);
            return pill;
        }

        // ------------------------------------------------------------------ collect button

        // The button sits in a card of a tab; after Collect the arrays were changed outside the bound lists, so the tab is bound again.
        private static VisualElement BuildCollect(VeneerCustomItemContext context, bool primary)
        {
            SerializedObject serializedObject = context.SerializedObject;
            VisualElement card = context.Builder.Current;
            context.Builder.Button("Collect from children", "refresh", () =>
            {
                ViewCollectionTools.Collect(serializedObject.targetObjects);
                serializedObject.Update();
                VisualElement scope = card.parent != null ? card.parent : card;
                scope.Bind(serializedObject);
            }, ViewCollectionTools.Tooltip, primary);
            return null;
        }

        // ------------------------------------------------------------------ animation timing

        // Duration and Ease are drawn by the standard rows but hide while both animations are None (nothing animates, so they do not
        // apply). A show-if rule of a layout entry cannot say "A or B", so the rule lives in the widget of the two properties.
        private static VisualElement BuildTiming(VeneerCustomItemContext context, SerializedProperty property, bool slider)
        {
            VeneerInspectorBuilder b = context.Builder;
            string path = property.propertyPath;
            VisualElement element = slider ? b.Slider(path, "Duration", "Fast", "Slow", unit: "s") : b.Field(path, "Ease");

            SerializedObject serializedObject = context.SerializedObject;
            bool popup = serializedObject.targetObject is PopupViewBase;
            SerializedProperty open = serializedObject.FindProperty(popup ? "popupOpenAnimation" : "openAnimation");
            SerializedProperty close = serializedObject.FindProperty(popup ? "popupCloseAnimation" : "closeAnimation");
            if (open != null && close != null)
            {
                b.VisibleWhen(element, () => open.hasMultipleDifferentValues || close.hasMultipleDifferentValues
                    || open.enumValueIndex != 0 || close.enumValueIndex != 0);
            }

            return element;
        }
    }
}
