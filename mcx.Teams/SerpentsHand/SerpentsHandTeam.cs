using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

using LabExtended.API;
using LabExtended.API.CustomTeams;

using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Utilities;
using LabExtended.Extensions;
using LabExtended.Attributes;

using MapGeneration;

using mcx.Loadouts;

using PlayerRoles;

using ProjectMER.Features;
using ProjectMER.Features.Objects;

using UnityEngine;

namespace mcx.Teams.SerpentsHand;

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
    /// Gets the minimum amount of players in a wave.
    /// </summary>
    public static int MinPlayers => PluginCore.StaticConfig.SerpentsHandMinPlayers;

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
    /// Gets the CASSIE announcement.
    /// </summary>
    public static bool CassieMessage => PluginCore.StaticConfig.SerpentsHandCassieMessage;
    
    /// <summary>
    /// Gets the current spawn position.
    /// </summary>
    public static Vector3 SpawnPosition { get; private set; }
    
    /// <summary>
    /// Gets the spawned hole.
    /// </summary>
    public static SchematicObject? HoleObject { get; private set; }
    
    /// <summary>
    /// Whether or not a wave was spawned this round.
    /// </summary>
    public static bool WasSpawned { get; private set; }
    
    /// <inheritdoc cref="CustomTeamHandler.Name"/>
    public override string? Name { get; } = "Serpent's Hand";

    /// <inheritdoc cref="CustomTeamHandler.IsSpawnable"/>
    public override bool IsSpawnable(ExPlayer player)
        => player.CanBeRespawned;

    /// <inheritdoc cref="CustomTeamHandler.SelectRole"/>
    public override RoleTypeId SelectRole(ExPlayer player, Dictionary<ExPlayer, RoleTypeId> selectedRoles)
        => RoleTypeId.Tutorial;

    /// <inheritdoc cref="CustomTeamHandler.OnRegistered"/>
    public override void OnRegistered()
    {
        base.OnRegistered();
        
        PlayerEvents.Death += Internal_Died;
        PlayerEvents.ChangedRole += Internal_ChangedRole;
        
        ExRoundEvents.Started += Internal_Started;
        
        LoadoutPlugin.Ensure("SerpentsHand", new LoadoutDefinition()
            .WithAmmo(ItemType.Ammo556x45, 120)
            .WithItems(ItemType.GunE11SR, ItemType.GrenadeHE, ItemType.KeycardChaosInsurgency, ItemType.Adrenaline, ItemType.SCP500, ItemType.SCP1344, ItemType.ArmorHeavy));
        
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

            SpawnPosition = centerPos;
            
            ApiLog.Debug("Serpent's Hand", $"Team Spawn Point set to &3{centerName}&r (&6{centerPos.ToPreciseString()}&r)");
            ApiLog.Debug("Serpent's Hand", $"Hole Spawn Point set to &3{holeName}&r (&6{holePos.ToPreciseString()}&r)");
            
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

        if (!ExRound.IsRunning)
            return;

        if (!args.OldRole.IsScp(false))
            return;

        if (SpawnPosition.GetZone() is not FacilityZone.Surface && Warhead.IsDetonated)
            return;

        if (SpawnPosition.GetZone() is FacilityZone.LightContainment && Decontamination.IsDecontaminating)
            return;

        TimingUtils.AfterSeconds(() =>
        {
            WasSpawned = Spawn(MinPlayers, MaxPlayers, player => player != args.Player).SpawnedWave != null;
        }, 0.2f);
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