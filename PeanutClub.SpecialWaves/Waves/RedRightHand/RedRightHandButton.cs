using AdminToys;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;
using LabExtended.API.Toys;
using LabExtended.API.Hints;

using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Extensions;
using LabExtended.Utilities;

using ProjectMER.Features;
using ProjectMER.Features.Objects;

using UnityEngine;

namespace PeanutClub.SpecialWaves.Waves.RedRightHand;

/// <summary>
/// Manages the button used to summon a wave.
/// </summary>
public static class RedRightHandButton
{
    /// <summary>
    /// Gets the name of the button schematic.
    /// </summary>
    public static string SchematicName => PluginCore.StaticConfig.RedRightHandButtonSchematicName;
    
    /// <summary>
    /// Gets the name of the button position.
    /// </summary>
    public static string PositionName => PluginCore.StaticConfig.RedRightHandButtonPositionName;

    /// <summary>
    /// Gets the name of the button animator.
    /// </summary>
    public static string AnimatorName => PluginCore.StaticConfig.RedRightHandButtonAnimatorName;

    /// <summary>
    /// Gets the name of the button press animation.
    /// </summary>
    public static string PressAnimationName => PluginCore.StaticConfig.RedRightHandButtonPressAnimationName;
    
    /// <summary>
    /// Gets the name of the button press animation.
    /// </summary>
    public static string IdleAnimationName => PluginCore.StaticConfig.RedRightHandButtonIdleAnimationName;

    /// <summary>
    /// The maximum amount of players to spawn.
    /// </summary>
    public static int MaxPlayers => PluginCore.StaticConfig.RedRightHandMaxPlayers;

    /// <summary>
    /// The spawn angle of the button.
    /// </summary>
    public static float ButtonAngle => PluginCore.StaticConfig.RedRightHandButtonAngle;
    
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

    /// <summary>
    /// Gets called once the button is succesfully used.
    /// </summary>
    public static event Action<ExPlayer>? Used;

    /// <summary>
    /// Gets called once the button is used resulting in a fail (missing O5 keycard, not enough players to spawn etc.).
    /// </summary>
    public static event Action<ExPlayer>? Failed; 

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

        try
        {
            ButtonObject?.AnimationController.Play(PressAnimationName, AnimatorName);
        }
        catch
        {
            ApiLog.Warn("Red Right Hand Button", "Could not play the button press animation!");
        }

        if (!player.Inventory!.HasItem(ItemType.KeycardO5))
        {
            ApiLog.Debug("Red Right Hand Button", $"Player &3{player.Nickname}&r (&6{player.UserId}&r) tried to spawn a wave without an O5 keycard.");
            
            player.SendHint("<color=red>Potřebuješ O5 kartu pro zavolání Red Right Hand!</color>", 5, true);
            
            Failed?.InvokeSafe(player);
            return;
        }

        if (RedRightHandTeam.Singleton.Spawn(MaxPlayers, false, false) != null)
        {
            WasUsed = true;
            
            player.ShowHint("<b>Zavolal jsi <color=red>Red Right Hand</color>!</b>", 10);
            
            Used?.InvokeSafe(player);
            
            ApiLog.Debug("Red Right Hand Button",
                $"Player &3{player.Nickname}&r (&6{player.UserId}&r) called a new wave!");
        }
        else
        {
            player.ShowHint("<b>Nelze zavolat <color=red>Red Right Hand</color>, zkus to později!</b>", 10);
            
            Failed?.InvokeSafe(player);
            
            ApiLog.Debug("Red Right Hand Button", "Could not spawn a new wave");
        }
    }
    
    private static void Internal_Started()
    {
        WasUsed = false;

        ButtonObject = null;
        
        if (MapUtilities.TryGet(PositionName, ButtonAngle, out Vector3 position, out Quaternion rotation))
        {
            if (ObjectSpawner.TrySpawnSchematic(SchematicName, position, rotation, out var schematic))
            {
                ButtonObject = schematic;
                
                try
                {
                    ButtonObject?.AnimationController.Play(IdleAnimationName, AnimatorName);
                }
                catch
                {
                    ApiLog.Warn("Red Right Hand Button", "Could not play the button idle animation!");
                }

                ButtonInteractable = new(position, rotation) { Scale = Vector3.one / 8.5f };
                ButtonInteractable.Shape = InvisibleInteractableToy.ColliderShape.Box;
                ButtonInteractable.InteractionDuration = 1f;

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