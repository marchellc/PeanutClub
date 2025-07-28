using AdminToys;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;
using LabExtended.API.Toys;
using LabExtended.API.Hints;

using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Utilities;
using LabExtended.Extensions;
using PeanutClub.OverlayAPI.Alerts;
using ProjectMER.Features;
using ProjectMER.Features.Objects;

using UnityEngine;

namespace PeanutClub.SpecialWaves.Waves.Archangels;

/// <summary>
/// Manages the spawning radio.
/// </summary>
public static class ArchangelsRadio
{
    /// <summary>
    /// Gets the number to divide the scale of the interactable toy by.
    /// </summary>
    public const float ScaleFactor = 8.5f;
    
    /// <summary>
    /// Gets the name of the radio schematic.
    /// </summary>
    public static string SchematicName => PluginCore.StaticConfig.ArchangelsSchematicName;

    /// <summary>
    /// Gets the name of the radio position.
    /// </summary>
    public static string PositionName => PluginCore.StaticConfig.ArchangelsPositionName;

    /// <summary>
    /// The maximum amount of players to summon once used.
    /// </summary>
    public static int MaxPlayers => PluginCore.StaticConfig.ArchangelsMaxPlayers;

    /// <summary>
    /// The minimum amount of players required to summon.
    /// </summary>
    public static int MinPlayers => PluginCore.StaticConfig.ArchangelsMinPlayers;
    
    /// <summary>
    /// Whether or not the radio was already used this round.
    /// </summary>
    public static bool WasUsed { get; private set; }
    
    /// <summary>
    /// Gets the spawned radio schematic.
    /// </summary>
    public static SchematicObject? RadioObject { get; private set; }

    /// <summary>
    /// Gets the spawned radio interactable toy.
    /// </summary>
    public static InteractableToy? RadioInteractable { get; private set; }

    /// <summary>
    /// Gets called once a player succesfully uses the radio.
    /// </summary>
    public static event Action<ExPlayer>? Used;

    /// <summary>
    /// Gets called once a player fails to use the radio (not enough players to spawn a wave, etc.)
    /// </summary>
    public static event Action<ExPlayer>? Failed; 

    private static void Internal_Interacted(PlayerSearchedToyEventArgs args)
    {
        if (WasUsed)
            return;

        if (args.Interactable?.Base == null || RadioInteractable?.Base == null)
            return;

        if (args.Interactable.Base != RadioInteractable.Base)
            return;

        if (args.Player is not ExPlayer player)
            return;

        if (ArchangelsTeam.Singleton.Spawn(MinPlayers, MaxPlayers).SpawnedWave != null)
        {
            WasUsed = true;
            
            player.SendAlert(AlertType.Info, 10f, "<b><color=green>Úspěšně</color> jsi zavolal</b>\n<color=green><b>Archangels</b></color>!");
            
            Used?.InvokeSafe(player);
            
            ApiLog.Debug("Archangels Radio",
                $"Player &3{player.Nickname}&r (&6{player.UserId}&r) called a new wave!");
        }
        else
        {
            player.SendAlert(AlertType.Warn, 10f, "Aktuálně <color=red>nelze</color> zavolat tým <color=green>Archangels</color>, zkus to znova později!");
            
            Failed?.InvokeSafe(player);
            
            ApiLog.Debug("Archangels Radio", "Could not spawn a new wave");
        }
    }
    
    private static void Internal_Started()
    {
        WasUsed = false;

        RadioObject = null;
        RadioInteractable = null;
        
        if (MapUtilities.TryGet(PositionName, null, out Vector3 position, out Quaternion rotation))
        {
            if (ObjectSpawner.TrySpawnSchematic(SchematicName, position, rotation, out var schematic))
            {
                RadioObject = schematic;
                
                RadioInteractable = new(position, rotation)
                {
                    InteractionDuration = 1f,
                    Scale = Vector3.one / ScaleFactor,
                    Shape = InvisibleInteractableToy.ColliderShape.Box,
                };
            }
            else
            {
                ApiLog.Warn("Archangels Radio", "Could not spawn the radio schematic");
            }
        }
        else
        {
            ApiLog.Warn("Archangels Radio", "Could not find the spawn point for Archangels radio");
        }
    }
    
    internal static void Internal_Init()
    {
        ExRoundEvents.Started += Internal_Started;
        PlayerEvents.SearchedToy += Internal_Interacted;
    }
}