namespace PeanutClub.RandomPickup.API
{
    /// <summary>
    /// Describes the status of a random pickup instance.
    /// </summary>
    public enum RandomPickupStatus
    {
        /// <summary>
        /// The instance has not been initialized yet.
        /// </summary>
        NotInitialized,

        /// <summary>
        /// The instance is waiting to be opened.
        /// </summary>
        Waiting,

        /// <summary>
        /// The instance has been opened.
        /// </summary>
        Opened,

        /// <summary>
        /// The instance has been destroyed.
        /// </summary>
        Destroyed,
    }
}