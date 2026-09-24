using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UniMVC
{
    /// <summary>A view on a <see cref="TMP_Dropdown"/>: override <see cref="OnValueChanged"/>.</summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    public abstract class DropdownViewBase : ViewBase
    {
        protected TMP_Dropdown Dropdown { get; private set; }

        public int Value => Dropdown.value;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Dropdown = GetComponent<TMP_Dropdown>();
            Dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>Replaces the options and selects <paramref name="selected"/>, without calling <see cref="OnValueChanged"/>.</summary>
        public void SetOptions(List<string> options, int selected = 0)
        {
            Dropdown.ClearOptions();
            Dropdown.AddOptions(options);
            Dropdown.SetValueWithoutNotify(selected);
        }

        /// <summary>Selects <paramref name="index"/> without calling <see cref="OnValueChanged"/>.</summary>
        public void SetValue(int index) => Dropdown.SetValueWithoutNotify(index);

        protected virtual void OnValueChanged(int index)
        {
        }
    }
}
