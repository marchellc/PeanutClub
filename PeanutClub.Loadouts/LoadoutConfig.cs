using System.ComponentModel;

namespace PeanutClub.Loadouts;

/// <summary>
/// The config class of the plugin.
/// </summary>
public class LoadoutConfig
{
    /// <summary>
    /// Gets or sets a list of config loadouts.
    /// </summary>
    [Description("Sets the list of loadouts.")]
    public List<LoadoutDefinition> Loadouts { get; set; } = new();
}