using InventorySystem.Items;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LabExtended.API;
using LabExtended.Core;
using LabExtended.Events;
using LabExtended.Events.Player;
using LabExtended.Extensions;
using LabExtended.Utilities;
using LabExtended.Utilities.Update;
using PlayerRoles;
using UnityEngine;

namespace PeanutClub.Dealer.API
{
    /// <summary>
    /// Manages the spawning logic of dealers.
    /// </summary>
    public static class DealerManager
    {
        private static float remainingStartWait = 0f;
        private static float remainingSpawnWait = 0f;

        /// <summary>
        /// Gets the active config instance.
        /// </summary>
        public static DealerConfig Config => DealerCore.ConfigStatic;

        /// <summary>
        /// Gets or sets the amount of dealers that should spawn this round.
        /// </summary>
        public static int SpawnThisRound { get; set; } = 0;

        /// <summary>
        /// Gets a randomly generated amount of dealers to spawn this round.
        /// </summary>
        public static int RandomSpawnCount => UnityEngine.Random.Range(DealerCore.ConfigStatic.MinPerRound, DealerCore.ConfigStatic.MaxPerRound);

        /// <summary>
        /// Gets a randomly selected spawn position name from the config.
        /// </summary>
        public static string RandomSpawnName
        {
            get
            {
                var random = DealerCore.ConfigStatic.SpawnPositions.RandomItem();

                while (Dealers.Any(x => !x.IsDestroyed && x.Id == random))
                    random = DealerCore.ConfigStatic.SpawnPositions.RandomItem();

                return random;
            }
        }

        /// <summary>
        /// A list of dealers that have spawned this round.
        /// </summary>
        public static List<DealerInstance> Dealers { get; } = new();

        /// <summary>
        /// A list of player unique inventories.
        /// </summary>
        public static Dictionary<string, Dictionary<string, DealerInventory>> Inventories { get; } = new();

        /// <summary>
        /// Gets called when a new dealer instance is spawned.
        /// </summary>
        public static event Action<DealerInstance>? Spawned;

        /// <summary>
        /// Spawns a new dealer NPC at the specified position and rotation.
        /// </summary>
        /// <remarks>The spawned dealer is initialized in god mode and is added to the global dealer list.
        /// The Spawned event is invoked after the dealer is created.</remarks>
        /// <param name="position">The world position where the dealer NPC will be spawned.</param>
        /// <param name="rotation">The orientation to assign to the dealer NPC upon spawning.</param>
        /// <returns>A DealerInstance representing the newly spawned dealer NPC.</returns>
        public static DealerInstance SpawnDealer(Vector3 position, Quaternion rotation, string id)
        {
            var npc = new ExPlayer("Dealer", true);
            var dealer = new DealerInstance(npc, id);

            npc.Role.Set(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);

            TimingUtils.AfterSeconds(() =>
            {
                npc.Position.Set(position);
                npc.Rotation.Set(rotation);

                npc.IsGodModeEnabled = true;

                Dealers.Add(dealer);

                dealer.Initialize();

                Spawned?.InvokeSafe(dealer);

                ApiLog.Debug("Dealer Manager", $"Spawned a new dealer instance at &3{position.ToPreciseString()}&r");
            }, 0.2f);

            return dealer;
        }

        /// <summary>
        /// Retrieves the inventory for a specified dealer and user, optionally forcing a refresh of the inventory data.
        /// </summary>
        /// <remarks>If the inventory is older than the maximum allowed age or if forceRefresh is set to
        /// true, the inventory data is refreshed before being returned. Otherwise, cached inventory data is
        /// used.</remarks>
        /// <param name="dealerId">The unique identifier of the dealer whose inventory is to be retrieved. Cannot be null.</param>
        /// <param name="userId">The unique identifier of the user for whom the inventory is requested. Cannot be null.</param>
        /// <param name="forceRefresh">true to force a refresh of the inventory data; otherwise, false to use cached data if available.</param>
        /// <returns>A DealerInventory object representing the current inventory for the specified dealer and user.</returns>
        public static DealerInventory GetDealerInventory(string dealerId, string userId, bool forceRefresh = false)
        {
            if (!Inventories.TryGetValue(dealerId, out var inventories))
                Inventories[dealerId] = inventories = new();

            if (inventories.TryGetValue(userId, out var inventory))
            {
                if (forceRefresh || (ExRound.RoundNumber - inventory.RoundNumber) >= Config.MaxInventoryAge)
                {
                    inventory.ResetInventory();
                    inventory.RoundNumber = ExRound.RoundNumber;

                    Internal_RefreshInventory(inventory);
                }

                return inventory;
            }
            else
            {
                inventories[userId] = inventory = new();

                inventory.ResetInventory();
                inventory.RoundNumber = ExRound.RoundNumber;

                Internal_RefreshInventory(inventory);
                return inventory;
            }
        }

