using System.ComponentModel;

namespace mcx.Overlays.Elements
{
    /// <summary>
    /// Represents the configuration settings for a progress bar, including its appearance and optional labels.
    /// </summary>
    public class ProgressBarSettings
    {
        /// <summary>
        /// Gets or sets the total width of the progress bar in characters.
        /// </summary>
        [Description("Total width of the progress bar in characters.")]
        public int Width { get; set; } = 30;

        /// <summary>
        /// Gets or sets a value indicating whether the percentage value is displayed next to the progress bar.
        /// </summary>
        [Description("Show percentage value next to the progress bar.")]
        public bool ShowPercent { get; set; } = true;

        /// <summary>
        /// Gets or sets the format string used for displaying percentages.
        /// </summary>
        [Description("Format string for the percentage display.")]
        public ConfigurableString PercentFormat { get; set; } = new() { Value = "{0}%" };

        /// <summary>
        /// Gets or sets the string used for the filled portion of the progress bar.
        /// </summary>
        [Description("String used for the filled portion of the progress bar.")]
        public ConfigurableString FilledPart { get; set; } = new() { Value = "█" };

        /// <summary>
        /// Gets or sets the string used to represent the empty portion of the progress bar.
        /// </summary>
        [Description("String used for the empty portion of the progress bar.")]
        public ConfigurableString EmptyPart { get; set; } = new() { Value = " " };

        /// <summary>
        /// Gets or sets the label displayed to the left of the progress bar.
        /// </summary>
        [Description("Label displayed to the left of the progress bar.")]
        public ConfigurableString LeftLabel { get; set; } = new();

        /// <summary>
        /// Gets or sets the label displayed to the right of the progress bar.
        /// </summary>
        [Description("Label displayed to the right of the progress bar.")]
        public ConfigurableString RightLabel { get; set; } = new();
    }
}
