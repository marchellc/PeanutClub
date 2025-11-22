using System.ComponentModel;

namespace SecretLabAPI.Actions
{
    /// <summary>
    /// Represents an action definition that includes a weight value used to influence selection probability among
    /// multiple actions.
    /// </summary>
    /// <remarks>A higher weight increases the likelihood that this action will be selected when choosing from
    /// a set of actions. The specific effect of the weight depends on the selection algorithm implemented by the
    /// caller.</remarks>
    public class WeightedActionDefinition : ActionDefinition
    {
        /// <summary>
        /// Gets or sets the key that identifies the multipliers group associated with this action.
        /// </summary>
        [Description("Sets the key of the multipliers group for this action.")]
        public string Multipliers { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the weight assigned to this action when selecting from a list of actions.
        /// </summary>
        /// <remarks>A higher weight increases the likelihood that this action will be chosen during
        /// selection. The interpretation of the weight value depends on the selection algorithm used by the
        /// caller.</remarks>
        [Description("Sets the weight of this action when selecting from a list of actions.")]
        public float Weight { get; set; } = 0f;
    }
}