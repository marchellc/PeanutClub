namespace mcx.Utilities.Actions.Interfaces
{
    /// <summary>
    /// Base interface for action sources.
    /// </summary>
    public interface IActionSource
    {
        /// <summary>
        /// Gets the ID of the source.
        /// </summary>
        string Id { get; }
    }
}