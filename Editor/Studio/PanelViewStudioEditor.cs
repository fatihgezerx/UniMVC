using Veneer;
using UnityEditor;

namespace UniMVC
{
    /// <summary>
    /// Studio-look Inspector of every panel and popup, drawn from a Veneer layout: the project's own copy made in the Inspector Designer
    /// when there is one, otherwise <see cref="PanelViewDefaultLayout"/> or <see cref="PopupViewDefaultLayout"/>. Tab Panel: how it opens
    /// and closes (animations, duration, ease) and its two behaviour switches; tab Views: the views inside it by kind with a "Collect
    /// from children" button. The fields a panel class adds are in the automatic "Other" card under the tabs. The CanvasGroup the
    /// animations fade is added as before.
    /// </summary>
    [CustomEditor(typeof(PanelViewBase), true)]
    [CanEditMultipleObjects]
    internal sealed class PanelViewStudioEditor : VeneerLayoutDrivenInspector<PanelViewBase>
    {
        /// <inheritdoc />
        protected override VeneerBrand Brand => UniMVCBrand.Value;

        private void OnEnable() => PanelCanvasGroupFix.EnsureOnNextTick(targets);
    }
}
