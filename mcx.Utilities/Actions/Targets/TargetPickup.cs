using InventorySystem.Items.Pickups;

using mcx.Utilities.Actions.Interfaces;

namespace mcx.Utilities.Actions.Targets
{
    /// <summary>
    /// Represents a targetable item pickup in the game world.
    /// </summary>
    public struct TargetPickup : IActionTarget
    {
        /// <summary>
        /// The targeted item pickup.
        /// </summary>
        public readonly ItemPickupBase Pickup;

        /// <summary>
        /// Initializes a new instance of the TargetPickup class with the specified item pickup.
        /// </summary>
        /// <param name="pickup">The item pickup to associate with this TargetPickup instance. Cannot be null.</param>
        public TargetPickup(ItemPickupBase pickup)
        {
            Pickup = pickup;
        }
    }
}