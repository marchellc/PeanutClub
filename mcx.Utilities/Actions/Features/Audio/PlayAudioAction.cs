using mcx.Utilities.Audio;
using mcx.Utilities.Actions.Interfaces;

namespace mcx.Utilities.Actions.Features.Audio
{
    /// <summary>
    /// Plays an audio clip.
    /// </summary>
    public class PlayAudioAction : IAction
    {
        /// <inheritdoc/>
        public string Id { get; } = "PlayAudio";

        /// <inheritdoc/>
        public string DebugAction(Dictionary<string, string> parameters)
        {
            parameters["Clip"] = "The name of the audio clip to play. Has to be defineed in the server's clip config if the target is a player.";
            parameters["Personal"] = "Whether the audio clip should only be heard by the target player(s). Defaults to false.";
            parameters["SendToOthers"] = "Whether the audio clip should be sent to other players around the target player(s) if Personal is true. Defaults to true.";
            
            return "Plays an audio clip for the target player(s) or at the target position(s).";
        }

        /// <inheritdoc/>
        public ActionResult Trigger(ref ActionContext context)
        {
            var clip = context.GetParameterOrDefault("Clip", 0, string.Empty);
            var isPersonal = context.GetParameterOrDefault("Personal", 0, bool.TryParse, false);
            var sendToOthers = context.GetParameterOrDefault("SendToOthers", 0, bool.TryParse, true);

            if (clip == string.Empty)
                return ActionResult.Failure;

            foreach (var target in context.Targets)
            {
                if (target.IsPlayer(out var player))
                {
                    player.Player.PlayClip(clip, 1f, isPersonal, sendToOthers);
                }
                else if (target.IsPosition(out var position))
                {
                    PlaybackUtils.PlayAt(clip, position.Position);
                }
            }

            return ActionResult.Success;
        }
    }
}