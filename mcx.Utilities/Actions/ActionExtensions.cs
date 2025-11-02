using mcx.Utilities.Actions.Interfaces;
using mcx.Utilities.Actions.Targets;

using UnityEngine;

namespace mcx.Utilities.Actions
{
    /// <summary>
    /// Extensions targeting the Actions API.
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        /// Determines whether the specified action target represents a player.
        /// </summary>
        /// <param name="target">The action target to evaluate. Cannot be null.</param>
        /// <returns>true if the target is a player; otherwise, false.</returns>
        public static bool IsPlayer(this IActionTarget target)
            => target is TargetPlayer;

        /// <summary>
        /// Determines whether the specified action target represents a player and retrieves the corresponding player
        /// target if available.
        /// </summary>
        /// <param name="target">The action target to evaluate.</param>
        /// <param name="playerTarget">When this method returns, contains the player target if <paramref name="target"/> is a player; otherwise,
        /// the default value.</param>
        /// <returns><see langword="true"/> if <paramref name="target"/> is a player; otherwise, <see langword="false"/>.</returns>
        public static bool IsPlayer(this IActionTarget target, out TargetPlayer playerTarget)
        {
            if (target is TargetPlayer player)
            {
                playerTarget = player;
                return true;
            }

            playerTarget = default;
            return false;
        }

        /// <summary>
        /// Determines whether the specified action target represents a pickup target.
        /// </summary>
        /// <param name="target">The action target to evaluate. Cannot be null.</param>
        /// <returns>true if the target is a pickup target; otherwise, false.</returns>
        public static bool IsPickup(this IActionTarget target)
            => target is TargetPickup;

        /// <summary>
        /// Determines whether the specified action target is a pickup target and provides the corresponding pickup
        /// target if available.
        /// </summary>
        /// <param name="target">The action target to evaluate.</param>
        /// <param name="pickupTarget">When this method returns, contains the pickup target if <paramref name="target"/> is a <c>TargetPickup</c>;
        /// otherwise, the default value for <c>TargetPickup</c>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if <paramref name="target"/> is a <c>TargetPickup</c>; otherwise, <see
        /// langword="false"/>.</returns>
        public static bool IsPickup(this IActionTarget target, out TargetPickup pickupTarget)
        {
            if (target is TargetPickup pickup)
            {
                pickupTarget = pickup;
                return true;
            }

            pickupTarget = default;
            return false;
        }

        /// <summary>
        /// Determines whether the specified action target represents a position.
        /// </summary>
        /// <param name="target">The action target to evaluate. Cannot be null.</param>
        /// <returns>true if the action target is a position; otherwise, false.</returns>
        public static bool IsPosition(this IActionTarget target)
            => target is TargetPosition;

        /// <summary>
        /// Determines whether the specified action target is a position target and, if so, returns it as a
        /// TargetPosition instance.
        /// </summary>
        /// <param name="target">The action target to evaluate.</param>
        /// <param name="positionTarget">When this method returns, contains the TargetPosition instance if the target is a position target;
        /// otherwise, the default value for TargetPosition.</param>
        /// <returns>true if the target is a TargetPosition; otherwise, false.</returns>
        public static bool IsPosition(this IActionTarget target, out TargetPosition positionTarget)
        {
            if (target is TargetPosition position)
            {
                positionTarget = position;
                return true;
            }
            positionTarget = default;
            return false;
        }

        /// <summary>
        /// Gets the world position associated with the specified action target.
        /// </summary>
        /// <param name="target">The action target from which to retrieve the position.</param>
        /// <param name="cameraPositionForPlayer">true to return the player's camera position if the target is a player; otherwise, false to return the
        /// player's main transform position. This parameter is ignored for non-player targets.</param>
        /// <returns>A Vector3 representing the position of the target. For player targets, returns either the camera or main
        /// transform position based on the value of cameraPositionForPlayer.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the target does not represent a position, player, or pickup.</exception>
        public static Vector3 GetPosition(this IActionTarget target, bool cameraPositionForPlayer = false)
        {
            if (target.IsPosition(out var positionTarget))
                return positionTarget.Position;

            if (target.IsPlayer(out var playerTarget))
                return cameraPositionForPlayer 
                    ? playerTarget.Player.CameraTransform.position
                    : playerTarget.Player.Transform.position;

            if (target.IsPickup(out var pickupTarget))
                return pickupTarget.Pickup.Position;

            throw new InvalidOperationException("The target does not have a position.");
        }

        /// <summary>
        /// Gets the world rotation of the specified action target.
        /// </summary>
        /// <param name="target">The action target from which to retrieve the rotation.</param>
        /// <param name="cameraRotationForPlayer">true to return the player's camera rotation if the target is a player; otherwise, false to return the
        /// player's body rotation. Ignored for non-player targets.</param>
        /// <returns>A Quaternion representing the rotation of the target. For player targets, returns either the camera or body
        /// rotation based on the cameraRotationForPlayer parameter. For pickup targets, returns the pickup's rotation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the target does not represent a player or a pickup.</exception>
        public static Quaternion GetRotation(this IActionTarget target, bool cameraRotationForPlayer = false)
        {
            if (target.IsPlayer(out var playerTarget))
                return cameraRotationForPlayer 
                    ? playerTarget.Player.CameraTransform.rotation
                    : playerTarget.Player.Transform.rotation;

            if (target.IsPickup(out var pickupTarget))
                return pickupTarget.Pickup.Rotation;

            throw new InvalidOperationException("The target does not have a rotation.");
        }

        /// <summary>
        /// Sets the position of the specified action target to the given coordinates.
        /// </summary>
        /// <param name="target">The action target whose position will be set. Must support position assignment.</param>
        /// <param name="position">The new position to assign to the target.</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified target does not support position assignment.</exception>
        public static void SetPosition(this IActionTarget target, Vector3 position)
        {
            if (target.IsPosition(out var positionTarget))
            {
                positionTarget.Position = position;
                return;
            }

            if (target.IsPlayer(out var playerTarget))
            {
                playerTarget.Player.Transform.position = position;
                return;
            }

            if (target.IsPickup(out var pickupTarget))
            {
                pickupTarget.Pickup.Position = position;
                return;
            }

            throw new InvalidOperationException("The target does not have a position to set.");
        }

        /// <summary>
        /// Sets the rotation of the specified action target, if supported.
        /// </summary>
        /// <remarks>This method applies the specified rotation to the target if it is a player or a
        /// pickup. For other types of targets, an exception is thrown.</remarks>
        /// <param name="target">The action target whose rotation is to be set. Must represent a player or a pickup that supports rotation.</param>
        /// <param name="rotation">The new rotation to apply to the target.</param>
        /// <exception cref="InvalidOperationException">Thrown if the target does not support setting a rotation.</exception>
        public static void SetRotation(this IActionTarget target, Quaternion rotation)
        {
            if (target.IsPlayer(out var playerTarget))
            {
                playerTarget.Player.Transform.rotation = rotation;
                return;
            }

            if (target.IsPickup(out var pickupTarget))
            {
                pickupTarget.Pickup.Rotation = rotation;
                return;
            }

            throw new InvalidOperationException("The target does not have a rotation to set.");
        }
    }
}