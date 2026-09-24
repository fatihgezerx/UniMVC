namespace UniMVC
{
    /// <summary>
    /// A screen or window: shown and hidden as a whole, with <see cref="OnShown"/> / <see cref="OnHidden"/>
    /// hooks that only run when the visibility actually changes.
    /// </summary>
    public abstract class PanelViewBase : ViewBase
    {
        public override void Show()
        {
            if (IsVisible)
            {
                return;
            }

            base.Show();
            OnShown();
        }

        public override void Hide()
        {
            if (!IsVisible)
            {
                return;
            }

            base.Hide();
            OnHidden();
        }

        public void Toggle()
        {
            if (IsVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnHidden()
        {
        }
    }
}
