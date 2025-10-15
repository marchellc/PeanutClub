using InventorySystem.Items;
using InventorySystem.Items.Pickups;

using LabExtended.API;
using LabExtended.Extensions;

using mcx.Utilities.Items.Interfaces;

using UnityEngine;

namespace mcx.Utilities.Items.Entries
{
    /// <summary>
    /// Implements support for base-game items.
    /// </summary>
    public class VanillaItemEntry : IItemEntry
    {
        /// <summary>
        /// Gets the template of the inventory item.
        /// </summary>
        public ItemBase Template { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public bool IsEffect { get; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VanillaItemEntry"/> class using the specified item template.
        /// </summary>
        /// <param name="template">The item template used to initialize this entry. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/> is <see langword="null"/>.</exception>
        public VanillaItemEntry(ItemBase template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            Template = template;
            Name = template.ItemTypeId.ToString().SpaceByUpperCase();
        }

        /// <inheritdoc/>
        public ItemBase AddToInventory(ExPlayer player)
        {
            return player.Inventory.AddItem(Template.ItemTypeId, ItemAddReason.AdminCommand);
        }

        /// <inheritdoc/>
        public ItemPickupBase SpawnPickup(Vector3 position, Quaternion rotation, bool spawnPickup)
        {
            return ExMap.SpawnItem(Template.ItemTypeId, position, Vector3.one, rotation, null, spawnPickup);
        }
    }
}