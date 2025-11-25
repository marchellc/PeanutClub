using Interactables.Interobjects.DoorUtils;

using InventorySystem.Items.Pickups;

using LabExtended.API;
using LabExtended.Extensions;
using LabExtended.Utilities.Update;

using UnityEngine;

namespace SecretLabAPI.Rays
{
    /// <summary>
    /// Provides static methods and events for performing raycast-based searches and interactions in the game, such as
    /// detecting item pickups, doors, and players within a specified distance from a player.
    /// </summary>
    public static class RayManager
    {
        private static int layerMask;
        private static int remainingFrames;

        private static PlayerUpdateComponent component = PlayerUpdateComponent.Create();

        /// <summary>
        /// Occurs when a hit is successfully detected by an ExPlayer during a raycast operation.
        /// </summary>
        public static event Action<ExPlayer, RaycastHit> HitSuccess;

        public static event Action? FrameFinished;

        /// <summary>
        /// Attempts to find an item pickup within a specified distance from the player and returns whether a pickup was
        /// found.
        /// </summary>
        /// <param name="source">The player from which the search for an item pickup originates.</param>
        /// <param name="distance">The maximum distance, in units, to search for an item pickup from the player's position.</param>
        /// <param name="mask">An optional layer mask used to filter which objects are considered during the search. If null, a default
        /// mask is used.</param>
        /// <param name="pickup">When this method returns, contains the found item pickup if one is located within the specified distance;
        /// otherwise, null.</param>
        /// <returns>true if an item pickup is found within the specified distance; otherwise, false.</returns>
        public static bool TryGetPickup(this ExPlayer source, float distance, int? mask, out ItemPickupBase? pickup)
        {
            pickup = null;

            if (!TryCast(source, distance, SecretLab.Config.RayManagerForwardOffset, mask ?? layerMask, out var hit))
                return false;

            return hit.collider.gameObject.TryFindComponent<ItemPickupBase>(out pickup);
        }

        /// <summary>
        /// Attempts to find a door within the specified distance from the player and returns information about the door
        /// if found.
        /// </summary>
        /// <param name="source">The player instance from which the search for a door is performed.</param>
        /// <param name="distance">The maximum distance, in units, to search for a door from the player's position.</param>
        /// <param name="mask">An optional layer mask used to filter which objects are considered during the search. If null, a default
        /// mask is used.</param>
        /// <param name="target">When this method returns, contains the found door variant if a door is detected within the specified
        /// distance; otherwise, null.</param>
        /// <returns>true if a door is found within the specified distance; otherwise, false.</returns>
        public static bool TryGetDoor(this ExPlayer source, float distance, int? mask, out DoorVariant? target)
        {
            target = null;

            if (!TryCast(source, distance, SecretLab.Config.RayManagerForwardOffset, mask ?? layerMask, out var hit))
                return false;

            return hit.collider.gameObject.TryFindComponent<DoorVariant>(out target);
        }

        /// <summary>
        /// Attempts to find another player within the specified distance from the source player, using an optional
        /// layer mask.
        /// </summary>
        /// <param name="source">The source player from which to search for another player.</param>
        /// <param name="distance">The maximum distance, in units, within which to search for a target player.</param>
        /// <param name="mask">An optional layer mask used to filter which objects are considered during the search. If null, a default
        /// mask is used.</param>
        /// <param name="target">When this method returns, contains the found player if one is detected within the specified distance and
        /// mask; otherwise, null.</param>
        /// <returns>true if a target player is found within the specified distance and mask; otherwise, false.</returns>
        public static bool TryGetPlayer(this ExPlayer source, float distance, int? mask, out ExPlayer? target)
        {
            target = null;

            if (!TryCast(source, distance, SecretLab.Config.RayManagerForwardOffset, mask ?? layerMask, out var hit))
                return false;

            if (!hit.collider.gameObject.TryFindComponent<ReferenceHub>(out var hub))
                return false;

            return ExPlayer.TryGet(hub, out target);
        }

        /// <summary>
        /// Attempts to perform a raycast from the player's camera position in the forward direction, optionally
        /// offsetting the direction, and returns whether a collider was hit.
        /// </summary>
        /// <remarks>The method modifies the ray direction by the specified forward offset before
        /// performing the raycast. The hit information is only valid if the method returns true.</remarks>
        /// <param name="player">The player from whose camera position and orientation the raycast is performed.</param>
        /// <param name="distance">The maximum distance, in world units, that the ray should check for collisions.</param>
        /// <param name="forwardOffset">The amount by which to offset the ray's forward direction along each axis before casting. Must be zero or
        /// positive.</param>
        /// <param name="layerMask">A bitmask that specifies which layers to include in the raycast collision check.</param>
        /// <param name="hit">When this method returns, contains information about the raycast hit if a collider was detected; otherwise,
        /// contains default values.</param>
        /// <returns>true if the raycast hit a collider; otherwise, false.</returns>
        public static bool TryCast(this ExPlayer player, float distance, float forwardOffset, int layerMask, out RaycastHit hit)
        {
            hit = default;

            var position = player.Position.Position;
            var direction = player.CameraTransform.forward;

            if (forwardOffset > 0f)
            {
                if (direction.x > 0f)
                    direction.x += forwardOffset;

                if (direction.y > 0f)
                    direction.y += forwardOffset;

                if (direction.z > 0f)
                    direction.z += forwardOffset;
            }

            if (!Physics.Raycast(position, direction, out hit, distance, layerMask))
                return false;

            if (hit.collider == null)
                return false;

            return true;
        }

        private static void OnUpdate()
        {
            if (ExPlayer.Count < 1)
                return;

            if (SecretLab.Config.RayManagerFrameSkip > 0)
            {
                if (remainingFrames > 0)
                {
                    remainingFrames--;
                    return;
                }

                remainingFrames = SecretLab.Config.RayManagerFrameSkip;
            }

            for (var i = 0; i < ExPlayer.Count; i++)
            {
                var player = ExPlayer.Players[i];

                if (player?.ReferenceHub == null || !player.IsAlive)
                    continue;

                if (!TryCast(player, SecretLab.Config.RayManagerDistance, SecretLab.Config.RayManagerForwardOffset, layerMask, out var hit))
                    continue;

                HitSuccess?.InvokeSafe(player, hit);

                if (!hit.collider.gameObject.TryFindComponent<RayComponent>(out var component))
                    continue;

                component.OnHit(player);
            }

            FrameFinished?.InvokeSafe();
        }

        internal static void Initialize()
        {
            layerMask = LayerMask.GetMask(SecretLab.Config.RayManagerLayers);

            component.OnLateUpdate += OnUpdate;
        }
    }
}