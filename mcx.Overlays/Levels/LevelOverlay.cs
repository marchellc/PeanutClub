using LabExtended.API;
using LabExtended.API.Enums;
using LabExtended.API.Hints.Elements.Personal;

using LabExtended.Extensions;

using mcx.Levels.API.Storage;
using mcx.Overlays.Levels.Entries;

using UnityEngine;

namespace mcx.Overlays.Levels
{
    /// <summary>
    /// Shows information about level and experience gains.
    /// </summary>
    public class LevelOverlay : PersonalHintElement
    {
        private float levelTime;
        private float experienceTime;

        private string? progressBar;

        /// <summary>
        /// Gets the options for the level overlay.
        /// </summary>
        public static LevelSettings Settings => OverlayCore.ConfigStatic.LevelOverlay;

        /// <summary>
        /// Gets the saved level data.
        /// </summary>
        public SavedLevel Level { get; set; }

        /// <summary>
        /// Gets the currently displayed level entry.
        /// </summary>
        public LevelGainEntry? CurrentLevelEntry { get; private set; }

        /// <summary>
        /// Gets the currently displayed experience entry.
        /// </summary>
        public ExperienceGainEntry? CurrentExperienceEntry { get; private set; }

        /// <summary>
        /// Gets a list of level entries.
        /// </summary>
        public List<LevelGainEntry> LevelEntries { get; } = new();

        /// <summary>
        /// Gets a list of experience entries.
        /// </summary>
        public List<ExperienceGainEntry> ExperienceEntries { get; } = new();

        /// <inheritdoc/>
        public override HintAlign Alignment => Settings.Align;

        /// <inheritdoc/>
        public override float VerticalOffset => Settings.VerticalOffset;

        /// <inheritdoc/>
        public override int PixelSpacing => Settings.PixelSpacing < 0
            ? base.PixelSpacing
            : Settings.PixelSpacing;

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (CurrentLevelEntry.HasValue && Time.realtimeSinceStartup >= levelTime)
                CurrentLevelEntry = null;

            if (CurrentExperienceEntry.HasValue && Time.realtimeSinceStartup >= experienceTime)
                CurrentExperienceEntry = null;

            if (LevelEntries.Count > 0 && !CurrentLevelEntry.HasValue)
                CurrentLevelEntry = LevelEntries.RemoveAndTake(0);

            if (ExperienceEntries.Count > 0 && !CurrentExperienceEntry.HasValue)
                CurrentExperienceEntry = ExperienceEntries.RemoveAndTake(0);

            if (CurrentLevelEntry != null)
                levelTime = Time.realtimeSinceStartup + Settings.LevelGainDuration;

            if (CurrentExperienceEntry != null)
                experienceTime = Time.realtimeSinceStartup + Settings.ExperienceGainDuration;
        }

        /// <inheritdoc/>
        public override bool OnDraw(ExPlayer player)
        {
            if (Builder is null)
                return false;

            if (Level != null && progressBar != null)
            {
                Builder.Append(progressBar);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Refreshes the progress bar display.
        /// </summary>
        public void RefreshBar()
        {
            if (Level is null)
            {
                progressBar = null;
                return;
            }

            var percentage = Mathf.CeilToInt((Level.Experience / Level.RequiredExperience) * 100);

            progressBar =
                $"<size=45%><color={GetBarColor(percentage)}>{Mathf.CeilToInt(Level.Experience)} XP " +
                $"<b>|</b><size=45%>{RenderBar(percentage)}</size><b>|</b> {Mathf.CeilToInt(Level.RequiredExperience)} XP</color></size>";
        }

        private static string GetBarColor(int percentage)
        {
            if (percentage >= 85)
                return "#1dde37";
            else if (percentage >= 70)
                return "#9deb21";
            else if (percentage >= 50)
                return "#d6f233";
            else if (percentage >= 30)
                return "#f2dc33";
            else if (percentage >= 15)
                return "#f27933";
            else
                return "#eb220c";
        }

        private static string RenderBar(int percentage)
        {
            percentage = Math.Min(percentage, 100);

            var symbols = "";
            var i = 5;

            for (; i <= 95; i += 10)
            {
                if (i >= percentage)
                    break;

                symbols += "█";
            }

            if (i - 5 < percentage)
                symbols += "▄";

            var symbolCount = symbols.Length;
            var invisibleBar = false;

            if (symbols.Length < 10)
            {
                symbols += "<alpha=#00>█";
                invisibleBar = true;
                symbolCount++;
            }

            for (i = 0; i < 10 - symbolCount; i++)
                symbols += "█";

            if (invisibleBar)
                symbols += "<alpha=#FF>";

            return symbols;
        }
    }
}