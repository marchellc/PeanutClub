using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;

using LabExtended.API;
using LabExtended.API.Custom.Items;

using LabExtended.Extensions;

using mcx.Utilities.Items.Entries;
using mcx.Utilities.Items.Interfaces;
using UnityEngine;

namespace mcx.Utilities.Items
{
    /// <summary>
    /// Provides various utility methods for handling items.
    /// </summary>
    public static class ItemHandler
    {
        private static Dictionary<string, IItemEntry> vanillaItems = new();
        private static Dictionary<string, IItemEntry> customItems = new();
        private static Dictionary<string, IItemEntry> effectItems = new();

        /// <summary>
        /// Attempts to apply an item to the player or add it to their inventory based on the specified item string.
        /// </summary>
        /// <remarks>This method processes the item string to determine the appropriate action (apply or
        /// add) for the specified player. If the operation fails, the method returns <see langword="false"/> without
        /// modifying the player's state.</remarks>
        /// <param name="player">The player to whom the item will be applied or added. Cannot be <see langword="null"/>.</param>
        /// <param name="itemString">A string representation of the item to be applied or added. Must not be <see langword="null"/> or empty.</param>
        /// <returns><see langword="true"/> if the item was successfully applied or added; otherwise, <see langword="false"/>.</returns>
        public static bool TryApplyOrAddItemFromString(this ExPlayer player, string itemString)
            => TryApplyOrAddItemFromString(player, itemString, out _, out _);

        /// <summary>
        /// Attempts to apply an item to the player or add it to their inventory based on the specified item string.
        /// </summary>
        /// <remarks>This method processes the item string to determine the appropriate action (apply or
        /// add) for the specified player. If the operation fails, the method returns <see langword="false"/> without
        /// modifying the player's state.</remarks>
        /// <param name="player">The player to whom the item will be applied or added. Cannot be <see langword="null"/>.</param>
        /// <param name="itemString">A string representation of the item to be applied or added. Must not be <see langword="null"/> or empty.</param>
        /// <returns><see langword="true"/> if the item was successfully applied or added; otherwise, <see langword="false"/>.</returns>
        public static bool TryApplyOrAddItemFromString(this ExPlayer player, string itemString, out ItemBase? item)
            => TryApplyOrAddItemFromString(player, itemString, out item, out _);

        /// <summary>
        /// Attempts to apply an item to the player or spawn it as a pickup, based on the provided item string.
        /// </summary>
        /// <param name="player">The player to whom the item will be applied or for whom the item will be spawned.</param>
        /// <param name="spawnPickup">A value indicating whether the item should be spawned as a pickup instead of being directly applied to the
        /// player. <see langword="true"/> to spawn the item as a pickup; otherwise, <see langword="false"/>.</param>
        /// <param name="itemString">A string representing the item to be applied or spawned. The format and validity of the string must match
        /// the expected item definition.</param>
        /// <returns><see langword="true"/> if the item was successfully applied or spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TryApplyOrSpawnItemFromString(this ExPlayer player, bool spawnPickup, string itemString)
            => TryApplyOrSpawnItemFromString(player, spawnPickup, itemString, out _, out _);

        /// <summary>
        /// Attempts to apply an item to the player or spawn it as a pickup, based on the provided item string.
        /// </summary>
        /// <param name="player">The player to whom the item will be applied or for whom the item will be spawned.</param>
        /// <param name="spawnPickup">A value indicating whether the item should be spawned as a pickup instead of being directly applied to the
        /// player. <see langword="true"/> to spawn the item as a pickup; otherwise, <see langword="false"/>.</param>
        /// <param name="itemString">A string representing the item to be applied or spawned. The format and validity of the string must match
        /// the expected item definition.</param>
        /// <returns><see langword="true"/> if the item was successfully applied or spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TryApplyOrSpawnItemFromString(this ExPlayer player, bool spawnPickup, string itemString, out ItemPickupBase? pickup)
            => TryApplyOrSpawnItemFromString(player, spawnPickup, itemString, out pickup, out _);

