using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;
using LabExtended.API.Hints;
using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Events.Round;

using LabExtended.Extensions;
using LabExtended.Utilities;

using NorthwoodLib.Pools;

using PeanutClub.SpecialWaves.Loadouts;

using PlayerRoles;

namespace PeanutClub.SpecialWaves.Roles.GuardCommander;

/// <summary>
/// Manages spawns of Guard Commanders.
/// </summary>
public static class GuardCommanderRole
{
    /// <summary>
    /// Gets the guard commander role selector.
    /// </summary>
    public static RoleSelector RoleSelector { get; private set; }
    
    /// <summary>
    /// Sets a specific player as the Guard Commander.
    /// </summary>
    /// <param name="player">The target player.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetCommander(this ExPlayer player)
    {
        if (player?.ReferenceHub == null)
            throw new ArgumentNullException(nameof(player));
        
        player.Role.Set(RoleTypeId.FacilityGuard, RoleChangeReason.RoundStart, RoleSpawnFlags.UseSpawnpoint);
        player.CustomInfo = "Guard Commander";

        if ((player.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
            player.InfoArea |= PlayerInfoArea.CustomInfo;
        
        player.ApplyLoadout("GuardCommander");
        player.ShowHint("<b>Jsi <color=green>Guard Commander</color>!</b>", 10, true);
        
        ApiLog.Debug("Guard Commander Role", $"Spawned player &3{player.Nickname}&r (&6{player.UserId}&r) as the Guard Commander");
    }

    private static void Internal_RoleChanged(PlayerChangedRoleEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Player.CustomInfo) && args.Player.CustomInfo == "Guard Commander")
        {
            args.Player.CustomInfo = string.Empty;
            args.Player.InfoArea &= ~PlayerInfoArea.CustomInfo;
        }
    }
    
    internal static void Internal_Init()
    {
        RoleSelector = new(PluginCore.StaticConfig.GuardCommanderSpawns, SetCommander, (_, role) => role is RoleTypeId.FacilityGuard);
        
        LoadoutManager.EnsureLoadout("GuardCommander", new LoadoutInfo()
            .WithGameAmmo(ItemType.Ammo556x45, 120)
            .WithGameItems(ItemType.GunE11SR, ItemType.KeycardMTFPrivate, ItemType.Medkit, ItemType.Adrenaline, ItemType.GrenadeFlash, ItemType.Radio, ItemType.ArmorCombat));
        
        PlayerEvents.ChangedRole += Internal_RoleChanged;
    }
}