using LabExtended.API;

using mcx.Utilities.Actions.Interfaces;

using Utils;

namespace mcx.Utilities.Actions.Features.Misc
{
    /// <summary>
    /// Represents an action that triggers an explosion effect or damage on specified targets.
    /// </summary>
    public class ExplodeAction : IAction
    {
        /// <inheritdoc/>
        public string Id { get; } = "Explode";

        /// <inheritdoc/>
        public string DebugAction(Dictionary<string, string> parameters)
        {
            parameters["Reason"] = "The death reason for the explosion (e.g., 'Exploded').";
            parameters["Type"] = "The type of grenade to simulate (e.g., 'GrenadeHE').";
            parameters["Velocity"] = "The velocity multiplier for the explosion effect.";
            parameters["EffectOnly"] = "Whether to only show the explosion effect without causing damage.";

            return "Triggers an explosion effect or damage on specified targets.";
        }

        /// <inheritdoc/>
        public ActionResult Trigger(ref ActionContext context)
        {
            var reason = context.GetParameterOrDefault("Reason", 0, "Exploded");
            var type = context.GetParameterOrDefault("Type", 0, Enum.TryParse, ItemType.GrenadeHE);
            var velocity = context.GetParameterOrDefault("Velocity", 0, float.TryParse, 1f);
            var effect = context.GetParameterOrDefault("EffectOnly", 0, bool.TryParse, true);

            foreach (var target in context.Targets)
            {
                if (target.IsPlayer(out var player))
                {
                    player.Player.Explode(type, reason, effect, velocity);
                }
                else if (target.IsPosition(out var position))
                {
                    if (effect)
                    {
                        ExplosionUtils.ServerSpawnEffect(position.Position, type);
                    }
                    else
                    {
                        ExplosionUtils.ServerExplode(position.Position, ExPlayer.Host.Footprint, ExplosionType.Grenade);
                    }
                }
            }

            return ActionResult.Success;
        }
    }
}