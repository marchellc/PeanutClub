using InventorySystem.Items;
using InventorySystem.Items.Firearms;

using LabExtended.API;
using LabExtended.Extensions;

using PeanutClub.Loadouts;
using PeanutClub.Overlays.Alerts;

namespace PeanutClub.Items.Weapons.SniperRifle;

/// <summary>
/// Handles sniper rifle logic.
/// </summary>
public static class SniperRifleHandler
{
    /// <summary>
    /// The tag of the sniper rifle.
    /// </summary>
    public const string LoadoutTag = "SniperRifle";

    /// <summary>
    /// Gets the static instance of the Sniper Rifle config properties.
    /// </summary>
    public static SniperRifleProperties ConfigProperties => ItemsCore.ConfigStatic.SniperRifle;

    /// <summary>
    /// Gets a NEW INSTANCE of the config properties.
    /// </summary>
    public static SniperRifleProperties DefaultProperties
    {
        get
        {
            var config = ConfigProperties;
            var props = new SniperRifleProperties();

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

    private static void Internal_SetupFirearm(ExPlayer? owner, Firearm firearm, CustomFirearmProperties properties)
    {
        if (properties is not SniperRifleProperties)
            return;

        if (owner?.ReferenceHub != null)
            owner.SendAlert(AlertType.Info, 10f, 
                "Dostal si <color=red>Sniper Rifle</color>!\n" +
                "Tato zbraň dává damage <color=yellow>250 HP</color> při <b>každé</b> ráně!");
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