        /// <summary>
        /// Determines whether the specified item is currently present in any dealer's active inventory.
        /// </summary>
        /// <remarks>If the item is not found in any dealer's active inventory or if the item is null, the
        /// method returns false and sets owningDealer to null.</remarks>
        /// <param name="item">The item to check for ownership by a dealer. Cannot be null.</param>
        /// <param name="owningDealer">When this method returns, contains the dealer that owns the item if found; otherwise, null.</param>
        /// <returns>true if the item is found in a dealer's active inventory; otherwise, false.</returns>
        public static bool IsDealerItem(this ItemBase item, out DealerInstance owningDealer)
        {
            owningDealer = null!;

            if (item == null)
                return false;

            for (var i = 0; i < Dealers.Count; i++)
            {
                var dealer = Dealers[i];

                if (dealer.IsDestroyed || !dealer.IsActive)
                    continue;

                if (dealer.ActiveInventory.ActiveMapping.ContainsKey(item))
                {
                    owningDealer = dealer;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified item is currently present in any dealer's active inventory.
        /// </summary>
        /// <remarks>If the item is not found in any dealer's active inventory, both out parameters are
        /// set to their default values. This method does not consider destroyed dealers or dealers without an active
        /// inventory.</remarks>
        /// <param name="item">The item to check for association with a dealer's active inventory. Cannot be null.</param>
        /// <param name="owningDealer">When this method returns, contains the dealer instance that owns the item if found; otherwise, null.</param>
        /// <param name="dealerItem">When this method returns, contains the dealer item instance associated with the specified item if found;
        /// otherwise, the default value.</param>
        /// <returns>true if the item is found in a dealer's active inventory; otherwise, false.</returns>
        public static bool IsDealerItem(this ItemBase item, out DealerInstance owningDealer, out DealerItemInstance dealerItem)
        {
            owningDealer = null!;
            dealerItem = default;

            if (item == null)
                return false;

            for (var i = 0; i < Dealers.Count; i++)
            {
                var dealer = Dealers[i];

                if (dealer.IsDestroyed || !dealer.IsActive)
                    continue;

                if (dealer.ActiveInventory.ActiveMapping.TryGetValue(item, out dealerItem))
                {
                    owningDealer = dealer;
                    return true;
                }
            }

            return false;
        }

        private static float Internal_GetChanceForRarity(byte rarityLevel)
        {
            rarityLevel = (byte)Mathf.Clamp(rarityLevel, 0f, 5f);
            
            switch (rarityLevel)
            {
                case 0: return 5f;
                case 1: return 10f;
                case 2: return 30f;
                case 3: return 50f;
                case 4: return 70f;
                case 5: return 80f;

                default: throw new ArgumentOutOfRangeException(nameof(rarityLevel)); // this can literally never happen
            }
        }

        private static void Internal_RefreshInventory(DealerInventory inventory)
        {
            var count = Config.InventorySize.GetRandom();

            while (inventory.Items.Count < count)
            {
                var entry = Config.InventoryItems.GetRandomWeighted(x => Internal_GetChanceForRarity(x.Rarity));

                if (entry != null)
                {
                    var price = entry.Price;
                    var discount = 0;

                    if (entry.DiscountChance > 0 && WeightUtils.GetBool(entry.DiscountChance))
                    {
                        discount = entry.DiscountRange.GetRandom();
                        price -= price * (discount / 100);
                    }

                    inventory.Items.Add(new(entry, entry.Price, price, discount));
                }
            }
        }

        private static void Internal_Update()
        {
            if (!ExRound.IsRunning)
                return;

            if (SpawnThisRound == 0)
                return;

            if (remainingStartWait > 0f)
            {
                remainingStartWait -= Time.deltaTime;
                return;
            }

            if (remainingSpawnWait > 0f)
            {
                remainingSpawnWait -= Time.deltaTime;
                return;
            }

            var spawnName = RandomSpawnName;

            if (MapUtilities.TryGet(spawnName, null, out var position, out var rotation))
            {
                SpawnDealer(position, rotation, spawnName);
                SpawnThisRound--;

                remainingSpawnWait = Config.SpawnDelay;
            }
            else
            {
                ApiLog.Error("Dealer Manager", $"Attempted to spawn a dealer at the position &1{spawnName}&r, but it does not exist! Please check your config.");
            }
        }

        private static void Internal_RoundWaiting()
        {
            Dealers.Clear(); // just in case

            SpawnThisRound = RandomSpawnCount;

            remainingStartWait = Config.WaitTime;
            remainingSpawnWait = 0f;
        }

        private static void Internal_RoundEnding(RoundEndingEventArgs _)
        {
            foreach (var dealer in Dealers.ToArray())
            {
                dealer.DestroyInstance();
            }
        }

        private static void Internal_Interacted(LabApi.Events.Arguments.PlayerEvents.PlayerSearchedToyEventArgs args)
        {
            if (args.Player is not ExPlayer player)
                return;

            foreach (var dealer in Dealers)
            {
                if (dealer.IsDestroyed)
                    continue;

                if (dealer.Interactable?.Base == null)
                    continue;

                if (dealer.Interactable.Base != args.Interactable.Base)
                    continue;

                dealer.Internal_Interacted(player);
                break;
            }
        }

        private static void Internal_SelectedItem(PlayerSelectedItemEventArgs args)
        {
            foreach (var dealer in Dealers)
            {
                if (dealer.IsDestroyed || !dealer.IsActive)
                    continue;

                if (dealer.ActivePlayer != args.Player)
                    continue;

                dealer.Internal_Selected(args.NewItem?.Base ?? null);
            }
        }

        private static void Internal_DroppingItem(PlayerDroppingItemEventArgs args)
        {
            foreach (var dealer in Dealers)
            {
                if (dealer.IsDestroyed || !dealer.IsActive)
                    continue;

                if (dealer.ActivePlayer != args.Player)
                    continue;

                args.IsAllowed = dealer.Internal_Dropping(args.Item?.Base ?? null, out var destroyItem);

                if (!args.IsAllowed)
                {
                    if (destroyItem)
                        args.Item?.Base.DestroyItem();

                    break;
                }
            }
        }

        private static void Internal_UsingItem(PlayerUsingItemEventArgs args)
        {
            if (!args.UsableItem.Base.IsDealerItem(out var dealer))
                return;

            args.IsAllowed = false;
        }

        private static void Internal_ThrowingProjectile(PlayerThrowingProjectileEventArgs args)
        {
            if (!args.ThrowableItem.Base.IsDealerItem(out var dealer))
                return;

            args.IsAllowed = false;
        }

        private static void Internal_ShootingWeapon(PlayerShootingWeaponEventArgs args)
        {
            if (!args.FirearmItem.Base.IsDealerItem(out var dealer))
                return;

            args.IsAllowed = false;
        }

        internal static void Internal_Init()
        {
            ServerEvents.RoundEnding += Internal_RoundEnding;

            PlayerUpdateHelper.OnUpdate += Internal_Update;

            PlayerEvents.UsingItem += Internal_UsingItem;
            PlayerEvents.SearchedToy += Internal_Interacted;
            PlayerEvents.DroppingItem += Internal_DroppingItem;
            PlayerEvents.ShootingWeapon += Internal_ShootingWeapon;
            PlayerEvents.ThrowingProjectile += Internal_ThrowingProjectile;

            ExPlayerEvents.SelectedItem += Internal_SelectedItem;
            ExRoundEvents.WaitingForPlayers += Internal_RoundWaiting;
        }
    }
}