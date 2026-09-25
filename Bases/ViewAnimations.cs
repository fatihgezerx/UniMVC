using UnityEngine;

namespace UniMVC
{
    /// <summary>
    /// The easing curve of a panel's animations. The same curves, names and values as DOTween's <c>Ease</c>,
    /// so a panel keeps its setting even in a project without DOTween, and the value converts straight to it.
    /// </summary>
    public enum AnimationEase
    {
        Linear = 1,
        InSine = 2,
        OutSine = 3,
        InOutSine = 4,
        InQuad = 5,
        OutQuad = 6,
        InOutQuad = 7,
        InCubic = 8,
        OutCubic = 9,
        InOutCubic = 10,
        InQuart = 11,
        OutQuart = 12,
        InOutQuart = 13,
        InQuint = 14,
        OutQuint = 15,
        InOutQuint = 16,
        InExpo = 17,
        OutExpo = 18,
        InOutExpo = 19,
        InCirc = 20,
        OutCirc = 21,
        InOutCirc = 22,
        InElastic = 23,
        OutElastic = 24,
        InOutElastic = 25,
        InBack = 26,
        OutBack = 27,
        InOutBack = 28,
        InBounce = 29,
        OutBounce = 30,
        InOutBounce = 31
    }

    /// <summary>How a <see cref="PanelViewBase"/> appears when shown. Every animation also fades in.</summary>
    public enum PanelOpenAnimation
    {
        /// <summary>Appears at once.</summary>
        None,

        /// <summary>Only fades in.</summary>
        FadeIn,

        /// <summary>Settles from a little larger (110%) to its size, as if the camera pulled back.</summary>
        ZoomOut,

        /// <summary>Slides in from the left, one panel width away.</summary>
        SlideFromLeft,

        /// <summary>Slides in from the right, one panel width away.</summary>
        SlideFromRight,

        /// <summary>Slides in from above, one panel height away.</summary>
        SlideFromTop,

        /// <summary>Slides in from below, one panel height away.</summary>
        SlideFromBottom,

        /// <summary>Opens out from a line: first sideways, then up and down.</summary>
        Unfold
    }

    /// <summary>How a <see cref="PanelViewBase"/> disappears when hidden. Every animation also fades out.</summary>
    public enum PanelCloseAnimation
    {
        /// <summary>Disappears at once.</summary>
        None,

        /// <summary>Only fades out.</summary>
        FadeOut,

        /// <summary>Grows a little (to 110%), as if the camera pushed in.</summary>
        ZoomIn,

        /// <summary>Slides out to the left, one panel width away.</summary>
        SlideToLeft,

        /// <summary>Slides out to the right, one panel width away.</summary>
        SlideToRight,

        /// <summary>Slides out upwards, one panel height away.</summary>
        SlideToTop,

        /// <summary>Slides out downwards, one panel height away.</summary>
        SlideToBottom,

        /// <summary>Unfold in reverse: closes up and down into a line, then sideways.</summary>
        Fold
    }

    /// <summary>How a <see cref="PopupViewBase"/> appears when shown. Every animation also fades in.</summary>
    public enum PopupOpenAnimation
    {
        /// <summary>Appears at once.</summary>
        None,

        /// <summary>Only fades in.</summary>
        FadeIn,

        /// <summary>Grows from nothing to its size - try the OutBack ease for an overshoot.</summary>
        PopIn,

        /// <summary>Drops in from above, one popup height away - try the OutBounce ease.</summary>
        DropIn,

        /// <summary>Turns upright (from -15°) while growing from 80% to its size.</summary>
        RotateIn,

        /// <summary>Opens out from a line: first sideways, then up and down.</summary>
        Unfold,

        /// <summary>Slides up from below, one popup height away.</summary>
        SlideUp,

        /// <summary>Turns face-on around the Y axis (from 90°), like a card being turned over.</summary>
        FlipIn
    }

    /// <summary>How a <see cref="PopupViewBase"/> disappears when hidden. Every animation also fades out.</summary>
    public enum PopupCloseAnimation
    {
        /// <summary>Disappears at once.</summary>
        None,

        /// <summary>Only fades out.</summary>
        FadeOut,

        /// <summary>Shrinks to nothing.</summary>
        PopOut,

        /// <summary>Falls away downwards, two popup heights - further and faster than SlideDown.</summary>
        DropOut,

        /// <summary>RotateIn in reverse: tilts (to -15°) while shrinking to 80%.</summary>
        RotateOut,

        /// <summary>Unfold in reverse: closes up and down into a line, then sideways.</summary>
        Fold,

        /// <summary>Slides down, one popup height away.</summary>
        SlideDown,

        /// <summary>FlipIn in reverse: turns edge-on around the Y axis (to 90°).</summary>
        FlipOut
    }

