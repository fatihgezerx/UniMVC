using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// Base of every view. A view is initialized exactly once - by the <see cref="UIManager"/> above it, or
    /// by whoever creates it at runtime through <see cref="Initialize"/> - and from then on can reach
    /// every other view through <see cref="UI"/>. Override <see cref="OnInitialize"/> to cache components
    /// and hook up listeners.
    /// </summary>
    public abstract class ViewBase : MonoBehaviour
    {
        /// <summary>The manager this view was initialized by, or null if it was initialized without one.</summary>
        public UIManager UI { get; private set; }

        public bool IsInitialized { get; private set; }

        public bool IsVisible => gameObject.activeSelf;

        /// <summary>Initializes the view once; later calls do nothing.</summary>
        public void Initialize(UIManager ui = null)
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;
            UI = ui;
            OnInitialize();
        }

        /// <summary>Called once, from <see cref="Initialize"/>. Runs even while the view is hidden.</summary>
        protected virtual void OnInitialize()
        {
        }

        public virtual void Show() => gameObject.SetActive(true);

        public virtual void Hide() => gameObject.SetActive(false);

        public void SetVisible(bool visible)
        {
            if (visible)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}
