using System;
using Veneer;

namespace UniMVC
{
    /// <summary>
    /// The design of the <see cref="UIManager"/> Inspector that UniMVC ships: tab Views (the Collect from children button and one list
    /// per kind of view) and tab Controllers (a plain Collect button and the controller list). The counts pill and the Collect buttons
    /// are custom items (<see cref="UniMVCItems"/>).
    /// </summary>
    internal sealed class UIManagerDefaultLayout : IVeneerDefaultLayoutProvider
    {
        /// <inheritdoc />
        public Type TargetType => typeof(UIManager);

        /// <inheritdoc />
        public VeneerLayout CreateLayout()
        {
            VeneerLayoutBuilder builder = new VeneerLayoutBuilder(typeof(UIManager), "UI Manager", "Initializes the views, then the controllers")
                .Page("Views", "layers");
            UniMVCLayout.AddViewKindCard(builder, "views", UniMVCItems.CollectId);

            VeneerLayout layout = builder
                .Custom(UniMVCItems.ManagerStateId)
                .Page("Controllers", "bolt")
                .Card("Controllers", "bolt")
                .Custom(UniMVCItems.CollectSecondaryId)
                .Field("controllers", "Controllers", widget: VeneerWidget.List)
                .Build();

            layout.ThemeBrand = UniMVCBrand.Value.Name;
            return layout;
        }

        /// <inheritdoc />
        public void RegisterItems()
        {
            UniMVCItems.RegisterManager();
        }
    }
}
