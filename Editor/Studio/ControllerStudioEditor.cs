using Veneer;
using UnityEditor;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of every controller, drawn from a Veneer layout: the project's own copy made in the Inspector Designer when
    /// there is one, otherwise <see cref="ControllerDefaultLayout"/> (the UniMVC header card and the automatic "Other" card with the
    /// controller's own fields).
    /// </summary>
    [CustomEditor(typeof(ControllerBase), true)]
    [CanEditMultipleObjects]
    internal sealed class ControllerStudioEditor : VeneerLayoutDrivenInspector<ControllerBase>
    {
        /// <inheritdoc />
        protected override VeneerBrand Brand => UniMVCBrand.Value;
    }
}
