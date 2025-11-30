using LabExtended.API;

using SecretLabAPI.Actions.API;
using SecretLabAPI.Actions.Attributes;

using UnityEngine;

using Utils;

namespace SecretLabAPI.Actions.Functions
{
    /// <summary>
    /// Provides map-related utility functions for performing actions such as spawning projectiles at player positions.
    /// </summary>
    /// <remarks>This static class contains methods that operate on game map contexts, enabling scripted
    /// actions to be performed for all players or entities within the context. All methods require a properly compiled
    /// action context with valid parameters. The class is intended for use in scenarios where map-based actions need to
    /// be triggered programmatically, such as in custom game modes or automated events.</remarks>
    public static class MapFunctions
    {
        /// <summary>
        /// Spawns an explosion of the specified type at the position of each player in the current action context.
        /// </summary>
        /// <remarks>The explosion type is determined by the 'Type' parameter in the context. This method
        /// affects all players present in the context and may have gameplay implications depending on the explosion
        /// type used.</remarks>
        /// <param name="context">The action context containing player information and parameters. Must be compiled and include a valid
        /// explosion type.</param>
        /// <returns>An ActionResultFlags value indicating the result of the operation. Returns SuccessDispose if explosions were
        /// spawned successfully.</returns>
        [Action("SpawnExplosion", "Spawns an explosion at each player's position.")]
        [ActionParameter("Type", "The type of explosion to spawn.")]
        public static ActionResultFlags SpawnExplosion(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) =>
            {
                return index switch
                {
                    0 => p.EnsureCompiled(Enum.TryParse, ExplosionType.Grenade), // Type

                    _ => false
                };
            });

            var type = context.GetValue<ExplosionType>(0);

            ExplosionUtils.ServerExplode(context.Player.ReferenceHub, type);
            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Spawns the specified item type at each player's current position, creating the given number of items per
        /// player with the specified scale.
        /// </summary>
        /// <remarks>The method uses the parameters 'Type', 'Amount', and 'Scale' from the context to
        /// determine the item type, quantity, and scale for spawning. Items are spawned for each player present in the
        /// context. If the item type is None or the amount is not positive, no items are spawned and the action
        /// completes successfully.</remarks>
        /// <param name="context">A reference to the action context containing parameters for item type, amount, and scale, as well as player
        /// information.</param>
        /// <returns>An ActionResultFlags value indicating the result of the spawn operation. Returns SuccessDispose if the
        /// action completes or if the item type is None or amount is less than or equal to zero.</returns>
        [Action("SpawnItem", "Spawns an item at each player's position.")]
        [ActionParameter("Type", "The type of item to spawn.")]
        [ActionParameter("Amount", "The amount of items to spawn.")]
        [ActionParameter("Scale", "The scale of the spawned item.")]
        public static ActionResultFlags SpawnItem(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) =>
            {
                return index switch
                {
                    0 => p.EnsureCompiled(Enum.TryParse, ItemType.None), // Type
                    1 => p.EnsureCompiled(int.TryParse, 1), // Amount
                    2 => p.EnsureCompiled(SecretLabAPI.Extensions.StringExtensions.TryParseVector3, Vector3.one), // Scale

                    _ => false
                };
            });

            var type = context.GetMemoryOrValue<ItemType>("ItemType", 0);
            var amount = context.GetMemoryOrValue<int>("ItemAmount", 1);
            var scale = context.GetMemoryOrValue<Vector3>("ItemScale", 2);

            if (type is ItemType.None || amount <= 0)
                return ActionResultFlags.SuccessDispose;

            for (var i = 0; i < amount; i++)
                ExMap.SpawnItem(type, context.Player.Position, scale, context.Player.Rotation);

            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Spawns a projectile of the specified type at each player's position, applying the given force and fuse time.
        /// </summary>
        /// <remarks>The method retrieves parameters from the context: 'Type' (projectile type), 'Amount'
        /// (number of projectiles), 'Force' (applied force), and 'Fuse' (fuse time). All players in the context will
        /// receive a projectile at their current position. Ensure that the context is properly compiled and contains
        /// valid parameter values before calling this method.</remarks>
        /// <param name="context">The action context containing parameters for projectile type, amount, force, and fuse time. Must be compiled
        /// before invocation.</param>
        /// <returns>An ActionResultFlags value indicating the result of the action. Returns SuccessDispose if the projectiles
        /// were spawned successfully.</returns>
        [Action("SpawnProjectile", "Spawns a projectile at each player's position.")]
        [ActionParameter("Type", "The type of projectile to spawn.")]
        [ActionParameter("Amount", "The amount of projectiles to spawn.")]
        [ActionParameter("Force", "The force to apply to the projectile.")]
        [ActionParameter("Fuse", "The fuse time of the projectile.")]
        public static ActionResultFlags SpawnProjectile(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) =>
            {
                return index switch
                {
                    0 => p.EnsureCompiled(Enum.TryParse, ItemType.GrenadeHE),
                    1 => p.EnsureCompiled(int.TryParse, 1),
                    2 => p.EnsureCompiled(float.TryParse, 10f),
                    3 => p.EnsureCompiled(float.TryParse, 3f),

                    _ => false
                };
            });

            var type = context.GetMemoryOrValue<ItemType>("Type", 0);
            var amount = context.GetMemoryOrValue<int>("Amount", 1);
            var force = context.GetMemoryOrValue<float>("Force", 2);
            var fuse = context.GetMemoryOrValue<float>("Fuse", 3);
            var velocity = context.GetValue<float>(4);

            var vectorVelocity = context.Player.Velocity;

            if (vectorVelocity.x > 0f)
                vectorVelocity.x *= velocity;

            if (vectorVelocity.y > 0f)
                vectorVelocity.y *= velocity;

            if (vectorVelocity.z > 0f)
                vectorVelocity.z *= velocity;

            for (var i = 0; i < amount; i++)
                ExMap.SpawnProjectile(type, context.Player.Position, Vector3.one, vectorVelocity, context.Player.Rotation, force, fuse);

            return ActionResultFlags.SuccessDispose;
        }
    }
}