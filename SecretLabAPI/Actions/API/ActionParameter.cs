namespace SecretLabAPI.Actions.API
{
    /// <summary>
    /// Represents a parameter of an action method.
    /// </summary>
    public class ActionParameter
    {
        /// <summary>
        /// The index of the parameter in the method's parameter list.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the ActionParameter class with the specified index, name and default
        /// value.
        /// </summary>
        /// <param name="index">The zero-based position of the parameter in the action's parameter list.</param>
        /// <param name="name">The name of the parameter as defined in the action signature. Cannot be null or empty.</param>
        /// <param name="defaultValue">The default value assigned to the parameter if no value is provided. May be null if the parameter does not
        /// have a default value.</param>
        public ActionParameter(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}