        /// <summary>
        /// Attempts to apply an effect or add an item to the player's inventory based on the provided item string.
        /// </summary>
        /// <remarks>If the item string represents an effect, the effect is applied to the player. If it
        /// represents an item, the item is added to the player's inventory.</remarks>
        /// <param name="player">The player to whom the item or effect will be applied or added.</param>
        /// <param name="itemString">A string representing the item or effect to process. The format of the string must be valid for parsing.</param>
        /// <param name="itemEntry">When this method returns, contains the parsed item entry if the operation was successful; otherwise, <see
        /// langword="null"/>.</param>
        /// <returns><see langword="true"/> if the item string was successfully processed and the effect was applied or the item
        /// was added to the inventory; otherwise, <see langword="false"/>.</returns>
        public static bool TryApplyOrAddItemFromString(this ExPlayer player, string itemString, out ItemBase? item, out IItemEntry itemEntry)
        {
            item = null;

            if (!TryGetItemFromString(itemString, out var effectString, out itemEntry))
                return false;

            if (itemEntry.IsEffect && itemEntry is EffectItemEntry effectItemEntry)
            {
                effectItemEntry.ApplyEffect(player, effectString);
            }
            else
            {
                item = itemEntry.AddToInventory(player);
            }

            return true;
        }

        /// <summary>
        /// Attempts to apply an effect or spawn an item based on the provided item string.
        /// </summary>
        /// <remarks>If the item string represents an effect, the effect is applied to the specified
        /// player.  If the item string represents an item, the item is spawned near the player's position,  with the
        /// option to spawn it as a pickup based on the <paramref name="spawnPickup"/> parameter.</remarks>
        /// <param name="player">The player to whom the effect is applied or near whom the item is spawned.</param>
        /// <param name="spawnPickup">A value indicating whether the item should be spawned as a pickup.  If <see langword="true"/>, the item will
        /// be spawned as a pickup; otherwise, it will not.</param>
        /// <param name="itemString">A string representing the item or effect to be applied or spawned.  The format of the string must be valid
        /// and recognizable by the method.</param>
        /// <param name="itemEntry">When this method returns, contains the <see cref="IItemEntry"/> instance representing the item or effect, 
        /// if the operation was successful; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the item string was successfully parsed and the effect was applied or the item was
        /// spawned;  otherwise, <see langword="false"/>.</returns>
        public static bool TryApplyOrSpawnItemFromString(this ExPlayer player, bool spawnPickup, string itemString, out ItemPickupBase? pickup, 
            out IItemEntry itemEntry)
        {
            pickup = null;

            if (!TryGetItemFromString(itemString, out var effectString, out itemEntry))
                return false;

            if (itemEntry.IsEffect && itemEntry is EffectItemEntry effectItemEntry)
            {
                effectItemEntry.ApplyEffect(player, effectString);
            }
            else
            {
                pickup = itemEntry.SpawnPickup(player.Position, player.Rotation, spawnPickup);
            }

            return true;
        }

        /// <summary>
        /// Attempts to apply an effect or spawn an item based on the provided item string.
        /// </summary>
        /// <remarks>If the item string represents an effect, the effect is applied to the specified
        /// player.  If the item string represents an item, the item is spawned near the player's position,  with the
        /// option to spawn it as a pickup based on the <paramref name="spawnPickup"/> parameter.</remarks>
        /// <param name="player">The player to whom the effect is applied or near whom the item is spawned.</param>
        /// <param name="spawnPickup">A value indicating whether the item should be spawned as a pickup.  If <see langword="true"/>, the item will
        /// be spawned as a pickup; otherwise, it will not.</param>
        /// <param name="itemString">A string representing the item or effect to be applied or spawned.  The format of the string must be valid
        /// and recognizable by the method.</param>
        /// <param name="itemEntry">When this method returns, contains the <see cref="IItemEntry"/> instance representing the item or effect, 
        /// if the operation was successful; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the item string was successfully parsed and the effect was applied or the item was
        /// spawned;  otherwise, <see langword="false"/>.</returns>
        public static bool TryApplyOrSpawnItemFromString(this ExPlayer player, Vector3 position, Quaternion rotation, bool spawnPickup, 
            string itemString, out ItemPickupBase? pickup, out IItemEntry itemEntry)
        {
            pickup = null;

            if (!TryGetItemFromString(itemString, out var effectString, out itemEntry))
                return false;

            if (itemEntry.IsEffect && itemEntry is EffectItemEntry effectItemEntry)
            {
                effectItemEntry.ApplyEffect(player, effectString);
            }
            else
            {
                pickup = itemEntry.SpawnPickup(position, rotation, spawnPickup);
            }

            return true;
        }

