using LabExtended.API;

namespace mcx.RandomPickup.API
{
    /// <summary>
    /// Base class for scenario-activated random pickups.
    /// </summary>
    public abstract class RandomPickupScenario
    {
        /// <summary>
        /// A list of all registered scenarios.
        /// </summary>
        public static List<RandomPickupScenario> AllScenarios { get; } = new()
        {

        };

        /// <summary>
        /// A list of players for which this scenario has been activated along with the number of the round it was activated in.
        /// </summary>
        public Dictionary<string, int> ActivatedPlayers { get; } = new();

        /// <summary>
        /// Determines whether the specified player meets the conditions to proceed in the scenario and outputs related
        /// scenario data.
        /// </summary>
        /// <param name="player">The player to evaluate for scenario processing. Cannot be null.</param>
        /// <param name="scenarioData">When this method returns, contains scenario-specific data if the player is eligible; otherwise, null. This
        /// parameter is passed uninitialized.</param>
        /// <returns>true if the player meets the health and chance requirements to proceed; otherwise, false.</returns>
        public abstract bool ProcessPlayer(ExPlayer player, out object scenarioData);

        internal static void Internal_UpdateScenarios()
        {

        }
    }
}