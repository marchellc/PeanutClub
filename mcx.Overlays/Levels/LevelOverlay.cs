using LabExtended.API;
using LabExtended.API.Enums;
using LabExtended.API.Hints.Elements.Personal;

using LabExtended.Extensions;

using mcx.Levels.API.Storage;

using mcx.Overlays.Elements;
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

        private float lastChangeTime = -1f;
        private string? progressBar;

        /// <summary>
        /// Gets the options for the level overlay.
        /// </summary>
        public static LevelSettings Settings => OverlayCore.ConfigStatic.LevelOverlay;

        /// <summary>
        /// Gets the options for the progress bar.
        /// </summary>
        public static ProgressBarSettings BarSettings => Settings.ProgressBar;

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

            if (lastChangeTime != Level.LastChangeTime)
            {
                progressBar = ProgressBarElement.RenderBarFraction(
                    (int)Level.Experience,
                    (int)Level.RequiredExperience,
                    
                    BarSettings);

                progressBar = progressBar
                    .Replace("$ExpRequired", Level.RequiredExperience.ToString())
                    .Replace("$ExpCurrent", Level.Experience.ToString())
                    .Replace("$LevelCurrent", Level.Level.ToString());

                if (Settings.Size > 0)
                    progressBar = $"<size={Settings.Size}>{progressBar}</size>";

                lastChangeTime = Level.LastChangeTime;
            }
        }

        /// <inheritdoc/>
        public override bool OnDraw(ExPlayer player)
        {
            if (Builder is null)
                return false;

            if (progressBar != null)
            {
                Builder.Append(progressBar);
                return true;
            }

            return false;
        }
    }
}