        /// <summary>
        /// Attempts to retrieve an item entry and its associated effect string from the specified item string.
        /// </summary>
        /// <remarks>The method checks multiple sources for the item entry, including vanilla items,
        /// custom items, and effect items. If the input string contains an underscore, the portion after the first
        /// underscore is treated as the effect string.</remarks>
        /// <param name="itemString">The input string representing the item. This may optionally include an effect string, separated by an
        /// underscore.</param>
        /// <param name="effectString">When this method returns, contains the effect string extracted from the input, if present; otherwise, an
        /// empty string.</param>
        /// <param name="itemEntry">When this method returns, contains the item entry corresponding to the input string, if found; otherwise,
        /// <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the item entry was successfully retrieved; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetItemFromString(string itemString, out string effectString, out IItemEntry itemEntry)
        {
            effectString = string.Empty;

            if (vanillaItems.TryGetValue(itemString, out itemEntry))
                return true;

            if (customItems.TryGetValue(itemString, out itemEntry))
                return true;

            if (itemString.TrySplit('_', true, null, out var parts))
            {
                itemString = parts[0].Trim();
                effectString = string.Join("_", parts.Skip(1)).Trim();
            }

            if (effectItems.TryGetValue(itemString, out itemEntry))
                return true;

            itemEntry = null!;
            return false;
        }

        /// <summary>
        /// Registers an effect with the specified key and entry.
        /// </summary>
        /// <remarks>If an effect with the specified key already exists, it will be replaced with the new
        /// entry.</remarks>
        /// <param name="effectKey">The unique key used to identify the effect. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="entry">The <see cref="EffectItemEntry"/> instance representing the effect to register. Cannot be <see
        /// langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="effectKey"/> is <see langword="null"/> or empty, or if <paramref name="entry"/> is
        /// <see langword="null"/>.</exception>
        public static void RegisterEffect(string effectKey, EffectItemEntry entry)
        {
            if (string.IsNullOrEmpty(effectKey))
                throw new ArgumentNullException(nameof(effectKey));

            if (entry is null)
                throw new ArgumentNullException(nameof(entry));

            effectItems[effectKey] = entry;
        }

        /// <summary>
        /// Unregisters an effect identified by the specified key.
        /// </summary>
        /// <param name="effectKey">The unique key identifying the effect to be unregistered. Cannot be null or empty.</param>
        /// <returns><see langword="true"/> if the effect was successfully unregistered; otherwise, <see langword="false"/> if
        /// the key was not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="effectKey"/> is <see langword="null"/> or empty.</exception>
        public static bool UnregisterEffect(string effectKey)
        {
            if (string.IsNullOrEmpty(effectKey))
                throw new ArgumentNullException(nameof(effectKey));

            return effectItems.Remove(effectKey);
        }

        private static void Internal_Registered(CustomItem customItem)
        {
            customItems[customItem.Id] = new CustomItemEntry(customItem);
        }

        private static void Internal_Unregistered(CustomItem customItem)
        {
            customItems.Remove(customItem.Id);
        }

        internal static void Internal_Init()
        {
            foreach (var itemType in EnumUtils<ItemType>.Values)
            {
                if (itemType == ItemType.None)
                    continue;

                if (!InventoryItemLoader.TryGetItem<ItemBase>(itemType, out var itemBase) || itemBase == null)
                    continue;

                vanillaItems[itemType.ToString()] = new VanillaItemEntry(itemBase);
            }

            foreach (var customItem in CustomItem.RegisteredObjects)
            {
                customItems[customItem.Key] = new CustomItemEntry(customItem.Value);
            }

            CustomItem.Registered += Internal_Registered;
            CustomItem.Unregistered += Internal_Unregistered;
        }
    }
}