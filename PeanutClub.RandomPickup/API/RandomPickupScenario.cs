using InventorySystem.Items.Pickups;

using LabExtended.API;

using PeanutClub.RandomPickup.API.Scenarios.LowHealth;

namespace PeanutClub.RandomPickup.API
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
            new LowHealthScenario()
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

        /// <summary>
        /// Populates the specified loot collection with items appropriate for the given player.
        /// </summary>
        /// <param name="player">The player for whom the loot is being generated. Cannot be null.</param>
        /// <param name="loot">The collection to which generated loot items will be added. Must not be null.</param>
        public abstract void FillLoot(ExPlayer player, object scenarioData, List<ItemPickupBase> loot);

        internal static void Internal_UpdateScenarios()
        {
            var anyActivated = false;

            for (var i = 0; i < ExPlayer.Players.Count; i++)
            {
                if (anyActivated)
                    return;

                var player = ExPlayer.Players[i];

                for (var x = 0; x < AllScenarios.Count; x++)
                {
                    if (anyActivated)
                        return;

                    var scenario = AllScenarios[x];

                    if (scenario.ActivatedPlayers.TryGetValue(player.UserId, out var activationRound)
                        && (ExRound.RoundNumber - activationRound) < RandomPickupCore.ConfigStatic.MinimumScenarioRoundDelay)
                        continue;

                    if (scenario.ProcessPlayer(player, out var scenarioData))
                    {
                        scenario.ActivatedPlayers[player.UserId] = ExRound.RoundNumber;

                        RandomPickupSpawner.TimerPaused = true;

                        var pickup = RandomPickupSpawner.SpawnInstance(player.Position, player.Rotation, player, scenario);

                        scenario.FillLoot(player, scenarioData, pickup.Loot);

                        anyActivated = true;
                        return;
                    }
                }
            }
        }
    }
}