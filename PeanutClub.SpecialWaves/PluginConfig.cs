using System.ComponentModel;
using LabExtended.Core.Configs.Objects;
using PeanutClub.SpecialWaves.Loadouts;
using UnityEngine;

namespace PeanutClub.SpecialWaves;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfig
{
    /// <summary>
    /// Player loadouts.
    /// </summary>
    [Description("Manages custom wave loadouts.")]
    public Dictionary<string, LoadoutConfig> Loadouts { get; set; } = new();

    /// <summary>
    /// Serpent's Hand spawn point size.
    /// </summary>
    [Description("Sets the size of the Serpent's Hand spawn point.")]
    public YamlVector3 SerpentsHandSpawnSize { get; set; } = new YamlVector3(5f, 0f, 5f);
}