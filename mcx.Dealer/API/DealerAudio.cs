using LabExtended.API;
using LabExtended.Core;

using mcx.Utilities.Audio;

using UnityEngine;

namespace mcx.Dealer.API
{
    /// <summary>
    /// Manages audio of a spawned dealer instance.
    /// </summary>
    public class DealerAudio
    {
        /// <summary>
        /// Describes the type of audio clip to play.
        /// </summary>
        public enum ClipType
        {
            /// <summary>
            /// A new trade has started.
            /// </summary>
            TradeStart,

            /// <summary>
            /// A trade has ended without any purchased items.
            /// </summary>
            TradeEndedNoPurchase,

            /// <summary>
            /// A trade has ended with a purchased item.
            /// </summary>
            TradeEndedWithPurchase,

            /// <summary>
            /// A purchase was attempted but the player could not afford it.
            /// </summary>
            PurchaseFailed,

            /// <summary>
            /// A purchase was succesfull.
            /// </summary>
            PurchaseSuccessful,
        }

        private Dictionary<ClipType, float> lastClipPlayTimes = new();
        private bool isPlaying;

        /// <summary>
        /// Gets the dealer instance this audio manager belongs to.
        /// </summary>
        public DealerInstance Dealer { get; }

        public DealerAudio(DealerInstance dealer)
        {
            if (dealer is null || dealer.IsDestroyed)
                throw new ArgumentNullException(nameof(dealer));

            Dealer = dealer;
        }

        /// <summary>
        /// Gets called when the parent dealer instance is being initialized.
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// Gets called when the parent dealer instance is being destroyed.
        /// </summary>
        public void Destroy()
        {
            lastClipPlayTimes.Clear();
        }

        /// <summary>
        /// Gets a random clip file name for a specific clip type.
        /// </summary>
        /// <param name="type">The clip type.</param>
        /// <returns>The name of the clip's audio file (or empty if no clips were loaded).</returns>
        public string GetRandomClip(ClipType type)
        {
            if (!DealerCore.ConfigStatic.ClipNames.TryGetValue(type, out var names) || names.Count == 0)
                return string.Empty;

            return names.RandomItem();
        }

        /// <summary>
        /// Plays a random clip of the specified type.
        /// </summary>
        public void PlayRandomClip(ClipType type)
        {
            if (isPlaying)
                return;

            if (DealerCore.ConfigStatic.ClipCooldown.TryGetValue(type, out var clipCooldown)
                && lastClipPlayTimes.TryGetValue(type, out var clipTime)
                && (Time.realtimeSinceStartup - clipTime) < clipCooldown)
                return;

            var clipPath = GetRandomClip(type);

            if (string.IsNullOrWhiteSpace(clipPath))
                return;

            isPlaying = PlaybackUtils.PlayAt(clipPath, Dealer.Player.Position, null, () =>
            {
                lastClipPlayTimes[type] = Time.realtimeSinceStartup;

                isPlaying = false;
            });
        }

        /// <summary>
        /// Gets called when a player gets detected as the closest player to the dealer.
        /// </summary>
        public void OnClosestPlayerDetected(ExPlayer closestPlayer, float distance)
        {

        }

        /// <summary>
        /// Gets called before an item is purchased.
        /// </summary>
        public void OnPurchasingItem(bool canAfford)
        {
            if (!canAfford)
            {
                PlayRandomClip(ClipType.PurchaseFailed);
            }
        }

        /// <summary>
        /// Gets called after an item purchase is finished.
        /// </summary>
        public void OnPurchasedItem()
        {
            PlayRandomClip(ClipType.PurchaseSuccessful);
        }

        /// <summary>
        /// Gets called after a trade is started.
        /// </summary>
        public void OnTradeStarted()
        {
            PlayRandomClip(ClipType.TradeStart);
        }

        /// <summary>
        /// Gets called after a trade is finished.
        /// </summary>
        public void OnTradeFinished(bool anyPurchased)
        {
            PlayRandomClip(anyPurchased 
                ? ClipType.TradeEndedWithPurchase 
                : ClipType.TradeEndedNoPurchase);
        }
    }
}