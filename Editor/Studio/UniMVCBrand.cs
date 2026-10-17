using Veneer;
using UnityEngine;

namespace UniMVC
{
    /// <summary>The identity of UniMVC in the shared editor look: name, indigo accent, logo and banner.</summary>
    internal static class UniMVCBrand
    {
        private const string BrandFolder = "Assets/Scripts/UniMVC/Editor/Brand/";

        /// <summary>The cached brand; its pictures load lazily from <c>Editor/Brand</c>.</summary>
        public static readonly VeneerBrand Value = VeneerBrand.FromAssets(
            name: "UniMVC",
            subtitle: "Views, panels and controllers",
            accent: new Color32(0x5B, 0x5B, 0xD6, 255),
            logoPath: BrandFolder + "logo.png",
            bannerPath: BrandFolder + "banner.png",
            iconFolder: null,
            styleSheetPath: BrandFolder + "UniMVCBrand.uss",
            accentClass: "vn-accent--violet");
    }
}
