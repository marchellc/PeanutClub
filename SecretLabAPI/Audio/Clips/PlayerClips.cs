using LabApi.Features.Wrappers;

using LabExtended.API;
using LabExtended.Events;

using NAudio.Wave;

using SecretLabAPI.Audio.Playback;

using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.SendEngines;

namespace SecretLabAPI.Audio.Clips
{
    /// <summary>
    /// Used for playback of player clips.
    /// </summary>
    public static class PlayerClips
    {
        /// <summary>
        /// Saves the state of a player's clip playback.
        /// </summary>
        public class PlayerState
        {
            /// <summary>
            /// The target player.
            /// </summary>
            public Player Target;

            /// <summary>
            /// The player created for the target.
            /// </summary>
            public AudioPlayer Player;

            /// <summary>
            /// The name of the last played clip.
            /// </summary>
            public string? Clip;

            /// <summary>
            /// The stream of the currently playing clip.
            /// </summary>
            public WaveStream? Stream;

            /// <summary>
            /// The provider of the currently playing clip.
            /// </summary>
            public IWaveProvider? Provider;

            internal void OnEnded()
            {
                Clip = null;
                Stream = null;
                Provider = null;
            }
        }

        /// <summary>
        /// Gets all spawned clip managers for players.
        /// </summary>
        public static Dictionary<string, PlayerState> Players { get; } = new();

        /// <summary>
        /// Whether or not a player has a clip playing.
        /// </summary>
        public static bool IsPlayingClip(this Player player)
        {
            if (player?.ReferenceHub == null
                || player.UserId is null)
                return false;

            return Players.TryGetValue(player.UserId, out var audio)
                && audio.Player != null
                && !audio.Player.HasEnded;
        }

        /// <summary>
        /// Whether or not a player has a specific clip playing.
        /// </summary>
        public static bool IsPlayingClip(this Player player, string clipName)
        {
            if (player?.ReferenceHub == null
                || player.UserId is null)
                return false;

            return Players.TryGetValue(player.UserId, out var audio)
                && audio.Player != null
                && !audio.Player.HasEnded
                && audio.Clip != null
                && audio.Clip == clipName;
        }

        /// <summary>
        /// Stops a player's currently playing clip.
        /// </summary>
        public static bool StopClip(this Player player, out string? clip)
        {
            clip = null;

            if (player?.ReferenceHub == null
                || player.UserId is null)
                return false;

            if (!Players.TryGetValue(player.UserId, out var audio))
                return false;

            clip = audio.Clip;

            if (audio.Stream != null)
            {
                audio.Stream.Dispose();
                audio.Stream = null;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Starts playing a random clip for a player.
        /// </summary>
        /// <param name="player">The player to play the clip for.</param>
        /// <param name="clipName">The name of the clip.</param>
        /// <param name="volume">The volue of the clip.</param>
        /// <returns>true if a clip started playing</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool PlayClip(this Player player, string clipName, float volume = 1f, bool isPersonal = false)
        {
            if (string.IsNullOrWhiteSpace(clipName))
                throw new ArgumentNullException(nameof(clipName));

            if (player?.ReferenceHub == null
                || player.UserId is null)
                return false;

            if (!Players.TryGetValue(player.UserId, out var state))
                Players[player.UserId] = state = InitState(player);

            if (state.Player == null)
                return false;

            if (state.Stream != null)
            {
                state.Stream.Dispose();
                state.Stream = null;
            }

            state.Clip = null;

            if (!PlaybackUtils.TryLoadClip(clipName, false, out var stream, out var provider))
                return false;
            
            if (isPersonal)
            {
                if (state.Player.SendEngine == null 
                    || state.Player.SendEngine is not SpecificPlayerSendEngine specificPlayerSendEngine
                    || specificPlayerSendEngine.Target != player)
                {
                    state.Player.WithSendEngine(new SpecificPlayerSendEngine(player));
                }
            }
            else
            {
                if (state.Player.SendEngine == null || state.Player.SendEngine != SendEngine.DefaultEngine)
                {
                    state.Player.WithSendEngine(SendEngine.DefaultEngine);
                }
            }

            state.Player.WithVolume(volume);
            state.Player.WithProvider(provider);

            state.Clip = clipName;
            state.Stream = stream;
            state.Provider = provider;

            return true;
        }

        private static PlayerState InitState(Player player)
        {
            var state = new PlayerState();

            state.Target = player;

            state.Player = AudioPlayer.Create(AudioPlayerPool.NextAvailableId, SpeakerSettings.Default, player.GameObject!.transform);
            state.Player.NoSamplesRead += state.OnEnded;

            return state;
        }

        private static void Left(ExPlayer player)
        {
            if (!Players.TryGetValue(player.UserId, out var state))
                return;

            Players.Remove(player.UserId);

            if (state.Player != null)
            {
                state.Player.NoSamplesRead -= state.OnEnded;
                state.Player.Destroy();
            }
        }

        internal static void Initialize()
        {
            ExPlayerEvents.Left += Left;
        }
    }
}