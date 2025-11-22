using LabExtended.API;
using LabExtended.Events;

using SecretLabAPI.Levels.Storage;

namespace SecretLabAPI.Levels.Events
{
    /// <summary>
    /// Gets called before a player's experience is increased.
    /// </summary>
    public class ChangingExperienceEventArgs : BooleanEventArgs
    {
        private bool targetChecked;
        internal ExPlayer? target;

        /// <summary>
        /// Gets the saved level data of the player.
        /// </summary>
        public SavedLevel Level { get; }

        /// <summary>
        /// Gets the user ID of the player who is gaining experience.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets the reason for the experience gain.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the current experience of the player before gaining.
        /// </summary>
        public int CurrentExp { get; }

        /// <summary>
        /// Gets the new experience of the player after gaining.
        /// </summary>
        public int NewExp { get; }

        /// <summary>
        /// Gets the target player associated with the current user.
        /// </summary>
        /// <remarks>The target player is retrieved based on the current user's ID. Once checked, the
        /// target is cached for subsequent accesses.</remarks>
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
        /// Provides data for the event that occurs when a user's experience points are changing.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose experience is changing. Cannot be null or empty.</param>
        /// <param name="currentExp">The current experience points of the user before the change.</param>
        /// <param name="newExp">The new experience points of the user after the change.</param>
        public ChangingExperienceEventArgs(SavedLevel level, string userId, string reason, int currentExp, int newExp)
        {
            Level = level;
            UserId = userId;
            Reason = reason;
            CurrentExp = currentExp;
            NewExp = newExp;
        }
    }
}