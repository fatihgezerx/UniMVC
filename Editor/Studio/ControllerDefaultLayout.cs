using System;
using Veneer;

namespace UniMVC
{
    /// <summary>
    /// The design of the <see cref="ControllerBase"/> Inspector that UniMVC ships: the UniMVC header card (bolt icon, subtitle
    /// "Controller") and nothing else. One editor serves every controller you write and their fields differ, so no card can be fixed:
    /// the layout's automatic "Other" card draws every visible field of the controller (arrays as lists, nested structs as cards,
    /// <c>[Range]</c> as sliders). Customize it in the Inspector Designer to add tabs and cards of your own.
    /// </summary>
    internal sealed class ControllerDefaultLayout : IVeneerDefaultLayoutProvider
    {
        /// <inheritdoc />
        public Type TargetType => typeof(ControllerBase);

        /// <inheritdoc />
        public VeneerLayout CreateLayout()
        {
            return UniMVCLayout.HeaderOnly(typeof(ControllerBase), "Controller", "bolt");
        }

        /// <inheritdoc />
        public void RegisterItems()
        {
        }
    }
}
