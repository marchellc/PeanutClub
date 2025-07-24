using InventorySystem.Items.Pickups;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;
using LabExtended.API.Hints;
using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Utilities;
using LabExtended.Extensions;
using LabExtended.Attributes;

using PeanutClub.SpecialWaves.Utilities;

using UnityEngine;

namespace PeanutClub.SpecialWaves.Waves.Archangels;

/// <summary>
/// Manages the spawning radio.
/// </summary>
public static class ArchangelsRadio
{
    /// <summary>
    /// The custom radio item tag.
    /// </summary>
    public const string ItemTag = "ArchangelsRadio";

    /// <summary>
    /// The maximum amount of players to summon once used.
    /// </summary>
    public static int MaxPlayers => PluginCore.StaticConfig.ArchangelsMaxPlayers;
    
    /// <summary>
    /// Gets the spawned radio item.
    /// </summary>
    public static ItemPickupBase? SpawnedRadio { get; private set; }
    
    /// <summary>
    /// Whether or not the radio was already used this round.
    /// </summary>
    public static bool WasUsed { get; private set; }

    /// <summary>
    /// Gets called once a player succesfully uses the radio.
    /// </summary>
    public static event Action<ExPlayer>? Used;

    /// <summary>
    /// Gets called once a player fails to use the radio (not enough players to spawn a wave, etc.)
    /// </summary>
    public static event Action<ExPlayer>? Failed; 
    
    private static void Internal_Using(PlayerUsingRadioEventArgs args)
    {
        if (WasUsed || args.RadioItem?.Base == null || !args.RadioItem.HasTag(ItemTag))
            return;

        if (args.Player is not ExPlayer player)
            return;

        if (ArchangelsTeam.Singleton.Spawn(MaxPlayers, false, false) != null)
        {
            WasUsed = true;

            (args.Player as ExPlayer)?.ShowHint("<b>Zavolal jsi <color=green>Archangels</color>!</b>", 5);

            Used?.InvokeSafe(player);
            
            ApiLog.Debug("Archangels Radio",
                $"Player &3{args.Player.Nickname}&r (&6{args.Player.UserId}&r) used the radio!");
        }
        else
        {
            Failed?.InvokeSafe(player);
            
            (args.Player as ExPlayer)?.ShowHint("<b>Nelze zavolat <color=green>Archangels</color>, zkus to znovu později!</b>", 5);
        }
    }

    private static void Internal_Started()
    {
        WasUsed = false;
        SpawnedRadio = null;
        
        if (MapUtilities.TryGet(ItemTag, null, out Vector3 position, out Quaternion rotation))
        {
            SpawnedRadio = ExMap.SpawnItem(ItemType.Radio, position, Vector3.one, rotation);
            SpawnedRadio.SetTag(ItemTag);
            
            ApiLog.Debug("Archangels Radio", $"Spawned the radio at &3{position.ToPreciseString()}&r");
        }
        else
        {
            ApiLog.Warn("Special Waves", "Could not find the spawn point for Archangels radio");
        }
    }
    
    internal static void Internal_Init()
    {
        ExRoundEvents.Started += Internal_Started;
        PlayerEvents.UsingRadio += Internal_Using;
    }
}