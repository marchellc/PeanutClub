using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms.Attachments;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;

using LabExtended.Events;
using LabExtended.Events.Player;

using LabExtended.Utilities.Firearms;

using PeanutClub.SpecialWaves.Loadouts;
using PeanutClub.SpecialWaves.Utilities;

using PlayerStatsSystem;

namespace PeanutClub.SpecialWaves.Weapons;

/// <summary>
/// Handles the sniper rifle behaviour.
/// </summary>
public static class SniperRifleHandler
{
    /// <summary>
    /// Gets the item tag for the sniper.
    /// </summary>
    public const string ItemTag = "SniperRifle";

    /// <summary>
    /// Gets the damage the sniper rifle deals at all times.
    /// </summary>
    public const float SniperDamage = 250f;

    /// <summary>
    /// How much ammo can be chambered in the sniper rifle at once.
    /// </summary>
    public const int SniperChambered = 1;

    /// <summary>
    /// A list of loadouts with the sniper rifle.
    /// </summary>
    public static string[] LoadoutsWithSniperRifle { get; } =
    [
        "Hand2", // Red Right Hand "Hand2" NtfSpecialist
        "Archangels3", // Archangels "Archangels3" ChaosMarauder
    ];

    /// <summary>
    /// An array of all blacklisted attachments.
    /// </summary>
    public static AttachmentName[] BlacklistedAttachments { get; } =
    [
        AttachmentName.ExtendedMagAP,
        AttachmentName.ExtendedMagFMJ,
        AttachmentName.ExtendedMagJHP,
        
        AttachmentName.LowcapMagAP,
        AttachmentName.LowcapMagFMJ,
        AttachmentName.LowcapMagJHP,
        
        AttachmentName.StandardMagAP,
        AttachmentName.StandardMagFMJ,
        AttachmentName.StandardMagJHP,
        
        AttachmentName.CylinderMag5,
        AttachmentName.CylinderMag6,
        AttachmentName.CylinderMag7,
    ];

    internal static void Internal_ProcessRifle(Firearm firearm)
    {
        firearm.SetTag(ItemTag);
        firearm.SetAttachments(x => x.IsEnabled && !BlacklistedAttachments.Contains(x.Name));
        
        if (firearm.TryGetModule<MagazineModule>(out var magazineModule))
        {
            magazineModule._defaultCapacity = SniperChambered;
            
            if (firearm.TryGetModule<AutomaticActionModule>(out var actionModule) && actionModule.AmmoStored > 0)
                actionModule.ServerCycleAction();
            
            if (magazineModule.AmmoStored > SniperChambered)
                magazineModule.ServerModifyAmmo(-(magazineModule.AmmoStored - SniperChambered));
        }
    }

    internal static void Internal_LoadoutApplied(string loadoutName, LoadoutInfo loadoutInfo, ExPlayer player)
    {
        if (!LoadoutsWithSniperRifle.Contains(loadoutName))
            return;
        
        foreach (var item in player.Inventory.Items)
        {
            if (item.ItemTypeId != ItemType.GunE11SR)
                continue;
            
            Internal_ProcessRifle((Firearm)item);
            break;
        }
    }

    private static void Internal_ChangingAttachments(PlayerChangingFirearmAttachmentsEventArgs args)
    {
        if (!args.Firearm.HasTag(ItemTag))
            return;

        args.ToEnable.RemoveAll(BlacklistedAttachments.Contains);
    }
    
    private static void Internal_Hurting(PlayerHurtingEventArgs args)
    {
        if (args.DamageHandler is FirearmDamageHandler firearmDamageHandler
            && firearmDamageHandler.Firearm.HasTag(ItemTag))
        {
            firearmDamageHandler.Damage = SniperDamage;
        }
    }
    
    internal static void Internal_Init()
    {
        PlayerEvents.Hurting += Internal_Hurting;
        LoadoutManager.Applied += Internal_LoadoutApplied;
        ExPlayerEvents.ChangingAttachments += Internal_ChangingAttachments;
    }
}