    /// <summary>
    /// What an open or close animation changes: the hidden-side state of the view, relative to its resting
    /// state. Opening animates from this state to rest, closing from rest to this state. Every animation
    /// fades; the rest is optional.
    /// </summary>
    internal readonly struct ViewTransition
    {
        private const float ZoomScale = 1.1f;
        private const float RotateScale = 0.8f;
        private static readonly Vector3 RotateAngle = new(0f, 0f, -15f);
        private static readonly Vector3 FlipAngle = new(0f, 90f, 0f);

        public static readonly ViewTransition None = default;

        /// <summary>Whether anything animates at all; false for None.</summary>
        public readonly bool IsAnimated;

        /// <summary>Scale on the hidden side, as a multiple of the resting scale (1 = unchanged).</summary>
        public readonly float Scale;

        /// <summary>Offset on the hidden side, in multiples of the view's own size (e.g. (-1, 0) = one width to the left).</summary>
        public readonly Vector2 Offset;

        /// <summary>Rotation on the hidden side, as Euler angles in degrees, on top of the resting rotation.</summary>
        public readonly Vector3 Rotation;

        /// <summary>Folded into nothing on the hidden side, unfolding sideways first, then up and down.</summary>
        public readonly bool Unfold;

        private ViewTransition(float scale = 1f, Vector2 offset = default, Vector3 rotation = default, bool unfold = false)
        {
            IsAnimated = true;
            Scale = scale;
            Offset = offset;
            Rotation = rotation;
            Unfold = unfold;
        }

        public bool IsNone => !IsAnimated;

        public bool ChangesScale => !Unfold && !Mathf.Approximately(Scale, 1f);

        public bool ChangesPosition => Offset != Vector2.zero;

        public bool ChangesRotation => Rotation != Vector3.zero;

        // Only the fade. Not "new()": that would be the struct's default, i.e. None.
        private static ViewTransition Fade => new(scale: 1f);

        public static ViewTransition Of(PanelOpenAnimation animation) => animation switch
        {
            PanelOpenAnimation.FadeIn => Fade,
            PanelOpenAnimation.ZoomOut => new ViewTransition(scale: ZoomScale),
            PanelOpenAnimation.SlideFromLeft => new ViewTransition(offset: Vector2.left),
            PanelOpenAnimation.SlideFromRight => new ViewTransition(offset: Vector2.right),
            PanelOpenAnimation.SlideFromTop => new ViewTransition(offset: Vector2.up),
            PanelOpenAnimation.SlideFromBottom => new ViewTransition(offset: Vector2.down),
            PanelOpenAnimation.Unfold => new ViewTransition(unfold: true),
            _ => None
        };

        public static ViewTransition Of(PanelCloseAnimation animation) => animation switch
        {
            PanelCloseAnimation.FadeOut => Fade,
            PanelCloseAnimation.ZoomIn => new ViewTransition(scale: ZoomScale),
            PanelCloseAnimation.SlideToLeft => new ViewTransition(offset: Vector2.left),
            PanelCloseAnimation.SlideToRight => new ViewTransition(offset: Vector2.right),
            PanelCloseAnimation.SlideToTop => new ViewTransition(offset: Vector2.up),
            PanelCloseAnimation.SlideToBottom => new ViewTransition(offset: Vector2.down),
            PanelCloseAnimation.Fold => new ViewTransition(unfold: true),
            _ => None
        };

        public static ViewTransition Of(PopupOpenAnimation animation) => animation switch
        {
            PopupOpenAnimation.FadeIn => Fade,
            PopupOpenAnimation.PopIn => new ViewTransition(scale: 0f),
            PopupOpenAnimation.DropIn => new ViewTransition(offset: Vector2.up),
            PopupOpenAnimation.RotateIn => new ViewTransition(scale: RotateScale, rotation: RotateAngle),
            PopupOpenAnimation.Unfold => new ViewTransition(unfold: true),
            PopupOpenAnimation.SlideUp => new ViewTransition(offset: Vector2.down),
            PopupOpenAnimation.FlipIn => new ViewTransition(rotation: FlipAngle),
            _ => None
        };

        public static ViewTransition Of(PopupCloseAnimation animation) => animation switch
        {
            PopupCloseAnimation.FadeOut => Fade,
            PopupCloseAnimation.PopOut => new ViewTransition(scale: 0f),
            PopupCloseAnimation.DropOut => new ViewTransition(offset: Vector2.down * 2f),
            PopupCloseAnimation.RotateOut => new ViewTransition(scale: RotateScale, rotation: RotateAngle),
            PopupCloseAnimation.Fold => new ViewTransition(unfold: true),
            PopupCloseAnimation.SlideDown => new ViewTransition(offset: Vector2.down),
            PopupCloseAnimation.FlipOut => new ViewTransition(rotation: FlipAngle),
            _ => None
        };
    }
}
