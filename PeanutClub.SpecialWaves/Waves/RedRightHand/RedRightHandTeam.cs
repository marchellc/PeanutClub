using LabExtended.API;
using LabExtended.Extensions;

using LabExtended.API.CustomTeams;
using LabExtended.API.CustomTeams.Internal;

using LabExtended.Attributes;
using LabExtended.Core;
using PeanutClub.SpecialWaves.Loadouts;

using PlayerRoles;
using PlayerRoles.Spectating;

using UnityEngine;

namespace PeanutClub.SpecialWaves.Waves.RedRightHand;

/// <summary>
/// Alpha-1 Red Right Hand team handler.
/// </summary>
[LoaderIgnore]
public class RedRightHandTeam : CustomTeamHandler<RedRightHandWave>
{
    /// <summary>
    /// A list of all NTF roles.
    /// </summary>
    public static RoleTypeId[] NtfRoles { get; } =
    [
        RoleTypeId.NtfCaptain,
        RoleTypeId.NtfSpecialist,
        RoleTypeId.NtfSergeant,
        RoleTypeId.NtfPrivate
    ];
    
    /// <summary>
    /// Gets the singleton instance of the team handler.
    /// </summary>
    public static RedRightHandTeam Singleton { get; private set; }
    
    /// <inheritdoc cref="CustomTeamHandlerBase.Name"/>
    public override string? Name { get; } = "Alpha-1 \"Red Right Hand\"";
    
    /// <inheritdoc cref="CustomTeamHandlerBase.IsSpawnable"/>
    public override bool IsSpawnable(ExPlayer player)
        => player?.ReferenceHub != null && player.Role.Role is SpectatorRole spectatorRole && spectatorRole.ReadyToRespawn;

    /// <inheritdoc cref="CustomTeamHandlerBase.SelectPosition"/>
    public override Vector3? SelectPosition(ExPlayer player)
        => NtfRoles
            .GetRandomItem()
            .GetSpawnPosition()
            .position;

    // Hand1 - Captain (1)
    // Hand2 - Sergeant (1)
    // Hand3 - Specialist
    
    /// <inheritdoc cref="CustomTeamHandlerBase.SelectRole"/>
    public override object SelectRole(ExPlayer player, Dictionary<ExPlayer, object> selectedRoles)
    {
        if (!selectedRoles.Any(x => x.Value is RoleTypeId roleType && roleType == RoleTypeId.NtfCaptain))
            return RoleTypeId.NtfCaptain;
        
        if (!selectedRoles.Any(x => x.Value is RoleTypeId roleType && roleType == RoleTypeId.NtfSergeant))
            return RoleTypeId.NtfSergeant;

        return RoleTypeId.NtfSpecialist;
    }

    /// <inheritdoc cref="CustomTeamHandlerBase.OnRegistered"/>
    public override void OnRegistered()
    {
        base.OnRegistered();
        
        RedRightHandButton.Internal_Init();

        Singleton = this;
        
        LoadoutManager.EnsureLoadout("Hand1", new LoadoutInfo()
            .WithHealth(250f)
            .WithGameAmmo(ItemType.Ammo556x45, 120)
            .WithGameItems(ItemType.GunFRMG0, ItemType.GrenadeHE, ItemType.KeycardMTFCaptain, ItemType.Medkit, ItemType.Adrenaline, ItemType.Radio, ItemType.ArmorHeavy));
        
        LoadoutManager.EnsureLoadout("Hand2", new LoadoutInfo()
            .WithHealth(250f)
            .WithGameAmmo(ItemType.Ammo556x45, 81)
            .WithGameAmmo(ItemType.Ammo9x19, 120)
            .WithGameItems(ItemType.GunCOM18, ItemType.GunE11SR, ItemType.GrenadeHE, ItemType.KeycardMTFCaptain, ItemType.Medkit, ItemType.Adrenaline, ItemType.Radio, ItemType.ArmorHeavy));
        
        LoadoutManager.EnsureLoadout("Hand3", new LoadoutInfo()
            .WithHealth(250f)
            .WithGameAmmo(ItemType.Ammo556x45, 120)
            .WithGameItems(ItemType.GunFRMG0, ItemType.GrenadeHE, ItemType.KeycardMTFCaptain, ItemType.Medkit, ItemType.Adrenaline, ItemType.Radio, ItemType.ArmorHeavy));
        
        ApiLog.Debug("Red Right Hand Team", "Registered");
    }

    public override void OnSpawned(RedRightHandWave instance)
    {
        base.OnSpawned(instance);

        ApiLog.Debug("Red Right Hand Team", $"Spawned a new team instance (ID={instance.Id})");
        
        for (var i = 0; i < instance.AlivePlayers.Count; i++)
        {
            var player = instance.AlivePlayers[i];
            var loadout = string.Empty;

            switch (player.Role.Type)
            {
                case RoleTypeId.NtfCaptain:
                    loadout = "Hand1";
                    break;
                
                case RoleTypeId.NtfSpecialist:
                    loadout = "Hand2";
                    break;
                
                case RoleTypeId.NtfSergeant:
                    loadout = "Hand3";
                    break;
            }

            if (!string.IsNullOrEmpty(loadout))
            {
                player.ApplyLoadout(loadout);
            }
        }
    }
}