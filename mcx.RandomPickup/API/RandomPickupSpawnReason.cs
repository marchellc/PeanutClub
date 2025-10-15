namespace mcx.RandomPickup.API
{
    /// <summary>
    /// Defines the reason of a random pickup spawning.
    /// </summary>
    public enum RandomPickupSpawnReason
    {
        /// <summary>
        /// Spawned at the start of the round from config locations.
        /// </summary>
        DefinedLocation,

        /// <summary>
        /// Spawned for a random player during the round.
        /// </summary>
        RandomPlayer,

        /// <summary>
        /// Spawned by a scenario.
        /// </summary>
        Scenario
    }
}