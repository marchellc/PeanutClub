using LabExtended.API;

using mcx.Utilities.Items.Entries;

using System.ComponentModel;

namespace mcx.Utilities.Items.Loot
{
    public class LootGroup
    {
        /// <summary>
        /// Gets or sets the base weight of this loot group.
        /// </summary>
        [Description("Sets the base weight of this loot group. Should be a value between 0 and 100.")]
        public float Weight { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the weight multipliers for the object requesting loot.
        /// </summary>
        /// <remarks>The keys in the dictionary can vary depending on the context. For player-related
        /// requests, the keys may include user-specific identifiers such as user ID, user IP, or their remote admin
        /// group key. Plugins may define additional cases for other types of requesting objects.</remarks>
        [Description(
            "Sets the weight multipliers for the object requesting loot.\n" +
            "# In case a player is requesting, can be user ID, user IP or their remote admin group key.\n" +
            "# Plugins should define other cases.")]
        public Dictionary<string, float> Multipliers { get; set; } = new();

        /// <summary>
        /// Gets or sets the loot table, where each entry specifies an item and the quantity to add.
        /// </summary>
        /// <remarks>The loot table is used to configure the items and their quantities for a specific
        /// context.  Ensure that the list contains valid <see cref="LootItem"/> objects to avoid unexpected
        /// behavior.</remarks>
        [Description("Sets  the loot table. The key is the item's name, the value is the amount of items to add.")]
        public List<LootItem> Loot { get; set; } = new() { new() };

        /// <summary>
        /// Applies a group of loot items to the specified player, performing actions such as adding items to the
        /// inventory, spawning pickups, or applying effects based on the loot configuration.
        /// </summary>
        /// <remarks>This method iterates through the loot group and performs the appropriate action for
        /// each loot item: - If the loot item represents an effect, the effect is applied to the player. - If the loot
        /// item is not an effect, it is either added to the player's inventory or spawned as a pickup at the player's
        /// position. Loot items with a quantity less than 1 are ignored.</remarks>
        /// <param name="target">The player to whom the loot group will be applied. Must not be <see langword="null"/> and must have a valid
        /// <see cref="ReferenceHub"/>.</param>
        public void ApplyGroup(ExPlayer target, bool dropItems = false)
        {
            if (target?.ReferenceHub == null)
                return;

            Loot.ForEach(loot =>
            {
                if (loot.ItemEntry is null)
                    return;

                if (loot.Amount < 1)
                    return;

                if (loot.ItemEntry.IsEffect)
                {
                    if (loot.ItemEntry is EffectItemEntry effectItemEntry)
                    {
                        effectItemEntry.ApplyEffect(target, loot.EffectString);
                    }
                    else
                    {
                        loot.ItemEntry.SpawnPickup(target.Position, target.Rotation, true);
                    }
                }
                else
                {
                    if (dropItems)
                    {
                        loot.ItemEntry.SpawnPickup(target.Position, target.Rotation, true);
                    }
                    else
                    {
                        loot.ItemEntry.AddToInventory(target);
                    }
                }
            });
        }
    }
}