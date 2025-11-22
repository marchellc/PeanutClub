using UnityEngine;

namespace SecretLabAPI.Elements.ProgressBar
{
    /// <summary>
    /// Provides methods to render a textual progress bar representation.
    /// </summary>
    public static class ProgressBarElement
    {
        /// <summary>
        /// Renders a progress bar as a formatted string based on the specified percentage, color, and progress bar
        /// settings.
        /// </summary>
        /// <param name="percentage">The completion percentage to display in the progress bar. Values greater than 100 are treated as 100.</param>
        /// <param name="color">The color to use for the progress bar. The format and usage depend on the rendering context.</param>
        /// <param name="progressBarSettings">An object containing settings that define the appearance and formatting of the progress bar, such as
        /// symbols, labels, and percent format.</param>
        /// <returns>A string representing the rendered progress bar, including any configured labels and percentage display.</returns>
        public static string RenderBar(int percentage, string color, ProgressBarSettings progressBarSettings)
        {
            percentage = Math.Min(percentage, 100);

            var symbols = "";
            var i = 5;

            var fillStr = progressBarSettings.FilledPart.GetValue();
            var lowerFillStr = progressBarSettings.LowerPart.GetValue();

            for (; i <= 95; i += 10)
            {
                if (i >= percentage)
                    break;

                symbols += fillStr;
            }

            if (i - 5 < percentage)
                symbols += lowerFillStr;

            var symbolCount = symbols.Length;
            var invisibleBar = false;

            if (symbols.Length < 10)
            {
                symbols += $"<alpha=#00>{fillStr}";
                invisibleBar = true;
                symbolCount++;
            }

            for (i = 0; i < 10 - symbolCount; i++)
                symbols += fillStr;

            if (invisibleBar)
                symbols += "<alpha=#FF>";

            var leftLabel = progressBarSettings.LeftLabel.GetValue();
            var rightLabel = progressBarSettings.RightLabel.GetValue();

            var percentFormat = progressBarSettings.PercentFormat.GetValue();
            var percent = progressBarSettings.ShowPercent
                ? " " + string.Format(percentFormat, Mathf.CeilToInt(percentage))
                : string.Empty;

            var left = string.IsNullOrEmpty(leftLabel) ? string.Empty : leftLabel + " ";
            var right = string.IsNullOrEmpty(rightLabel) ? string.Empty : " " + rightLabel;

            return $"{left}{symbols}{percent}{right}";
        }
    }
}