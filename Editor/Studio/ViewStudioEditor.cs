using Veneer;
using UnityEditor;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of every view that has no editor of its own - buttons, toggles, dropdowns, texts, images and views you write
    /// yourself. Panels, popups and sliders have dedicated editors, which win over this one.
    /// </summary>
    [CustomEditor(typeof(ViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class ViewStudioEditor : VeneerAutoInspector
    {
        protected override VeneerBrand Brand => UniMVCBrand.Value;

        protected override string Subtitle => Kind() + " view" + (targets.Length > 1 ? ", " + targets.Length + " objects selected" : string.Empty);

        protected override string IconName => target switch
        {
            ButtonViewBase => "cursor",
            ToggleViewBase => "check-circle",
            DropdownViewBase => "chevron-down",
            TextViewBase => "text",
            ImageViewBase => "image",
            _ => null
        };

        private string Kind() => target switch
        {
            ButtonViewBase => "Button",
            ToggleViewBase => "Toggle",
            DropdownViewBase => "Dropdown",
            TextViewBase => "Text",
            ImageViewBase => "Image",
            _ => "Custom"
        };
    }
}
