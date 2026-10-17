#if HAS_DOTWEEN
using DG.Tweening;
using DG.Tweening.Core;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace UniMVC
{
    /// <summary>
    /// A view on a <see cref="UnityEngine.UI.Slider"/>: override <see cref="OnValueChanged"/>. Values set from code
    /// (<see cref="SetValue(float)"/>) can be animated - see <see cref="SliderAnimation"/>.
    /// </summary>
    /// <remarks>
    /// With the Trail animation the slider makes a second bar of its own behind the fill (a copy of the fill's image, tinted with
    /// the Trail Color) and drives it with DOTween on unscaled time - no Update, nothing to wire. A value that drops leaves the
    /// trail where it was; after the delay it glides down to the fill. Like the panels' animations it needs DOTween (the
    /// HAS_DOTWEEN symbol, set by UniMVC's setup script); without it the trail simply follows the fill at once. Only values set
    /// by code animate: a slider the player drags is left alone, so leave the animation off for those.
    /// </remarks>
    [RequireComponent(typeof(Slider))]
    public abstract class SliderViewBase : ViewBase
    {
        [Tooltip("How a change of value is shown. Trail: a drop leaves a lighter bar behind that glides down to the fill (e.g. a health bar).")]
        [SerializeField] private SliderAnimation valueAnimation;

        [Tooltip("The colour of the trail bar: a lighter tone of the fill's.")]
        [SerializeField] private Color trailColor = new(1f, 0.62f, 0.45f, 1f);

        [Tooltip("Seconds the trail waits, after a drop, before it starts to glide down.")]
        [Range(0f, 2f)] [SerializeField] private float trailDelay = 0.4f;

        [Tooltip("Seconds the trail takes to glide down to the fill.")]
        [Range(0.05f, 2f)] [SerializeField] private float trailDuration = 0.5f;

        [Tooltip("The easing curve of the glide.")]
        [SerializeField] private AnimationEase trailEase = AnimationEase.OutQuad;

        private const string TrailName = "Trail";

        private RectTransform _trailRect;
        private Image _trailImage;
        private float _trailNormalized;
#if HAS_DOTWEEN
        private Tween _trailTween;
#endif

        protected Slider Slider { get; private set; }

        public float Value => Slider.value;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Slider = GetComponent<Slider>();
            Slider.onValueChanged.AddListener(OnValueChanged);

            if (valueAnimation == SliderAnimation.Trail)
            {
                CreateTrail();
            }
        }

        /// <summary>Sets the value without calling <see cref="OnValueChanged"/>. A drop is animated if the slider has a Trail.</summary>
        public void SetValue(float value)
        {
            var before = Slider.normalizedValue;
            Slider.SetValueWithoutNotify(value);
            FollowTrail(before);
        }

        /// <summary>
        /// Sets the maximum, then the value, without calling <see cref="OnValueChanged"/>. A drop within the same maximum is
        /// animated if the slider has a Trail; a new maximum starts the trail over, so it never glides across a changed scale.
        /// </summary>
        public void SetValue(float value, float maxValue)
        {
            var sameRange = Mathf.Approximately(Slider.maxValue, maxValue);
            var before = Slider.normalizedValue;
            Slider.maxValue = maxValue;
            Slider.SetValueWithoutNotify(value);

            if (sameRange)
            {
                FollowTrail(before);
            }
            else
            {
                SnapTrail();
            }
        }

        protected virtual void OnValueChanged(float value)
        {
        }

#if HAS_DOTWEEN
        private void OnDestroy() => _trailTween?.Kill();
#endif

        #region Trail

        // The trail bar: a sibling of the fill, in the same Fill Area and just behind it, shaped like it.
        private void CreateTrail()
        {
            var fill = Slider.fillRect;
            if (fill == null)
            {
                Debug.LogWarning($"[UniMVC] '{name}' has a Trail animation but its Slider has no Fill Rect, so there is nothing to trail.", this);
                return;
            }

            // A copy of an already initialized slider (a template cloned at run time) carries its trail along: use it, make no second one.
            var existing = fill.parent.Find(TrailName);
            var go = existing != null ? existing.gameObject : new GameObject(TrailName, typeof(RectTransform), typeof(Image));
            _trailRect = (RectTransform)go.transform;
            if (existing == null)
            {
                _trailRect.SetParent(fill.parent, false);
                _trailRect.SetSiblingIndex(fill.GetSiblingIndex());
            }

            _trailRect.offsetMin = Vector2.zero;
            _trailRect.offsetMax = Vector2.zero;

            _trailImage = go.GetComponent<Image>();
            _trailImage.raycastTarget = false;
            _trailImage.color = trailColor;
            if (fill.TryGetComponent<Image>(out var fillImage))
            {
                _trailImage.sprite = fillImage.sprite;
                _trailImage.type = fillImage.type;
                _trailImage.material = fillImage.material;
                if (fillImage.type == Image.Type.Filled)
                {
                    _trailImage.fillMethod = fillImage.fillMethod;
                    _trailImage.fillOrigin = fillImage.fillOrigin;
                    _trailImage.fillClockwise = fillImage.fillClockwise;
                }
            }

            SnapTrail();
        }

        // After a change from <before> (normalized): a rise carries the trail with it; a drop leaves it where the bar was (or where it still
        // is, if it is still gliding from an earlier drop), waits, then glides down to the new value.
        private void FollowTrail(float before)
        {
            if (_trailRect == null)
            {
                return;
            }

            var now = Slider.normalizedValue;
            if (now >= before)
            {
                SnapTrail();
                return;
            }

#if HAS_DOTWEEN
            _trailTween?.Kill();
            ApplyTrail(Mathf.Max(_trailNormalized, before));
            _trailTween = DOTween.Sequence()
                .AppendInterval(trailDelay)
                .Append(DOTween.To((DOGetter<float>)(() => _trailNormalized), (DOSetter<float>)ApplyTrail, now, trailDuration)
                    .SetEase((Ease)trailEase))
                .OnComplete(() => _trailTween = null)
                .SetUpdate(true)
                .SetLink(gameObject);
#else
            SnapTrail();
#endif
        }

        // The trail sits exactly under the fill.
        private void SnapTrail()
        {
            if (_trailRect == null)
            {
                return;
            }

#if HAS_DOTWEEN
            _trailTween?.Kill();
            _trailTween = null;
#endif
            ApplyTrail(Slider.normalizedValue);
        }

        // Shapes the trail like the slider shapes its fill, for a normalized value: anchors along the slider's direction, or the fill amount.
        private void ApplyTrail(float normalized)
        {
            _trailNormalized = normalized;
            if (_trailImage.type == Image.Type.Filled)
            {
                _trailImage.fillAmount = normalized;
                return;
            }

            switch (Slider.direction)
            {
                case Slider.Direction.RightToLeft:
                    _trailRect.anchorMin = new Vector2(1f - normalized, 0f);
                    _trailRect.anchorMax = Vector2.one;
                    break;
                case Slider.Direction.BottomToTop:
                    _trailRect.anchorMin = Vector2.zero;
                    _trailRect.anchorMax = new Vector2(1f, normalized);
                    break;
                case Slider.Direction.TopToBottom:
                    _trailRect.anchorMin = new Vector2(0f, 1f - normalized);
                    _trailRect.anchorMax = Vector2.one;
                    break;
                default:
                    _trailRect.anchorMin = Vector2.zero;
                    _trailRect.anchorMax = new Vector2(normalized, 1f);
                    break;
            }
        }

        #endregion
    }
}
