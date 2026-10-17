using System;
using Veneer;

namespace UniMVC
{
    /// <summary>
    /// The default design of the Inspector of the views that have no editor of their own (buttons, toggles, dropdowns, texts, images and
    /// views you write yourself): the UniMVC header card with the kind's icon and subtitle ("Button view"...), and the automatic "Other"
    /// card for the fields (the fields differ per class, so no card can be fixed). Providers are found by type, so one small class per
    /// kind keeps the icon and subtitle the old Inspector chose by kind. Panels, popups and sliders have layouts of their own.
    /// </summary>
    internal abstract class ViewKindDefaultLayout : IVeneerDefaultLayoutProvider
    {
        /// <inheritdoc />
        public abstract Type TargetType { get; }

        /// <summary>The word before "view" in the subtitle (Button, Toggle, Custom...).</summary>
        protected abstract string Kind { get; }

        /// <summary>The kit icon of the header; null shows the brand's logo.</summary>
        protected abstract string Icon { get; }

        /// <inheritdoc />
        public VeneerLayout CreateLayout()
        {
            return UniMVCLayout.HeaderOnly(TargetType, Kind + " view", Icon);
        }

        /// <inheritdoc />
        public void RegisterItems()
        {
        }
    }

    /// <summary>Default layout for views that derive straight from <see cref="ViewBase"/> (shown as "Custom view").</summary>
    internal sealed class CustomViewDefaultLayout : ViewKindDefaultLayout
    {
        /// <inheritdoc />
        public override Type TargetType => typeof(ViewBase);

        /// <inheritdoc />
        protected override string Kind => "Custom";

        /// <inheritdoc />
        protected override string Icon => null;
    }

    /// <summary>Default layout for <see cref="ButtonViewBase"/> views.</summary>
    internal sealed class ButtonViewDefaultLayout : ViewKindDefaultLayout
    {
        /// <inheritdoc />
        public override Type TargetType => typeof(ButtonViewBase);

        /// <inheritdoc />
        protected override string Kind => "Button";

        /// <inheritdoc />
        protected override string Icon => "cursor";
    }

    /// <summary>Default layout for <see cref="ToggleViewBase"/> views.</summary>
    internal sealed class ToggleViewDefaultLayout : ViewKindDefaultLayout
    {
        /// <inheritdoc />
        public override Type TargetType => typeof(ToggleViewBase);

        /// <inheritdoc />
        protected override string Kind => "Toggle";

        /// <inheritdoc />
        protected override string Icon => "check-circle";
    }

    /// <summary>Default layout for <see cref="DropdownViewBase"/> views.</summary>
    internal sealed class DropdownViewDefaultLayout : ViewKindDefaultLayout
    {
        /// <inheritdoc />
        public override Type TargetType => typeof(DropdownViewBase);

        /// <inheritdoc />
        protected override string Kind => "Dropdown";

        /// <inheritdoc />
        protected override string Icon => "chevron-down";
    }

    /// <summary>Default layout for <see cref="TextViewBase"/> views.</summary>
    internal sealed class TextViewDefaultLayout : ViewKindDefaultLayout
    {
        /// <inheritdoc />
        public override Type TargetType => typeof(TextViewBase);

        /// <inheritdoc />
        protected override string Kind => "Text";

        /// <inheritdoc />
        protected override string Icon => "text";
    }

    /// <summary>Default layout for <see cref="ImageViewBase"/> views.</summary>
    internal sealed class ImageViewDefaultLayout : ViewKindDefaultLayout
    {
        /// <inheritdoc />
        public override Type TargetType => typeof(ImageViewBase);

        /// <inheritdoc />
        protected override string Kind => "Image";

        /// <inheritdoc />
        protected override string Icon => "image";
    }
}
