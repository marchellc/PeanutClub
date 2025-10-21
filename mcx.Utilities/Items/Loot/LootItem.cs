using mcx.Utilities.Items.Interfaces;

using System.ComponentModel;

using YamlDotNet.Serialization;

namespace mcx.Utilities.Items.Loot
{
    /// <summary>
    /// Represents an item in a loot table.
    /// </summary>
    public class LootItem
    {
        private bool checkedForEntry = false;
        private string[]? entryEffectString = null;
        private IItemEntry? itemEntry = null;

        /// <summary>
        /// Gets or sets the type of the item to be given as loot.
        /// </summary>
        [Description("Sets the type of the item to be given as loot.")]
        public string Item { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the amount of the item to be given as loot.
        /// </summary>
        [Description("Sets the amount of the item to be given as loot.")]
        public int Amount { get; set; } = 1;

        /// <summary>
        /// Gets the item entry associated with this loot item.
        /// </summary>
        [YamlIgnore]
        public IItemEntry? ItemEntry
        {
            get
            {
                if (!checkedForEntry)
                {
                    ItemHandler.TryGetItemFromString(Item, out entryEffectString, out itemEntry);

                    checkedForEntry = true;
                }

                return itemEntry;
            }
        }

        /// <summary>
        /// Gets the effect string associated with this loot item.
        /// </summary>
        [YamlIgnore]
        public string[]? EffectString
        {
            get
            {
                if (!checkedForEntry)
                {
                    ItemHandler.TryGetItemFromString(Item, out entryEffectString, out itemEntry);

                    checkedForEntry = true;
                }

                return entryEffectString;
            }
        }
    }
}