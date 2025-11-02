using LabExtended.API;
using LabExtended.API.Enums;
using LabExtended.API.Hints;

using mcx.Utilities.Extensions;

namespace mcx.Overlays.Common
{
    /// <summary>
    /// Displays static strings.
    /// </summary>
    public class StaticStringOverlay : HintElement
    {
        /// <summary>
        /// Gets the options for this overlay.
        /// </summary>
        public OverlayOptions Options { get; }

        public StaticStringOverlay(OverlayOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            Options = options;
        }

        /// <summary>
        /// Gets the server name string.
        /// </summary>
        public string? Content { get; private set; }

        /// <inheritdoc/>
        public override HintAlign GetAlignment(ExPlayer player)
            => Options.Align;

        /// <inheritdoc/>
        public override int GetPixelSpacing(ExPlayer player)
            => Options.PixelSpacing;

        /// <inheritdoc/>
        public override float GetVerticalOffset(ExPlayer player)
            => Options.VerticalOffset;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            Content = Options.OverlayString
                .GetValue()
                .ReplaceEmojis();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            Content = null;
        }

        /// <inheritdoc/>
        public override bool OnDraw(ExPlayer player)
        {
            if (Builder == null)
                return false;

            if (Content != null)
                Builder.Append(Content);

            return Builder.Length > 0;
        }
    }
}