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
    /// The player chosen as the Guard Commander.
    /// </summary>
    public static ExPlayer? ChosenCommander { get; private set; }
    
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
    
    private static void Internal_Started()
    {
        TimingUtils.AfterSeconds(() =>
        {
            if (ChosenCommander?.ReferenceHub != null)
            {
                ChosenCommander.SetCommander();
                ChosenCommander = null;
            }
        }, 0.2f);
    }

    private static void Internal_RoleChanged(PlayerChangedRoleEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Player.CustomInfo) && args.Player.CustomInfo == "Guard Commander")
        {
            args.Player.CustomInfo = string.Empty;
            args.Player.InfoArea &= ~PlayerInfoArea.CustomInfo;
        }
    }

    private static void Internal_AssigningRoles(AssigningRolesEventArgs args)
    {
        ChosenCommander = null;

        var validPairs = ListPool<KeyValuePair<ExPlayer, RoleTypeId>>.Shared.Rent();

        foreach (var pair in args.Roles)
        {
            if (pair.Value != RoleTypeId.FacilityGuard)
                continue;
            
            validPairs.Add(pair);
        }
        
        if (validPairs.Count > 0)
        {
            if (validPairs.Count == 1)
            {
                if (WeightUtils.GetBool(50f, 50f))
                {
                    ChosenCommander = validPairs[0].Key;
                }
            }
            else
            {
                ChosenCommander = validPairs.GetRandomItem().Key;
            }

            if (ChosenCommander?.ReferenceHub != null)
            {
                args.Roles.Remove(ChosenCommander);
            }
        }
        
        ListPool<KeyValuePair<ExPlayer, RoleTypeId>>.Shared.Return(validPairs);
    }
    
    internal static void Internal_Init()
    {
        LoadoutManager.EnsureLoadout("GuardCommander", new LoadoutInfo()
            .WithGameAmmo(ItemType.Ammo556x45, 120)
            .WithGameItems(ItemType.GunE11SR, ItemType.KeycardMTFPrivate, ItemType.Medkit, ItemType.Adrenaline, ItemType.GrenadeFlash, ItemType.Radio, ItemType.ArmorCombat));
        
        ExRoundEvents.Started += Internal_Started;
        ExRoundEvents.AssigningRoles += Internal_AssigningRoles;
        
        PlayerEvents.ChangedRole += Internal_RoleChanged;
    }
}