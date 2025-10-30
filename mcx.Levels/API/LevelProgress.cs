using mcx.Levels.API.Events;
using mcx.Levels.API.Storage;

namespace mcx.Levels.API
{
    /// <summary>
    /// Manages level progression.
    /// </summary>
    public static class LevelProgress
    {
        /// <summary>
        /// Gets the active level config.
        /// </summary>
        public static LevelsConfig Config => LevelsPlugin.StaticConfig;

        /// <summary>
        /// Calculates the level corresponding to the specified amount of experience points.
        /// </summary>
        /// <param name="experience">The total experience points to evaluate. Must be greater than or equal to 0.</param>
        /// <returns>The level that matches the given experience points. Returns 1 if the experience is less than the requirement
        /// for the next level.</returns>
        public static int GetLevelForExperience(float experience)
        {
            var level = 1;

            while (experience > 0f)
            {
                var requiredExp = GetExperienceForLevel(level + 1);

                if (experience < requiredExp)
                    break;

                experience -= requiredExp;

                level++;
            }

            return level;
        }

        /// <summary>
        /// Calculates the total experience required to reach the specified level.
        /// </summary>
        /// <remarks>The experience required for each level is determined by the base step value and any
        /// configured step offsets. The calculation is cumulative, summing the experience needed for each level up to,
        /// but not including, the specified level.</remarks>
        /// <param name="level">The target level for which to calculate the cumulative experience. Must be greater than or equal to 1.</param>
        /// <returns>The total experience points required to reach the specified level. Returns 0 if the level is 1.</returns>
        public static float GetExperienceForLevel(int level)
        {
            var exp = 0f;
            var step = Config.LevelStep;

            for (var i = 1; i < level; i++)
            {
                if (Config.StepOffsets.TryGetValue(i, out var offset))
                    step += offset;

                exp += step;
            }

            return exp;
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