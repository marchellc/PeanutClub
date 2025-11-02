namespace mcx.Utilities.Actions.Interfaces
{
    /// <summary>
    /// Base interface for all actions.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets the ID of the action.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Adds parameters for debugging purposes.
        /// </summary>
        string DebugAction(Dictionary<string, string> parameters);

        /// <summary>
        /// Triggers the specified action using the provided context.
        /// </summary>
        /// <param name="context">A reference to the <see cref="ActionContext"/> that provides information about the action to be triggered.
        /// The context may be modified by the method.</param>
        ActionResult Trigger(ref ActionContext context);
    }
}