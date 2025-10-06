using mcx.RandomPickup.API.Scenarios.LowHealth;
using mcx.Utilities.Randomness;

using System.ComponentModel;

namespace mcx.RandomPickup
{
    /// <summary>
    /// Configuration file for the RandomPickup plugin.
    /// </summary>
    public class RandomPickupConfig
    {
        /// <summary>
        /// Gets or sets the name of the pickup schematic to use.
        /// </summary>
        [Description("Sets the name of the pickup schematic.")]
        public string SchematicName { get; set; } = "RandomPickup";

        /// <summary>
        /// Gets or sets the minimum number of rounds that must pass between each scenario activation for a player.
        /// </summary>
        [Description("Sets the minimum number of rounds between each scenario activation (per player).")]
        public int MinimumScenarioRoundDelay { get; set; } = 2;

        /// <summary>
        /// Gets or sets the minimum number of rounds that must pass before a player is eligible to receive a random
        /// pickup again.
        /// </summary>
        [Description("Sets how many rounds must pass before a player can have a chance to get a random pickup again.")]
        public int MinimumRoundCount { get; set; } = 2;

        /// <summary>
        /// Gets or sets a value indicating whether SCPs are eligible to receive random pickups.
        /// </summary>
        [Description("Whether or not SCPs can receive random pickups.")]
        public bool TargetScps { get; set; }

        /// <summary>
        /// Gets or sets the range specifying the minimum and maximum number of random pickups that can spawn per round.
        /// </summary>
        [Description("Sets the maximum number of random pickups that can spawn per round.")]
        public Int32Range SpawnCount { get; set; } = new()
        {
            MinValue = 1,
            MaxValue = 10
        };

        /// <summary>
        /// Gets or sets the range of time, in seconds, to wait between each spawn event.
        /// </summary>
        [Description("Sets the delay range between each spawn.")]
        public FloatRange SpawnDelay { get; set; } = new()
        {
            MinValue = 30f,
            MaxValue = 300
        };

        public LowHealthScenarioConfig LowHealthScenario { get; set; } = new();
    }
}