using LabExtended.API.Custom.Items;

using LabExtended.Core;
using LabExtended.Extensions;

using SecretLabAPI.Actions.API;
using SecretLabAPI.Actions.Attributes;

using SecretLabAPI.Extensions;
using SecretLabAPI.Utilities;

using UnityEngine;

namespace SecretLabAPI.Actions.Functions
{
    /// <summary>
    /// Contains player functions.
    /// </summary>
    public static class PlayerFunctions
    {
        /// <summary>
        /// Detonates a grenade at each player in the specified context, causing them to explode with customizable
        /// effects and ragdoll velocity.
        /// </summary>
        /// <remarks>The explosion parameters are determined by values in the context: grenade type, death
        /// reason, whether the explosion affects other players, and the ragdoll velocity multiplier. This method
        /// applies the explosion to all players in the context.</remarks>
        /// <param name="context">A reference to the action context containing player information and parameters for the explosion, including
        /// grenade type, death reason, effect-only mode, and velocity multiplier.</param>
        /// <returns>An ActionResultFlags value indicating the result of the action. Returns SuccessDispose if the explosion was
        /// performed successfully.</returns>
        [Action("Explode", "Explodes a player.")]
        [ActionParameter("Type", "The type of the grenade to spawn.")]
        [ActionParameter("Reason", "The reason for the player's death.")]
        [ActionParameter("EffectOnly", "Whether or not the grenade should damage other players.")]
        [ActionParameter("Velocity", "The velocity multiplier for the player's ragdoll.")]
        public static ActionResultFlags Explode(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) =>
            {
                return index switch
                {
                    0 => p.EnsureCompiled(Enum.TryParse, ItemType.GrenadeHE),
                    1 => p.EnsureCompiled("You get five big booms!"),
                    2 => p.EnsureCompiled(bool.TryParse, true),
                    3 => p.EnsureCompiled(float.TryParse, 10f),

                    _ => false
                };
            });

            var type = context.GetMemoryOrValue<ItemType>("GrenadeType", 0);
            var reason = context.GetMemoryOrValue("DeathReason", 1);
            var effect = context.GetMemoryOrValue<bool>("EffectOnly", 2);
            var velocity = context.GetMemoryOrValue<float>("Velocity", 3);

