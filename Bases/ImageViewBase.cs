using UnityEngine;
using UnityEngine.UI;

namespace UniMVC
{
    /// <summary>A view on an <see cref="UnityEngine.UI.Image"/>.</summary>
    public abstract class ImageViewBase : ViewBase
    {
        protected Image Image { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Image = GetComponent<Image>();
        }

        public void SetSprite(Sprite sprite) => Image.sprite = sprite;
    }
}
