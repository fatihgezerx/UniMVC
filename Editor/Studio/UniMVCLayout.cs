using System;
using Veneer;

namespace UniMVC
{
    /// <summary>Pieces shared by the default layouts of the studio-look Inspectors of UniMVC.</summary>
    internal static class UniMVCLayout
    {
        // Serialized fields of ViewCollection, in the order the old Inspector listed them, with the label of each list.
        private static readonly (string Field, string Label)[] ViewKinds =
        {
            ("panels", "Panel views"),
            ("popups", "Popup views"),
            ("buttons", "Button views"),
            ("toggles", "Toggle views"),
            ("sliders", "Slider views"),
            ("dropdowns", "Dropdown views"),
            ("texts", "Text views"),
            ("images", "Image views")
        };

        /// <summary>
        /// A layout with only the header (the title is the type's name split into words, <paramref name="subtitle"/> under it, an optional
        /// <paramref name="icon"/>) and the automatic "Other" card, which draws every visible field of the component. It is the design of
        /// the editors that serve many subclasses with different fields (controllers and the plain views): no card or tab can be fixed
        /// for them.
        /// </summary>
        /// <param name="type">The base type the layout is registered for.</param>
        /// <param name="subtitle">The muted line under the title.</param>
        /// <param name="icon">A kit icon name for the header; null shows the brand's logo.</param>
        public static VeneerLayout HeaderOnly(Type type, string subtitle, string icon)
        {
            VeneerLayout layout = new VeneerLayoutBuilder(type, null, subtitle, icon).Build();
            layout.ThemeBrand = UniMVCBrand.Value.Name;
            return layout;
        }

        /// <summary>
        /// Starts a card "Views by kind" in the current tab of <paramref name="builder"/>: the Collect item <paramref name="collectId"/>
        /// first, then one list per kind of view of the <see cref="ViewCollection"/> at <paramref name="collectionPath"/>.
        /// </summary>
        /// <param name="builder">The builder of the layout.</param>
        /// <param name="collectionPath">The path of the ViewCollection field (<c>views</c>, <c>childViews</c>).</param>
        /// <param name="collectId">The id of the registered "Collect from children" item.</param>
        public static VeneerLayoutBuilder AddViewKindCard(VeneerLayoutBuilder builder, string collectionPath, string collectId)
        {
            builder.Card("Views by kind", "layers").Custom(collectId);
            for (int i = 0; i < ViewKinds.Length; i++)
            {
                builder.Field(collectionPath + "." + ViewKinds[i].Field, ViewKinds[i].Label, widget: VeneerWidget.List);
            }

            return builder;
        }
    }
}
