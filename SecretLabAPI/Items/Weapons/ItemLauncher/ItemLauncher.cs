using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.ThrowableProjectiles;

using LabExtended.API;
using LabExtended.API.Custom.Items;

using LabExtended.Events.Player;

using System.ComponentModel;

using UnityEngine;

namespace SecretLabAPI.Items.Weapons.ItemLauncher
{
    /// <summary>
    /// A firearm that launches items.
    /// </summary>
    public class ItemLauncher : CustomFirearm
    {
        internal string launcherId;

        /// <inheritdoc/>
        public override string Id => launcherId!;

        /// <inheritdoc/>
        public override string Name { get; } = "Item Launcher";

        /// <inheritdoc/>
        public override ItemType PickupType { get; set; } = ItemType.GunCOM15;

        /// <inheritdoc/>
        public override ItemType InventoryType { get; set; } = ItemType.GunCOM15;

        /// <summary>
        /// Gets or sets the default properties of the Item Launcher.
        /// </summary>
        [Description("Sets the default properties of the Item Launcher.")]
        public ItemLauncherProperties DefaultProperties { get; set; }

        /// <inheritdoc/>
        public override ItemBase AddItem(ExPlayer target, object? itemData = null, bool setHeld = false)
        {
            if (itemData is not ItemLauncherProperties)
                itemData = DefaultProperties;

            return base.AddItem(target, itemData, setHeld);
        }

        /// <inheritdoc/>
        public override ItemBase CreateItem(object? itemData = null)
        {
            if (itemData is not ItemLauncherProperties)
                itemData = DefaultProperties;

            return base.CreateItem(itemData);
        }

        /// <inheritdoc/>
        public override ItemPickupBase SpawnItem(Vector3 position, Quaternion? rotation, object? pickupData = null)
        {
            if (pickupData is not ItemLauncherProperties)
                pickupData = DefaultProperties;

            return base.SpawnItem(position, rotation, pickupData);
        }

        /// <inheritdoc/>
        public override void OnShooting(PlayerShootingFirearmEventArgs args, ref object? firearmData)
        {
            base.OnShooting(args, ref firearmData);

            if (firearmData is not ItemLauncherProperties properties)
                properties = DefaultProperties;

            args.IsAllowed = false;

            if (!properties.LaunchedItem.TryGetTemplate<ItemBase>(out var template))
                return;

            if (template is ThrowableItem throwable)
                ThrowProjectileItem(args.Player, throwable, properties);
            else
                ThrowBasicItem(args.Player, template, properties);
        }

        private void ThrowBasicItem(ExPlayer player, ItemBase template, ItemLauncherProperties properties)
        {
            for (var i = 0; i < properties.Amount; i++)
                player.Inventory.ThrowItem<ItemPickupBase>(template.ItemTypeId, properties.Force, properties.Scale.Vector);
        }

        private void ThrowProjectileItem(ExPlayer player, ThrowableItem template, ItemLauncherProperties properties)
        {
            for (var i = 0; i < properties.Amount; i++)
                ExMap.SpawnProjectile(template.ItemTypeId, player.CameraTransform.position, properties.Scale.Vector,
                    player.CameraTransform.forward * properties.Force, player.Rotation, properties.Force, properties.FuseTime);
        }

        internal static void Initialize()
        {
            var path = Path.Combine(SecretLab.RootDirectory, "item_launchers");
            var example = Path.Combine(path, "example.yml");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var exampleProps = new ItemLauncherProperties();

            exampleProps.Launcher.launcherId = "example_launcher";
            exampleProps.Launcher.DefaultProperties = exampleProps;

            SecretLab.SaveConfigPath(false, example, new ItemLauncherProperties());

            foreach (var file in Directory.GetFiles(path, "*.yml"))
            {
                if (file == example)
                    continue;

                var name = Path.GetFileNameWithoutExtension(file);
                var props = SecretLab.LoadConfigPath(false, file, () => new ItemLauncherProperties());

                props.Launcher.DefaultProperties = props;
                props.Launcher.launcherId = name;
                props.Launcher.Register();
            }
        }
    }
}