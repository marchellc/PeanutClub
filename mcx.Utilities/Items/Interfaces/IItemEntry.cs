using InventorySystem.Items;
using InventorySystem.Items.Pickups;

using LabExtended.API;

using UnityEngine;

namespace mcx.Utilities.Items.Interfaces
{
    public interface IItemEntry
    {
        /// <summary>
        /// Gets the item's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether or not the item is an effect.
        /// </summary>
        bool IsEffect { get; }

        /// <summary>
        /// Adds an item to the specified player's inventory.
        /// </summary>
        /// <param name="player">The player to whom the item will be added. Cannot be <see langword="null"/>.</param>
        /// <returns>The item that was added to the player's inventory.</returns>
        ItemBase AddToInventory(ExPlayer player);

        /// <summary>
        /// Spawns a new item pickup at the specified position and rotation.
        /// </summary>
        /// <remarks>The method creates a new item pickup at the given location and orientation. Ensure
        /// that the position and rotation are valid within the context of the game world to avoid unexpected
        /// behavior.</remarks>
        /// <param name="position">The position in world space where the item pickup will be spawned.</param>
        /// <param name="rotation">The rotation to apply to the item pickup when it is spawned.</param>
        /// <returns>An instance of <see cref="ItemPickupBase"/> representing the spawned item pickup.</returns>
        ItemPickupBase SpawnPickup(Vector3 position, Quaternion rotation, bool spawnPickup = true);
    }
}