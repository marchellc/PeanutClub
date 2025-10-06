using LabApi.Loader.Features.Paths;

using LabExtended.API;
using LabExtended.API.Toys;
using LabExtended.API.Images;

using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Extensions;

using LabExtended.Images.Playback;
using LabExtended.Utilities.Update;

using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.FileReading;

using NAudio.Wave;

using UnityEngine;

namespace mcx.Utilities.Features.NextBots
{
    /// <summary>
    /// Represents a next bot (an image with audio that chases you).
    /// </summary>
    public class NextBotInstance
    {
        /// <summary>
        /// Gets a list of all spawned next bot instances.
        /// </summary>
        public static Dictionary<uint, NextBotInstance> Instances { get; } = new();

        private WaveStream clip;

        /// <summary>
        /// Gets the spawned text formatted to the image.
        /// </summary>
        public TextToy Toy { get; private set; }

        /// <summary>
        /// Gets the loaded image file to display on the text toy.
        /// </summary>
        public ImageFile Image { get; private set; }

        /// <summary>
        /// Gets the audio player for playing sounds.
        /// </summary>
        public AudioPlayer Player { get; private set; }

        /// <summary>
        /// Gets or sets the target to be chased by the next bot.
        /// </summary>
        public ExPlayer? Target { get; set; }

        /// <summary>
        /// Initializes a new instance of the NextBotInstance class with the specified text toy and image file.
        /// </summary>
        /// <param name="toy">The text toy to associate with this instance. Cannot be null.</param>
        /// <param name="image">The image file to associate with this instance. Cannot be null.</param>
        public NextBotInstance(TextToy toy, ImageFile image)
        {
            Toy = toy;
            Image = image;

            Player = AudioPlayerPool.Rent(SpeakerSettings.Default, toy.Transform);

            Instances[toy.NetId] = this;
        }

        /// <summary>
        /// Starts the next bot and audio playback.
        /// </summary>
        /// <param name="clipName">The name of the audio file to play.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Start(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
                throw new ArgumentNullException(nameof(clipName));

            var path = Path.Combine(PathManager.SecretLab.FullName, "Audio");

            if (!Directory.Exists(path))
            {
                ApiLog.Warn("Next Bots", "Audio directory does not exist, creating ..");

                Directory.CreateDirectory(path);
                return;
            }

            var clipPath = Path.Combine(path, clipName);

            if (!File.Exists(clipPath))
            {
                ApiLog.Warn("Next Bots", $"Clip &3{clipName}&r does not exist!");
                return;
            }

            if (!TryCreateAudioReader.Stream(clipPath, out var stream))
            {
                ApiLog.Warn("Next Bots", $"Clip &3{clipName}&r could not be loaded!");
                return;
            }

            Toy.PlaybackDisplay.EnableOption(PlaybackFlags.Loop);
            Toy.PlaybackDisplay.Play(Image);

            clip = stream;

            Player.WithProvider(stream.Loop());

            PlayerUpdateHelper.OnUpdate += Internal_Update;
        }

        /// <summary>
        /// Stops playback and releases associated resources. Optionally destroys the underlying objects and clears
        /// references.
        /// </summary>
        /// <param name="destroy">true to destroy and clear all related objects and references; otherwise, false to only stop playback and
        /// release resources.</param>
        public void Stop(bool destroy)
        {
            Target = null!;

            if (clip != null)
            {
                clip.Dispose();
                clip = null!;
            }

            if (Toy != null)
            {
                Toy.PlaybackDisplay.Stop();
                Toy.Clear();
            }

            if (Toy != null && destroy && Instances.Remove(Toy.NetId))
            {
                PlayerUpdateHelper.OnUpdate -= Internal_Update;

                if (Player != null)
                    AudioPlayerPool.Return(Player);

                if (Toy != null)
                {
                    Toy.Delete();
                    Toy = null!;
                }

                Image = null!;
                Player = null!;
            }
        }

        private void Internal_Update()
        {
            if (Target?.ReferenceHub != null && Target.IsAlive)
            {
                Toy.Rotation = Quaternion.LookRotation(Target.CameraTransform.position - Toy.Position, Toy.Transform.up);
                Toy.Position = Vector3.MoveTowards(Toy.Position, Target.CameraTransform.position, Time.deltaTime * 3f);
            }
        }

        private static void Internal_RoundEnd()
        {
            foreach (var instance in Instances.ToDictionary())
                instance.Value.Stop(true);

            Instances.Clear();
        }

        internal static void Internal_Init()
        {
            ExRoundEvents.Ended += Internal_RoundEnd;
        }
    }
}