using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// A set of views grouped by kind - panels, popups, buttons, toggles, sliders, dropdowns, texts, images - as
    /// shown under their headers in the Inspector of a <see cref="UIManager"/> or a
    /// <see cref="PanelViewBase"/>. <see cref="Initialize"/> registers every view in it with the
    /// <see cref="UIManager"/> and initializes it, so a panel initializes the views it owns in turn.
    /// </summary>
    [Serializable]
    public sealed class ViewCollection
    {
        [SerializeField] private PanelViewBase[] panels = Array.Empty<PanelViewBase>();
        [SerializeField] private PopupViewBase[] popups = Array.Empty<PopupViewBase>();
        [SerializeField] private ButtonViewBase[] buttons = Array.Empty<ButtonViewBase>();
        [SerializeField] private ToggleViewBase[] toggles = Array.Empty<ToggleViewBase>();
        [SerializeField] private SliderViewBase[] sliders = Array.Empty<SliderViewBase>();
        [SerializeField] private DropdownViewBase[] dropdowns = Array.Empty<DropdownViewBase>();
        [SerializeField] private TextViewBase[] texts = Array.Empty<TextViewBase>();
        [SerializeField] private ImageViewBase[] images = Array.Empty<ImageViewBase>();

        public IReadOnlyList<PanelViewBase> Panels => panels;
        public IReadOnlyList<PopupViewBase> Popups => popups;
        public IReadOnlyList<ButtonViewBase> Buttons => buttons;
        public IReadOnlyList<ToggleViewBase> Toggles => toggles;
        public IReadOnlyList<SliderViewBase> Sliders => sliders;
        public IReadOnlyList<DropdownViewBase> Dropdowns => dropdowns;
        public IReadOnlyList<TextViewBase> Texts => texts;
        public IReadOnlyList<ImageViewBase> Images => images;

        /// <summary>Registers every view with <paramref name="ui"/> (if any) and initializes it. Empty entries are skipped.</summary>
        public void Initialize(UIManager ui)
        {
            Initialize(panels, ui);
            Initialize(popups, ui);
            Initialize(buttons, ui);
            Initialize(toggles, ui);
            Initialize(sliders, ui);
            Initialize(dropdowns, ui);
            Initialize(texts, ui);
            Initialize(images, ui);
        }

        /// <summary>Adds <paramref name="view"/> to the list of its kind, unless it is already there.</summary>
        public void Add(ViewBase view)
        {
            switch (view)
            {
                case PopupViewBase popup:
                    popups = Append(popups, popup);
                    break;
                case PanelViewBase panel:
                    panels = Append(panels, panel);
                    break;
                case ButtonViewBase button:
                    buttons = Append(buttons, button);
                    break;
                case ToggleViewBase toggle:
                    toggles = Append(toggles, toggle);
                    break;
                case SliderViewBase slider:
                    sliders = Append(sliders, slider);
                    break;
                case DropdownViewBase dropdown:
                    dropdowns = Append(dropdowns, dropdown);
                    break;
                case TextViewBase text:
                    texts = Append(texts, text);
                    break;
            }
        }

        /// <summary>
        /// Refills every list with the views below <paramref name="root"/> that belong to
        /// <paramref name="owner"/>: those whose closest panel or popup above them is <paramref name="owner"/>
        /// (null for a <see cref="UIManager"/>, i.e. views that aren't inside any panel). Views inside
        /// another panel belong to that panel instead, so every view is listed exactly once.
        /// </summary>
        public void CollectFrom(Transform root, PanelViewBase owner)
        {
            Clear();
            foreach (var view in root.GetComponentsInChildren<ViewBase>(true))
            {
                if (view != owner && OwnerOf(view) == owner)
                {
                    Add(view);
                }
            }
        }

        public void Clear()
        {
            panels = Array.Empty<PanelViewBase>();
            popups = Array.Empty<PopupViewBase>();
            buttons = Array.Empty<ButtonViewBase>();
            toggles = Array.Empty<ToggleViewBase>();
            sliders = Array.Empty<SliderViewBase>();
            dropdowns = Array.Empty<DropdownViewBase>();
            texts = Array.Empty<TextViewBase>();
            images = Array.Empty<ImageViewBase>();
        }

        // The closest panel (or popup) above the view, not counting the view itself.
        private static PanelViewBase OwnerOf(ViewBase view)
        {
            var parent = view.transform.parent;
            return parent != null ? parent.GetComponentInParent<PanelViewBase>(true) : null;
        }

        private static void Initialize<T>(T[] views, UIManager ui) where T : ViewBase
        {
            if (views == null)
            {
                return;
            }

            foreach (var view in views)
            {
                if (view == null)
                {
                    continue;
                }

                if (ui != null)
                {
                    ui.Register(view);
                }
                else
                {
                    view.Initialize();
                }
            }
        }

        private static T[] Append<T>(T[] views, T view) where T : ViewBase
        {
            views ??= Array.Empty<T>();
            if (Array.IndexOf(views, view) >= 0)
            {
                return views;
            }

            var result = new T[views.Length + 1];
            views.CopyTo(result, 0);
            result[views.Length] = view;
            return result;
        }
    }
}
