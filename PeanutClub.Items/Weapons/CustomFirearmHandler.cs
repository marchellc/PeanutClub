using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;

using InventorySystem.Items.Pickups;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabApi.Features.Wrappers;

using LabExtended.API;
using LabExtended.Extensions;
using LabExtended.Utilities.Firearms;

using LabExtended.Events;
using LabExtended.Events.Player;

using PeanutClub.Overlays.Alerts;

using PlayerStatsSystem;

using UnityEngine;

using FirearmPickup = InventorySystem.Items.Firearms.FirearmPickup;

namespace PeanutClub.Items.Weapons
{
    public static class CustomFirearmHandler
    {
        /// <summary>
        /// An event for additional setup when a custom firearm is given to a player.
        /// </summary>
        public static event Action<ExPlayer, Firearm, CustomFirearmProperties>? SetupFirearm;

        /// <summary>
        /// Gets called when a custom firearm is removed via the <see cref="Remove(ushort, bool)"/> method.
        /// </summary>
        public static event Action<ushort, CustomFirearmProperties>? Removed;

        /// <summary>
        /// Gets called when a custom firearm is destroyed via the <see cref="Remove(ushort, bool)"/> method.
        /// </summary>
        public static event Action<ExPlayer?, Firearm?, FirearmPickup?, CustomFirearmProperties>? Destroyed;

        /// <summary>
        /// Gets a list of all tracked item serials.
        /// </summary>
        public static Dictionary<ushort, CustomFirearmProperties> TrackedItems { get; } = new();

        /// <summary>
        /// Whether or not a specific item serial is tracked as a custom firearm.
        /// </summary>
        /// <param name="itemSerial">The serial number of the item.</param>
        /// <returns>true if the serial number is tracked as a sniper rifle</returns>
        public static bool IsCustomFirearm(ushort itemSerial)
            => itemSerial != 0 && TrackedItems.ContainsKey(itemSerial);

        /// <summary>
        /// Whether or not a specific item serial is tracked as a custom firearm.
        /// </summary>
        /// <param name="itemSerial">The serial number of the item.</param>
        /// <returns>true if the serial number is tracked as a sniper rifle</returns>
        public static bool IsCustomFirearm<T>(ushort itemSerial) where T : CustomFirearmProperties
            => itemSerial != 0 && TrackedItems.TryGetValue(itemSerial, out var props) && props is T;

        /// <summary>
        /// Whether or not a specific item serial is tracked as a custom firearm.
        /// </summary>
        /// <param name="itemSerial">The serial number of the item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the serial number is tracked as a sniper rifle</returns>
        public static bool IsCustomFirearm(ushort itemSerial, out CustomFirearmProperties properties)
        {
            properties = null!;

            if (itemSerial == 0 || !TrackedItems.TryGetValue(itemSerial, out properties))
                return false;

            return true;
        }

        /// <summary>
        /// Whether or not a specific item serial is tracked as a custom firearm.
        /// </summary>
        /// <param name="itemSerial">The serial number of the item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the serial number is tracked as a sniper rifle</returns>
        public static bool IsCustomFirearm<T>(ushort itemSerial, out T properties) where T : CustomFirearmProperties
        {
            properties = null!;

            if (itemSerial == 0 || !TrackedItems.TryGetValue(itemSerial, out properties))
                return false;

            return true;
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this Item item)
            => item?.Base != null && IsCustomFirearm(item.Serial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this Item item) where T : CustomFirearmProperties
            => item?.Base != null && IsCustomFirearm<T>(item.Serial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this Item item, out CustomFirearmProperties properties)
        {
            properties = null!;

            if (item?.Base == null)
                return false;

            return IsCustomFirearm(item.Serial, out properties);
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this Item item, out T properties) where T : CustomFirearmProperties
        {
            properties = null!;

            if (item?.Base == null)
                return false;

            return IsCustomFirearm(item.Serial, out properties);
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this Pickup item)
            => item != null && IsCustomFirearm(item.Serial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this Pickup item) where T : CustomFirearmProperties
            => item != null && IsCustomFirearm<T>(item.Serial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this Pickup item, out CustomFirearmProperties properties)
        {
            properties = null!;

            if (item?.Base == null)
                return false;

            return IsCustomFirearm(item.Serial, out properties);
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this Pickup item, out T properties) where T : CustomFirearmProperties
        {
            properties = null!;

            if (item?.Base == null)
                return false;

            return IsCustomFirearm(item.Serial, out properties);
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this ItemBase item)
            => item != null && IsCustomFirearm(item.ItemSerial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this ItemBase item) where T: CustomFirearmProperties
            => item != null && IsCustomFirearm<T>(item.ItemSerial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this ItemBase item, out CustomFirearmProperties properties)
        {
            properties = null!;

            if (item == null)
                return false;

            return IsCustomFirearm(item.ItemSerial, out properties);
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this ItemBase item, out T properties) where T : CustomFirearmProperties
        {
            properties = null!;

            if (item == null)
                return false;

            return IsCustomFirearm(item.ItemSerial, out properties);
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this ItemPickupBase item)
            => item != null && IsCustomFirearm(item.Info.Serial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this ItemPickupBase item) where T : CustomFirearmProperties
            => item != null && IsCustomFirearm<T>(item.Info.Serial);

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm(this ItemPickupBase item, out CustomFirearmProperties properties)
        {
            properties = null!;

            if (item == null)
                return false;

            return IsCustomFirearm(item.Info.Serial, out properties);
        }

