using System.ComponentModel;

namespace mcx.Utilities.Items
{
    /// <summary>
    /// Represents a group of items.
    /// </summary>
    public class ItemGroup
    {
        /// <summary>
        /// Gets or sets the relative likelihood that this group will be selected compared to others.
        /// </summary>
        [Description("Sets the chance of this group being picked over others.")]
        public float Chance { get; set; } = 0;

        /// <summary>
        /// Gets or sets the collection of items in the group and their associated selection weights.
        /// </summary>
        /// <remarks>Each entry in the dictionary represents an item and its relative chance of being
        /// selected. Higher values increase the likelihood of an item being picked compared to others in the group. The
        /// weights do not need to sum to any particular value.</remarks>
        [Description("Sets the items in this group and their relative item count.")]
        public Dictionary<string, int> Items { get; set; } = new()
        {
            ["Medkit"] = 1,
            ["Adrenaline"] = 1
        };
    }
}