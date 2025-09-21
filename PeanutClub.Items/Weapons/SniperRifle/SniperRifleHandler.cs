using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabApi.Features.Wrappers;

using LabExtended.API;
using LabExtended.Events;
using LabExtended.Events.Player;
using LabExtended.Extensions;
using LabExtended.Utilities.Firearms;

using PeanutClub.Loadouts;
using PeanutClub.Overlays.Alerts;

using PlayerStatsSystem;

using UnityEngine;

using FirearmPickup = InventorySystem.Items.Firearms.FirearmPickup;

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
    
    /// <summary>
    /// Gets a list of all tracked item serials.
    /// </summary>
    public static Dictionary<ushort, SniperRifleProperties> TrackedItems { get; } = new();
    
    /// <summary>
    /// Whether or not a specific item serial is tracked as a sniper rifle.
    /// </summary>
    /// <param name="itemSerial">The serial number of the item.</param>
    /// <returns>true if the serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(ushort itemSerial)
        => itemSerial != 0 && TrackedItems.ContainsKey(itemSerial);

    /// <summary>
    /// Whether or not a specific item serial is tracked as a sniper rifle.
    /// </summary>
    /// <param name="itemSerial">The serial number of the item.</param>
    /// <param name="properties">The properties of the sniper rifle.</param>
    /// <returns>true if the serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(ushort itemSerial, out SniperRifleProperties properties)
    {
        properties = null!;
        
        if (itemSerial == 0 || !TrackedItems.TryGetValue(itemSerial, out properties))
            return false;

        return true;
    }
    
    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this Item item)
        => item != null && IsSniperRifle(item.Serial);

    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="properties">The properties of the sniper rifle.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this Item item, out SniperRifleProperties properties)
    {
        properties = null!;
        
        if (item?.Base == null)
            return false;

        return IsSniperRifle(item.Serial, out properties);
    }

    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this Pickup item)
        => item != null && IsSniperRifle(item.Serial);
    
    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="properties">The properties of the sniper rifle.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this Pickup item, out SniperRifleProperties properties)
    {
        properties = null!;
        
        if (item?.Base == null)
            return false;

        return IsSniperRifle(item.Serial, out properties);
    }
    
    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this ItemBase item)
        => item != null && IsSniperRifle(item.ItemSerial);
    
    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="properties">The properties of the sniper rifle.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this ItemBase item, out SniperRifleProperties properties)
    {
        properties = null!;
        
        if (item == null)
            return false;

        return IsSniperRifle(item.ItemSerial, out properties);
    }
    
    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this ItemPickupBase item)
        => item != null && IsSniperRifle(item.Info.Serial);
    
    /// <summary>
    /// Whether or not a specific item is tracked as a sniper rifle.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="properties">The properties of the sniper rifle.</param>
    /// <returns>true if the item's serial number is tracked as a sniper rifle</returns>
    public static bool IsSniperRifle(this ItemPickupBase item, out SniperRifleProperties properties)
    {
        properties = null!;
        
        if (item == null)
            return false;

        return IsSniperRifle(item.Info.Serial, out properties);
    }

    /// <summary>
    /// Attempts to remove a Sniper Rifle and optionally destroy it's item.
    /// </summary>
    /// <param name="sniperSerial">The serial of the Sniper Rifle.</param>
    /// <param name="destroyItem">Whether or not to destroy the item.</param>
    /// <returns>true if the rifle was removed</returns>
    public static bool Remove(ushort sniperSerial, bool destroyItem = false)
    {
        if (!TrackedItems.Remove(sniperSerial))
            return false;

        if (destroyItem)
        {
            if (InventoryExtensions.ServerTryGetItemWithSerial(sniperSerial, out var item) && item != null)
            {
                item.DestroyItem();
            }
            
            if (ExMap.Pickups.TryGetFirst(x => x != null && x.Info.Serial == sniperSerial, out var pickup))
            {
                pickup.DestroySelf();
            }
        }

        return true;
    }

    /// <summary>
    /// Creates a new Sniper Rifle item.
    /// </summary>
    /// <param name="firearmType">The type of the item of the Sniper Rifle.</param>
    /// <param name="properties">The custom properties of the Sniper Rifle (uses config if null).</param>
    /// <returns>The created Sniper Rifle item.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static Firearm CreateSniperRifle(ItemType firearmType = ItemType.GunE11SR, SniperRifleProperties? properties = null)
    {
        if (!firearmType.TryGetTemplate<Firearm>(out var template))
            throw new Exception($"Could not get the item template for item type {firearmType}");

        var firearm = UnityEngine.Object.Instantiate(template);

        firearm.ServerAddReason = ItemAddReason.AdminCommand;
        firearm.ItemSerial = ItemSerialGenerator.GenerateNext();

        properties ??= DefaultProperties;

        TrackedItems[firearm.ItemSerial] = properties;
        return firearm;
    }

    /// <summary>
    /// Adds the Sniper Rifle to a player's inventory.
    /// </summary>
    /// <param name="player">The player to add the Sniper Rifle to.</param>
    /// <param name="firearmType">The type of the item of the Sniper Rifle.</param>
    /// <param name="properties">The custom properties of the Sniper Rifle (uses config if null).</param>
    /// <returns>The added Firearm Sniper Rifle item.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static Firearm GiveSniperRifle(this ExPlayer player, ItemType firearmType = ItemType.GunE11SR, SniperRifleProperties? properties = null)
    {
        if (player?.ReferenceHub == null)
            throw new ArgumentNullException(nameof(player));

        var firearm = player.Inventory.AddItem<Firearm>(firearmType, ItemAddReason.AdminCommand);

        if (firearm == null)
            throw new Exception("Could not add Sniper Rifle");

        properties ??= DefaultProperties;

        TrackedItems[firearm.ItemSerial] = properties;
        
        SetupRifle(firearm, properties);
        SendHint(firearm);

        return firearm;
    }

    /// <summary>
    /// Spawns a Sniper Rifle pickup.
    /// </summary>
    /// <param name="position">The position to spawn the pickup at.</param>
    /// <param name="firearmType">The type of the Sniper Rifle item.</param>
    /// <param name="properties">The properties of the Sniper Rifle (uses config if null).</param>
    /// <returns>The spawned Firearm pickup.</returns>
    /// <exception cref="Exception"></exception>
    public static FirearmPickup SpawnSniperRifle(Vector3 position, ItemType firearmType = ItemType.GunE11SR, SniperRifleProperties? properties = null)
    {
        var pickup = ExMap.SpawnItem<FirearmPickup>(firearmType, position, Vector3.one, Quaternion.identity);

        if (pickup == null)
            throw new Exception("Could not spawn Sniper Rifle");

        TrackedItems[pickup.Info.Serial] = properties ?? DefaultProperties;
        return pickup;
    }

    private static void SendHint(Firearm rifle)
    {
        if (!ExPlayer.TryGet(rifle.Owner, out var player))
            return;
        
        player.SendAlert(AlertType.Info, 10f,
            "Dostal jsi <color=red>Sniper Rifle</color>!\nTato zbraň dává damage <color=yellow>250 HP</color> při <b>každé</b> ráně!");
    }

    private static void SetupRifle(Firearm rifle, SniperRifleProperties properties)
    {
        if (properties.DefaultAttachments?.Count > 0)
            rifle.SetAttachments(x => properties.DefaultAttachments.Contains(x.Name));
        else if (properties.BlacklistedAttachments?.Count > 0)
            rifle.SetAttachments(x => x.IsEnabled && !properties.BlacklistedAttachments.Contains(x.Name));

        if (rifle.TryGetModule<MagazineModule>(out var magazineModule))
        {
            magazineModule._defaultCapacity = properties.MaxAmmo;

            if (rifle.TryGetModule<AutomaticActionModule>(out var automaticActionModule) &&
                automaticActionModule.AmmoStored > 0)
                automaticActionModule.ServerCycleAction();

            if (magazineModule.AmmoStored > properties.MaxAmmo)
                magazineModule.ServerModifyAmmo(-(magazineModule.AmmoStored - properties.MaxAmmo));
        }
    }

    private static void Internal_PickedUpItem(PlayerPickedUpItemEventArgs args)
    {
        if (args.Item?.Base == null || !args.Item.IsSniperRifle(out var properties))
            return;
        
        SetupRifle((Firearm)args.Item.Base, properties);
        SendHint((Firearm)args.Item.Base);
    }

    private static void Internal_LoadoutAddedVanillaItem(ExPlayer player, LoadoutDefinition loadout, LoadoutItem item, ItemBase result)
    {
        if (result == null || result is not Firearm firearm || item.ItemTag is null || item.ItemTag != LoadoutTag)
            return;

        var properties = DefaultProperties;

        TrackedItems[firearm.ItemSerial] = properties;
        
        SetupRifle(firearm, properties);
        SendHint(firearm);
    }
    
    private static void Internal_ChangingAttachments(PlayerChangingFirearmAttachmentsEventArgs args)
    {
        if (!args.Firearm.IsSniperRifle(out var properties))
            return;

        if (!properties.AllowAttachmentsChanging)
        {
            args.Player.SendAlert(AlertType.Warn, 5f, "Nemůžeš upravovat <color=yellow>Sniper Rifle</color>!");
            args.IsAllowed = false;
            
            return;
        }

        if (properties.BlacklistedAttachments?.Count > 0)
            args.ToEnable.RemoveAll(properties.BlacklistedAttachments.Contains);
    }

    private static void Internal_Hurting(PlayerHurtingEventArgs args)
    {
        if (args.DamageHandler is FirearmDamageHandler firearmDamageHandler
            && firearmDamageHandler.Firearm.IsSniperRifle(out var properties))
        {
            if (!properties.BaseDamage.TryGetValue(firearmDamageHandler.Hitbox, out var baseDamage))
            {
                if (properties.BaseDamage.Count > 0)
                {
                    baseDamage = properties.BaseDamage.First().Value;
                }
                else
                {
                    baseDamage = firearmDamageHandler.Damage;
                }
            }

            var hasRoleMultiplier = properties.RoleMultipliers.TryGetValue(args.Player.Role, out var roleMultiplier);
            var hasTeamMultiplier = properties.TeamMultipliers.TryGetValue(args.Player.Team, out var teamMultiplier);

            if (hasTeamMultiplier)
            {
                if (hasRoleMultiplier)
                {
                    baseDamage *= roleMultiplier;
                }
                else
                {
                    baseDamage *= teamMultiplier;
                }
            }
            else
            {
                if (hasRoleMultiplier)
                {
                    baseDamage *= roleMultiplier;
                }
            }

            firearmDamageHandler.Damage = baseDamage;
        }
    }

    private static void Internal_Reloading(PlayerReloadingWeaponEventArgs args)
    {
        if (!args.FirearmItem.IsSniperRifle(out var properties))
            return;

        if (args.FirearmItem.Base.TryGetModule<AutomaticActionModule>(out var automaticActionModule)
            && automaticActionModule.AmmoStored >= properties.MaxAmmo)
            args.IsAllowed = false;
    }
    
    private static void Internal_WaitingForPlayers()
    {
        TrackedItems.Clear();
    }
    
    internal static void Internal_Init()
    {
        PlayerEvents.Hurting += Internal_Hurting;
        PlayerEvents.PickedUpItem += Internal_PickedUpItem;
        PlayerEvents.ReloadingWeapon += Internal_Reloading;
        
        ExRoundEvents.WaitingForPlayers += Internal_WaitingForPlayers;
        ExPlayerEvents.ChangingAttachments += Internal_ChangingAttachments;

        LoadoutPlugin.AddedVanillaItem += Internal_LoadoutAddedVanillaItem;
    }
}