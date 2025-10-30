using mcx.Utilities.Audio;

using System.ComponentModel;

namespace mcx.Utilities
{
    /// <summary>
    /// Config for the utilities plugin.
    /// </summary>
    public class UtilitiesConfig
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
        /// Gets or sets the audio clips for player events.
        /// </summary>
        [Description("Sets the audio clips for player events.")]
        public ClipConfig<string> PlayerClips { get; set; } = new();
    }
}