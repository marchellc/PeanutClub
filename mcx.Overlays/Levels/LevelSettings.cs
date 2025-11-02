using mcx.Overlays.Elements;

using System.ComponentModel;

namespace mcx.Overlays.Levels
{
    /// <summary>
    /// Settings for the level overlay.
    /// </summary>
    public class LevelSettings : OverlayOptions
    {
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