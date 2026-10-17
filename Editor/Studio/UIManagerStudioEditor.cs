using Veneer;
using UnityEditor;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of <see cref="UIManager"/>, drawn from a Veneer layout: the project's own copy made in the Inspector Designer
    /// when there is one, otherwise <see cref="UIManagerDefaultLayout"/>. Tab Views: the views outside any panel by kind, with a "Collect
    /// from children" button; tab Controllers: the controller list. The header pill counts what is listed.
    /// </summary>
    [CustomEditor(typeof(UIManager))]
    [CanEditMultipleObjects]
    internal sealed class UIManagerStudioEditor : VeneerLayoutDrivenInspector<UIManager>
    {
        /// <inheritdoc />
        protected override VeneerBrand Brand => UniMVCBrand.Value;
    }
}
