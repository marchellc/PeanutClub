namespace mcx.Utilities.Actions
{
    /// <summary>
    /// Describes the result of an action execution.
    /// </summary>
    public enum ActionResult
    {
        /// <summary>
        /// The action completed successfully.
        /// </summary>
        Success,

        /// <summary>
        /// The action failed to complete.
        /// </summary>
        Failure,

        /// <summary>
        /// The execution of further actions should be stopped.
        /// </summary>
        Stop,

        /// <summary>
        /// The execution of further actions should be stopped and resources disposed.
        /// </summary>
        StopAndDispose,
    }
}