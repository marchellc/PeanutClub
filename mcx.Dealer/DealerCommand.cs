using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using LabExtended.Extensions;

using mcx.Dealer.API;

using UnityEngine;

namespace mcx.Dealer
{
    [Command("dealer", "Dealer management commands.")]
    public class DealerCommand : CommandBase, IServerSideCommand
    {
        /// <summary>
        /// Lists all active dealers.
        /// </summary>
        [CommandOverload("list", "Lists all active dealers.")]
        public void List()
        {
            if (DealerManager.Dealers.Count == 0)
            {
                Fail($"No dealers active.");
                return;
            }

            Ok(x =>
            {
                x.AppendLine();

                foreach (var dealer in DealerManager.Dealers)
                {
                    if (dealer.IsDestroyed)
                        continue;

                    x.AppendLine($"- {dealer.Id} ({dealer.Player.Position.Room?.Name}) [{dealer.Player.Position.DistanceTo(Sender)}m]");
                }
            });
        }

        /// <summary>
        /// Destroys a dealer by ID.
        /// </summary>
        [CommandOverload("destroy", "Destroys an active dealer instance.")]
        public void Destroy(
            [CommandParameter("ID", "The ID of the instance")] string dealerId)
        {
            if (!DealerManager.Dealers.TryGetFirst(x => x.Id == dealerId && !x.IsDestroyed, out var dealer))
            {
                Fail($"Unknown dealer instance.");
                return;
            }

            dealer.DestroyInstance();

            Ok($"Destroyed dealer ID: {dealer.Id}");
        }

        /// <summary>
        /// Spawns a new dealer.
        /// </summary>
        [CommandOverload("spawn", "Spawns a new dealer instance.")]
        public void Spawn(
            [CommandParameter("ID", "The ID of the new instance.")] string dealerId)
        {
            var position = Sender.Position.Position;

            position.y += 0.5f;

            var instance = DealerManager.SpawnDealer(position, Sender.Rotation, dealerId);

            Ok($"Spawned a new dealer instance (ID: {instance.Id})");
        }

