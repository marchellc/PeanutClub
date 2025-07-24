using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;

using LabExtended.API.CustomTeams;
using LabExtended.API.CustomTeams.Internal;

using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Utilities;
using LabExtended.Extensions;
using LabExtended.Attributes;

using PeanutClub.SpecialWaves.Loadouts;

using PlayerRoles;
using PlayerRoles.Spectating;

using ProjectMER.Features;
using ProjectMER.Features.Objects;

using UnityEngine;

namespace PeanutClub.SpecialWaves.Waves.SerpentsHand;

/// <summary>
/// Serpents Hand team handler.
/// </summary>
[LoaderIgnore]
public class SerpentsHandTeam : CustomTeamHandler<SerpentsHandWave>
{
    /// <summary>
    /// Gets the maximum amount of players in a wave.
    /// </summary>
    public static int MaxPlayers => PluginCore.StaticConfig.SerpentsHandMaxPlayers;

    /// <summary>
    /// Gets the name of the hole schematic.
    /// </summary>
    public static string HoleSchematicName => PluginCore.StaticConfig.SerpentsHandHoleSchematicName;

    /// <summary>
    /// Gets the name of the hole position.
    /// </summary>
    public static string HolePositionName => PluginCore.StaticConfig.SerpentsHandHolePositionName;
    
    /// <summary>
    /// Gets the name of the spawn position.
    /// </summary>
    public static string SpawnPositionName => PluginCore.StaticConfig.SerpentsHandSpawnPositionName;
    
    /// <summary>
    /// Gets the spawn point bounds.
    /// </summary>
    public static Bounds SpawnBounds { get; private set; }
    
    /// <summary>
    /// Gets the spawned hole.
    /// </summary>
    public static SchematicObject? HoleObject { get; private set; }
    
    /// <summary>
    /// Whether or not a wave was spawned this round.
    /// </summary>
    public static bool WasSpawned { get; private set; }
    
    /// <inheritdoc cref="CustomTeamHandlerBase.Name"/>
    public override string? Name { get; } = "Serpent's Hand";
    
    /// <inheritdoc cref="CustomTeamHandlerBase.IsSpawnable"/>
    public override bool IsSpawnable(ExPlayer player)
        => player?.ReferenceHub != null && player.Role.Role is SpectatorRole spectatorRole && spectatorRole.ReadyToRespawn;

    /// <inheritdoc cref="CustomTeamHandlerBase.SelectPosition"/>
    public override Vector3? SelectPosition(ExPlayer player)
        => SpawnBounds.GetRandom(false);

    /// <inheritdoc cref="CustomTeamHandlerBase.SelectRole"/>
    public override object SelectRole(ExPlayer player, Dictionary<ExPlayer, object> selectedRoles)
        => RoleTypeId.Tutorial;

    /// <inheritdoc cref="CustomTeamHandlerBase.OnRegistered"/>
    public override void OnRegistered()
    {
        base.OnRegistered();
        
        PlayerEvents.Death += Internal_Died;
        PlayerEvents.ChangedRole += Internal_ChangedRole;
        
        ExRoundEvents.Started += Internal_Started;
        
        LoadoutManager.EnsureLoadout("SerpentsHand", new LoadoutInfo()
            .WithGameAmmo(ItemType.Ammo556x45, 120)
            .WithGameItems(ItemType.GunE11SR, ItemType.GrenadeHE, ItemType.KeycardChaosInsurgency, ItemType.Adrenaline, ItemType.SCP500, ItemType.SCP1344, ItemType.ArmorHeavy));
        
        ApiLog.Debug("Serpent's Hand", "Registered");
    }

    private void Internal_Started()
    {
        WasSpawned = false;
        HoleObject = null;

        var index = 0;

        while (true)
        {
            var centerName = $"{SpawnPositionName}{index}";
            var holeName = $"{HolePositionName}{index}";

            if (!MapUtilities.Positions.ContainsKey(centerName))
            {
                ApiLog.Warn("Serpent's Hand", "Failed to find a suitable spawn point!");
                break;
            }

            if (!MapUtilities.TryGet(centerName, out Vector3 centerPos))
            {
                index++;
                continue;
            }

            if (!MapUtilities.TryGet(holeName, out Vector3 holePos))
            {
                index++;
                continue;
            }
            
            ApiLog.Debug("Serpent's Hand", $"Team Spawn Point set to &3{centerName}&r (&6{centerPos.ToPreciseString()}&r)");
            ApiLog.Debug("Serpent's Hand", $"Hole Spawn Point set to &3{holeName}&r (&6{holePos.ToPreciseString()}&r)");
            
            SpawnBounds = new(centerPos, PluginCore.StaticConfig.SerpentsHandSpawnSize);
            SpawnBounds.SetMinMax(Vector3.zero, PluginCore.StaticConfig.SerpentsHandSpawnSize);
            
            if (!ObjectSpawner.TrySpawnSchematic(HoleSchematicName, holePos, out var spawnedHole))
            {
                ApiLog.Warn("Serpent's Hand", "Could not spawn the hole schematic!");
                return;
            }
            
            HoleObject = spawnedHole;
        
            ApiLog.Debug("Serpent's Hand", "Spawned the hole schematic!");
            break;
        }
    }

    private void Internal_Died(PlayerDeathEventArgs args)
    {
        if (WasSpawned)
            return;

        if (!args.OldRole.IsScp(false))
            return;

        WasSpawned = Spawn(MaxPlayers, false, false) != null;
        
        if (WasSpawned)
            ApiLog.Debug("Serpent's Hand", "Spawned instance");
        else
            ApiLog.Warn("Serpent's Hand", "Could not spawn instance");
    }

    private void Internal_ChangedRole(PlayerChangedRoleEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Player.CustomInfo) && args.Player.CustomInfo == Name)
        {
            args.Player.CustomInfo = string.Empty;
            args.Player.InfoArea &= ~PlayerInfoArea.CustomInfo;
        }
    }
}