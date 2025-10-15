using LabExtended.API;
using LabExtended.Utilities;

namespace mcx.RandomPickup.API.Scenarios.LowHealth
{
    /// <summary>
    /// Represents a scenario that triggers when a player's health falls below a specified threshold, awarding loot
    /// based on low health conditions.
    /// </summary>
    /// <remarks>This scenario is typically used to provide players with additional resources or rewards when
    /// their health is critically low. The configuration for this scenario, including health thresholds and loot
    /// settings, is accessible via the Config property.</remarks>
    public class LowHealthScenario : RandomPickupScenario
    {
        /// <summary>
        /// Gets the config instance of this scenario.
        /// </summary>
        public LowHealthScenarioConfig Config => RandomPickupCore.ConfigStatic.LowHealthScenario;

        /// <inheritdoc/>
        public override bool ProcessPlayer(ExPlayer player, out object scenarioData)
        {
            scenarioData = null!;

            if (Config.BaseChance < 1)
                return false;

            var healthPercent = (double)player.Stats.CurHealth / player.Stats.MaxHealth * 100;

            if (healthPercent < Config.HealthPercentageThreshold)
                return false;

            if (!WeightUtils.GetBool(Config.BaseChance))
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override void FillLoot(ExPlayer player, object scenarioData, List<string> loot)
        {
            Config.LowHealthLoot.GetLoot(player, Config.ItemCount.GetRandom(), loot.Add);
        }
    }
}