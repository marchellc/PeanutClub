namespace SecretLabAPI.Actions.Attributes
{
    /// <summary>
    /// Specifies metadata for an action method parameter, including its name, type, and optional default value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ActionParameterAttribute : Attribute
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The default value of the parameter.
        /// </summary>
        public object? DefaultValue { get; }

        /// <summary>
        /// Initializes a new instance of the ActionParameterAttribute class with the specified parameter name,
        /// and optional default value.
        /// </summary>
        /// <param name="name">The name of the action parameter. This value is used to identify the parameter in the action method
        /// signature.</param>
        /// <param name="defaultValue">The default value to assign to the parameter if no value is provided. If null, the parameter does not have a
        /// default value.</param>
        public ActionParameterAttribute(string name, string description, object? defaultValue = null)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
        }
    }
}