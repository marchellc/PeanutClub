using LabApi.Loader.Features.Paths;

using LabExtended.Core;
using LabExtended.Extensions;

using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.FileReading;

using UnityEngine;

namespace mcx.Utilities.Audio
{
    /// <summary>
    /// Audio-related utilities.
    /// </summary>
    public static class PlaybackUtils
    {
        /// <summary>
        /// Attempts to play an audio clip at the specified position using the provided speaker settings.
        /// </summary>
        /// <remarks>If the audio directory or the specified clip does not exist, or if the clip cannot be
        /// loaded, the method logs a warning and returns false. The method does not throw for missing files or
        /// directories.</remarks>
        /// <param name="clipName">The name of the audio clip file to play. Cannot be null, empty, or whitespace.</param>
        /// <param name="position">The world position at which the audio clip will be played.</param>
        /// <param name="settings">Optional speaker settings to use for playback. If null, default settings are applied.</param>
        /// <returns>true if the audio clip was successfully played; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clipName is null, empty, or consists only of whitespace.</exception>
        public static bool PlayAt(string clipName, Vector3 position, SpeakerSettings? settings = null, Action? destroyCallback = null)
        {
            if (string.IsNullOrWhiteSpace(clipName))
                throw new ArgumentNullException(nameof(clipName));
            
            var path = Path.Combine(PathManager.SecretLab.FullName, "Audio");

            if (!Directory.Exists(path))
            {
                ApiLog.Warn("Audio", "Audio directory does not exist, creating ..");
                    
                Directory.CreateDirectory(path);
                return false;
            }

            var clipPath = Path.Combine(path, clipName);

            if (!File.Exists(clipPath))
            {
                ApiLog.Warn("Audio", $"Clip &3{clipName}&r does not exist!");
                return false;
            }
            
            if (!TryCreateAudioReader.Stream(clipPath, out var stream))
            {
                ApiLog.Warn("Audio", $"Clip &3{clipName}&r could not be loaded!");
                return false;
            }

            var player = AudioPlayerPool.Rent(settings ?? SpeakerSettings.Default, null, position)
                .WithProvider(stream)
                .PoolOnEnd()
                .DisposeOnDestroy(stream);

            if (destroyCallback != null)
            {
                Action? destroyHandler = null;

                destroyHandler = new Action(() =>
                {
                    player.Destroyed -= destroyHandler;

                    destroyCallback.InvokeSafe();
                });

                player.Destroyed += destroyHandler;
            }

            return true;
        }
    }
}