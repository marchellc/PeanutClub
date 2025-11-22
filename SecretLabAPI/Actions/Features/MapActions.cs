using LabExtended.API;
using LabExtended.API.Custom.Items;

using SecretLabAPI.Utilities;

using UnityEngine;

using Utils;

namespace SecretLabAPI.Actions.Features
{
    /// <summary>
    /// Actions related to the map.
    /// </summary>
    public static class MapActions
    {
        private static bool SpawnItem(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not Vector3 position)
            {
                if (target is ExPlayer player)
                {
                    position = player.Position;
                }
                else
                {
                    return true;
                }
            }

            var type = info.GetValue(0, "None");
            var amount = info.GetValue(1, int.TryParse, 1);
            var scale = info.GetValue(2, Extensions.StringExtensions.TryParseVector3, Vector3.one);

            if (amount > 0)
            {
                if (Enum.TryParse<ItemType>(type, true, out var itemType))
                {
                    for (var i = 0; i < amount; i++)
                    {
                        ExMap.SpawnItem(itemType, position, scale, Quaternion.identity);
                    }
                }
                else if (CustomItem.TryGet(type, out var item))
                {
                    for (var i = 0; i < amount; i++)
                    {
                        item.SpawnItem(position, null);
                    }
                }
            }

            return true;
        }

        private static bool SpawnProjectile(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not Vector3 position)
            {
                if (target is ExPlayer player)
                {
                    position = player.Position;
                }
                else
                {
                    return true;
                }
            }

            var type = info.GetValue(0, "None");
            var force = info.GetValue(1, float.TryParse, 3f);
            var velocity = info.GetValue(2, Extensions.StringExtensions.TryParseVector3, Vector3.forward);
            var scale = info.GetValue(3, Extensions.StringExtensions.TryParseVector3, Vector3.one);

            ItemHelper.TrySpawnCustomOrBaseProjectile(type, position, scale, velocity, Quaternion.identity, force, out _);
            return true;
        }

        private static bool SpawnExplosion(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not Vector3 position)
            {
                if (target is ExPlayer player)
                {
                    position = player.Position;
                }
                else
                {
                    return true;
                }
            }

            var type = info.GetValue(0, Enum.TryParse, ExplosionType.Grenade);

            ExplosionUtils.ServerExplode(position, ExPlayer.Host.Footprint, type);
            return true;
        }
    }
}