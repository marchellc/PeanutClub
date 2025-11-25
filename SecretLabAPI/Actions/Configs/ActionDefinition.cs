using Newtonsoft.Json;

using SecretLabAPI.Actions.API;

using System.ComponentModel;

using YamlDotNet.Serialization;

namespace SecretLabAPI.Actions
{
    /// <summary>
    /// Definition of an action in a config file.
    /// </summary>
    public class ActionDefinition
    {
        /// <summary>
        /// Gets or sets the list of actions to be invoked, where each action is represented as a formatted string.
        /// </summary>
        /// <remarks>Each string in the list should specify an action and its arguments using a
        /// semicolon-delimited format. Multiple actions can be provided either on a single line, separated by commas,
        /// or on separate lines. The expected format is: "ActionID; Argument1; Argument2". Ensure that each action
        /// string follows the required formatting for correct processing.</remarks>
        [Description("Sets the list of actions to be invoked.\n" +
            "# Multiple actions on one line are formatted like this: - ActionID; Argument1, ActionID2; Argument1; Argument2\n" +
            "# Or can be formatted each on a single line:\n" +
            "# - ActionID; Argument1; Argument2; Argument3\n" +
            "# - ActionID2; Argument1; Argument2; Argument3")]
        public List<string> Actions { get; set; } = new();

        /// <summary>
        /// Gets the list of parsed actions that are cached for efficient access.
        /// </summary>
        /// <remarks>The returned list is generated from the underlying action data and cached on first
        /// access. Subsequent accesses return the same cached list, improving performance when retrieving actions
        /// multiple times. The list reflects the current state of the underlying action data at the time of first
        /// access; changes to the source data after caching are not reflected until the cache is reset.</remarks>
        [YamlIgnore]
        [JsonIgnore]
        public List<CompiledAction> CachedActions
        {
            get
            {
                if (field is null)
                {
                    field = new();

                    Actions.ParseActions(field);
                }

                return field;
            }
        }
    }
}