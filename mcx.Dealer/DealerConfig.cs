using mcx.Dealer.API;

using mcx.Utilities.Configs;

using System.ComponentModel;

namespace mcx.Dealer
{
    /// <summary>
    /// Structure of the plugin's config file.
    /// </summary>
    public class DealerConfig
    {
        /// <summary>
        /// Gets or sets the maximum number of rounds that a dealer's inventory can remain active before it is
        /// refreshed.
        /// </summary>
        [Description("Sets the maximum amount of rounds a dealer's inventory can last before it is refreshed.")]
        public int MaxInventoryAge { get; set; } = 5;

        /// <summary>
        /// Gets or sets the maximum amount of dealers that can spawn per round.
        /// </summary>
        [Description("Sets the maximum amount of dealers that can spawn per round.")]
        public int MaxPerRound { get; set; } = 2;

        /// <summary>
        /// Gets or sets the minimum amount of dealers that have to spawn per round.
        /// </summary>
        [Description("Sets the minimum amount of dealers that have to spawn per round.")]
        public int MinPerRound { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of seconds to wait before attempting to spawn a dealer after the round starts.
        /// </summary>
        [Description("Sets how long (in seconds) the plugin should wait before attempting to spawn a dealer after the round starts.")]
        public float WaitTime { get; set; } = 100f;

        /// <summary>
        /// Gets or sets the delay, in seconds, between dealer spawn attempts.
        /// </summary>
        [Description("Sets how long (in seconds) the plugin should wait between dealer spawn attempts.")]
        public float SpawnDelay { get; set; } = 300f;

        /// <summary>
        /// Gets or sets the maximum distance, in meters, that a player can be from a dealer before their trade is
        /// terminated.
        /// </summary>
        [Description("Sets the maximum distance (in meters) a player can be from a dealer before their trade terminates.")]
        public float MaxDistance { get; set; } = 10f;

        /// <summary>
        /// Gets or sets the maximum distance, in meters, at which a player can be from a dealer  for the player close
        /// audio clip to play.
        /// </summary>
        [Description("Sets the maximum distance (in meters) a player can be from a dealer for the player close audio clip to play.")]
        public float MaxAudioDistance { get; set; } = 5f;

        /// <summary>
        /// Gets or sets the minimum cooldown period, in seconds, required between playbacks of each audio clip type.
        /// </summary>
        [Description("Sets the minimum required amount of seconds between playbacks of audio clips.")]
        public Dictionary<DealerAudio.ClipType, float> ClipCooldown { get; set; } = new()
        {
            [DealerAudio.ClipType.PurchaseFailed] = 5f,
            [DealerAudio.ClipType.PurchaseSuccessful] = 5f,

            [DealerAudio.ClipType.TradeStart] = 0f,

            [DealerAudio.ClipType.TradeEndedNoPurchase] = 0f,
            [DealerAudio.ClipType.TradeEndedWithPurchase] = 0f
        };

        /// <summary>
        /// Gets or sets the collection of audio clip file names associated with each clip type.
        /// </summary>
        [Description("Sets clip file names for each audio clip.")]
        public Dictionary<DealerAudio.ClipType, List<string>> ClipNames { get; set; } = new()
        {
            [DealerAudio.ClipType.PurchaseFailed] = new(),
            [DealerAudio.ClipType.PurchaseSuccessful] = new(),

            [DealerAudio.ClipType.TradeStart] = new(),

            [DealerAudio.ClipType.TradeEndedNoPurchase] = new(),
            [DealerAudio.ClipType.TradeEndedWithPurchase] = new()
        };

        /// <summary>
        /// Gets or sets the list of available spawn position names.
        /// </summary>
        [Description("Sets the list of available spawn position names.")]
        public List<string> SpawnPositions { get; set; } = new();

        /// <summary>
        /// Gets or sets the range of items that a dealer can have in their inventory.
        /// </summary>
        [Description("Sets the range for how many items a dealer can have in their inventory.")]
        public Int32Range InventorySize { get; set; } = new()
        {
            MinValue = 1,
            MaxValue = 3
        };

        /// <summary>
        /// Gets or sets the collection of items available for dealers to sell.
        /// </summary>
        [Description("Sets the list of items that dealers can sell.")]
        public List<DealerEntry> InventoryItems { get; set; } = new()
        {
            new()
            {
                Item = "Medkit",
                Price = 2,
                Rarity = 4,
            },

            new()
            {
                Item = "Painkillers",
                Price = 1,
                Rarity = 5
            },

            new()
            {
                Item = "Flashlight",
                Price = 1,
                Rarity = 5,
            },

            new()
            {
                Item = "Adrenaline",
                Price = 4,
                Rarity = 3,
            },

            new()
            {
                Item = "SCP500",
                Price = 4,
                Rarity = 3,
            },

            new()
            {
                Item = "GunCom45",
                Price = 6,
                Rarity = 2,
            },

            new()
            {
                Item = "GunCOM15",
                Price = 6,
                Rarity = 2,
            },

            new()
            {
                Item = "GunCOM18",
                Price = 6,
                Rarity = 2,
            },

            new()
            {
                Item = "SniperRifle",
                Price = 15,
                Rarity = 1
            }
        };
    }
}