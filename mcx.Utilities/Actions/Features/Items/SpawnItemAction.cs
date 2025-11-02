using LabExtended.API;
using LabExtended.API.Custom.Items;

using UnityEngine;

using mcx.Utilities.Extensions;
using mcx.Utilities.Actions.Interfaces;

namespace mcx.Utilities.Actions.Features.Items
{
    /// <summary>
    /// Spawns an item on the ground.
    /// </summary>
    public class SpawnItemAction : IAction
    {
        /// <inheritdoc/>
        public string Id { get; } = "SpawnItem";

        /// <inheritdoc/>
        public string DebugAction(Dictionary<string, string> parameters)
        {
            parameters["Type"] = "The type of item to spawn, can be an item ID from the ItemType enum or a custom item ID.";
            parameters["Amount"] = "The amount of items to spawn.";
            parameters["Scale"] = "The scale of the spawned item - formatted as a Vector3 split by commas, example: 1,1,1";

            return "Spawns an item on the ground.";
        }

        /// <inheritdoc/>
        public ActionResult Trigger(ref ActionContext context)
        {
            var amount = context.GetParameterOrDefault("Amount", 0, int.TryParse, 1);
            var scale = context.GetParameterOrDefault("Scale", 0, StringExtensions.TryParseVector3, Vector3.one);

            var type = context.GetParameterOrDefault("Type", 0, "None");

            if (amount < 1
                || scale == Vector3.zero)
                return ActionResult.Failure;

            if (Enum.TryParse<ItemType>(type, true, out var itemType))
            {
                foreach (var target in context.Targets)
                {
                    var position = target.GetPosition();

                    for (var i = 0; i < amount; i++)
                        ExMap.SpawnItem(itemType, position, scale, Quaternion.identity);
                }

                return ActionResult.Success;
            }
            else if (CustomItem.TryGet(type, out var customItem))
            {
                foreach (var target in context.Targets)
                {
                    var position = target.GetPosition();

                    for (var i = 0; i < amount; i++)
                        customItem.SpawnItem(position, null);
                }

                return ActionResult.Success;
            }

            return ActionResult.Failure;
        }
    }
}