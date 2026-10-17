using Veneer;
using UnityEditor;

namespace UniMVC
{
    /// <summary>Studio-look Inspector of every controller: the UniMVC header card and the controller's own fields.</summary>
    [CustomEditor(typeof(ControllerBase), true)]
    [CanEditMultipleObjects]
    internal sealed class ControllerStudioEditor : VeneerAutoInspector
    {
        protected override VeneerBrand Brand => UniMVCBrand.Value;

        protected override string Subtitle => targets.Length > 1 ? "Controller, " + targets.Length + " objects selected" : "Controller";

        protected override string IconName => "bolt";
    }
}