        /// <summary>
        /// Whether or not a specific item is tracked as a custom firearm.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="properties">The properties of the custom firearm.</param>
        /// <returns>true if the item's serial number is tracked as a custom firearm.</returns>
        public static bool IsCustomFirearm<T>(this ItemPickupBase item, out T properties) where T : CustomFirearmProperties
        {
            properties = null!;

            if (item == null)
                return false;

            return IsCustomFirearm(item.Info.Serial, out properties);
        }

        /// <summary>
        /// Attempts to remove a custom firearm and optionally destroy it's item.
        /// </summary>
        /// <param name="rifleSerial">The serial of the custom firearm.</param>
        /// <param name="destroyItem">Whether or not to destroy the item.</param>
        /// <returns>true if the custom firearm was removed</returns>
        public static bool Remove(ushort rifleSerial, bool destroyItem = false)
        {
            if (!TrackedItems.TryGetValue(rifleSerial, out var properties))
                return false;

            TrackedItems.Remove(rifleSerial);

            Removed?.Invoke(rifleSerial, properties);

            if (destroyItem)
            {
                if (InventoryExtensions.ServerTryGetItemWithSerial(rifleSerial, out var item) && item != null)
                {
                    Destroyed?.Invoke(ExPlayer.Get(item.Owner), item as Firearm, null, properties);

                    item.DestroyItem();
                }
                else if (ExMap.Pickups.TryGetFirst(x => x != null && x.Info.Serial == rifleSerial, out var pickup))
                {
                    Destroyed?.Invoke(ExPlayer.Get(pickup.PreviousOwner), null, pickup as FirearmPickup, properties);

                    pickup.DestroySelf();
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a new custom firearm.
        /// </summary>
        /// <param name="firearmType">The type of the item of the custom firearm.</param>
        /// <param name="properties">The custom properties of the firearm.</param>
        /// <returns>The created Sniper Rifle item.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static Firearm CreateCustomFirearm(ItemType firearmType, CustomFirearmProperties properties, Action<Firearm>? firearmSetup = null)
        {
            if (!firearmType.TryGetTemplate<Firearm>(out var template))
                throw new Exception($"Could not get the item template for item type {firearmType}");

            if (properties is null)
                throw new ArgumentNullException(nameof(properties));

            var firearm = UnityEngine.Object.Instantiate(template);

            firearm.ServerAddReason = ItemAddReason.AdminCommand;
            firearm.ItemSerial = ItemSerialGenerator.GenerateNext();

            TrackedItems[firearm.ItemSerial] = properties;

            firearmSetup?.Invoke(firearm);
            return firearm;
        }

        /// <summary>
        /// Adds a custom firearm to a player's inventory.
        /// </summary>
        /// <param name="player">The player to add the custom firearm to.</param>
        /// <param name="firearmType">The type of the item of the custom firearm.</param>
        /// <param name="properties">The custom properties of the firearm.</param>
        /// <returns>The added custom firearem item.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static Firearm GiveCustomFirearm(this ExPlayer player, ItemType firearmType, CustomFirearmProperties properties)
        {
            if (player?.ReferenceHub == null)
                throw new ArgumentNullException(nameof(player));

            if (properties is null)
                throw new ArgumentNullException(nameof(properties));

            var firearm = player.Inventory.AddItem<Firearm>(firearmType, ItemAddReason.AdminCommand);

            if (firearm == null)
                throw new Exception("Could not add custom firearm");

            TrackedItems[firearm.ItemSerial] = properties;

            SetupRifle(firearm, properties);
            return firearm;
        }

        /// <summary>
        /// Spawns a custom firearm pickup.
        /// </summary>
        /// <param name="position">The position to spawn the pickup at.</param>
        /// <param name="firearmType">The type of the custom firearm item.</param>
        /// <param name="properties">The properties of the custom firearm</param>
        /// <returns>The spawned custom firearm pickup.</returns>
        /// <exception cref="Exception"></exception>
        public static FirearmPickup SpawnCustomFirearm(Vector3 position, ItemType firearmType, CustomFirearmProperties properties)
        {
            var pickup = ExMap.SpawnItem<FirearmPickup>(firearmType, position, Vector3.one, Quaternion.identity);

            if (pickup == null)
                throw new Exception("Could not spawn custom firearm");

            TrackedItems[pickup.Info.Serial] = properties;
            return pickup;
        }

        /// <summary>
        /// Configures the specified rifle with custom properties, including attachments and ammunition settings.
        /// </summary>
        /// <remarks>This method applies attachment and ammunition settings based on the provided
        /// properties. If both default and blacklisted attachments are specified, default attachments take precedence.
        /// The method may also trigger additional setup actions via the SetupFirearm event if it is
        /// subscribed.</remarks>
        /// <param name="rifle">The firearm instance to configure. Must not be null.</param>
        /// <param name="properties">The custom properties to apply to the rifle, such as default or blacklisted attachments and maximum
        /// ammunition. Must not be null.</param>
        public static void SetupRifle(this Firearm rifle, CustomFirearmProperties properties)
        {
            if (properties.DefaultAttachments?.Count > 0)
                rifle.SetAttachments(x => properties.DefaultAttachments.Contains(x.Name));
            else if (properties.BlacklistedAttachments?.Count > 0)
                rifle.SetAttachments(x => x.IsEnabled && !properties.BlacklistedAttachments.Contains(x.Name));

            if (properties.MaxAmmo.HasValue)
            {
                if (rifle.TryGetModule<MagazineModule>(out var magazineModule))
                {
                    magazineModule._defaultCapacity = properties.MaxAmmo.Value;

                    if (rifle.TryGetModule<AutomaticActionModule>(out var automaticActionModule) &&
                        automaticActionModule.AmmoStored > 0)
                        automaticActionModule.ServerCycleAction();

                    if (magazineModule.AmmoStored > properties.MaxAmmo)
                        magazineModule.ServerModifyAmmo(-(magazineModule.AmmoStored - properties.MaxAmmo.Value));
                }
            }

            SetupFirearm?.Invoke(ExPlayer.Get(rifle.Owner)!, rifle, properties);
        }

        private static void Internal_PickedUpItem(PlayerPickedUpItemEventArgs args)
        {
            if (args.Item?.Base == null || !args.Item.IsCustomFirearm(out var properties))
                return;

            SetupRifle((Firearm)args.Item.Base, properties);
        }

        private static void Internal_ChangingAttachments(PlayerChangingFirearmAttachmentsEventArgs args)
        {
            if (!args.Firearm.IsCustomFirearm(out var properties))
                return;

            if (!properties.AllowAttachmentsChanging)
            {
                args.Player.SendAlert(AlertType.Warn, 5f, "<b>Tato zbraň <color=red>nelze</color> upravit!</b>", true);
                args.IsAllowed = false;

                return;
            }

            if (properties.BlacklistedAttachments?.Count > 0)
                args.ToEnable.RemoveAll(properties.BlacklistedAttachments.Contains);
        }

        private static void Internal_Hurting(PlayerHurtingEventArgs args)
        {
            if (args.DamageHandler is FirearmDamageHandler firearmDamageHandler
                && firearmDamageHandler.Firearm.IsCustomFirearm(out var properties))
            {
                if (!properties.BaseDamage.TryGetValue(firearmDamageHandler.Hitbox, out var baseDamage))
                {
                    if (properties.BaseDamage.Count > 0)
                    {
                        baseDamage = properties.BaseDamage.First().Value;
                    }
                    else
                    {
                        baseDamage = firearmDamageHandler.Damage;
                    }
                }

                var hasRoleMultiplier = properties.RoleMultipliers.TryGetValue(args.Player.Role, out var roleMultiplier);
                var hasTeamMultiplier = properties.TeamMultipliers.TryGetValue(args.Player.Team, out var teamMultiplier);

                if (hasTeamMultiplier)
                {
                    if (hasRoleMultiplier)
                    {
                        baseDamage *= roleMultiplier;
                    }
                    else
                    {
                        baseDamage *= teamMultiplier;
                    }
                }
                else
                {
                    if (hasRoleMultiplier)
                    {
                        baseDamage *= roleMultiplier;
                    }
                }

                firearmDamageHandler.Damage = baseDamage;
            }
        }

        private static void Internal_Reloading(PlayerReloadingWeaponEventArgs args)
        {
            if (!args.FirearmItem.IsCustomFirearm(out var properties) || !properties.MaxAmmo.HasValue)
                return;

            if (args.FirearmItem.Base.TryGetModule<AutomaticActionModule>(out var automaticActionModule)
                && automaticActionModule.AmmoStored >= properties.MaxAmmo)
                args.IsAllowed = false;
        }

        private static void Internal_WaitingForPlayers()
        {
            TrackedItems.Clear();
        }

        internal static void Internal_Init()
        {
            PlayerEvents.Hurting += Internal_Hurting;
            PlayerEvents.PickedUpItem += Internal_PickedUpItem;
            PlayerEvents.ReloadingWeapon += Internal_Reloading;

            ExRoundEvents.WaitingForPlayers += Internal_WaitingForPlayers;
            ExPlayerEvents.ChangingAttachments += Internal_ChangingAttachments;
        }
    }
}
