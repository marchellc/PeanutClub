using LabApi.Loader.Features.Paths;

using LabExtended.Core;

using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.FileReading;

using UnityEngine;

using NAudio.Wave;

namespace SecretLabAPI.Audio.Playback
{
    /// <summary>
    /// Audio-related utilities.
    /// </summary>
    public static class PlaybackUtils
    {
        /// <summary>
        /// Gets a list of all loaded audio clips in memory.
        /// </summary>
        public static Dictionary<string, KeyValuePair<byte[], string>> LoadedClips { get; } = new();

        /// <summary>
        /// Attempts to load an audio clip by its name and create an audio stream and provider.
        /// </summary>
        /// <remarks>This method searches for the audio clip in the "Audio" directory within the
        /// application's secret lab path. If the directory does not exist, it will be created, and the method will
        /// return <see langword="false"/>. If the specified clip does not exist or cannot be loaded, the method logs a
        /// warning and returns <see langword="false"/>.</remarks>
        /// <param name="clipName">The name of the audio clip to load. Cannot be null, empty, or whitespace.</param>
        /// <param name="loopClip">A value indicating whether the audio clip should loop. If <see langword="true"/>, the returned provider will
        /// loop the clip; otherwise, it will play the clip once.</param>
        /// <param name="stream">When this method returns, contains the <see cref="WaveStream"/> representing the audio clip, or <see
        /// langword="null"/> if the method fails.</param>
        /// <param name="provider">When this method returns, contains the <see cref="IWaveProvider"/> for playing the audio clip, or <see
        /// langword="null"/> if the method fails.</param>
        /// <returns><see langword="true"/> if the audio clip was successfully loaded; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="clipName"/> is <see langword="null"/>, empty, or consists only of whitespace.</exception>
        public static bool TryLoadClip(string clipName, bool loopClip, out WaveStream? stream, out IWaveProvider? provider)
        {
            provider = null;
            stream = null!;

            if (string.IsNullOrWhiteSpace(clipName))
                throw new ArgumentNullException(nameof(clipName));

            if (LoadedClips.TryGetValue(clipName, out var clipData))
            {
                var fileStream = new MemoryStream(clipData.Key);

                if (!TryCreateAudioReader.Stream(fileStream, clipData.Value, true, out stream))
                {
                    ApiLog.Warn("PlaybackUtils", $"Clip &3{clipName}&r could not be loaded from memory!");
                    return false;
                }

                provider = loopClip
                    ? stream.Loop()
                    : stream;

                return true;
            }

            var path = Path.Combine(PathManager.SecretLab.FullName, "audio");
            var clipPath = Path.Combine(path, clipName);

            if (!File.Exists(clipPath))
            {
                ApiLog.Warn("PlaybackUtils", $"Clip &3{clipName}&r does not exist!");
                return false;
            }

            if (!TryCreateAudioReader.Stream(clipPath, out stream))
            {
                ApiLog.Warn("PlaybackUtils", $"Clip &3{clipName}&r could not be loaded!");
                return false;
            }

            provider = loopClip
                ? stream.Loop()
                : stream;

            return true;
        }

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
        public static PlaybackHandle? PlayParented(string clipName, Transform transform, SpeakerSettings? settings = null, bool loop = false, Action? destroyCallback = null)
        {
            if (string.IsNullOrWhiteSpace(clipName))
                throw new ArgumentNullException(nameof(clipName));

            if (!TryLoadClip(clipName, loop, out var stream, out var provider)
                || provider == null || stream == null)
                return null;

            var player = AudioPlayerPool.Rent(settings ?? SpeakerSettings.Default, transform)
                .WithProvider(provider)
                .PoolOnEnd()
                .DisposeOnDestroy(stream);

            return new(clipName, player, stream, provider, destroyCallback);
        }

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
        public static PlaybackHandle? PlayAt(string clipName, Vector3 position, SpeakerSettings? settings = null, bool loop = false, Action? destroyCallback = null)
        {
            if (string.IsNullOrWhiteSpace(clipName))
                throw new ArgumentNullException(nameof(clipName));

            if (!TryLoadClip(clipName, loop, out var stream, out var provider)
                || provider == null || stream == null)
                return null;

            var player = AudioPlayerPool.Rent(settings ?? SpeakerSettings.Default, null, position)
                .WithProvider(provider)
                .PoolOnEnd()
                .DisposeOnDestroy(stream);

            return new(clipName, player, stream, provider, destroyCallback);
        }

        internal static void Initialize()
        {
            var path = Path.Combine(PathManager.SecretLab.FullName, "audio");

            if (!Directory.Exists(path))
            {
                ApiLog.Warn("PlaybackUtils", "Audio directory does not exist, creating ..");

                Directory.CreateDirectory(path);
            }

            var uniqueCount = 0;

            foreach (var file in Directory.GetFiles(path))
            {
                var data = File.ReadAllBytes(file);
                var extension = Path.GetExtension(file);

                var nameExtension = Path.GetFileName(file);
                var nameNoExtension = Path.GetFileNameWithoutExtension(file);

                if (!LoadedClips.ContainsKey(nameExtension)) LoadedClips.Add(nameExtension, new(data, extension));
                if (!LoadedClips.ContainsKey(nameNoExtension)) LoadedClips.Add(nameNoExtension, new(data, extension));

                uniqueCount++;
            }

            foreach (var directory in Directory.GetDirectories(path))
            {
                var name = Path.GetFileName(directory);

                foreach (var file in Directory.GetFiles(directory))
                {
                    var data = File.ReadAllBytes(file);
                    var extension = Path.GetExtension(file);

                    var nameExtensionWithDirectory = $"{name}/{Path.GetFileName(file)}";
                    var nameExtensionWithoutDirectory = Path.GetFileName(file);

                    var nameNoExtensionWithDirectory = Path.GetFileNameWithoutExtension(file);
                    var nameNoExtensionWithoutDirectory = $"{name}/{Path.GetFileNameWithoutExtension(file)}";

                    if (!LoadedClips.ContainsKey(nameExtensionWithDirectory)) LoadedClips.Add(nameExtensionWithDirectory, new(data, extension));
                    if (!LoadedClips.ContainsKey(nameExtensionWithoutDirectory)) LoadedClips.Add(nameExtensionWithoutDirectory, new(data, extension));

                    if (!LoadedClips.ContainsKey(nameNoExtensionWithDirectory)) LoadedClips.Add(nameNoExtensionWithDirectory, new(data, extension));
                    if (!LoadedClips.ContainsKey(nameNoExtensionWithoutDirectory)) LoadedClips.Add(nameNoExtensionWithoutDirectory, new(data, extension));

                    uniqueCount++;
                }
            }

            ApiLog.Info("PlaybackUtils", $"Loaded &a{uniqueCount}&r audio clips from &3{path}&r");
        }
    }
}