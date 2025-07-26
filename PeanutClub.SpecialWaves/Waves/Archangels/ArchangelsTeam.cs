using LabExtended.API;

using LabExtended.API.CustomTeams;
using LabExtended.API.CustomTeams.Internal;

using LabExtended.Attributes;

using LabExtended.Core;
using LabExtended.Extensions;

using PeanutClub.LoadoutAPI;
using PeanutClub.SpecialWaves.Weapons;

using PlayerRoles;
using PlayerRoles.Spectating;

using UnityEngine;

namespace PeanutClub.SpecialWaves.Waves.Archangels;

/// <summary>
/// Zeta-3 Archangels team handler.
/// </summary>
[LoaderIgnore]
public class ArchangelsTeam : CustomTeamHandler<ArchangelsWave>
{
    /// <summary>
    /// An array of all chaos roles.
    /// </summary>
    public static RoleTypeId[] ChaosRoles { get; } =
        [RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRifleman];
    
    /// <summary>
    /// Gets the singleton instance of the team handler.
    /// </summary>
    public static ArchangelsTeam Singleton { get; private set; }

    /// <inheritdoc cref="CustomTeamHandlerBase.Name"/>
    public override string? Name { get; } = "Zeta-3 \"Archangels\"";

    /// <inheritdoc cref="CustomTeamHandlerBase.IsSpawnable"/>
    public override bool IsSpawnable(ExPlayer player)
        => player?.ReferenceHub != null && player.Role.Role is SpectatorRole spectatorRole && spectatorRole.ReadyToRespawn;

    /// <inheritdoc cref="CustomTeamHandlerBase.SelectPosition"/>
    public override Vector3? SelectPosition(ExPlayer player)
        => ChaosRoles
            .RandomItem()
            .GetSpawnPosition()
            .position;

    // Archangels 1 - Repressor
    // Archangels 2 - Rifleman (1)
    // Archangels 3 - Marauder (1)
    /// <inheritdoc cref="CustomTeamHandlerBase.SelectRole"/>
    public override object SelectRole(ExPlayer player, Dictionary<ExPlayer, object> selectedRoles)
    {
        if (!selectedRoles.Any(x => x.Value is RoleTypeId roleType && roleType == RoleTypeId.ChaosRifleman))
            return RoleTypeId.ChaosRifleman;

        if (!selectedRoles.Any(x => x.Value is RoleTypeId roleType && roleType == RoleTypeId.ChaosMarauder))
            return RoleTypeId.ChaosMarauder;

        return RoleTypeId.ChaosRepressor;
    }
    
    /// <inheritdoc cref="CustomTeamHandlerBase.OnRegistered"/>
    public override void OnRegistered()
    {
        base.OnRegistered();
        
        ArchangelsRadio.Internal_Init();

        Singleton = this;
        
        LoadoutPlugin.Ensure("Archangels1", new LoadoutDefinition()
            .WithHealth(250f, 250f)
            .WithAmmo(ItemType.Ammo762x39, 200)
            .WithItems(ItemType.GunLogicer, ItemType.KeycardChaosInsurgency, ItemType.GrenadeHE, ItemType.Medkit, ItemType.Adrenaline, ItemType.ArmorHeavy));
        
        LoadoutPlugin.Ensure("Archangels2", new LoadoutDefinition()
            .WithHealth(250f, 250f)
            .WithAmmo(ItemType.Ammo762x39, 200)
            .WithItems(ItemType.GunLogicer, ItemType.Jailbird, ItemType.KeycardChaosInsurgency, ItemType.GrenadeHE, ItemType.Medkit, ItemType.Adrenaline, ItemType.ArmorHeavy));
        
        LoadoutPlugin.Ensure("Archangels3",  new LoadoutDefinition()
            .WithHealth(250f, 250f)
            .WithAmmo(ItemType.Ammo556x45, 120)
            .WithAmmo(ItemType.GunCOM18, 120)
            .WithItem(ItemType.GunE11SR, SniperRifleHandler.ItemTag)
            .WithItems(ItemType.GunCOM18, ItemType.KeycardChaosInsurgency, ItemType.Medkit, ItemType.Adrenaline, ItemType.ArmorHeavy));
        
        ApiLog.Debug("Archangels Team", "Registered");
    }

    /// <inheritdoc cref="CustomTeamHandler{TInstance}.OnSpawned"/>
    public override void OnSpawned(ArchangelsWave instance)
    {
        base.OnSpawned(instance);
        
        ApiLog.Debug("Archangels Team", $"Spawned a new team instance (ID={instance.Id})");

        for (var i = 0; i < instance.AlivePlayers.Count; i++)
        {
            var player = instance.AlivePlayers[i];
            var loadout = string.Empty;

            switch (player.Role.Type)
            {
                // Archangels 1
                case RoleTypeId.ChaosRepressor:
                    loadout = "Archangels1";
                    break;
                
                // Archangels2
                case RoleTypeId.ChaosRifleman:
                    loadout = "Archangels2";
                    break;
                
                // Archangels 3
                case RoleTypeId.ChaosMarauder:
                    loadout = "Archangels3";
                    break;
            }

            if (!string.IsNullOrEmpty(loadout))
            {
                LoadoutPlugin.TryApply(player, loadout);
            }
        }
    }
}