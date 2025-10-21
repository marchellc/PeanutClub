using InventorySystem.Items;
using InventorySystem.Items.Pickups;

using LabApi.Loader.Features.Plugins;

using LabExtended.API;
using LabExtended.API.Custom.Items;

using LabExtended.Extensions;

using mcx.Items.Entries;
using mcx.Items.Spawning;
using mcx.Items.Stacking;

using mcx.Utilities.Items;

using Mirror;

using UnityEngine;

namespace mcx.Items;

/// <summary>
/// The main class of the plugin.
/// </summary>
public class ItemsCore : Plugin<ItemsConfig>
{
    /// <summary>
    /// Gets the static instance of the plugin.
    /// </summary>
    public static ItemsCore PluginStatic { get; private set; }
    
    /// <summary>
    /// Gets the static instance of the plugin's config.
    /// </summary>
    public static ItemsConfig ConfigStatic { get; private set; }
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override string Name { get; } = "mcx.Items";

    /// <inheritdoc cref="Plugin.Author"/>
    public override string Author { get; } = "marchellcx";

    /// <inheritdoc cref="Plugin.Description"/>
    public override string Description { get; } = "Plugin that adds custom items.";

    /// <inheritdoc cref="Plugin.Version"/>
    public override Version Version { get; } = null!;

    /// <inheritdoc cref="Plugin.RequiredApiVersion"/>
    public override Version RequiredApiVersion { get; } = null!;
    
    /// <inheritdoc cref="Plugin.Enable"/>
    public override void Enable()
    {
        PluginStatic = this;
        ConfigStatic = Config!;

        ItemStacker.Internal_Init();

        SpawnPositions.Internal_Init();
        SpawnPrevention.Internal_Init();

        ItemHandler.RegisterEffect("Explosion", new ExplosionEntry());

        if (Config != null)
        {
            Config.AirsoftGun.Register();
            Config.SniperRifle.Register();

            foreach (var pair in Config.ItemLaunchers)
            {
                pair.Value.launcherId = pair.Key;
                pair.Value.Register();
            }
        }
    }

    /// <inheritdoc cref="Plugin.Disable"/>
    public override void Disable()
    {

    }

    /// <summary>
    /// Adds an item to the specified player, resolving the item name as either a base item type or a registered custom
    /// item.
    /// </summary>
    /// <remarks>If the item name matches a base item type, the corresponding item is instantiated and
    /// transferred to the player. If the item name matches a registered custom item, that custom item is added to the
    /// player. The method prioritizes base item types over custom items when both exist with the same name.</remarks>
    /// <param name="player">The player to whom the item will be added.</param>
    /// <param name="item">The name of the item to add. This can be either a base item type name or the identifier of a registered custom
    /// item. The comparison is case-insensitive.</param>
    /// <returns>An instance of the item that was added to the player.</returns>
    /// <exception cref="Exception">Thrown if the specified item name does not correspond to a valid base item type or a registered custom item.</exception>
    public static ItemBase AddBaseOrCustomItem(ExPlayer player, string item)
    {
        if (Enum.TryParse<ItemType>(item, true, out var itemType))
        {
            var itemInstance = itemType.GetItemInstance<ItemBase>()!;

            itemInstance.TransferItem(player.ReferenceHub);
            return itemInstance;
        }
        else if (CustomItem.RegisteredObjects.TryGetValue(item, out var customItem))
        {
            return customItem.AddItem(player);
        }
        else
        {
            throw new Exception($"Unknown item: {item}");
        }
    }

    /// <summary>
    /// Spawns an item pickup at the specified position and rotation, using either a base item type or a registered
    /// custom item.
    /// </summary>
    /// <remarks>If <paramref name="item"/> matches a base item type, the corresponding item is spawned. If it
    /// matches a registered custom item, that custom item is spawned instead. The method prioritizes base item types
    /// over custom items if both exist with the same name.</remarks>
    /// <param name="item">The name of the item to spawn. This can be the name of a base item type or a registered custom item. The
    /// comparison is case-insensitive.</param>
    /// <param name="position">The world position where the item will be spawned.</param>
    /// <param name="rotation">The rotation to apply to the spawned item.</param>
    /// <returns>An instance of <see cref="ItemPickupBase"/> representing the spawned item pickup.</returns>
    /// <exception cref="Exception">Thrown if <paramref name="item"/> does not match any known base item type or registered custom item.</exception>
    public static ItemPickupBase SpawnBaseOrCustomItem(string item, Vector3 position, Quaternion rotation, bool spawnItem = true)
    {
        if (Enum.TryParse<ItemType>(item, true, out var itemType))
        {
            return ExMap.SpawnItem(itemType, position, Vector3.one, rotation, null, spawnItem);
        }
        else if (CustomItem.RegisteredObjects.TryGetValue(item, out var customItem))
        {
            var pickup = customItem.SpawnItem(position, rotation);

            if (!spawnItem)
                NetworkServer.UnSpawn(pickup.gameObject);

            return pickup;
        }
        else
        {
            throw new Exception($"Unknown item: {item}");
        }
    }
}