using InventorySystem.Items;
using InventorySystem.Items.Pickups;

using LabExtended.API;

using mcx.Utilities.Items.Interfaces;

using UnityEngine;

namespace mcx.Utilities.Items.Entries
{
    /// <summary>
    /// Implements support for base-game items.
    /// </summary>
    public abstract class EffectItemEntry : IItemEntry
    {
        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public bool IsEffect { get; } = true;

        /// <summary>
        /// Applies the effect of this item to the given player.
        /// </summary>
        /// <param name="player">The player.</param>
        public abstract void ApplyEffect(ExPlayer player, string[]? effectString);

        /// <inheritdoc/>
        public ItemBase AddToInventory(ExPlayer player)
        {
            ApplyEffect(player, null);
            return null!;
        }

        /// <inheritdoc/>
        public ItemPickupBase SpawnPickup(Vector3 position, Quaternion rotation, bool spawnPickup = true)
        {
            throw new NotImplementedException();
        }
    }
}