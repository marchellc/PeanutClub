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
            player.AddHintElement(new LevelOverlay() { Level = level });
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

            levelOverlay.ExperienceEntries.Add(new(amount, args.NewExp > args.PreviousExp));
        }

        internal static void Initialize()
        {
            LevelEvents.Loaded += OnLoaded;

            LevelEvents.ChangedLevel += OnChangedLevel;
            LevelEvents.ChangedExperience += OnChangedExperience;
        }
    }
}