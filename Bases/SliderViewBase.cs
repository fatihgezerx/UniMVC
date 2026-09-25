using UnityEngine;
using UnityEngine.UI;

namespace UniMVC
{
    /// <summary>A view on a <see cref="UnityEngine.UI.Slider"/>: override <see cref="OnValueChanged"/>.</summary>
    [RequireComponent(typeof(Slider))]
    public abstract class SliderViewBase : ViewBase
    {
        protected Slider Slider { get; private set; }

        public float Value => Slider.value;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Slider = GetComponent<Slider>();
            Slider.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>Sets the value without calling <see cref="OnValueChanged"/>.</summary>
        public void SetValue(float value) => Slider.SetValueWithoutNotify(value);

        /// <summary>Sets the maximum, then the value, without calling <see cref="OnValueChanged"/>.</summary>
        public void SetValue(float value, float maxValue)
        {
            Slider.maxValue = maxValue;
            Slider.SetValueWithoutNotify(value);
        }

        protected virtual void OnValueChanged(float value)
        {
        }
    }
}
