using LabExtended.API;

using mcx.Levels.API.Storage;

namespace mcx.Levels.API.Events
{
    /// <summary>
    /// Gets called after a player's level is increased.
    /// </summary>
    public class ChangedLevelEventArgs : EventArgs
    {
        private bool targetChecked;

        internal ExPlayer? target;

        /// <summary>
        /// Gets the saved level data of the player.
        /// </summary>
        public SavedLevel Level { get; }

        /// <summary>
        /// Gets the user ID of the player who has leveled up.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the reason for the level change.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the previous level of the player before leveling up.
        /// </summary>
        public int PreviousLevel { get; }

        /// <summary>
        /// Gets the new level of the player after leveling up.
        /// </summary>
        public int NewLevel { get; }

        /// <summary>
        /// Gets the target player associated with the current user.
        /// </summary>
        /// <remarks>The target player is retrieved based on the user's ID. Once checked, the target is
        /// cached for subsequent accesses.</remarks>
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
        /// <param name="prevLevel">The user's level before leveling up. Must be a non-negative integer.</param>
        /// <param name="newLevel">The user's level after leveling up. Must be greater than <paramref name="currentLevel"/>.</param>
        public ChangedLevelEventArgs(SavedLevel level, string userId, string reason, int prevLevel, int newLevel)
        {
            Level = level;
            UserId = userId;
            Reason = reason;
            PreviousLevel = prevLevel;
            NewLevel = newLevel;
        }
    }
}