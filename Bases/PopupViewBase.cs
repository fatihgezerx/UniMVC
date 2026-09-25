using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// A popup (a dialog, a confirmation, a tooltip window...): works exactly like a
    /// <see cref="PanelViewBase"/> - shown and hidden as a whole, owning the views inside it - but is
    /// listed under its own Popups header, so popups stay apart from full screens in the Inspector. It has
    /// animations of its own, e.g. popping in from nothing (see <see cref="PopupOpenAnimation"/>).
    /// </summary>
    public abstract class PopupViewBase : PanelViewBase
    {
        [Tooltip("How the popup appears when shown.")]
        [SerializeField] private PopupOpenAnimation popupOpenAnimation;

        [Tooltip("How the popup disappears when hidden.")]
        [SerializeField] private PopupCloseAnimation popupCloseAnimation;

        private protected override ViewTransition OpenTransition => ViewTransition.Of(popupOpenAnimation);

        private protected override ViewTransition CloseTransition => ViewTransition.Of(popupCloseAnimation);
    }
}
