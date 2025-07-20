using AdminToys;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

using LabExtended.API;
using LabExtended.API.Hints;

using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Extensions;
using LabExtended.Utilities;

using MapGeneration;

using ProjectMER.Features;
using ProjectMER.Features.Objects;

using UnityEngine;

using PlayerInteractedToyEventArgs = LabApi.Events.Arguments.PlayerEvents.PlayerInteractedToyEventArgs;

namespace PeanutClub.SpecialWaves.Waves.RedRightHand;

/// <summary>
/// Manages the button used to summon a wave.
/// </summary>
public static class RedRightHandButton
{
    /// <summary>
    /// Gets the name of the button schematic.
    /// </summary>
    public const string SchematicName = "RedRightHandButton";

    /// <summary>
    /// The maximum amount of players to spawn.
    /// </summary>
    public const int MaxPlayers = 6;
    
    /// <summary>
    /// Whether or not the button was used this round.
    /// </summary>
    public static bool WasUsed { get; private set; }
    
    /// <summary>
    /// Gets the spawned button schematic.
    /// </summary>
    public static SchematicObject? ButtonObject { get; private set; }
    
    /// <summary>
    /// Gets the spawned button interactable toy.
    /// </summary>
    public static InteractableToy? ButtonInteractable { get; private set; }

    private static void Internal_Interacted(PlayerSearchedToyEventArgs args)
    {
        if (WasUsed)
            return;

        if (args.Interactable?.Base == null || ButtonInteractable?.Base == null)
            return;

        if (args.Interactable.Base != ButtonInteractable.Base)
            return;

        if (args.Player is not ExPlayer player)
            return;

        if (!player.Inventory.HasItem(ItemType.KeycardO5))
        {
            ApiLog.Debug("Red Right Hand Button", $"Player &3{player.Nickname}&r (&6{player.UserId}&r) tried to spawn a wave without an O5 keycard.");
            
            player.SendHint("<color=red>Potřebuješ O5 kartu pro zavolání Red Right Hand!</color>", 5, true);
            return;
        }

        if (RedRightHandTeam.Singleton.Spawn(MaxPlayers, false, false) != null)
        {
            WasUsed = true;
            
            player.ShowHint("<b>Zavolal jsi <color=red>Red Right Hand</color>!</b>", 10);
            
            ApiLog.Debug("Red Right Hand Button",
                $"Player &3{player.Nickname}&r (&6{player.UserId}&r) called a new wave!");
        }
        else
        {
            player.ShowHint("<b>Nelze zavolat <color=red>Red Right Hand</color>, zkus to později!</b>", 10);
            
            ApiLog.Debug("Red Right Hand Button", "Could not spawn a new wave");
        }
    }
    
    private static void Internal_Started()
    {
        WasUsed = false;

        ButtonObject = null;
        
        if (MapUtilities.TryGet(SchematicName, out MapUtilities.NamedPosition namedPosition))
        {
            if (!RoomUtils.TryFindRoom(namedPosition.RoomName, namedPosition.RoomZone, namedPosition.RoomShape,
                    out var room))
            {
                ApiLog.Warn("Special Waves", $"Could not find the target room for the Red Right Hand spawn point!");
                return;
            }

            var position = room.transform.TransformPoint(namedPosition.Position.Vector);
            var rotation = Quaternion.Euler(0f, room.transform.rotation.eulerAngles.y + 180f, 0f);
            
            if (ObjectSpawner.TrySpawnSchematic(SchematicName, position, rotation, out var schematic))
            {
                ButtonObject = schematic;
                ButtonObject.AnimationController.Animators.ForEach(anim => ButtonObject.AnimationController.Stop(anim.name));
                
                ButtonInteractable = InteractableToy.Create(position, rotation, Vector3.one * 5f);
                ButtonInteractable.Shape = InvisibleInteractableToy.ColliderShape.Box;
                ButtonInteractable.InteractionDuration = 0.1f;

                ApiLog.Debug("Special Waves", $"Spawned the Red Right Hand button schematic at &3{position.ToPreciseString()}&r!");
            }
            else
            {
                ApiLog.Warn("Special Waves", "The Red Right Hand button schematic could not be spawned!");
            }
        }
        else
        {
            ApiLog.Warn("Special Waves", "Could not find the spawn point for Red Right Hand button schematic");
        }
    }
    
    internal static void Internal_Init()
    {
        ExRoundEvents.Started += Internal_Started;
        
        PlayerEvents.SearchedToy += Internal_Interacted;
    }
}