        /// <summary>
        /// Refreshes dealer inventories.
        /// </summary>
        [CommandOverload("refresh", "Refreshes dealer inventories.")]
        public void Refresh(
            [CommandParameter("ID", "The ID of the targeted dealer (or 'all' for all dealers)")] string dealerId, 
            [CommandParameter("UserID", "The user ID of the player for whom to refresh the inventory (or 'all' for all players).")] string targetId)
        {
            var count = 0;

            if (string.Equals(dealerId, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                if (DealerManager.Dealers.Count == 0)
                {
                    Fail("No active dealers.");
                    return;
                }

                foreach (var dealer in DealerManager.Dealers)
                {
                    if (dealer.IsDestroyed)
                        continue;

                    if (!DealerManager.Inventories.TryGetValue(dealer.Id, out var inventories))
                        continue;

                    if (string.Equals(targetId, "all", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var id in inventories.Keys.ToArray())
                        {
                            _ = DealerManager.GetDealerInventory(dealer.Id, id, true);

                            count++;
                        }
                    }
                    else
                    {
                        _ = DealerManager.GetDealerInventory(dealer.Id, targetId, true);

                        count++;
                    }
                }
            }
            else
            {
                if (!DealerManager.Dealers.TryGetFirst(x => x.Id == dealerId, out var dealer))
                {
                    Fail($"Unknown dealer ID");
                    return;
                }

                if (!DealerManager.Inventories.TryGetValue(dealer.Id, out var inventories))
                {
                    Fail($"No inventories found for this dealer.");
                    return;
                }

                if (string.Equals(targetId, "all", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var id in inventories.Keys.ToArray())
                    {
                        _ = DealerManager.GetDealerInventory(dealer.Id, id, true);

                        count++;
                    }
                }
                else
                {
                    _ = DealerManager.GetDealerInventory(dealer.Id, targetId, true);

                    count++;
                }
            }

            Ok($"Refreshed {count} inventories.");
        }

        /// <summary>
        /// Shows the inventory for a specific player.
        /// </summary>
        [CommandOverload("view", "Shows the inventory for a specific player.")]
        public void View(
            [CommandParameter("ID", "The ID of the target dealer.")] string dealerId, 
            [CommandParameter("UserID", "The user ID of the target player.")] string targetId)
        {
            if (!DealerManager.Dealers.TryGetFirst(x => x.Id == dealerId, out var dealer))
            {
                Fail($"Unknown dealer ID");
                return;
            }

            if (!DealerManager.Inventories.TryGetValue(dealer.Id, out var inventories))
            {
                Fail($"No inventories found for this dealer.");
                return;
            }

            if (!inventories.TryGetValue(targetId, out var inventory))
            {
                Fail($"Inventory for user ID {targetId} has not been generated.");
                return;
            }

            Ok(x =>
            {
                x.AppendLine($"Inventory for dealer ID {dealer.Id} and user ID {targetId}:");
                x.AppendLine();

                for (var i = 0; i < inventory.Items.Count; i++)
                {
                    var item = inventory.Items[i];

                    x.AppendLine($"- [{i}] {item.Entry.Item} ({item.CurrentPrice} coins)");
                }
            });
        }

        /// <summary>
        /// Adds a new item to the inventory of a specified dealer for sale.
        /// </summary>
        [CommandOverload("add", "Adds a new item to be sold by a dealer.")]
        public void Add(
            [CommandParameter("ID", "The ID of the targeted dealer.")] string dealerId, 
            [CommandParameter("UserID", "The user ID of the targeted inventory.")] string targetId, 
            [CommandParameter("Item", "The type of the item to add (ItemType enum names or a name of a custom item)")] string itemType,
            [CommandParameter("Rarity", "The rarity level of the item (0 - 5)")] byte rarityLevel,
            [CommandParameter("Price", "The price of the item (in coins).")] int itemPrice)
        {
            if (!DealerManager.Dealers.TryGetFirst(x => x.Id == dealerId, out var dealer))
            {
                Fail($"Unknown dealer ID");
                return;
            }

            if (!DealerManager.Inventories.TryGetValue(dealer.Id, out var inventories))
            {
                Fail($"No inventories found for this dealer.");
                return;
            }

            if (!inventories.TryGetValue(targetId, out var inventory))
            {
                Fail($"Inventory for user ID {targetId} has not been generated.");
                return;
            }

            var entry = new DealerEntry()
            {
                Item = itemType,
                Price = itemPrice,
                Rarity = (byte)Mathf.Clamp(rarityLevel, 0f, 5f)
            };

            inventory.Items.Add(new(entry, itemPrice, itemPrice, 0));

            Ok($"Added item to inventory. ({dealerId} -> {targetId})");
        }

        /// <summary>
        /// Removes an item from a dealer's inventory.
        /// </summary>
        [CommandOverload("remove", "Removes an item from a dealer's inventory.")]
        public void Remove(
            [CommandParameter("ID", "The ID of the targeted dealer.")] string dealerId,
            [CommandParameter("UserID", "The user ID of the targeted inventory.")] string targetId,
            [CommandParameter("Index", "The index of the item to remove.")] int index)
        {
            if (!DealerManager.Dealers.TryGetFirst(x => x.Id == dealerId, out var dealer))
            {
                Fail($"Unknown dealer ID");
                return;
            }

            if (!DealerManager.Inventories.TryGetValue(dealer.Id, out var inventories))
            {
                Fail($"No inventories found for this dealer.");
                return;
            }

            if (!inventories.TryGetValue(targetId, out var inventory))
            {
                Fail($"Inventory for user ID {targetId} has not been generated.");
                return;
            }

            if (index < 0 || index >= inventory.Items.Count)
            {
                Fail($"Invalid item index.");
                return;
            }

            var item = inventory.Items.RemoveAndTake(index);

            Ok($"Removed item from inventory ({dealerId} -> {targetId}), removed: {item.Entry.Item} for {item.CurrentPrice} coins");
        }
    }
}