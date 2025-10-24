using mcx.Levels.API.Events;
using mcx.Levels.API.Storage;

namespace mcx.Levels.API
{
    /// <summary>
    /// Manages level progression.
    /// </summary>
    public static class LevelProgress
    {
        public static LevelsConfig Config => LevelsPlugin.StaticConfig;

        /// <summary>
        /// Determines the level corresponding to a given amount of experience.
        /// </summary>
        /// <remarks>The method calculates the level by comparing the accumulated experience against the
        /// required experience for each level, starting from level 1. The calculation continues until the experience is
        /// less than the required experience for the next level or the maximum level is reached.</remarks>
        /// <param name="experience">The total experience points accumulated by the user. Must be non-negative.</param>
        /// <returns>The level that corresponds to the specified experience points. Returns 1 if the experience is less than the
        /// starting level experience.</returns>
        public static int GetLevelForExperience(float experience)
        {
            if (experience < Config.StartLevelExperience)
                return 1;

            var level = 1;
            var required = Config.StartLevelExperience;

            while (experience >= required && (Config.MaxLevel < 1 || level < Config.MaxLevel))
            {
                level++;
                required = GetExperienceForLevel(level);
            }

            return level - 1;
        }

        /// <summary>
        /// Calculates the total experience required to reach a specified level.
        /// </summary>
        /// <remarks>The experience calculation is based on a starting experience value and a series of
        /// multipliers defined in the configuration. The method iteratively applies these multipliers for each level up
        /// to the specified level.</remarks>
        /// <param name="level">The target level for which to calculate the required experience.</param>
        /// <returns>The total experience required to reach the specified level.</returns>
        public static float GetExperienceForLevel(int level)
        {
            if (level <= 2)
                return Config.StartLevelExperience;

            var required = Config.StartLevelExperience;
            var multiplier = 1f;

            foreach (var pair in Config.Multipliers)
            {
                if (level < pair.Key)
                {
                    multiplier = pair.Key;
                }
            }

            for (var i = 2; i < level; i++)
            {
                required *= multiplier;
            }

            return required;
        }

        internal static void CheckProgress(string userId, string reason, SavedLevel level)
        {
            var newLevel = GetLevelForExperience(level.Experience);

            if (level.Level == newLevel)
                return;

            var changingLevelArgs = new ChangingLevelEventArgs(level, userId, reason, level.Level, newLevel);

            if (!LevelEvents.OnChangingLevel(changingLevelArgs))
                return;

            level.Level = newLevel;
            level.RequiredExperience = GetExperienceForLevel(newLevel + 1);

            LevelEvents.OnChangedLevel(new ChangedLevelEventArgs(level, userId, reason, changingLevelArgs.CurrentLevel, newLevel), changingLevelArgs.target);
        }
    }
}