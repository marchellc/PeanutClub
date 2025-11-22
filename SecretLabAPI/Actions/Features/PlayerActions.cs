using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms.ShotEvents;

using LabExtended.API;
using LabExtended.API.Containers;
using LabExtended.API.Custom.Items;

using LabExtended.Extensions;
using LabExtended.Utilities;

using PlayerRoles;
using PlayerStatsSystem;

using SecretLabAPI.Audio.Clips;
using SecretLabAPI.Utilities;
using UnityEngine;

namespace SecretLabAPI.Actions.Features
{
    /// <summary>
    /// Actions related to players.
    /// </summary>
    public static class PlayerActions
    {
        private static bool GetHost(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            target = ExPlayer.Host;
            return true;
        }

        private static bool GetPlayer(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            var query = info.GetValue(0, "");

            if (query == "")
                return false;

            if (!ExPlayer.TryGet(query, out var player))
                return false;

            target = player;
            return true;
        }

        private static bool PlayClip(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var name = info.GetValue(0);

            if (name?.Length < 1)
                return true;

            player.PlayClip(name,
                info.GetValue(1, float.TryParse, 1f),
                info.GetValue(2, bool.TryParse, false));

            return true;
        }

        private static bool StopClip(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            player.StopClip(out _);
            return true;
        }

        private static bool AddItem(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var type = info.GetValue(0, "None");
            var amount = info.GetValue<int>(1, int.TryParse, 1);

            if (amount > 0)
            {
                if (Enum.TryParse<ItemType>(type, true, out var itemType))
                {
                    if (itemType.IsAmmo())
                    {
                        player.Ammo.AddAmmo(itemType, (ushort)amount);

                    }
                    else
                    {
                        for (var i = 0; i < amount; i++)
                        {
                            player.Inventory.AddOrSpawnItem(itemType);
                        }
                    }
                }
                else if (CustomItem.TryGet(type, out var item))
                {
                    for (var i = 0; i < amount; i++)
                    {
                        item.AddItem(player);
                    }
                }
            }

            return true;
        }

        private static bool Explode(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var item = info.GetValue(0, Enum.TryParse, ItemType.GrenadeHE);
            var reason = info.GetValue(1, "Boom!");
            var effect = info.GetValue(2, bool.TryParse, true);
            var velocity = info.GetValue(3, float.TryParse, 1f);

            player.Explode(item, reason, effect, velocity);
            return true;
        }

        private static bool Scale(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var scale = info.GetValue(0, Extensions.StringExtensions.TryParseVector3, Vector3.one);

            player.Scale = scale;
            return true;
        }

        private static bool Gravity(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var gravity = info.GetValue(0, Extensions.StringExtensions.TryParseVector3, PositionContainer.DefaultGravity);

            player.Gravity = gravity;
            return true;
        }

        private static bool Pitch(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var pitch = info.GetValue(0, float.TryParse, 1f);

            player.VoicePitch = pitch;
            return true;
        }

        private static bool SetHealth(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var health = info.GetValue(0, float.TryParse, -1f);

            if (health == -1f)
                return true;

            player.Health = health;
            return true;
        }

        private static bool SetMaxHealth(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var maxHealth = info.GetValue(0, float.TryParse, -1f);
            var maxHeal = info.GetValue(1, bool.TryParse, true);

            if (maxHealth == -1f)
                return true;

            player.MaxHealth = maxHealth;

            if (maxHeal || player.Health > player.MaxHealth)
                player.Health = player.MaxHealth;

            return true;
        }

        private static bool Kill(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            player.Kill(info.GetValue(0, "Slipped .."));
            return true;
        }

        private static bool Disintegrate(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            if (!ItemType.ParticleDisruptor.TryGetItemPrefab<ParticleDisruptor>(out var disruptor))
                return true;

            var direction = info.GetValue(0, Extensions.StringExtensions.TryParseVector3, Vector3.up);

            var disruptorShot = new DisruptorShotEvent(disruptor, DisruptorActionModule.FiringState.FiringSingle);
            var disruptorHandler = new DisruptorDamageHandler(disruptorShot, direction, -1f);

            player.ReferenceHub.playerStats.KillPlayer(disruptorHandler);
            return true;
        }

        private static bool SetRole(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var role = info.GetValue(0, Enum.TryParse, RoleTypeId.None);

            var keepPosition = info.GetValue(1, bool.TryParse, false);
            var keepInventory = info.GetValue(2, bool.TryParse, false);
            var assignInventory = info.GetValue(3, bool.TryParse, true);

            if (keepInventory)
                assignInventory = false;

            if (role == RoleTypeId.None)
                return true;

            if (keepInventory)
            {
                var items = player.Inventory.Items.ToList();

                var ammo = player.Ammo.Ammo.ToDictionary();
                var customAmmo = player.Ammo.CustomAmmo.ToDictionary();

                player.Role.Set(role, RoleChangeReason.RemoteAdmin, keepPosition ? RoleSpawnFlags.None : RoleSpawnFlags.UseSpawnpoint);

                TimingUtils.AfterFrames(() =>
                {
                    player.Ammo.ClearAmmo();
                    player.Ammo.ClearCustomAmmo();

                    player.Ammo.Ammo.AddRange(ammo);
                    player.Ammo.CustomAmmo.AddRange(customAmmo);

                    ammo.Clear();
                    customAmmo.Clear();

                    player.Inventory.Clear();

                    foreach (var item in items)
                        player.Inventory.UserInventory.Items[item.ItemSerial] = item;

                    items.Clear();

                    player.Inventory.Inventory.SendItemsNextFrame = true;
                    player.Inventory.Inventory.SendAmmoNextFrame = true;
                }, 2);
            }
            else
            {
                if (assignInventory)
                    player.Role.Set(role, RoleChangeReason.RemoteAdmin, keepPosition ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.All);
                else
                    player.Role.Set(role, RoleChangeReason.RemoteAdmin, keepPosition ? RoleSpawnFlags.None : RoleSpawnFlags.UseSpawnpoint);
            }

            return true;
        }

        private static bool Teleport(ref object target, ActionInfo info, int index, List<ActionInfo> actions)
        {
            if (target is not ExPlayer player)
                return true;

            var position = info.GetValue(0, string.Empty);
            var angle = info.GetValue(1, float.TryParse, -1f);

            if (position == string.Empty)
                return true;

            if (MapUtilities.TryGet(position, angle == -1f ? null : angle, out var worldPosition, out var rotation))
            {
                player.Position.Position = worldPosition;
                player.Rotation.Rotation = rotation;
            }
            else if (Extensions.StringExtensions.TryParseVector3(position, out worldPosition))
            {
                player.Position.Position = worldPosition;
            }

            return true;
        }

        private static bool ClearBroadcasts(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            player.ClearBroadcasts();
            return true;
        }

        private static bool Broadcast(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var message = info.GetValue(0, "");
            var duration = info.GetValue(1, ushort.TryParse, (ushort)5);
            var truncate = info.GetValue(2, bool.TryParse, false);

            if (message == "")
                return true;

            player.SendBroadcast(message, duration, truncate ? global::Broadcast.BroadcastFlags.Truncated : global::Broadcast.BroadcastFlags.Normal);
            return true;
        }

        private static bool Hint(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            if (target is not ExPlayer player)
                return true;

            var message = info.GetValue(0, "");
            var duration = info.GetValue(1, ushort.TryParse, (ushort)5);
            var priority = info.GetValue(2, bool.TryParse, false);

            if (message == "")
                return true;

            player.SendHint(message, duration, priority);
            return true;
        }
    }
}