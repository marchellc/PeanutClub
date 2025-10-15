using LabExtended.API;
using LabExtended.Utilities;

using System.ComponentModel;

namespace mcx.Utilities.Items
{
    /// <summary>
    /// Represents the configuration and logic for generating loot items, including user- and group-specific chance
    /// multipliers and available item groups.
    /// </summary>
    /// <remarks>Use this class to define loot drop behavior, including customizing drop chances for
    /// individual users or Remote Admin groups. The class provides methods to generate loot for players based on
    /// weighted random selection, taking into account the configured multipliers and item groups.</remarks>
    public class ItemLoot
    {
        /// <summary>
        /// Gets or sets the chance multipliers for specific users and item groups.
        /// </summary>
        [Description("Sets chance multipliers per user ID.")]
        public Dictionary<string, Dictionary<string, float>> UserChanceMultipliers { get; set; } = new()
        {
            ["UserIdOne"] = new()
            {
                ["ItemGroupNameOne"] = 0.2f,
            }
        };

        /// <summary>
        /// Gets or sets the chance multipliers for each Remote Admin group and item group combination.
        /// </summary>
        [Description("Sets chance multipliers per Remote Admin group key.")]
        public Dictionary<string, Dictionary<string, float>> GroupChanceMultipliers { get; set; } = new()
        {
            ["RaGroupNameOne"] = new()
            {
                ["ItemGroupNameOne"] = 0.2f,
            }
        };

        /// <summary>
        /// Gets or sets the collection of item groups available for loot, indexed by group name.
        /// </summary>
        [Description("Defines the item groups available for loot.")]
        public Dictionary<string, ItemGroup> ItemGroups { get; set; } = new()
        {
            ["ItemGroupOne"] = new(),
        };

        /// <summary>
        /// Generates loot items for the specified player and invokes a callback for each item awarded.
        /// </summary>
        /// <remarks>The method uses weighted random selection to determine which loot items are awarded,
        /// taking into account user- and group-specific chance multipliers if available. The <paramref
        /// name="lootAdder"/> callback is called once for each item awarded. If <paramref name="duplicateChecker"/> is
        /// provided, items for which it returns <see langword="true"/> are not awarded and do not count toward the
        /// total.</remarks>
        /// <param name="targetPlayer">The player who will receive the generated loot. Cannot be null and must have a valid reference hub.</param>
        /// <param name="itemCount">The number of loot items to generate and award to the player. Must be greater than zero.</param>
        /// <param name="lootAdder">A callback action that is invoked for each generated loot item. The string parameter represents the item
        /// identifier. Cannot be null.</param>
        /// <param name="duplicateChecker">An optional function used to check for duplicate items. If provided, it is called with each potential item
        /// identifier; if the function returns <see langword="true"/>, the item is skipped.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="targetPlayer"/> is null or does not have a valid reference hub, or if <paramref
        /// name="lootAdder"/> is null.</exception>
        public void GetLoot(ExPlayer targetPlayer, int itemCount, Action<string> lootAdder)
        {
            if (targetPlayer?.ReferenceHub == null)
                throw new ArgumentNullException(nameof(targetPlayer), "Target player cannot be null.");

            if (lootAdder is null)
                throw new ArgumentNullException(nameof(lootAdder), "Loot adder action cannot be null.");

            float WeightPicker(KeyValuePair<string, ItemGroup> pair)
            {
                var baseChance = pair.Value.Chance;

                if (UserChanceMultipliers.TryGetValue(targetPlayer.UserId, out var userMultipliers))
                {
                    if (userMultipliers.TryGetValue(pair.Key, out var userMultiplier))
                    {
                        baseChance *= userMultiplier;
                    }
                }

                if (!string.IsNullOrEmpty(targetPlayer.PermissionsGroupName))
                {
                    if (GroupChanceMultipliers.TryGetValue(targetPlayer.PermissionsGroupName!, out var groupMultipliers))
                    {
                        if (groupMultipliers.TryGetValue(pair.Key, out var groupMultiplier))
                        {
                            baseChance *= groupMultiplier;
                        }
                    }
                }

                return baseChance;
            }

            var pickedGroup = ItemGroups.GetRandomWeighted(WeightPicker);

            while (pickedGroup.Value is null)
                pickedGroup = ItemGroups.GetRandomWeighted(WeightPicker);

            while (itemCount > 0)
            {
                var pickedItem = pickedGroup.Value.Items.ElementAtOrDefault(UnityEngine.Random.Range(0, pickedGroup.Value.Items.Count));

                if (pickedItem.Value < 1 || pickedItem.Key is null)
                    continue;

                for (var i = 0; i < pickedItem.Value; i++)
                {
                    lootAdder(pickedItem.Key);

                    itemCount--;

                    if (itemCount < 1)
                        break;
                }
            }
        }
    }
}