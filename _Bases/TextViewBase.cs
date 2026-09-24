using TMPro;

namespace UniMVC
{
    /// <summary>A view on a <see cref="TMP_Text"/> label.</summary>
    public abstract class TextViewBase : ViewBase
    {
        protected TMP_Text Text { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Text = GetComponent<TMP_Text>();
        }

        public void SetText(string text) => Text.SetText(text);
    }
}
