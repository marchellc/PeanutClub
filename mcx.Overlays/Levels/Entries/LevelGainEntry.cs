namespace mcx.Overlays.Levels.Entries
{
    /// <summary>
    /// Represents an entry detailing the gain in level or experience amount.
    /// </summary>
    public struct LevelGainEntry
    {
        /// <summary>
        /// Represents the amount as an integer value.
        /// </summary>
        public readonly int Amount;

        /// <summary>
        /// Indicates whether the current state represents a level-up condition.
        /// </summary>
        public readonly bool IsLevelUp;

        /// <summary>
        /// Represents an entry detailing the gain in level or experience amount.
        /// </summary>
        /// <param name="amount">The amount of experience gained. Must be a non-negative integer.</param>
        /// <param name="isLevelUp">A <see langword="true"/> if the gain results in a level up; otherwise, <see langword="false"/>.</param>
        public LevelGainEntry(int amount, bool isLevelUp)
        {
            Amount = amount;
            IsLevelUp = isLevelUp;
        }
    }
}