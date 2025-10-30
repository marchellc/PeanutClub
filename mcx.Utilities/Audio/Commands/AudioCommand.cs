using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using UnityEngine;

using SecretLabNAudio.Core;

namespace mcx.Utilities.Audio.Commands
{
    [Command("audio", "Base command for audio utilities")]
    public class AudioCommand : CommandBase, IServerSideCommand
    {
        [CommandOverload("position", "Starts playing an audio clip at a specified position.", null)]
        public void StartAt(
            [CommandParameter("Position", "The position to play the audio at.")] Vector3 position, 
            [CommandParameter("Clip", "The name of the audio clip file.")] string clip, 
            [CommandParameter("Loop", "Whether or not the clip shoud loop.")] bool loop = false)
        {
            if (PlaybackUtils.PlayAt(clip, position, SpeakerSettings.Default, loop).HasValue)
            {
                Ok($"Started playing clip '{clip}' at {position.ToPreciseString()}");
            }
            else
            {
                Fail($"Could not start playing clip '{clip}' at {position.ToPreciseString()}");
            }
        }

        [CommandOverload("player", "Starts playing an audio clip at a specified player.", null)]
        public void StartPlayer(
            [CommandParameter("Target", "The player to start playing the clip for.")] ExPlayer player,
            [CommandParameter("Clip", "The name of the audio clip file.")] string clip,
            [CommandParameter("IsPersonal", "Whether or not the audio should be heard by the targeted player only.")] bool isPersonal = false,
            [CommandParameter("SendToOthers", "Whether or not the audio should be sent to player's in proximity.")] bool sendToOthers = true)
        {
            player ??= Sender;
            
            if (player.PlayClip(clip, 1f, isPersonal, sendToOthers))
            {
                Ok($"Started playing clip '{clip}' for player {player.ToCommandString()}.");
            }
            else
            {
                Fail($"Failed to play clip '{clip}' for player {player.ToCommandString()}. Make sure the clip exists and is a valid audio file.");
            }
        }
    }
}