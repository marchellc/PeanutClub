using LabExtended.Core.Configs.Objects;

using mcx.RandomPickup.API;
using mcx.RandomPickup.API.Scenarios.LowHealth;

using mcx.Utilities.Audio;
using mcx.Utilities.Items.Loot;
using mcx.Utilities.Randomness;

using System.ComponentModel;

using UnityEngine;

namespace mcx.RandomPickup
{
    /// <summary>
    /// Configuration file for the RandomPickup plugin.
    /// </summary>
    public class RandomPickupConfig
    {
        /// <summary>
        /// Gets or sets the name of the pickup schematic to use.
        /// </summary>
        [Description("Sets the name of the pickup schematic.")]
        public string SchematicName { get; set; } = "RandomPickup";

        /// <summary>
        /// Gets or sets the minimum number of rounds that must pass between each scenario activation for a player.
        /// </summary>
        [Description("Sets the minimum number of rounds between each scenario activation (per player).")]
        public int MinimumScenarioRoundDelay { get; set; } = 2;

        /// <summary>
        /// Gets or sets the minimum number of rounds that must pass before a player is eligible to receive a random
        /// pickup again.
        /// </summary>
        [Description("Sets how many rounds must pass before a player can have a chance to get a random pickup again.")]
        public int MinimumRoundCount { get; set; } = 2;

        /// <summary>
        /// Gets or sets the minimum duration, in seconds, that a round must be active before pickups can start
        /// spawning.
        /// </summary>
        [Description("Sets how many seconds the round must be active before pickups can start spawning.")]
        public int MinimumRoundDuration { get; set; } = 120;

        /// <summary>
        /// Gets or sets the chance, as a percentage, for a pickup to spawn.
        /// </summary>
        [Description("Sets the chance (in percentage) for a pickup to spawn.")]
        public float SpawnChance { get; set; } = 25f;

