namespace SecretLabAPI.Actions.API
{
    /// <summary>
    /// Represents a registered action method.
    /// </summary>
    public class ActionMethod
    {
        /// <summary>
        /// The ID of the action.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the compiled delegate representing the action method.
        /// </summary>
        public ActionDelegate Delegate { get; }

        /// <summary>
        /// Gets the registered parameters.
        /// </summary>
        public ActionParameter[] Parameters { get; }

        /// <summary>
        /// Whether or not this action is an evaluator.
        /// </summary>
        public bool IsEvaluator { get; }

        /// <summary>
        /// Whether or not to save overflow arguments.
        /// </summary>
        public bool SaveArgumentsOverflow { get; }

        /// <summary>
        /// Initializes a new instance of the ActionMethod class with the specified identifier, delegate, and
        /// parameters.
        /// </summary>
        /// <param name="id">The unique identifier for the action method. Cannot be null.</param>
        /// <param name="actionDelegate">The delegate that defines the action to be executed. Cannot be null.</param>
        /// <param name="parameters">An array of parameters required by the action method. Cannot be null.</param>
        public ActionMethod(string id, bool isEvaluator, bool argsOverflow, ActionDelegate actionDelegate, ActionParameter[] parameters)
        {
            Id = id;
            IsEvaluator = isEvaluator;
            SaveArgumentsOverflow = argsOverflow;
            Delegate = actionDelegate;
            Parameters = parameters;
        }
    }
}