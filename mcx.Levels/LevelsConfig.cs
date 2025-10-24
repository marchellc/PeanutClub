using System.ComponentModel;

namespace mcx.Levels;

/// <summary>
/// The config class of the plugin.
/// </summary>
public class LevelsConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether the level is displayed in custom information.
    /// </summary>
    [Description("Whether or not to show level in custom info.")]
    public bool ShowInCustomInfo { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether shared storage is used for levels.
    /// </summary>
    [Description("Whether or not to use shared storage for levels.")]
    public bool UseShared { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum level a player can reach. Set to -1 for no limit.
    /// </summary>
    [Description("Sets the maximum level a player can reach. Set to -1 for no limit.")]
    public int MaxLevel { get; set; } = -1;

    /// <summary>
    /// Gets or sets the starting experience required to reach level 2.
    /// </summary>
    [Description("Sets the starting experience required for level 2.")]
    public float StartLevelExperience { get; set; } = 100;

    /// <summary>
    /// 
    /// </summary>
    [Description("Sets the level multipliers for different level ranges.")]
    public Dictionary<int, float> Multipliers { get; set; } = new()
    {
        [2] = 2
    };
}