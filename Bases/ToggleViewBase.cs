using UnityEngine;
using UnityEngine.UI;

namespace UniMVC
{
    /// <summary>A view on a <see cref="UnityEngine.UI.Toggle"/>: override <see cref="OnValueChanged"/>.</summary>
    [RequireComponent(typeof(Toggle))]
    public abstract class ToggleViewBase : ViewBase
    {
        protected Toggle Toggle { get; private set; }

        public bool IsOn => Toggle.isOn;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Toggle = GetComponent<Toggle>();
            Toggle.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>Sets the toggle without calling <see cref="OnValueChanged"/>.</summary>
        public void SetIsOn(bool isOn) => Toggle.SetIsOnWithoutNotify(isOn);

        protected virtual void OnValueChanged(bool isOn)
        {
        }
    }
}
