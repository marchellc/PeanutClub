using SecretLabAPI.Utilities;

namespace SecretLabAPI.Actions
{
    /// <summary>
    /// Represents information about an executable action, including its identifier, arguments, and the delegate to
    /// invoke.
    /// </summary>
    public struct ActionInfo
    {
        /// <summary>
        /// Gets the unique identifier associated with this instance.
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// Contains the action arguments.
        /// </summary>
        public readonly string[] Args;

        /// <summary>
        /// Represents the action to perform for a given player and action information.
        /// </summary>
        /// <remarks>Assign a delegate to this field to define custom behavior when an action is triggered
        /// for a player. The delegate receives the player and the associated action details as parameters.</remarks>
        public readonly ActionDelegate Action;

        /// <summary>
        /// Initializes a new instance of the ActionInfo class with the specified action identifier, argument list, and
        /// action delegate.
        /// </summary>
        /// <param name="id">The unique identifier for the action. Cannot be null.</param>
        /// <param name="args">An array of argument names or values associated with the action. Cannot be null.</param>
        /// <param name="action">The delegate to execute for the action. Receives the ExPlayer and the ActionInfo instance as parameters.
        /// Cannot be null.</param>
        public ActionInfo(string id, string[] args, ActionDelegate action)
        {
            Id = id;
            Args = args;
            Action = action;
        }

        /// <summary>
        /// Retrieves the argument value at the specified index, or returns the provided default value if the index is
        /// out of range.
        /// </summary>
        /// <param name="index">The zero-based index of the argument to retrieve. Must be greater than or equal to 0 and less than the
        /// number of available arguments.</param>
        /// <param name="defaultValue">The value to return if the specified index is outside the bounds of the argument list. If not specified, an
        /// empty string is returned.</param>
        /// <returns>The argument value at the specified index if it exists; otherwise, the value of <paramref
        /// name="defaultValue"/>.</returns>
        public string GetValue(int index, string defaultValue = "")
        {
            if (index < 0 || index >= Args.Length)
                return defaultValue;

            return Args[index];
        }

        /// <summary>
        /// Attempts to parse the argument at the specified index using the provided delegate and returns the parsed
        /// value, or a default value if parsing fails or the index is out of range.
        /// </summary>
        /// <remarks>This method is useful for safely retrieving and converting command-line or input
        /// arguments to a specific type without throwing exceptions on invalid input. If the argument at the specified
        /// index cannot be parsed, or if the index is outside the bounds of the argument array, the method returns the
        /// provided default value.</remarks>
        /// <typeparam name="T">The type to which the argument should be parsed.</typeparam>
        /// <param name="index">The zero-based index of the argument to parse. Must be within the bounds of the argument array.</param>
        /// <param name="tryParseDelegate">A delegate that attempts to parse the argument string into the specified type. The delegate should return
        /// <see langword="true"/> if parsing succeeds; otherwise, <see langword="false"/>.</param>
        /// <param name="defaultValue">The value to return if parsing fails or the index is out of range. If not specified, the default value for
        /// type <typeparamref name="T"/> is used.</param>
        /// <returns>The parsed value of type <typeparamref name="T"/> if parsing succeeds; otherwise, <paramref
        /// name="defaultValue"/>.</returns>
        public T GetValue<T>(int index, TryParseDelegate<T> tryParseDelegate, T defaultValue = default!)
        {
            if (index < 0 || index >= Args.Length)
                return defaultValue;

            if (tryParseDelegate(Args[index], out T result))
                return result;

            return defaultValue;
        }
    }
}