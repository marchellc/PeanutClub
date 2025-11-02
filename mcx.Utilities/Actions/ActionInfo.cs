using mcx.Utilities.Actions.Interfaces;

using System.ComponentModel;

using YamlDotNet.Serialization;

namespace mcx.Utilities.Actions
{
    /// <summary>
    /// Used to define config actions.
    /// </summary>
    public class ActionInfo
    {
        private IAction? action;
        private bool actionChecked;

        /// <summary>
        /// Gets or sets the ID of the action to be triggered.
        /// </summary>
        [Description("Sets the ID of the action to be triggered.")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of parameters associated with the action.
        /// </summary>
        [Description("Sets the parameters for the action.")]
        public Dictionary<string, string[]> Parameters { get; set; } = new();

        /// <summary>
        /// Gets the action associated with this instance, if one exists.
        /// </summary>
        [YamlIgnore]
        public IAction? Action
        {
            get
            {
                if (!actionChecked)
                {
                    actionChecked = true;

                    ActionManager.Actions.TryGetValue(Id, out action);
                }

                return action;
            }
        }
    }
}