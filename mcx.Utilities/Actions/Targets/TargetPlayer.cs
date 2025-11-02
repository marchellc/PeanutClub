using LabExtended.API;

using mcx.Utilities.Actions.Interfaces;

namespace mcx.Utilities.Actions.Targets
{
    /// <summary>
    /// Represents a target that refers to a specific player for use in action targeting operations.
    /// </summary>
    public struct TargetPlayer : IActionTarget
    {
        /// <summary>
        /// The targeted player.
        /// </summary>
        public readonly ExPlayer Player;

        /// <summary>
        /// Initializes a new instance of the TargetPlayer class with the specified player.
        /// </summary>
        /// <param name="player">The player to associate with this TargetPlayer instance. Cannot be null.</param>
        public TargetPlayer(ExPlayer player)
        {
            Player = player;
        }
    }
}