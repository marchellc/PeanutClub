using LabExtended.Core.Configs.Objects;

using mcx.RandomPickup.API;

using mcx.Utilities.Audio;
using mcx.Utilities.Actions;
using mcx.Utilities.Configs;

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
        public Int32Range OpenExperienceGain { get; set; } = new()
        {
            MinValue = 15,
            MaxValue = 30
        };

        /// <summary>
        /// Gets or sets the possible spawn locations and their associated spawn chances.
        /// </summary>
        [Description("Sets the possible spawn locations and their associated chance.")]
        public Dictionary<string, float> SpawnLocations { get; set; } = new()
        {
            ["ExamplePosition"] = 0f,
        };

        /// <summary>
        /// Gets or sets the loot groups available in the random pickup.
        /// </summary>
        [Description("Sets the loot groups available in the random pickup.")]
        public Dictionary<string, List<ActionLoot.Group>> Loot { get; set; } = new()
        {
            { "DefinedLocationRandomPickup", new() { defaultHealthLoot, defaultExplodeGroup } },
            { "PlayerRandomPickup", new() { defaultHealthLoot, defaultExplodeGroup } },
            { "ScenarioRandomPickup", new() { defaultHealthLoot, defaultExplodeGroup } }
        };

        private static ActionLoot.Group defaultExplodeGroup
        {
            get
            {
                return new ActionLoot.Group()
                {
                    Weight = 5f,

                    Actions = new()
                    {
                        new()
                        {
                            Id = "Explode",

                            Parameters = new()
                            {
                                { "EffectOnly", ["true"] },
                                { "Velocity", ["10"] },
                                { "Type", ["GrenadeHE"] },
                                { "Reason", ["BOO"] }
                            }
                        }
                    }
                };
            }
        }

        private static ActionLoot.Group defaultHealthLoot
        {
            get
            {
                return new ActionLoot.Group()
                {
                    Weight = 80f,

                    Actions = new()
                    {
                        new()
                        {
                            Id = "AddItem",

                            Parameters = new()
                            {
                                { "Type", ["Medkit"] },
                                { "Amount", ["1"] },
                                { "Spawn", ["true"] }
                            }
                        },

                        new()
                        {
                            Id = "AddItem",

                            Parameters = new()
                            {
                                { "Type", ["Adrenaline"] },
                                { "Amount", ["1"] },
                                { "Spawn", ["true"] }
                            }
                        },

                        new()
                        {
                            Id = "AddItem",

                            Parameters = new()
                            {
                                { "Type", ["SCP500"] },
                                { "Amount", ["1"] },
                                { "Spawn", ["true"] }
                            }
                        }
                    }
                };
            }
        }
    }
}