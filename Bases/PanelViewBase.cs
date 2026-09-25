#if HAS_DOTWEEN
using DG.Tweening;
using DG.Tweening.Core;
#endif
using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// A screen or window: shown and hidden as a whole, with <see cref="OnShown"/> / <see cref="OnHidden"/>
    /// hooks that only run when the visibility actually changes. A panel owns the views inside it - listed
    /// by kind under <see cref="ChildViews"/> in the Inspector - and initializes them when it is initialized.
    /// </summary>
    /// <remarks>
    /// It can animate as it opens and closes - fade, zoom, slide from an edge, unfold; see
    /// <see cref="PanelOpenAnimation"/> - with DOTween on unscaled time, so a pause menu animates too.
    /// DOTween is optional: without it (no HAS_DOTWEEN symbol, set by UniMVC's setup script) panels open and
    /// close at once, keeping their animation settings. <see cref="OnShown"/> runs when the
    /// panel starts opening and <see cref="OnHidden"/> when it starts closing; the object is deactivated
    /// once the close animation ends. Showing it again while it closes turns it straight back. The fade
    /// runs on the panel's own CanvasGroup, which it requires; while any animation plays, that CanvasGroup
    /// is not interactable, so nothing can be clicked on a panel that is still arriving or already leaving.
    /// </remarks>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PanelViewBase : ViewBase
    {
        [Tooltip("How the panel appears when shown.")]
        [SerializeField] private PanelOpenAnimation openAnimation;

        [Tooltip("How the panel disappears when hidden.")]
        [SerializeField] private PanelCloseAnimation closeAnimation;

        [Tooltip("Seconds an open or close animation takes.")]
        [Range(0.1f, 1f)] [SerializeField] private float animationDuration = 0.25f;

        [Tooltip("The easing curve of the open and close animations.")]
        [SerializeField] private AnimationEase animationEase = AnimationEase.OutQuad;

        [Tooltip("The views inside this panel, by kind. Initialized (and registered with the UIManager) when this panel is.")]
        [SerializeField] private ViewCollection childViews = new();

#if HAS_DOTWEEN
        private Tween _animation;
        private bool IsAnimating => _animation != null;
#else
        private bool IsAnimating => false;
#endif
        private bool _isClosing;
        private CanvasGroup _canvasGroup;

        // The scale, position and rotation the animations start from and return to, captured when one
        // starts from rest and kept while one interrupts another.
        private bool _hasRest;
        private Vector3 _restScale;
        private Vector2 _restPosition;
        private Quaternion _restRotation;

#if HAS_DOTWEEN
        // The rotation an animation has added on top of the resting rotation, as Euler angles in degrees.
        private Vector3 _rotation;
#endif

        // Whether an animation has made the CanvasGroup non-interactable, and what it was before.
        private bool _isLocked;
        private bool _unlockedInteractable;

        /// <summary>The views inside this panel, by kind.</summary>
        public ViewCollection ChildViews => childViews;

        /// <summary>Whether the panel is shown and not on its way out (<see cref="ViewBase.IsVisible"/> stays true while it closes).</summary>
        public bool IsOpen => IsVisible && !_isClosing;

        private protected virtual ViewTransition OpenTransition => ViewTransition.Of(openAnimation);

        private protected virtual ViewTransition CloseTransition => ViewTransition.Of(closeAnimation);

        // Panels added before the CanvasGroup was required may still lack one until they are inspected.
        private CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null && !TryGetComponent(out _canvasGroup))
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }

                return _canvasGroup;
            }
        }

        /// <summary>Initializes every child view. Call <c>base.OnInitialize()</c> when overriding.</summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            childViews.Initialize(UI);
        }

        public override void Show()
        {
            if (IsOpen)
            {
                return;
            }

            // Turning back while closing: carry on from where the close animation is.
            var interrupted = IsAnimating;
            StopAnimation();

            base.Show();
            PlayOpen(interrupted);
            OnShown();
        }

        public override void Hide()
        {
            if (!IsOpen)
            {
                return;
            }

            StopAnimation();

            var transition = CloseTransition;
            if (!CanAnimate(transition))
            {
                RestoreRest();
                Unlock();
                base.Hide();
                OnHidden();
                return;
            }

#if HAS_DOTWEEN
            // Not interactable from the moment it starts leaving; interactable again only once it is
            // deactivated, ready for its next open.
            Lock();
            _isClosing = true;
            CaptureRest();
            _animation = Animate(transition, jumpToStart: false, closing: true)
                .OnComplete(() =>
                {
                    _animation = null;
                    _isClosing = false;
                    Deactivate();
                    RestoreRest();
                    Unlock();
                });
            OnHidden();
#endif
        }

        public void Toggle()
        {
            if (IsOpen)
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

        // base.Hide from the close animation's callback.
        private void Deactivate() => base.Hide();

        // Whether an animation can play: the transition has one, DOTween is installed, and the panel is
        // really on screen (an inactive parent would hide it all along).
        private bool CanAnimate(ViewTransition transition)
        {
#if HAS_DOTWEEN
            return !transition.IsNone && gameObject.activeInHierarchy;
#else
            return false;
#endif
        }

        private void PlayOpen(bool interrupted)
        {
            var transition = OpenTransition;
            if (!CanAnimate(transition))
            {
                RestoreRest();
                Unlock();
                return;
            }

#if HAS_DOTWEEN
            // Not interactable until it has fully arrived.
            Lock();
            CaptureRest();
            _animation = Animate(transition, jumpToStart: !interrupted, closing: false)
                .OnComplete(() =>
                {
                    _animation = null;
                    _hasRest = false;
                    Unlock();
                });
#endif
        }

#if HAS_DOTWEEN

        // Animates between rest and the transition's hidden-side state: towards it when closing, away from
        // it when opening. Every animation fades; scale, position and rotation are only touched when the
        // transition changes them, so a panel inside a layout group keeps its position.
        private Tween Animate(ViewTransition transition, bool jumpToStart, bool closing)
        {
            var sequence = DOTween.Sequence();

            var group = CanvasGroup;
            if (jumpToStart)
            {
                group.alpha = 0f;
            }

            sequence.Join(TweenTo(() => group.alpha, (float alpha) => group.alpha = alpha,
                closing ? 0f : 1f, animationDuration));

            if (transition.Unfold)
            {
                sequence.Join(Unfold(jumpToStart, closing));
            }
            else if (transition.ChangesScale)
            {
                var hiddenScale = _restScale * transition.Scale;
                if (jumpToStart)
                {
                    transform.localScale = hiddenScale;
                }

                sequence.Join(TweenTo(() => transform.localScale, (Vector3 scale) => transform.localScale = scale,
                    closing ? hiddenScale : _restScale, animationDuration));
            }

            if (transition.ChangesPosition && transform is RectTransform rect)
            {
                // One step is the view's own size, so a slide from the left starts just outside its place.
                var hiddenPosition = _restPosition + Vector2.Scale(transition.Offset, rect.rect.size);
                if (jumpToStart)
                {
                    rect.anchoredPosition = hiddenPosition;
                }

                sequence.Join(TweenTo(() => rect.anchoredPosition, (Vector2 position) => rect.anchoredPosition = position,
                    closing ? hiddenPosition : _restPosition, animationDuration));
            }

            if (transition.ChangesRotation)
            {
                if (jumpToStart)
                {
                    SetRotation(transition.Rotation);
                }

                sequence.Join(TweenTo(() => _rotation, SetRotation,
                    closing ? transition.Rotation : Vector3.zero, animationDuration));
            }

            // The ease is on each tween; a sequence ease would curve them a second time.
            return sequence.SetEase(Ease.Linear).SetUpdate(true).SetLink(gameObject);
        }

        // Unfold: from nothing to a thin line sideways, then up and down to full size, each in half the
        // duration. Fold is the same played backwards.
        private Sequence Unfold(bool jumpToStart, bool closing)
        {
            const float lineThickness = 0.05f;
            var folded = new Vector3(0f, _restScale.y * lineThickness, _restScale.z);
            var line = new Vector3(_restScale.x, _restScale.y * lineThickness, _restScale.z);
            var half = animationDuration * 0.5f;

            if (jumpToStart)
            {
                transform.localScale = folded;
            }

            return DOTween.Sequence()
                .Append(TweenTo(() => transform.localScale, (Vector3 scale) => transform.localScale = scale, line, half))
                .Append(TweenTo(() => transform.localScale, (Vector3 scale) => transform.localScale = scale,
                    closing ? folded : _restScale, half));
        }

        private Tween TweenTo(DOGetter<float> getter, DOSetter<float> setter, float end, float duration) =>
            DOTween.To(getter, setter, end, duration).SetEase((Ease)animationEase);

        private Tween TweenTo(DOGetter<Vector2> getter, DOSetter<Vector2> setter, Vector2 end, float duration) =>
            DOTween.To(getter, setter, end, duration).SetEase((Ease)animationEase);

        private Tween TweenTo(DOGetter<Vector3> getter, DOSetter<Vector3> setter, Vector3 end, float duration) =>
            DOTween.To(getter, setter, end, duration).SetEase((Ease)animationEase);

        private void SetRotation(Vector3 rotation)
        {
            _rotation = rotation;
            transform.localRotation = _restRotation * Quaternion.Euler(rotation);
        }
#endif

        // Makes the CanvasGroup non-interactable for an animation, remembering what it was.
        private void Lock()
        {
            if (_isLocked)
            {
                return;
            }

            var group = CanvasGroup;
            _isLocked = true;
            _unlockedInteractable = group.interactable;
            group.interactable = false;
        }

        // Gives the CanvasGroup back the interactable it had before the animation.
        private void Unlock()
        {
            if (!_isLocked)
            {
                return;
            }

            _isLocked = false;
            CanvasGroup.interactable = _unlockedInteractable;
        }

        private void StopAnimation()
        {
#if HAS_DOTWEEN
            _animation?.Kill();
            _animation = null;
#endif
            _isClosing = false;
        }

        private void CaptureRest()
        {
            if (_hasRest)
            {
                return;
            }

            _hasRest = true;
            _restScale = transform.localScale;
            _restPosition = transform is RectTransform rect ? rect.anchoredPosition : Vector2.zero;
            _restRotation = transform.localRotation;
#if HAS_DOTWEEN
            _rotation = Vector3.zero;
#endif
        }

        // Puts back whatever an animation left changed, so the next open starts from the real layout.
        private void RestoreRest()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }

            if (!_hasRest)
            {
                return;
            }

            _hasRest = false;
            transform.localScale = _restScale;
            transform.localRotation = _restRotation;
#if HAS_DOTWEEN
            _rotation = Vector3.zero;
#endif
            if (transform is RectTransform rect)
            {
                rect.anchoredPosition = _restPosition;
            }
        }
    }
}
