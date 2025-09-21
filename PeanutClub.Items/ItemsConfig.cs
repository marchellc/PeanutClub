using System.ComponentModel;

using PeanutClub.Items.Weapons.SniperRifle;

namespace PeanutClub.Items;

/// <summary>
/// Config of the Items plugin.
/// </summary>
public class ItemsConfig
{
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
        ["ExamplePosition"] = new()
        {
            "None"
        }
    };

    /// <summary>
    /// Gets or sets the maximum stack sizes for individual inventory item types.
    /// </summary>
    /// <remarks>Each entry specifies the maximum number of items of a given type that can be stacked together
    /// in a single inventory slot. Modifying this dictionary allows customization of stacking behavior for different
    /// item types.</remarks>
    [Description("Enables stacking for individual inventory items and sets their maximum stack size.")]
    public Dictionary<ItemType, ushort> ItemStacks { get; set; } = new()
    {
        [ItemType.Coin] = 100,
    };

    /// <summary>
    /// The default properties of the Sniper Rifle.
    /// </summary>
    [Description("Sets the default properties of the Sniper Rifle.")]
    public SniperRifleProperties SniperRifle { get; set; } = new();
}