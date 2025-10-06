using mcx.Utilities.Items;
using mcx.Utilities.Randomness;
using System.ComponentModel;

namespace mcx.RandomPickup.API.Scenarios.LowHealth
{
    /// <summary>
    /// Represents the configuration settings for a low health scenario.
    /// </summary>
    public class LowHealthScenarioConfig
    {
        /// <summary>
        /// Gets or sets the base chance, as a percentage, for the low health scenario to trigger.
        /// </summary>
        [Description("Sets the base chance (in percentage) for the low health scenario to trigger.")]
        public int BaseChance { get; set; } = 10;

        /// <summary>
        /// Gets or sets the minimum health percentage required before the entity is considered to have low health.
        /// </summary>
        /// <remarks>Set this value to define the threshold below which the entity is treated as being in
        /// a low health state. This can be used to trigger warnings or special behaviors when health drops below the
        /// specified percentage.</remarks>
        [Description("Sets the health percentage required to be considered low health.")]
        public int HealthPercentageThreshold { get; set; } = 3;

        /// <summary>
        /// Gets or sets the range specifying the minimum and maximum number of items awarded to a player.
        /// </summary>
        [Description("Sets the minimum and maximum amount of items given to a player.")]
        public Int32Range ItemCount { get; set; } = new()
        {
            MinValue = 1,
            MaxValue = 3
        };

        /// <summary>
        /// Gets or sets the loot to be awarded when a low health scenario is triggered.
        /// </summary>
        [Description("Sets the loot to be given when the low health scenario triggers.")]
        public ItemLoot LowHealthLoot { get; set; } = new();
    }
}