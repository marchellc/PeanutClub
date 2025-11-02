using mcx.Utilities.Actions.Interfaces;

namespace mcx.Utilities.Actions.Features.Configs
{
    /// <summary>
    /// Represents a config-defined action.
    /// </summary>
    public class ConfigAction : IAction
    {
        /// <inheritdoc/>
        public string Id { get; }

        /// <summary>
        /// Initializes a new instance of the ConfigAction class with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier for the configuration action. Cannot be null or empty.</param>
        public ConfigAction(string id)
            => Id = id;

        /// <summary>
        /// Gets the list of valid actions to perform.
        /// </summary>
        public List<ActionInfo> Actions { get; } = new();

        /// <inheritdoc/>
        public string DebugAction(Dictionary<string, string> parameters)
            => string.Empty;

        /// <inheritdoc/>
        public ActionResult Trigger(ref ActionContext context)
        {
            Actions.TriggerMany(context.Source, context.Targets);
            return ActionResult.Success;
        }
    }
}