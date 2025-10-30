using LabExtended.API.Enums;

using System.ComponentModel;

namespace mcx.Overlays.Levels
{
    /// <summary>
    /// Settings for the level overlay.
    /// </summary>
    public class LevelSettings
    {
        /// <summary>
        /// Whether the level overlay is enabled or disabled.
        /// </summary>
        [Description("Enables or disables the level overlay.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the alignment of the level overlay on the screen.
        /// </summary>
        [Description("Sets the alignment of the level overlay on the screen.")]
        public HintAlign Align { get; set; } = HintAlign.Center;

        /// <summary>
        /// Gets or sets the vertical offset of the level overlay on the screen.
        /// </summary>
        [Description("Sets the vertical offset of the level overlay on the screen.")]
        public float VerticalOffset { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the pixel spacing on the level overlay.
        /// </summary>
        [Description("Sets the pixel spacing on the level overlay. Set to -1 to use default spacing.")]
        public int PixelSpacing { get; set; } = -1;

        /// <summary>
        /// Gets or sets the duration, in seconds, for which the level gain is displayed in the overlay.
        /// </summary>
        [Description("Sets the amount of seconds level gain is shown for in the overlay.")]
        public float LevelGainDuration { get; set; } = 5f;

        /// <summary>
        /// Gets or sets the duration, in seconds, for which the experience gain is displayed in the overlay.
        /// </summary>
        [Description("Sets the amount of seconds experience gain is shown for in the overlay.")]
        public float ExperienceGainDuration { get; set; } = 5f;
    }
}