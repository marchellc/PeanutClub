using System.ComponentModel;

using PeanutClub.Items.Weapons.SniperRifle;

namespace PeanutClub.Items;

/// <summary>
/// Config of the Items plugin.
/// </summary>
public class ItemsConfig
{
    /// <summary>
    /// The default properties of the Sniper Rifle.
    /// </summary>
    [Description("Sets the default properties of the Sniper Rifle.")]
    public SniperRifleProperties SniperRifle { get; set; } = new();
}