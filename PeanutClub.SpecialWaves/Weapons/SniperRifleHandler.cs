using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms.Attachments;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;

using LabExtended.Events;
using LabExtended.Events.Player;

using LabExtended.Utilities.Firearms;

using PeanutClub.LoadoutAPI;
using PeanutClub.OverlayAPI.Alerts;

using PlayerStatsSystem;

namespace PeanutClub.SpecialWaves.Weapons;

/// <summary>
/// Handles the sniper rifle behaviour.
/// </summary>
public static class SniperRifleHandler
{
    /// <summary>
    /// Gets the damage the sniper rifle deals at all times.
    /// </summary>
    public static float SniperDamage => PluginCore.StaticConfig.SniperRifleDamage;

    /// <summary>
    /// How much ammo can be chambered in the sniper rifle at once.
    /// </summary>
    public static int SniperChambered => PluginCore.StaticConfig.SniperRifleCapacity;

    /// <summary>
    /// Whether or not players can change the sniper rifle attachments.
    /// </summary>
    public static bool AllowAttachments => PluginCore.StaticConfig.SniperChangingAttachments;

    /// <summary>
    /// Gets a list of sniper rifle item serials.
    /// </summary>
    public static HashSet<ushort> Rifles { get; } = new();

    /// <summary>
    /// A list of default attachments.
    /// </summary>
    public static List<AttachmentName> DefaultAttachments => PluginCore.StaticConfig.SniperDefaultAttachments;
    
    /// <summary>
    /// A list of all blacklisted attachments.
    /// </summary>
    public static List<AttachmentName> BlacklistedAttachments => PluginCore.StaticConfig.SniperBlacklistedAttachments;

    internal static void Internal_AddedVanillaItem(ExPlayer player, LoadoutDefinition loadout, LoadoutItem loadoutItem, ItemBase item)
    {
        if (item == null || loadoutItem.ItemTag == null || loadoutItem.ItemTag != "SniperRifle" || item is not Firearm firearm)
            return;

        Rifles.Add(item.ItemSerial);
        
        player.SendAlert(AlertType.Info, 10f, "Dostal jsi <color=red>Sniper Rifle</color>!\nTato zbraň dává damage <color=yellow>250 HP</color> při <b>každé</b> ráně!");

        if (DefaultAttachments.Count > 0)
            firearm.SetAttachments(x => DefaultAttachments.Contains(x.Name));
        else
            firearm.SetAttachments(x => x.IsEnabled && !BlacklistedAttachments.Contains(x.Name));
        
        if (firearm.TryGetModule<MagazineModule>(out var magazineModule))
        {
            magazineModule._defaultCapacity = SniperChambered;
            
            if (firearm.TryGetModule<AutomaticActionModule>(out var automaticActionModule) && automaticActionModule.AmmoStored > 0)
                automaticActionModule.ServerCycleAction();

            if (magazineModule.AmmoStored > SniperChambered)
                magazineModule.ServerModifyAmmo(-(magazineModule.AmmoStored - SniperChambered));
        }
    }

    private static void Internal_ChangingAttachments(PlayerChangingFirearmAttachmentsEventArgs args)
    {
        if (!Rifles.Contains(args.Firearm.ItemSerial))
            return;

        if (!AllowAttachments)
        {
            args.Player.SendAlert(AlertType.Warn, 5f, "Nemůžeš upravovat <color=yellow>Sniper Rifle</color>!");
            args.IsAllowed = false;
            
            return;
        }

        args.ToEnable.RemoveAll(BlacklistedAttachments.Contains);
    }
    
    private static void Internal_Hurting(PlayerHurtingEventArgs args)
    {
        if (args.DamageHandler is FirearmDamageHandler firearmDamageHandler
            && Rifles.Contains(firearmDamageHandler.Firearm.ItemSerial))
        {
            firearmDamageHandler.Damage = SniperDamage;
        }
    }

    private static void Internal_Reloading(PlayerReloadingWeaponEventArgs args)
    {
        if (!Rifles.Contains(args.FirearmItem.Serial))
            return;

        if (args.FirearmItem.Base.TryGetModule<AutomaticActionModule>(out var automaticActionModule)
            && automaticActionModule.AmmoStored >= SniperChambered)
            args.IsAllowed = false;
    }

    private static void Internal_Waiting()
    {
        Rifles.Clear();
    }
    
    internal static void Internal_Init()
    {
        PlayerEvents.Hurting += Internal_Hurting;
        PlayerEvents.ReloadingWeapon += Internal_Reloading;
        
        LoadoutPlugin.AddedVanillaItem += Internal_AddedVanillaItem;

        ExRoundEvents.WaitingForPlayers += Internal_Waiting;
        ExPlayerEvents.ChangingAttachments += Internal_ChangingAttachments;
    }
}