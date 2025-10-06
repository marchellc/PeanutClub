using LabExtended.API;
using LabExtended.API.Toys;
using LabExtended.API.Images;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

namespace mcx.Utilities.Features.NextBots
{
    /// <summary>
    /// Next Bots commands.
    /// </summary>
    [Command("nextbots", "Commands related to NextBots", "nb")]
    public class NextBotCommand : CommandBase, IServerSideCommand
    {
        [CommandOverload("spawn", "Spawns a new next bot.")]
        public void Spawn(
            [CommandParameter("Image", "Name of the image file.")] string imageName, 
            [CommandParameter("Audio", "Name of the audio file.")] string audioName)
        {
            if (!ImageLoader.TryGet(imageName, out var image))
            {
                Fail($"'{imageName}' is not a valid image name!");
                return;
            }

            var toy = new TextToy(Sender.Position, Sender.Rotation);
            var bot = new NextBotInstance(toy, image);

            bot.Start(audioName);

            Ok($"Spawned a new next bot instance ({toy.NetId})");
        }

        [CommandOverload("destroy", "Destroys a next bot instance.")]
        public void Destroy(
             [CommandParameter("ID", "The ID of the next bot")] uint botId)
        {
            if (!NextBotInstance.Instances.TryGetValue(botId, out var bot))
            {
                Fail($"No next bot with ID '{botId}' exists!");
                return;
            }

            bot.Stop(true);

            Ok($"Bot destroyed!");
        }

        [CommandOverload("target", "Sets a new target.")]
        public void Target(
            [CommandParameter("ID", "The ID of the next bot")] uint botId,
            [CommandParameter("Target", "The new target (set to 'null' to stop chasing.")] string newTarget)
        {
            if (!NextBotInstance.Instances.TryGetValue(botId, out var bot))
            {
                Fail($"No next bot with ID '{botId}' exists!");
                return;
            }

            if (string.Equals(newTarget, "null", StringComparison.InvariantCultureIgnoreCase))
            {
                bot.Target = null;

                Ok($"Stopped chasing!");
            }
            else
            {
                if (ExPlayer.TryGet(newTarget, out var player))
                {
                    bot.Target = player;

                    Ok($"Started chasing {player.ToCommandString()}");
                }
                else
                {
                    Fail($"Could not find player '{newTarget}'");
                }
            }
        }
    }
}
