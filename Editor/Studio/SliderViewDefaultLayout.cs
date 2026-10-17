using System;
using Veneer;

namespace UniMVC
{
    /// <summary>
    /// The design of the <see cref="SliderViewBase"/> Inspector that UniMVC ships: a Value animation card (None or Trail; the trail
    /// colour, delay, duration and ease show only while Trail is chosen). The fields a slider class adds are drawn by the automatic
    /// "Other" card below it. The state pill is a custom item (<see cref="UniMVCItems"/>).
    /// </summary>
    internal sealed class SliderViewDefaultLayout : IVeneerDefaultLayoutProvider
    {
        private const string AnimationTooltip =
            "None: the bar moves at once.\n" +
            "Trail: a drop leaves a lighter bar behind that glides down to the fill (e.g. a health bar).";

        /// <inheritdoc />
        public Type TargetType => typeof(SliderViewBase);

        /// <inheritdoc />
        public VeneerLayout CreateLayout()
        {
            VeneerLayout layout = new VeneerLayoutBuilder(typeof(SliderViewBase), null, "Slider view", "sliders")
                .Page("Slider", "sliders")
                .Card("Value animation", "graph")
                .Enum("valueAnimation", "Value animation", "minus-circle,graph")
                .Field("trailColor", "Trail color", widget: VeneerWidget.Color)
                .ShowIf("valueAnimation", VeneerCondition.Equals, "Trail")
                .Slider("trailDelay", "Trail delay", "Instant", "Long", "s")
                .ShowIf("valueAnimation", VeneerCondition.Equals, "Trail")
                .Slider("trailDuration", "Trail duration", "Fast", "Slow", "s")
                .ShowIf("valueAnimation", VeneerCondition.Equals, "Trail")
                .Field("trailEase", "Trail ease")
                .ShowIf("valueAnimation", VeneerCondition.Equals, "Trail")
                .Custom(UniMVCItems.SliderStateId)
                .Build();

            // The segmented buttons have icons only, so the explanation of both values goes on the row.
            VeneerLayoutEntry animation = layout.FindEntry("valueAnimation", out _, out _);
            if (animation != null)
            {
                animation.Tooltip = AnimationTooltip;
            }

            layout.ThemeBrand = UniMVCBrand.Value.Name;
            return layout;
        }

        /// <inheritdoc />
        public void RegisterItems()
        {
            UniMVCItems.RegisterSlider();
        }
    }
}
