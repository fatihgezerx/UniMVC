using Veneer;
using UnityEditor;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of every slider view, drawn from a Veneer layout: the project's own copy made in the Inspector Designer when
    /// there is one, otherwise <see cref="SliderViewDefaultLayout"/> (a Value animation card, None or Trail, whose trail rows show only
    /// while Trail is chosen; the fields the slider class adds are in the automatic "Other" card).
    /// </summary>
    [CustomEditor(typeof(SliderViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class SliderViewStudioEditor : VeneerLayoutDrivenInspector<SliderViewBase>
    {
        /// <inheritdoc />
        protected override VeneerBrand Brand => UniMVCBrand.Value;
    }
}
