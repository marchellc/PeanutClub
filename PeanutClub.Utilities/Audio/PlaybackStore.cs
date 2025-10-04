using LabApi.Features.Stores;
using LabApi.Features.Wrappers;

using LabApi.Loader.Features.Paths;

using NAudio.Wave;

using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.SendEngines;
using SecretLabNAudio.Core.FileReading;

using LabExtended.Core;
using LabExtended.Extensions;

namespace PeanutClub.Utilities.Audio
{
    public class PlaybackStore : CustomDataStore<PlaybackStore>
    {
        /// <summary>
        /// Gets called when a playback is finished.
        /// </summary>
        public static event Action<PlaybackStore>? Finished;

        /// <summary>
        /// Gets called when a playback starts.
        /// </summary>
        public static event Action<PlaybackStore>? Started;

        /// <summary>
        /// Gets the current track being played.
        /// </summary>
        public WaveStream? CurrentTrack { get; private set; }

        /// <summary>
        /// Gets the audio player assigned for this player.
        /// </summary>
        public AudioPlayer Player { get; private set; }

        /// <summary>
        /// Whether or not the player is playing.
        /// </summary>
        public bool IsPlaying => CurrentTrack != null && !Player.HasEnded;

        /// <summary>
        /// Whether or not the playback is paused.
        /// </summary>
        public bool IsPaused
        {
            get => Player.IsPaused;
            set => Player.IsPaused = value;
        }

        /// <summary>
        /// Gets or sets the volume of the speaker.
        /// </summary>
        public float Volume
        {
            get => Player.Speaker.Volume;
            set => Player.WithVolume(value);
        }

        public PlaybackStore(Player owner) : base(owner)
        {
            Player = AudioPlayer.Create(AudioPlayerPool.NextAvailableId, SpeakerSettings.Default, owner.GameObject!.transform)
                .WithSendEngine(new SpecificPlayerSendEngine(owner));
        }

        /// <summary>
        /// Starts playback of an audio clip.
        /// </summary>
        /// <param name="clipName">The name of the clip file to play.</param>
        /// <returns>true if the clip started playing</returns>
        public bool Play(string clipName, bool loop = false)
        {
            if (string.IsNullOrEmpty(clipName))
                return false;

            return Internal_Play(clipName, loop);
        }

        /// <summary>
        /// Stops playback of the current track if one is playing.
        /// </summary>
        public bool Stop()
        {
            if (CurrentTrack == null || Player.HasEnded)
                return false;

            CurrentTrack.Dispose();
            CurrentTrack = null;

            return true;
        }

        /// <inheritdoc>/>
        protected override void OnInstanceDestroyed()
        {
            base.OnInstanceDestroyed();

            if (CurrentTrack != null)
            {
                CurrentTrack.Dispose();
                CurrentTrack = null;
            }

            if (Player != null)
            {
                Player.Destroy();
                Player = null!;
            }
        }

        private bool Internal_Play(string clipName, bool loop)
        {
            var directory = Path.Combine(PathManager.SecretLab.FullName, "Audio");

            if (!Directory.Exists(directory))
            {
                ApiLog.Warn("PlaybackStore", $"[&3{clipName}&r] Audio directory does not exist, creating ..");

                Directory.CreateDirectory(directory);
                return false;
            }

            var filePath = Path.Combine(directory, clipName);

            if (!File.Exists(filePath))
            {
                ApiLog.Warn("PlaybackStore", $"[&3{clipName}&r] File does not exist!");
                return false;
            }

            if (!TryCreateAudioReader.Stream(filePath, out var stream))
            {
                ApiLog.Warn("PlaybackStore", $"[&3{clipName}&r] File is not a valid audio file!");
                return false;
            }
            
            if (CurrentTrack != null)
            {
                CurrentTrack.Dispose();
                CurrentTrack = null;
            }

            CurrentTrack = stream;
            
            Player.WithProvider(loop
                ? stream.Loop()
                : stream);

            Started?.InvokeSafe(this);
            return true;
        }
    }
}