            context.Player.Explode(type, reason, effect, velocity);
            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Adds an item or a specified amount of items to each player's inventory within the provided action context.
        /// Supports both standard item types and custom items, as well as ammo types.
        /// </summary>
        /// <remarks>The item type can be specified as a value from the ItemType enumeration or as the
        /// identifier of a custom item. If the item type represents ammo, the amount parameter determines the quantity
        /// of ammo to add; otherwise, it determines the number of items to add. The operation is performed for all
        /// players in the context.</remarks>
        /// <param name="context">A reference to the action context containing player information and parameters specifying the item type and
        /// amount to add. Must not be null.</param>
        /// <returns>An ActionResultFlags value indicating the result of the operation. Returns SuccessDispose if the items were
        /// added successfully.</returns>
        [Action("AddItem", "Adds an item to a player's inventory.")]
        [ActionParameter("Type", "Sets the type of the item to add (can be an item from the ItemType enum or an ID of a custom item).")]
        [ActionParameter("Amount", "Sets the amount of items to add (or the amount of ammo to add if the Type is an ammo).")]
        public static ActionResultFlags AddItem(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) =>
            {
                return index switch
                {
                    0 => p.EnsureCompiled("None"),
                    1 => p.EnsureCompiled(int.TryParse, 1),

                    _ => false
                };
            });

            var item = context.GetMemoryOrValue("ItemType", 0);
            var amount = context.GetMemoryOrValue<int>("ItemAmount", 1);
            var p = context.Player;

            if (Enum.TryParse<ItemType>(item, true, out var itemType))
            {
                if (itemType.IsAmmo())
                {
                    p.Ammo.AddAmmo(itemType, (ushort)amount);
                }
                else
                {
                    for (var i = 0; i < amount; i++)
                    {
                        p.Inventory.AddItem(itemType);
                    }
                }
            }
            else if (CustomItem.TryGet(item, out var customItem))
            {
                for (var i = 0; i < amount; i++)
                {
                    customItem.AddItem(p);
                }
            }

            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Adds multiple items to the player's inventory based on the provided item list in the action context.
        /// </summary>
        /// <remarks>The item list in the context can include both standard item types and custom item
        /// IDs. Each entry must specify the item and the amount, separated by a colon. Invalid or unrecognized items
        /// are skipped, and warnings are logged. The method processes all players in the context, adding the specified
        /// items to each player's inventory.</remarks>
        /// <param name="context">A reference to the action context containing the item list to add. The context must include a value
        /// formatted as 'ItemTypeOrCustomItemID:Amount,Item2:Amount2,Item3:Amount3'.</param>
        /// <returns>An ActionResultFlags value indicating the result of the operation. Returns SuccessDispose if the items were
        /// added successfully.</returns>
        [Action("AddItems", "Adds multiple items to a player's inventory.")]
        [ActionParameter("List", "The list of items to add (formatted as 'ItemTypeOrCustomItemID:Amount,Item2:Amount2,Item3:Amount3')")]
        public static ActionResultFlags AddItems(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) => p.EnsureCompiled(string.Empty));

            var str = context.GetValue(0);

            var items = context.GetMetadata<List<(ItemType BaseItem, CustomItem? CustomItem, int Amount)>>("ParsedItems", () =>
            {
                var list = new List<(ItemType, CustomItem?, int)>();
                var array = str.SplitEscaped(',');

                foreach (var item in array)
                {
                    var parts = item.SplitEscaped(':');

                    if (parts.Length != 2)
                    {
                        if (parts.Length == 1)
                            parts = [parts[0], "1"];

                        ApiLog.Warn("ActionManager", $"[&6AddItems&r] Invalid formatting: &3{item}&r");
                        continue;
                    }

                    var itemPart = parts[0];
                    var amountPart = parts[1];

                    if (!int.TryParse(amountPart, out var amount))
                    {
                        ApiLog.Warn("ActionManager", $"[&6AddItems&r] Invalid formatting: &3{item}&r (amount could not be parsed)");
                        continue;
                    }

                    if (amount < 1)
                    {
                        ApiLog.Warn("ActionManager", $"[&6AddItems&r] Invalid formatting: &3{item}&r (amount is less than one)");
                        continue;
                    }

                    if (Enum.TryParse<ItemType>(itemPart, true, out var itemType))
                    {
                        if (itemType != ItemType.None)
                        {
                            list.Add((itemType, null, amount));
                        }
                        else
                        {
                            ApiLog.Warn("ActionManager", $"[&6AddItems&r] Invalid formatting: &3{item}&r (item type cannot be None)");
                            continue;
                        }
                    }
                    else if (CustomItem.TryGet(itemPart, out var customItem))
                    {
                        list.Add((ItemType.None, customItem, amount));
                    }
                    else
                    {
                        ApiLog.Warn("ActionManager", $"[&6AddItems&r] Invalid formatting: &3{item}&r (item could not be parsed and no custom items were found)");
                        continue;
                    }
                }

                return list;
            });

            var p = context.Player;

            foreach (var item in items)
            {
                for (var i = 0; i < item.Amount; i++)
                {
                    if (item.CustomItem != null)
                    {
                        item.CustomItem.AddItem(p);
                    }
                    else
                    {
                        p.Inventory.AddItem(item.BaseItem);
                    }
                }
            }

            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Clears items and/or ammunition from the player's inventory based on the specified context parameters.
        /// </summary>
        /// <remarks>Set the 'Items' and 'Ammo' parameters in the context to control which parts of the
        /// inventory are cleared. This method affects all players referenced in the context.</remarks>
        /// <param name="context">A reference to the action context containing parameters that determine whether to clear items and/or
        /// ammunition. Must be compiled and provide boolean values for 'Items' and 'Ammo'.</param>
        /// <returns>An ActionResultFlags value indicating the result of the operation. Returns SuccessDispose if the inventory
        /// was cleared as specified.</returns>
        [Action("ClearInventory", "Clears the player's inventory.")]
        [ActionParameter("Items", "Whether or not to clear items.")]
        [ActionParameter("Ammo", "Whether or not to clear ammo.")]
        public static ActionResultFlags ClearInventory(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) => p.EnsureCompiled<bool>(bool.TryParse, true));

            var items = context.GetValue<bool>(0);
            var ammo = context.GetValue<bool>(1);
            var p = context.Player;

            if (items)
            {
                p.Inventory.Clear();
            }

            if (ammo)
            {
                p.Ammo.ClearAmmo();
                p.Ammo.ClearCustomAmmo();
            }

            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Removes a specified number of items from each player's inventory, optionally filtering by item type.
        /// </summary>
        /// <remarks>If the item type is specified, only items matching that type are removed. If no type
        /// is specified, items are removed in order from the inventory. The amount parameter determines how many items
        /// to remove per player. This method modifies the inventories of all players included in the context.</remarks>
        /// <param name="context">A reference to the action context containing parameters for item type and amount, and the target players
        /// whose inventories will be modified.</param>
        /// <returns>An ActionResultFlags value indicating the result of the operation. Returns SuccessDispose if the items were
        /// cleared successfully.</returns>
        [Action("ClearItems", "Clears a specific amount of items from a player's inventory.")]
        [ActionParameter("Type", "The type of the item to clear.")]
        [ActionParameter("Amount", "The amount of items to clear.")]
        public static ActionResultFlags ClearItems(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) =>
            {
                return index switch
                {
                    0 => p.EnsureCompiled(Enum.TryParse, ItemType.None),
                    1 => p.EnsureCompiled(int.TryParse, 1),

                    _ => false
                };
            });

            var type = context.GetValue<ItemType>(0);
            var amount = context.GetValue<int>(1);
            var p = context.Player;

            if (amount <= 1)
            {
                if (type != ItemType.None)
                {
                    if (p.Inventory.Items.TryGetFirst(x => x.ItemTypeId == type, out var targetItem))
                    {
                        p.Inventory.RemoveItem(targetItem);
                    }
                }
                else
                {
                    if (p.Inventory.ItemCount > 0)
                    {
                        p.Inventory.RemoveItem(p.Inventory.Items.First());
                    }
                }
            }
            else
            {
                if (type != ItemType.None)
                {
                    p.Inventory.RemoveItems(type, amount);
                }
                else
                {
                    amount = Mathf.Min(amount, p.Inventory.ItemCount);

                    var items = p.Inventory.Items.Take(amount);

                    items.ToList().ForEach(item => p.Inventory.RemoveItem(item));
                }
            }

            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Removes a specified number of items from each player's inventory, optionally filtering by item type.
        /// </summary>
        /// <remarks>If the item type is specified, only items matching that type are removed. If no type
        /// is specified, items are removed in order from the inventory. The amount parameter determines how many items
        /// to remove per player. This method modifies the inventories of all players included in the context.</remarks>
        /// <param name="context">A reference to the action context containing parameters for item type and amount, and the target players
        /// whose inventories will be modified.</param>
        /// <returns>An ActionResultFlags value indicating the result of the operation. Returns SuccessDispose if the items were
        /// dropped successfully.</returns>
        [Action("DropItems", "Drops a specific amount of items from a player's inventory.")]
        [ActionParameter("Type", "The type of the item to drop.")]
        [ActionParameter("Amount", "The amount of items to drop.")]
        public static ActionResultFlags DropItems(ref ActionContext context)
        {
            context.EnsureCompiled((index, p) =>
            {
                return index switch
                {
                    0 => p.EnsureCompiled(Enum.TryParse, ItemType.None),
                    1 => p.EnsureCompiled(int.TryParse, 1),

                    _ => false
                };
            });

            var type = context.GetValue<ItemType>(0);
            var amount = context.GetValue<int>(1);
            var player = context.Player;

            if (amount <= 1)
            {
                if (type != ItemType.None)
                {
                    if (context.Player.Inventory.Items.TryGetFirst(x => x.ItemTypeId == type, out var targetItem))
                    {
                        context.Player.Inventory.DropItem(targetItem);
                    }
                }
                else
                {
                    if (context.Player.Inventory.ItemCount > 0)
                    {
                        context.Player.Inventory.DropItem(context.Player.Inventory.Items.First());
                    }
                }
            }
            else
            {
                if (type != ItemType.None)
                {
                    var count = context.Player.Inventory.Items.Count(x => x.ItemTypeId == type);
                    var items = context.Player.Inventory.Items.Where(x => x.ItemTypeId == type).Take(Mathf.Min(amount, count));

                    items.ToList().ForEach(item => player.Inventory.DropItem(item));
                }
                else
                {
                    var items = player.Inventory.Items.Take(amount);

                    items.ToList().ForEach(item => player.Inventory.DropItem(item));
                }
            }

            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Drops the currently held item from each player's inventory within the specified action context.
        /// </summary>
        /// <param name="context">A reference to the action context containing the players whose held items will be dropped. Cannot be null.</param>
        /// <returns>An ActionResultFlags value indicating the result of the drop operation. Returns SuccessDispose if the
        /// operation completes successfully.</returns>
        [Action("DropHeldItem", "Drops the currently held item from a player's inventory.")]
        public static ActionResultFlags DropHeldItem(ref ActionContext context)
        {
            context.Player.Inventory.DropHeldItem();
            return ActionResultFlags.SuccessDispose;
        }

        /// <summary>
        /// Removes the currently held item from each player's inventory within the specified action context.
        /// </summary>
        /// <param name="context">A reference to the action context containing the players whose held items will be removed.</param>
        /// <returns>An ActionResultFlags value indicating that the held items were successfully removed and the action should be
        /// disposed.</returns>
        [Action("RemoveHeldItem", "Removes the currently held item from the player's inventory.")]
        public static ActionResultFlags RemoveHeldItem(ref ActionContext context)
        {
            context.Player.Inventory.RemoveHeldItem();
            return ActionResultFlags.SuccessDispose;
        }
    }
}