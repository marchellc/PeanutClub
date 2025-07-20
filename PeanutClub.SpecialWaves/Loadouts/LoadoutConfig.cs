using System.ComponentModel;

namespace PeanutClub.SpecialWaves.Loadouts;

/// <summary>
/// A YAML-serializable loadout.
/// </summary>
public class LoadoutConfig
{
    [Description("Sets the loadout's starting heath.")]
    public float Health { get; set; } = -1f;

    [Description("Sets the loadout's starting inventory, accepts custom item IDs and item types.")]
    public List<string> Items { get; set; } = new();

    [Description("Sets the loadout's starting ammo, accepts item types and custom ammo IDs.")]
    public Dictionary<string, int> Ammo { get; set; } = new();
}