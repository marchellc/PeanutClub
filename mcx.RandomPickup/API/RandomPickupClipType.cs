namespace mcx.RandomPickup.API
{
    /// <summary>
    /// Represents the different states of a random pickup clip in the system.
    /// </summary>
    public enum RandomPickupClipType
    {
        /// <summary>
        /// The clip played when the random pickup is spawned.
        /// </summary>
        Spawned,

        /// <summary>
        /// The clip played when the random pickup is waiting to be opened.
        /// </summary>
        Waiting,

        /// <summary>
        /// The clip played when the random pickup is opened.
        /// </summary>
        Opened
    }
}