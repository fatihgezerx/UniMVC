using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// A screen or window: shown and hidden as a whole, with <see cref="OnShown"/> / <see cref="OnHidden"/>
    /// hooks that only run when the visibility actually changes. A panel owns the views inside it - listed
    /// by kind under <see cref="ChildViews"/> in the Inspector - and initializes them when it is initialized.
    /// </summary>
    public abstract class PanelViewBase : ViewBase
    {
        [Tooltip("The views inside this panel, by kind. Initialized (and registered with the UIManager) when this panel is.")]
        [SerializeField] private ViewCollection childViews = new();

        /// <summary>The views inside this panel, by kind.</summary>
        public ViewCollection ChildViews => childViews;

        /// <summary>Initializes every child view. Call <c>base.OnInitialize()</c> when overriding.</summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            childViews.Initialize(UI);
        }

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
