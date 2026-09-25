using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// The link between your systems and your views: a controller listens to what happens in the game
    /// (events, callbacks) and tells the right views about it through <see cref="UI"/>. Controllers sit on
    /// the <see cref="UIManager"/>'s object, which is always active, so they keep listening while the
    /// panels they update are closed - views stay passive and only show what they are told.
    /// </summary>
    /// <remarks>
    /// Listed under the Controllers header of the <see cref="UIManager"/> and initialized by it once, after
    /// every view. Subscribe in <see cref="OnInitialize"/> and unsubscribe in <c>OnDestroy</c>.
    /// </remarks>
    public abstract class ControllerBase : MonoBehaviour
    {
        /// <summary>The manager this controller was initialized by: reach views with <c>UI.Get&lt;T&gt;()</c>.</summary>
        public UIManager UI { get; private set; }

        public bool IsInitialized { get; private set; }

        /// <summary>Initializes the controller once; later calls do nothing.</summary>
        public void Initialize(UIManager ui)
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;
            UI = ui;
            OnInitialize();
        }

        /// <summary>Called once, from <see cref="Initialize"/>, after every view has been initialized.</summary>
        protected virtual void OnInitialize()
        {
        }
    }
}
