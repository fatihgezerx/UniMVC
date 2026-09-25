namespace UniMVC
{
    /// <summary>
    /// A popup (a dialog, a confirmation, a tooltip window...): works exactly like a
    /// <see cref="PanelViewBase"/> - shown and hidden as a whole, owning the views inside it - but is
    /// listed under its own Popups header, so popups stay apart from full screens in the Inspector.
    /// </summary>
    public abstract class PopupViewBase : PanelViewBase
    {
    }
}
