using InventorySystem.Items;
using InventorySystem.Items.Firearms;

using LabExtended.API;
using LabExtended.Extensions;

using PeanutClub.Loadouts;
using PeanutClub.Overlays.Alerts;

namespace PeanutClub.Items.Weapons.AirsoftGun
{
    public static class AirsoftGunHandler
    {
        /// <summary>
        /// Gets the loadout tag used to identify airsoft guns.
        /// </summary>
        public const string LoadoutTag = "AirsoftGun";

        /// <summary>
        /// Gets the static instance of the Sniper Rifle config properties.
        /// </summary>
        public static AirsoftGunProperties ConfigProperties => ItemsCore.ConfigStatic.AirsoftGun;

        /// <summary>
        /// Gets a NEW INSTANCE of the config properties.
        /// </summary>
        public static AirsoftGunProperties DefaultProperties
        {
            get
            {
                var config = ConfigProperties;
                var props = new AirsoftGunProperties();

                props.MaxAmmo = config.MaxAmmo;
                props.AllowAttachmentsChanging = config.AllowAttachmentsChanging;

                props.BaseDamage.Clear();
                props.TeamMultipliers.Clear();
                props.RoleMultipliers.Clear();
                props.DefaultAttachments.Clear();
                props.BlacklistedAttachments.Clear();

                if (config.BaseDamage?.Count > 0)
                    props.BaseDamage.AddRange(config.BaseDamage);

                if (config.TeamMultipliers?.Count > 0)
                    props.TeamMultipliers.AddRange(config.TeamMultipliers);

                if (config.RoleMultipliers?.Count > 0)
                    props.RoleMultipliers.AddRange(config.RoleMultipliers);

                if (config.DefaultAttachments?.Count > 0)
                    props.DefaultAttachments.AddRange(config.DefaultAttachments);

                if (config.BlacklistedAttachments?.Count > 0)
                    props.BlacklistedAttachments.AddRange(config.BlacklistedAttachments);

                return props;
            }
        }

        private static void Internal_SetupFirearm(ExPlayer? owner, Firearm item, CustomFirearmProperties properties)
        {
            if (properties is not AirsoftGunProperties)
                return;

            if (owner?.ReferenceHub != null)
                owner.SendAlert(AlertType.Info, 10f,
                    $"Dostal si <color=red>Airsoft Gun</color>!\n" +
                    $"Tato zbraň dává damage <color=yellow>5 HP</color> při <b>každé</b> ráně!");
        }

        private static void Internal_LoadoutAddedVanillaItem(ExPlayer player, LoadoutDefinition loadout, LoadoutItem item, ItemBase result)
        {
            if (result == null || result is not Firearm firearm || item.ItemTag is null || item.ItemTag != LoadoutTag)
                return;

            var properties = DefaultProperties;

            CustomFirearmHandler.TrackedItems[firearm.ItemSerial] = properties;
            CustomFirearmHandler.SetupRifle(firearm, properties);
        }

        internal static void Internal_Init()
        {
            LoadoutPlugin.AddedVanillaItem += Internal_LoadoutAddedVanillaItem;

            CustomFirearmHandler.SetupFirearm += Internal_SetupFirearm;
        }
    }
}