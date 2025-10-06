using LabExtended.Core.Configs.Objects;

using System.ComponentModel;

using UnityEngine;

namespace mcx.Items.Weapons.ItemLauncher
{
    /// <summary>
    /// Properties of an item launcher.
    /// </summary>
    public class ItemLauncherProperties
    {
        /// <summary>
        /// Gets or sets the identifier of the item to be launched. The value can be a standard item type or a custom
        /// item ID.
        /// </summary>
        [Description("Sets the item that will be launched.")]
        public ItemType LaunchedItem { get; set; } = ItemType.GrenadeHE;

        /// <summary>
        /// Gets or sets the force with which the item will be launched.
        /// </summary>
        [Description("Sets the force with which the item will be launched. Default is 3.")]
        public float Force { get; set; } = 3f;

        /// <summary>
        /// Gets or sets the fuse time, in seconds, for throwable projectiles after they are launched.
        /// </summary>
        [Description("Sets the fuse time for throwable projectiles when launched.")]
        public float FuseTime { get; set; } = 3f;

        /// <summary>
        /// Gets or sets the number of items to launch per shot.
        /// </summary>
        [Description("Sets how many items to launch per shot.")]
        public int Amount { get; set; } = 1;

        /// <summary>
        /// Gets or sets the scale applied to launched items.
        /// </summary>
        [Description("Sets the scale of launched items.")]
        public YamlVector3 Scale { get; set; } = new(Vector3.one);
    }
}