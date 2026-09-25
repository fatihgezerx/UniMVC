using UnityEngine;
using UnityEngine.UI;

namespace UniMVC
{
    /// <summary>A view on a <see cref="UnityEngine.UI.Button"/>: override <see cref="OnClick"/>.</summary>
    [RequireComponent(typeof(Button))]
    public abstract class ButtonViewBase : ViewBase
    {
        protected Button Button { get; private set; }

        public bool Interactable
        {
            get => Button.interactable;
            set => Button.interactable = value;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Button = GetComponent<Button>();
            Button.onClick.AddListener(OnClick);
        }

        protected virtual void OnClick()
        {
        }
    }
}
