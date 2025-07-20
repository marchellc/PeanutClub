using InventorySystem.Items;

using LabExtended.API;

using LabExtended.API.CustomItems;
using LabExtended.API.CustomItems.Behaviours;

using LabExtended.Core;
using LabExtended.Extensions;

using UnityEngine;

namespace PeanutClub.SpecialWaves.Loadouts;

/// <summary>
/// Manages config loadouts.
/// </summary>
public static class LoadoutManager
{
    /// <summary>
    /// Gets called once a loadout is applied.
    /// </summary>
    public static event Action<string, LoadoutInfo, ExPlayer>? Applied; 
    
    /// <summary>
    /// A list of all loaded loadouts.
    /// </summary>
    public static Dictionary<string, LoadoutInfo> Loadouts { get; } = new();
    
    /// <summary>
    /// Gets a specific loadout by it's name.
    /// </summary>
    /// <param name="name">The name of the loadout.</param>
    /// <returns>The loaded loadout.</returns>
    public static LoadoutInfo GetLoadout(string name)
        => Loadouts[name];

    /// <summary>
    /// Applies a saved loadout.
    /// </summary>
    /// <param name="player">The player receiving the loadout.</param>
    /// <param name="loadoutName">The name of the loadout.</param>
    /// <param name="itemProcessor">The delegate used to process added items.</param>
    /// <param name="customItemProcessor">The delegate used to process added custom items.</param>
    public static void ApplyLoadout(this ExPlayer player, string loadoutName, Action<ItemBase>? itemProcessor = null,
        Action<CustomItemInventoryBehaviour>? customItemProcessor = null)
    { 
        if (Loadouts.TryGetValue(loadoutName, out var loadout))
        {
            player.Ammo.ClearAmmo();
            player.Inventory.Clear();
            
            if (loadout.Health.HasValue)
            {
                player.MaxHealth = loadout.Health.Value;
                player.Health = loadout.Health.Value;
            }

            foreach (var pair in loadout.GameAmmo)
            {
                player.Ammo.SetAmmo(pair.Key, (ushort)Mathf.Min(pair.Value, ushort.MaxValue));
            }

            foreach (var pair in loadout.CustomAmmo)
            {
                player.Inventory.CustomAmmo.Set(pair.Key, pair.Value);
            }

            foreach (var item in loadout.GameItems)
            {
                var instance = player.Inventory.AddItem(item);
                
                if (instance != null)
                    itemProcessor?.InvokeSafe(instance);
            }

            foreach (var item in loadout.CustomItems)
            {
                if (!CustomItemRegistry.TryGetHandler(item, out var handler))
                    continue;

                var instance = handler.Give(player);
                
                if (instance != null)
                    customItemProcessor?.InvokeSafe(instance);
            }
            
            Applied?.InvokeSafe(loadoutName, loadout, player);
        }
        else
        {
            ApiLog.Warn("Loadout Manager", $"Loadout &3{loadoutName}&r could not be loaded!");
        }
    }

    /// <summary>
    /// Ensures that a loadout with a specific name exists.
    /// </summary>
    /// <param name="name">The name of the loadout.</param>
    /// <param name="defaultLoadout">The default loadout if none was defined.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static void EnsureLoadout(string name, LoadoutInfo? defaultLoadout = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        if (Loadouts.ContainsKey(name))
            return;

        if (defaultLoadout is null)
            throw new Exception($"Loadout '{name}' is not present in config and a default was not provided.");
        
        Loadouts.Add(name, defaultLoadout);

        PluginCore.StaticConfig.Loadouts[name] = defaultLoadout.ToConfig();
        PluginCore.SaveConfig();
    }
}