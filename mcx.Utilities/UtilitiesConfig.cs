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
    }
}