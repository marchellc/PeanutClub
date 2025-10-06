using LabExtended.API;

using PlayerStatsSystem;

using UnityEngine;

using Utils;

namespace mcx.Utilities.Items
{
    /// <summary>
    /// Provides static methods and utilities for creating and managing explosion-related visual or audio effects.
    /// </summary>
    public static class ExplosionEffects
    {
        /// <summary>
        /// Triggers an explosion at the player's position, simulating the detonation of the specified grenade type and
        /// optionally causing the player's death with a custom reason.
        /// </summary>
        /// <remarks>If the player is not alive, this method has no effect. The explosion type and visual
        /// effect are determined by the specified grenade type. When <paramref name="effectOnly"/> is <see
        /// langword="false"/>, the player is killed with the provided death reason and a simulated explosion
        /// force.</remarks>
        /// <param name="player">The player who will be affected by the explosion. Must be alive for the explosion to occur.</param>
        /// <param name="grenadeType">The type of grenade to simulate for the explosion effect. Determines the visual and damage type of the
        /// explosion.</param>
        /// <param name="deathReason">The custom reason to record for the player's death if the explosion is lethal.</param>
        /// <param name="effectOnly">If <see langword="true"/>, only the explosion effect is shown and the player is not killed. If <see
        /// langword="false"/>, the player is killed by the explosion.</param>
        public static void Explode(this ExPlayer player, ItemType grenadeType, string deathReason, bool effectOnly = false)
        {
            if (!player.Role.IsAlive)
                return;

            if (effectOnly)
            {
                ExplosionUtils.ServerSpawnEffect(player.Position, grenadeType);
            }
            else
            {
                var explosionType = ExplosionType.Grenade;

                switch (grenadeType)
                {
                    case ItemType.ParticleDisruptor:
                        explosionType = ExplosionType.Disruptor;
                        break;

                    case ItemType.SCP018:
                        explosionType = ExplosionType.SCP018;
                        break;

                    case ItemType.SCP207:
                        explosionType = ExplosionType.Cola;
                        break;

                    case ItemType.SCP330:
                        explosionType = ExplosionType.PinkCandy;
                        break;

                    case ItemType.Jailbird:
                        explosionType = ExplosionType.Jailbird;
                        break;
                }

                ExplosionUtils.ServerExplode(player.Position, player.Footprint, explosionType);
            }

            var velocity = player.Rotation.Rotation * Vector3.back;

            velocity.y = 1f;
            velocity.Normalize();

            velocity *= 5f;
            velocity.y += 2f;

            var damageHandler = new CustomReasonDamageHandler(deathReason, -1f);

            damageHandler.ApplyDamage(player.ReferenceHub);
            damageHandler.StartVelocity = velocity;

            player.ReferenceHub.playerStats.KillPlayer(damageHandler);
        }
    }
}