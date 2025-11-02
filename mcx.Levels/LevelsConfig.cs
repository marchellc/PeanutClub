using System.ComponentModel;

namespace mcx.Levels;

/// <summary>
/// The config class of the plugin.
/// </summary>
public class LevelsConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether shared storage is used for levels.
    /// </summary>
    [Description("Whether or not to use shared storage for levels.")]
    public bool UseShared { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the level is displayed in custom information.
    /// </summary>
    [Description("Whether or not to show level in custom info.")]
    public bool ShowInCustomInfo { get; set; } = true;

    /// <summary>
    /// Gets or sets the experience increase per-level.
    /// </summary>
    [Description("Sets the experience increase per-level.")]
    public int LevelStep { get; set; } = 100;

    /// <summary>
    /// Gets or sets the step offsets for different level ranges.
    /// </summary>
    [Description("Sets the level step offsets for different level ranges.")]
    public Dictionary<int, int> StepOffsets { get; set; } = new()
    {
        [21] = 1900
    };
}