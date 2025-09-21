using LabExtended.API;
using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Utilities;

using PeanutClub.Items.Weapons.SniperRifle;

using UnityEngine;

namespace PeanutClub.Items.Spawning
{
    /// <summary>
    /// Spawns items at predefined positions.
    /// </summary>
    public static class SpawnPositions
    {
        /// <summary>
        /// Gets the list of custom item spawn positions from the config.
        /// </summary>
        public static Dictionary<string, List<string>> Positions => ItemsCore.ConfigStatic.CustomSpawns;

        private static void Internal_RoundStarted()
        {
            foreach (var pair in Positions)
            {
                if (pair.Key != "ExamplePosition")
                {
                    if (MapUtilities.TryGet(pair.Key, null, out var position, out var rotation))
                    {
                        foreach (var item in pair.Value)
                        {
                            if (Enum.TryParse<ItemType>(item, true, out var itemType))
                            {
                                if (itemType != ItemType.None)
                                {
                                    ExMap.SpawnItem(itemType, position, Vector3.one, rotation);
                                }
                                else
                                {
                                    ApiLog.Error("Item Spawnpoint", "Cannot spawn item of type &1None&r");
                                }
                            }
                            else
                            {
                                switch (item)
                                {
                                    case "SniperRifle":
                                        SniperRifleHandler.SpawnSniperRifle(position);
                                        break;

                                    default:
                                        ApiLog.Error("Item Spawnpoint", $"Could not parse item type &1{item}&r");
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ApiLog.Error("Item Spawnpoints", $"Could not find spawn position &1{pair.Key}&r");
                    }
                }
            }
        }

        internal static void Internal_Init()
        {
            ExRoundEvents.Started += Internal_RoundStarted;
        }
    }
}