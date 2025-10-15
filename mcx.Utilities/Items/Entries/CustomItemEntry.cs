using InventorySystem.Items;
using InventorySystem.Items.Pickups;

using LabExtended.API;
using LabExtended.API.Custom.Items;

using mcx.Utilities.Items.Interfaces;

using Mirror;

using UnityEngine;

namespace mcx.Utilities.Items.Entries
{
    /// <summary>
    /// Implements support for custom items.
    /// </summary>
    public class CustomItemEntry : IItemEntry
    {
        /// <summary>
        /// Gets the custom item.
        /// </summary>
        public CustomItem Item { get; }

        /// <inheritdoc/>
        public string Name => Item.Name;

        /// <inheritdoc/>
        public bool IsEffect { get; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemEntry"/> class with the specified item.
        /// </summary>
        /// <param name="item">The <see cref="CustomItem"/> instance to associate with this entry. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <see langword="null"/>.</exception>
        public CustomItemEntry(CustomItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Item = item;
        }

        /// <inheritdoc/>
        public ItemBase AddToInventory(ExPlayer player)
        {
            return Item.AddItem(player);
        }

        /// <inheritdoc/>
        public ItemPickupBase SpawnPickup(Vector3 position, Quaternion rotation, bool spawnPickup = true)
        {
            var pickup = Item.SpawnItem(position, rotation);

            if (!spawnPickup)
                NetworkServer.UnSpawn(pickup.gameObject);

            return pickup;
        }
    }
}