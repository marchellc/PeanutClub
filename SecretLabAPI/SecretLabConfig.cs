using SecretLabAPI.Elements.Levels;
using SecretLabAPI.Utilities.Configs;

using System.ComponentModel;

namespace SecretLabAPI
{
    /// <summary>
    /// Config for the plugin.
    /// </summary>
    public class SecretLabConfig
    {
        /// <summary>
        /// Gets or sets whether persistent overwatch is enabled.
        /// </summary>
        [Description("Whether persistent overwatch is enabled.")]
        public bool PersistentOverwatchEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to use shared storage for persistent overwatch.
        /// </summary>
        [Description("Whether or not to use shared storage for persistent overwatch.")]
        public bool PersistentOverwatchShared { get; set; } = true;

        /// <summary>
        /// Whether or not to load animated textures.
        /// </summary>
        [Description("Whether or not to load animated textures.")]
        public bool LoadAnimatedTextures { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether shared storage is used for levels.
        /// </summary>
        [Description("Whether or not to use shared storage for levels.")]
        public bool LevelsUseShared { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the level is displayed in custom information.
        /// </summary>
        [Description("Whether or not to show level in custom info.")]
        public bool LevelsShowInCustomInfo { get; set; } = true;

        /// <summary>
        /// Gets or sets the experience increase per-level.
        /// </summary>
        [Description("Sets the experience increase per-level.")]
        public int LevelStep { get; set; } = 100;

        /// <summary>
        /// Gets or sets the step offsets for different level ranges.
        /// </summary>
        [Description("Sets the level step offsets for different level ranges.")]
        public Dictionary<int, int> LevelStepOffsets { get; set; } = new()
        {
            [21] = 1900
        };

        /// <summary>
        /// Gets or sets the collection of static overlay elements and their configuration options.
        /// </summary>
        [Description("Configures a list of static string elements.")]
        public Dictionary<string, OverlayOptions> StaticOverlays { get; set; } = new()
        {
            { "ServerName", new() }
        };

        /// <summary>
        /// Gets or sets the level overlay settings.
        /// </summary>
        [Description("Manages options for the level overlay.")]
        public LevelSettings LevelOverlay { get; set; } = new LevelSettings();

        /// <summary>
        /// Gets or sets the maximum stack sizes for individual inventory item types.
        /// </summary>
        /// <remarks>Each entry specifies the maximum number of items of a given type that can be stacked together
        /// in a single inventory slot. Modifying this dictionary allows customization of stacking behavior for different
        /// item types.</remarks>
        [Description("Enables stacking for individual inventory items and sets their maximum stack size.")]
        public Dictionary<ItemType, ushort> ItemStacks { get; set; } = new()
        {
            { ItemType.Coin, 100 }
        };

        /// <summary>
        /// List of items that should be prevented from spawning on round start.
        /// </summary>
        [Description("List of items that should be prevented from spawning on round start.")]
        public List<ItemType> PreventSpawn { get; set; } = new();

        /// <summary>
        /// List of items that should be spawned at custom positions.
        /// </summary>
        [Description("List of items that should be spawned at custom positions.")]
        public Dictionary<string, List<string>> CustomSpawns { get; set; } = new()
        {
            { "ExamplePosition", new() { "None" } }
        };
    }
}