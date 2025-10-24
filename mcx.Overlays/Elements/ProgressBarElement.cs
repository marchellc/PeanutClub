namespace mcx.Overlays.Elements
{
    /// <summary>
    /// Provides methods to render a textual progress bar representation.
    /// </summary>
    public static class ProgressBarElement
    {
        /// <summary>
        /// Renders a progress bar as a string representation based on the current progress relative to the total.
        /// </summary>
        /// <remarks>The method calculates the fraction of progress and renders a visual representation
        /// using specified characters. If <paramref name="total"/> is zero or negative, the progress is considered as
        /// 0%.</remarks>
        /// <param name="current">The current progress value. Must be non-negative.</param>
        /// <param name="total">The total value representing 100% progress. Must be greater than zero.</param>
        /// <returns>A string representing the progress bar, including optional labels and percentage.</returns>
        public static string RenderBarFraction(int current, int total, ProgressBarSettings progressBarSettings)
        {
            if (total <= 0)
                return RenderBar(0.0, progressBarSettings);

            var progress = current / (double)total;
            return RenderBar(progress, progressBarSettings);
        }

        /// <summary>
        /// Renders a progress bar as a string representation based on the specified progress value.
        /// </summary>
        /// <param name="progress">A double value representing the progress, where 0.0 indicates no progress and 1.0 indicates full progress.
        /// Values outside this range will be clamped.</param>
        /// <param name="width">The total width of the progress bar in characters. Must be greater than 0.</param>
        /// <param name="fillChar">The character used to represent the filled portion of the progress bar.</param>
        /// <param name="emptyChar">The character used to represent the unfilled portion of the progress bar.</param>
        /// <param name="showPercent">A boolean indicating whether to display the percentage of progress next to the bar. <see langword="true"/>
        /// to show the percentage; otherwise, <see langword="false"/>.</param>
        /// <param name="leftLabel">An optional label to display to the left of the progress bar.</param>
        /// <param name="rightLabel">An optional label to display to the right of the progress bar.</param>
        /// <returns>A string representing the progress bar, including optional labels and percentage.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="width"/> is less than 1.</exception>
        public static string RenderBar(double progress, ProgressBarSettings progressBarSettings)
        {
            if (progressBarSettings.Width < 1)
                throw new ArgumentOutOfRangeException(nameof(progressBarSettings.Width));

            if (double.IsNaN(progress)) 
                progress = 0.0;

            progress = Math.Max(0.0, Math.Min(1.0, progress));

            var filled = (int)Math.Round(progress * progressBarSettings.Width);

            if (filled > progressBarSettings.Width) 
                filled = progressBarSettings.Width;

            if (filled < 0) 
                filled = 0;

            var filledPart = string.Empty;
            var emptyPart = string.Empty;

            var filledString = progressBarSettings.FilledPart.GetValue();
            var emptyString = progressBarSettings.EmptyPart.GetValue();

            for (var i = 0; i < filled; i++)
                filledPart += filledString;

            for (var i = 0; i < progressBarSettings.Width - filled; i++)
                emptyPart += emptyString;

            var leftLabel = progressBarSettings.LeftLabel.GetValue();
            var rightLabel = progressBarSettings.RightLabel.GetValue();

            var percentFormat = progressBarSettings.PercentFormat.GetValue();
            var percent = progressBarSettings.ShowPercent 
                ? " " + string.Format(percentFormat, Math.Round(progress * 100)) 
                : string.Empty;

            var left = string.IsNullOrEmpty(leftLabel) ? string.Empty : leftLabel + " ";
            var right = string.IsNullOrEmpty(rightLabel) ? string.Empty : " " + rightLabel;

            return $"{left}{filledPart}{emptyPart}{percent}{right}";
        }
    }
}