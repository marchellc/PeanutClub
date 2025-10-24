namespace mcx.Overlays.Levels.Entries
{
    /// <summary>
    /// Represents an entry detailing the gain in experience amount.
    /// </summary>
    public struct ExperienceGainEntry
    {
        /// <summary>
        /// Represents the amount as a floating-point number.
        /// </summary>
        public readonly float Amount;

        /// <summary>
        /// Whether or not experience was added.
        /// </summary>
        public readonly bool IsGain;

        /// <summary>
        /// Represents an entry for experience gain or loss.
        /// </summary>
        /// <param name="amount">The amount of experience to be gained or lost. Must be a non-negative value.</param>
        /// <param name="isGain">Indicates whether the experience is a gain. <see langword="true"/> if it is a gain; otherwise, <see
        /// langword="false"/> for a loss.</param>
        public ExperienceGainEntry(float amount, bool isGain)
        {
            Amount = amount;
            IsGain = isGain;
        }
    }
}