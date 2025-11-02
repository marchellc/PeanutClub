using mcx.Utilities.Audio;
using mcx.Utilities.Actions.Interfaces;

namespace mcx.Utilities.Actions.Features.Audio
{
    /// <summary>
    /// Stops audio playback for the specified targets.
    /// </summary>
    public class StopAudioAction : IAction
    {
        /// <inheritdoc/>
        public string Id { get; } = "StopAudio";

        /// <inheritdoc/>
        public string DebugAction(Dictionary<string, string> parameters)
        {
            return "Stops playback of player audio clips - works only on players.";
        }

        /// <inheritdoc/>
        public ActionResult Trigger(ref ActionContext context)
        {
            foreach (var target in context.Targets)
            {
                if (target.IsPlayer(out var player))
                {
                    player.Player.StopClip(out _);
                }
            }

            return ActionResult.Success;
        }
    }
}