using LabExtended.API;
using LabExtended.API.Custom.Items;

using mcx.Utilities.Actions.Interfaces;

using UnityEngine;

using InventorySystem.Items;

namespace mcx.Utilities.Actions.Features.Items
{
    /// <summary>
    /// Adds an item to a player's inventory.
    /// </summary>
    public class AddItemAction : IAction
    {
        /// <inheritdoc/>
        public string Id { get; } = "AddItem";

        /// <inheritdoc/>
        public string DebugAction(Dictionary<string, string> parameters)
        {
            parameters["Type"] = "The type of the item to add (item IDs from the ItemType enum or IDs of custom items).";
            parameters["Amount"] = "The amount of items to add. Default is 1.";
            parameters["Spawn"] = "Whether to spawn the item if the inventory is full. Default is true.";

            return "Adds an item to a player's inventory.";
        }

        /// <inheritdoc/>
        public ActionResult Trigger(ref ActionContext context)
        {
            var amount = context.GetParameterOrDefault("Amount", 0, int.TryParse, 1);
            var spawn = context.GetParameterOrDefault("Spawn", 0, bool.TryParse, true);

            var type = context.GetParameterOrDefault("Type", 0, "None");

            if (amount < 1)
                return ActionResult.Failure;

            if (Enum.TryParse<ItemType>(type, true, out var itemType))
            {
                if (itemType is ItemType.None)
                    return ActionResult.Failure;

                foreach (var target in context.Targets)
                {
                    if (!target.IsPlayer(out var player))
                        continue;

                    for (var i = 0; i < amount; i++)
                    {
                        if (player.Player.Inventory.ItemCount >= 8)
                        {
                            ExMap.SpawnItem(itemType, player.Player.Position, Vector3.one, player.Player.Rotation);
                        }
                        else
                        {
                            player.Player.Inventory.AddItem(itemType, ItemAddReason.AdminCommand);
                        }
                    }
                }

                return ActionResult.Success;
            }
            else if (CustomItem.TryGet(type, out var customItem))
            {
                foreach (var target in context.Targets)
                {
                    if (!target.IsPlayer(out var player))
                        continue;

                    for (var i = 0; i < amount; i++)
                    {
                        if (player.Player.Inventory.ItemCount >= 8)
                        {
                            customItem.SpawnItem(player.Player.Position, player.Player.Rotation);
                        }
                        else
                        {
                            customItem.AddItem(player.Player);
                        }
                    }
                }

                return ActionResult.Success;
            }

            return ActionResult.Failure;
        }
    }
}