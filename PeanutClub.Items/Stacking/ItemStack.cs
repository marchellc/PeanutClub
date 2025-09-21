﻿using InventorySystem.Items;

namespace PeanutClub.Items.Stacking
{
    /// <summary>
    /// Represents a stack of items for a player.
    /// </summary>
    public class ItemStack
    {
        /// <summary>
        /// A list of all stacked items categorized by their item type.
        /// </summary>
        public Dictionary<ItemType, List<ItemBase>> StackedItems { get; } = new();
    }
}