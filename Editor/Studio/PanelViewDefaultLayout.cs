using System;
using Veneer;

namespace UniMVC
{
    /// <summary>
    /// The design of the <see cref="PanelViewBase"/> Inspector that UniMVC ships: tab Panel (an Animation card with the open and close
    /// animation, Duration and Ease, and a Behaviour card with two chips) and tab Views (the Collect from children button and one list
    /// per kind of view). The fields a panel class adds are drawn by the automatic "Other" card under the tabs. The state pill and the
    /// Collect button are custom items, Duration and Ease are custom widgets (<see cref="UniMVCItems"/>). Popups use
    /// <see cref="PopupViewDefaultLayout"/>, which has the same design on the popup animation fields.
    /// </summary>
    internal sealed class PanelViewDefaultLayout : IVeneerDefaultLayoutProvider
    {
        /// <inheritdoc />
        public Type TargetType => typeof(PanelViewBase);

        /// <inheritdoc />
        public VeneerLayout CreateLayout()
        {
            return Create(typeof(PanelViewBase), false);
        }

        /// <inheritdoc />
        public void RegisterItems()
        {
            UniMVCItems.RegisterPanel();
        }

        /// <summary>Makes the layout of a panel (<paramref name="popup"/> false) or of a popup (true).</summary>
        internal static VeneerLayout Create(Type type, bool popup)
        {
            string openPath = popup ? "popupOpenAnimation" : "openAnimation";
            string closePath = popup ? "popupCloseAnimation" : "closeAnimation";
            string icon = popup ? "overlay" : "layers";

            VeneerLayoutBuilder builder = new VeneerLayoutBuilder(type, null, popup ? "Popup view" : "Panel view", icon)
                .Page("Panel", icon)
                .Card("Animation", "play")
                .Field(openPath, "Open animation")
                .Field(closePath, "Close animation")
                .Field("animationDuration", "Duration")
                .Field("animationEase", "Ease");
#if !HAS_DOTWEEN
            builder.Note(
                "These animations need DOTween, which isn't in this project. Until it's installed, the panel opens and closes " +
                "at once; the settings are kept.");
#endif
            builder.Custom(UniMVCItems.PanelStateId)
                .Card("Behaviour", "settings")
                .Field("blocksGameplay", "Blocks gameplay", widget: VeneerWidget.Toggle).Icon("lock")
                .Field("rebuildLayoutOnShow", "Rebuild layout on show", widget: VeneerWidget.Toggle).Icon("refresh").SameRow()
                .Page("Views", "list");
            UniMVCLayout.AddViewKindCard(builder, "childViews", UniMVCItems.CollectId);

            VeneerLayout layout = builder.Build();
            if (popup)
            {
                // A popup has animations of its own; the ones it inherits from the panel do nothing for it.
                layout.HiddenPaths.Add("openAnimation");
                layout.HiddenPaths.Add("closeAnimation");
            }

            layout.ThemeBrand = UniMVCBrand.Value.Name;
            return layout;
        }
    }

    /// <summary>The default design of the <see cref="PopupViewBase"/> Inspector: the panel design on the popup animation fields.</summary>
    internal sealed class PopupViewDefaultLayout : IVeneerDefaultLayoutProvider
    {
        /// <inheritdoc />
        public Type TargetType => typeof(PopupViewBase);

        /// <inheritdoc />
        public VeneerLayout CreateLayout()
        {
            return PanelViewDefaultLayout.Create(typeof(PopupViewBase), true);
        }

        /// <inheritdoc />
        public void RegisterItems()
        {
            UniMVCItems.RegisterPanel();
        }
    }
}
