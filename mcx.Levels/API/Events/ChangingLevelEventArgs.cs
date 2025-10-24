using LabExtended.API;
using LabExtended.Events;

using mcx.Levels.API.Storage;

namespace mcx.Levels.API.Events
{
    /// <summary>
    /// Gets called before a player's level is increased.
    /// </summary>
    public class ChangingLevelEventArgs : BooleanEventArgs
    {
        private bool targetChecked;
        internal ExPlayer? target;

        /// <summary>
        /// Gets the saved level data of the player.
        /// </summary>
        public SavedLevel Level { get; }

        /// <summary>
        /// Gets the user ID of the player who is leveling up.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the reason for the level change.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the current level of the player before leveling up.
        /// </summary>
        public int CurrentLevel { get; }

        /// <summary>
        /// Gets the new level of the player after leveling up.
        /// </summary>
        public int NewLevel { get; }

        public ExPlayer? Target
        {
            get
            {
                if (!targetChecked)
                {
                    target = ExPlayer.GetByUserId(UserId);
                    targetChecked = true;
                }

                return target;
            }
        }

        /// <summary>
        /// Provides data for the event that occurs when a user levels up.
        /// </summary>
        /// <param name="userId">The unique identifier of the user who has leveled up. Cannot be null or empty.</param>
        /// <param name="currentLevel">The user's level before leveling up. Must be a non-negative integer.</param>
        /// <param name="newLevel">The user's level after leveling up. Must be greater than <paramref name="currentLevel"/>.</param>
        public ChangingLevelEventArgs(SavedLevel level, string userId, string reason, int currentLevel, int newLevel)
        {
            Level = level;
            UserId = userId;
            Reason = reason;
            CurrentLevel = currentLevel;
            NewLevel = newLevel;
        }
    }
}