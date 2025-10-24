using LabExtended.API;
using LabExtended.Extensions;

using mcx.Levels.API.Events;
using mcx.Levels.API.Storage;

namespace mcx.Levels.API
{
    /// <summary>
    /// Provides events related to player leveling actions, including leveling up and having leveled up.
    /// </summary>
    /// <remarks>This static class contains events that are triggered during the leveling process of a player.
    /// The <see cref="LevelingUp"/> event is invoked before a player levels up, allowing subscribers to handle or
    /// cancel the leveling process. The <see cref="LeveledUp"/> event is invoked after a player has successfully
    /// leveled up, allowing subscribers to perform actions post-leveling.</remarks>
    public static class LevelEvents
    {
        /// <summary>
        /// Gets called when a player's level data is loaded.
        /// </summary>
        public static event Action<ExPlayer, SavedLevel>? Loaded;

        /// <inheritdoc cref="ChangingLevelEventArgs"/>
        public static event Action<ChangingLevelEventArgs>? ChangingLevel;

        /// <inheritdoc cref="ChangedLevelEventArgs"/>
        public static event Action<ChangedLevelEventArgs>? ChangedLevel;

        /// <inheritdoc cref="ChangingExperienceEventArgs"/>
        public static event Action<ChangingExperienceEventArgs>? ChangingExperience;

        /// <inheritdoc cref="ChangedExperienceEventArgs"/>
        public static event Action<ChangedExperienceEventArgs>? ChangedExperience;

        /// <summary>
        /// Invokes the <see cref="Loaded"/> event.
        /// </summary>
        public static void OnLoaded(ExPlayer player, SavedLevel levelData)
            => Loaded?.InvokeSafe(player, levelData);

        /// <summary>
        /// Invokes the <see cref="ChangingLevel"/> event.
        /// </summary>
        public static bool OnChangingLevel(ChangingLevelEventArgs args)
            => ChangingLevel.InvokeBooleanEvent(args);

        /// <summary>
        /// Invokes the <see cref="ChangedLevel"/> event.
        /// </summary>
        public static void OnChangedLevel(ChangedLevelEventArgs args, ExPlayer? target = null)
        {
            args.target = target;

            ChangedLevel.InvokeEvent(args);
        }

        /// <summary>
        /// Invokes the <see cref="ChangingExperience"/> event.
        /// </summary>
        public static bool OnChangingExperience(ChangingExperienceEventArgs args)
            => ChangingExperience.InvokeBooleanEvent(args);

        /// <summary>
        /// Invokes the <see cref="ChangedExperience"/> event.
        /// </summary>
        public static void OnChangedExperience(ChangedExperienceEventArgs args, ExPlayer? target = null)
        {
            args.target = target;

            ChangedExperience.InvokeEvent(args);
        }
    }
}