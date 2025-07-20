namespace PeanutClub.SpecialWaves.Loadouts;

/// <summary>
/// Describes a loaded config loadout.
/// </summary>
public class LoadoutInfo
{
    /// <summary>
    /// Gets the loadout's custom health.
    /// </summary>
    public float? Health { get; set; }
    
    /// <summary>
    /// A list of the game's items.
    /// </summary>
    public List<ItemType> GameItems { get; } = new();

    /// <summary>
    /// A list of custom items.
    /// </summary>
    public List<ushort> CustomItems { get; } = new();
    
    /// <summary>
    /// Gets the loadout's ammo.
    /// </summary>
    public Dictionary<ItemType, int> GameAmmo { get; } = new();
    
    /// <summary>
    /// Gets the loadout's custom ammo.
    /// </summary>
    public Dictionary<ushort, int> CustomAmmo { get; } = new();

    /// <summary>
    /// Sets the <see cref="Health"/> property.
    /// </summary>
    public LoadoutInfo WithHealth(float health)
    {
        Health = health;
        return this;
    }
    
    /// <summary>
    /// Sets the <see cref="GameItems"/> property.
    /// </summary>
    public LoadoutInfo WithGameItems(params ItemType[] items)
    {
        GameItems.AddRange(items);
        return this;
    }

    /// <summary>
    /// Sets the <see cref="CustomItems"/> property.
    /// </summary>
    public LoadoutInfo WithCustomItem(ushort itemId)
    {
        CustomItems.Add(itemId);
        return this;
    }
    
    /// <summary>
    /// Sets the <see cref="GameAmmo"/> property.
    /// </summary>
    public LoadoutInfo WithGameAmmo(ItemType ammoType, int amount)
    {
        GameAmmo[ammoType] = amount;
        return this;
    }
    
    /// <summary>
    /// Sets the <see cref="CustomAmmo"/> property.
    /// </summary>
    public LoadoutInfo WithCustomAmmo(ushort ammoId, int amount)
    {
        CustomAmmo[ammoId] = amount;
        return this;
    }
    
    /// <summary>
    /// Converts the loadout to a config value.
    /// </summary>
    /// <returns>The converted loadout.</returns>
    public LoadoutConfig ToConfig()
    {
        var config = new LoadoutConfig();

        config.Health = Health ?? -1f;
        
        foreach (var itemType in GameItems)
            config.Items.Add(itemType.ToString());
        
        foreach (var customItem in CustomItems)
            config.Items.Add(customItem.ToString());

        foreach (var pair in GameAmmo)
            config.Ammo[pair.Key.ToString()] = pair.Value;

        foreach (var pair in CustomAmmo)
            config.Ammo[pair.Key.ToString()] = pair.Value;

        return config;
    }
}