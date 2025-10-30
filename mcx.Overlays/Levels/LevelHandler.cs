using LabExtended.API;
using LabExtended.API.Hints;

using mcx.Levels.API;
using mcx.Levels.API.Events;
using mcx.Levels.API.Storage;

namespace mcx.Overlays.Levels
{
    /// <summary>
    /// Handles level events.
    /// </summary>
    public static class LevelHandler
    {
        private static void OnLoaded(ExPlayer player, SavedLevel level)
        {
            if (player.TryGetHintElement<LevelOverlay>(out var curOverlay))
            {
                curOverlay.Level = level;
                curOverlay.RefreshBar();

                return;
            }

            var overlay = new LevelOverlay() { Level = level };

            if (!player.AddHintElement(overlay))
                return;

            overlay.RefreshBar();
        }

        private static void OnRemoved(ExPlayer player, SavedLevel level)
        {
            if (player.TryGetHintElement<LevelOverlay>(out var overlay))
            {
                overlay.Level = null!;
                overlay.RefreshBar();
            }
        }

        private static void OnChangedLevel(ChangedLevelEventArgs args)
        {
            if (args.Target is null
                || !args.Target.TryGetHintElement<LevelOverlay>(out var levelOverlay))
                return;

            var amount = 0;

            if (args.NewLevel > args.PreviousLevel)
                amount = args.NewLevel - args.PreviousLevel;
            else if (args.NewLevel < args.PreviousLevel)
                amount = args.PreviousLevel - args.NewLevel;

            levelOverlay.Level = args.Level;

            levelOverlay.RefreshBar();
            levelOverlay.LevelEntries.Add(new(amount, args.NewLevel > args.PreviousLevel));
        }

        private static void OnChangedExperience(ChangedExperienceEventArgs args)
        {
            if (args.Target is null
                || !args.Target.TryGetHintElement<LevelOverlay>(out var levelOverlay))
                return;

            var amount = 0f;

            if (args.NewExp > args.PreviousExp)
                amount = args.NewExp - args.PreviousExp;
            else if (args.NewExp < args.PreviousExp)
                amount = args.PreviousExp - args.NewExp;

            levelOverlay.Level = args.Level;

            levelOverlay.RefreshBar();
            levelOverlay.ExperienceEntries.Add(new(amount, args.NewExp > args.PreviousExp));
        }

        internal static void Initialize()
        {
            if (!OverlayCore.ConfigStatic.LevelOverlay.IsEnabled)
                return;

            LevelEvents.Loaded += OnLoaded;
            LevelEvents.Removed += OnRemoved;

            LevelEvents.ChangedLevel += OnChangedLevel;
            LevelEvents.ChangedExperience += OnChangedExperience;
        }
    }
}