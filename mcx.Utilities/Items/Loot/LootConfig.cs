using LabExtended.API;
using LabExtended.Utilities;

using System.ComponentModel;

namespace mcx.Utilities.Items.Loot
{
    public class LootConfig
    {
        /// <summary>
        /// Gets or sets the list of loot groups associated with this loot table.
        /// </summary>
        [Description("Sets the list of loot groups for this loot table.")]
        public List<LootGroup> Groups { get; set; } = new() { new() };

        /// <summary>
        /// Retrieves a loot group for the specified player based on weighted probabilities.
        /// </summary>
        /// <remarks>If multiple loot groups are available, the selection is determined using weighted
        /// probabilities.  The weight of each group may be influenced by the player's unique attributes, such as their
        /// user ID,  IP address, or permissions group name. If only one loot group exists, it is returned
        /// directly.</remarks>
        /// <param name="player">The player for whom the loot group is being determined. Cannot be <see langword="null"/>.</param>
        /// <returns>The selected <see cref="LootGroup"/> based on the player's attributes and group weights,  or <see
        /// langword="null"/> if no groups are available or the player is invalid.</returns>
        public LootGroup? GetGroup(ExPlayer player)
        {
            if (player?.ReferenceHub == null)
                return null;

            if (Groups is null || Groups.Count == 0)
                return null;

            if (Groups.Count == 1)
                return Groups[0];

            return Groups.GetRandomWeighted(group =>
            {
                var weight = group.Weight;

                if (weight > 100f)
                    weight = 100f;

                if (weight == 100f)
                    return weight;

                if (weight > 0f)
                {
                    var multiplier = 1f;

                    if (group.Multipliers.TryGetValue(player.UserId, out var userIdMultiplier))
                        multiplier += userIdMultiplier;

                    if (group.Multipliers.TryGetValue(player.IpAddress, out var ipMultiplier))
                        multiplier += ipMultiplier;

                    if (!string.IsNullOrEmpty(player.PermissionsGroupName)
                        && group.Multipliers.TryGetValue(player.PermissionsGroupName!, out var groupMultiplier))
                        multiplier += groupMultiplier;

                    if (multiplier != 1f)
                        weight *= multiplier;
                }

                return weight;
            });
        }
    }
}