        /// <summary>
        /// Gets or sets the speed at which the pickup floats up and down.
        /// </summary>
        [Description("Sets the speed at which the pickup floats up and down.")]
        public float FloatSpeed { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the amplitude value as a floating-point number.
        /// </summary>
        [Description("Sets the float amplitude value.")]
        public float FloatAmplitude { get; set; } = 0.25f;

        /// <summary>
        /// Gets or sets the rotation angle of the pickup, in degrees.
        /// </summary>
        [Description("Sets the rotation angle of the pickup.")]
        public float RotationAngle { get; set; } = 45f;

        /// <summary>
        /// Gets or sets a value indicating whether the pickup should rotate.
        /// </summary>
        [Description("Whether or not the pickup should rotate.")]
        public bool RotatePickup { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the pickup should float up and down.
        /// </summary>
        [Description("Whether or not the pickup should float up and down.")]
        public bool FloatPickup { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether a light toy should be spawned with the pickup.
        /// </summary>
        [Description("Whether or not a light toy should be spawned with the pickup.")]
        public bool SpawnLight { get; set; } = true;

        /// <summary>
        /// Gets or sets the color of the light toy.
        /// </summary>
        [Description("Sets the color of the light toy.")]
        public YamlColor LightColor { get; set; } = new(Color.red);

        /// <summary>
        /// Gets or sets the intensity of the light toy.
        /// </summary>
        [Description("Sets the intensity of the light toy.")]
        public float LightIntensity { get; set; } = 0.6f;

        /// <summary>
        /// Gets or sets the range of the light toy.
        /// </summary>
        [Description("Sets the range of the light toy.")]
        public float LightRange { get; set; } = 1.5f;

        /// <summary>
        /// Gets or sets a value indicating whether SCPs are eligible to receive random pickups.
        /// </summary>
        [Description("Whether or not SCPs can receive random pickups.")]
        public bool TargetScps { get; set; }

        /// <summary>
        /// Gets or sets the configuration for audio clips, including their types and associated cooldowns.
        /// </summary>
        /// <remarks>The <see cref="Clips"/> property allows customization of audio behavior by
        /// associating specific audio clips with predefined event types, such as <see
        /// cref="RandomPickupClipType.Spawned"/>, <see cref="RandomPickupClipType.Waiting"/>, and <see
        /// cref="RandomPickupClipType.Opened"/>. Cooldown values determine the minimum time between consecutive plays
        /// of the same clip type.</remarks>
        [Description("Sets the list of audio clips to play.")]
        public ClipConfig<RandomPickupClipType> Clips { get; set; } = new()
        {
            Clips = new()
            {
                [RandomPickupClipType.Spawned] = new() { new() },
                [RandomPickupClipType.Waiting] = new() { new() },
                [RandomPickupClipType.Opened] = new() { new() }
            },

            Cooldowns = new()
            {
                [RandomPickupClipType.Spawned] = 0f,
                [RandomPickupClipType.Waiting] = 0f,
                [RandomPickupClipType.Opened] = 0f
            }
        };

        /// <summary>
        /// Gets or sets the range specifying the minimum and maximum number of random pickups that can spawn per round.
        /// </summary>
        [Description("Sets the maximum number of random pickups that can spawn per round.")]
        public Int32Range SpawnCount { get; set; } = new()
        {
            MinValue = 1,
            MaxValue = 10
        };

        /// <summary>
        /// Gets or sets the range of the number of pickups to spawn at the start of the round on each spawn location.
        /// </summary>
        [Description("Sets the range of how many pickups to spawn at the start of the round on each spawn location.")]
        public Int32Range DefinedSpawnCount { get; set; } = new()
        {
            MinValue = 1,
            MaxValue = 3
        };

        /// <summary>
        /// Gets or sets the range of time, in seconds, to wait between each spawn event.
        /// </summary>
        [Description("Sets the delay range between each spawn.")]
        public FloatRange SpawnDelay { get; set; } = new()
        {
            MinValue = 30f,
            MaxValue = 300
        };

        /// <summary>
        /// Gets or sets the range of time, in seconds, that a pickup can remain active before despawning.
        /// </summary>
        [Description("Sets the maximum amount of time (in seconds) a pickup can remain active before despawning.")]
        public FloatRange PickupLifetime { get; set; } = new()
        {
            MinValue = 60f,
            MaxValue = 300f
        };

        /// <summary>
        /// Gets or sets the range of experience gain when opening a pickup.
        /// </summary>
        [Description("Sets the experience gain range when opening a pickup.")]
        public FloatRange OpenExperienceGain { get; set; } = new()
        {
            MinValue = 15f,
            MaxValue = 30f
        };

        /// <summary>
        /// Gets or sets the loot configuration for each randomly spawned pickup.
        /// </summary>
        [Description("Sets the loot for each randomly spawned pickup.")]
        public LootConfig SpawnedLoot { get; set; } = new();

        /// <summary>
        /// Gets or sets the possible spawn locations and their associated spawn chances.
        /// </summary>
        /// <remarks>The dictionary allows you to define multiple spawn locations, each with a specific
        /// probability of being selected. Ensure that the sum of all spawn chances does not exceed 1.0 if they are
        /// intended to represent a probability distribution.</remarks>
        [Description("Sets the possible spawn locations and their associated chance.")]
        public Dictionary<string, float> SpawnLocations { get; set; } = new()
        {
            ["ExamplePosition"] = 0f,
        };

        /// <summary>
        /// Gets or sets the loot configuration for each spawn location.
        /// </summary>
        /// <remarks>Use this property to define or retrieve the loot settings for specific spawn
        /// locations.  Each key in the dictionary corresponds to a unique spawn location, and the associated value
        /// defines the loot configuration for that location.</remarks>
        [Description("Sets the loot for each spawn location.")]
        public Dictionary<string, LootConfig> SpawnLocationsLoot { get; set; } = new()
        {
            ["ExamplePosition"] = new(),
            ["ExamplePosition2"] = new()
        };

        /// <summary>
        /// Gets or sets the configuration for the low health scenario.
        /// </summary>
        [Description("Configuration for the low health scenario.")]
        public LowHealthScenarioConfig LowHealthScenario { get; set; } = new();
    }
}