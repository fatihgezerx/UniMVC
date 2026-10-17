using Veneer;
using UnityEditor;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of every view that has no editor of its own - buttons, toggles, dropdowns, texts, images and views you write
    /// yourself. Panels, popups and sliders have dedicated editors, which win over this one. It is drawn from a Veneer layout: the
    /// project's own copy when there is one, otherwise the default layout of the view's kind (<see cref="ViewKindDefaultLayout"/>:
    /// the header with the kind's icon and subtitle, and the automatic "Other" card with the view's fields).
    /// </summary>
    [CustomEditor(typeof(ViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class ViewStudioEditor : VeneerLayoutDrivenInspector<ViewBase>
    {
        /// <inheritdoc />
        protected override VeneerBrand Brand => UniMVCBrand.Value;
    }
}
