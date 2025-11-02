using mcx.Utilities.Actions.Interfaces;

using NorthwoodLib.Pools;

using LabExtended.Core.Pooling.Pools;

namespace mcx.Utilities.Actions
{
    /// <summary>
    /// Provides contextual information for an action, including its source, targets, variables, and configuration
    /// parameters.
    /// </summary>
    public struct ActionContext : IDisposable
    {
        /// <summary>
        /// The source triggering the action.
        /// </summary>
        public readonly IActionSource Source;

        /// <summary>
        /// The index of the current action.
        /// </summary>
        public int CurrentIndex;

        /// <summary>
        /// The next action to trigger after the current one.
        /// </summary>
        public IAction? NextAction;

        /// <summary>
        /// The action that is currently being executed.
        /// </summary>
        public IAction CurrentAction;

        /// <summary>
        /// The action that was triggered before the current one.
        /// </summary>
        public IAction? PreviousAction;

        /// <summary>
        /// The list of actions.
        /// </summary>
        public List<ActionInfo> Actions;

        /// <summary>
        /// The targets of the action.
        /// </summary>
        public List<IActionTarget> Targets;

        /// <summary>
        /// The list of variables defined by actions.
        /// </summary>
        public Dictionary<string, object> Memory;

        /// <summary>
        /// Config parameters for the currently executing action.
        /// </summary>
        public Dictionary<string, string[]> Parameters;

        /// <summary>
        /// Initializes a new instance of the ActionContext class with the specified action source, targets, and
        /// parameters.
        /// </summary>
        /// <param name="source">The source of the action. Cannot be null.</param>
        /// <param name="parameters">A dictionary containing parameter names and their associated values for the action. Cannot be null.</param>
        public ActionContext(IActionSource source, Dictionary<string, string[]> parameters)
        {
            Source = source;
            Parameters = parameters;

            Actions = ListPool<ActionInfo>.Shared.Rent();
            Targets = ListPool<IActionTarget>.Shared.Rent();

            Memory = DictionaryPool<string, object>.Shared.Rent();
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            if (Actions != null)
                ListPool<ActionInfo>.Shared.Return(Actions);

            if (Targets != null)
                ListPool<IActionTarget>.Shared.Return(Targets);

            if (Memory != null)
                DictionaryPool<string, object>.Shared.Return(Memory);

            Actions = null!;
            Memory = null!;
            Targets = null!;
        }

        /// <summary>
        /// Retrieves the string array associated with the specified key, or returns a default value if the key is not
        /// found.
        /// </summary>
        /// <param name="key">The key whose associated string array is to be retrieved. Cannot be null.</param>
        /// <param name="defaultValue">The value to return if the specified key does not exist. If null, the method returns null when the key is
        /// not found.</param>
        /// <returns>The string array associated with the specified key, or the specified default value if the key is not
        /// present.</returns>
        public string[]? GetParameterArray(string key, string[]? defaultValue = null)
        {
            if (!Parameters.TryGetValue(key, out var array))
                return defaultValue;

            return array;
        }

        /// <summary>
        /// Retrieves the parameter value at the specified index for the given key, or returns a default value if the
        /// key does not exist or the index is out of range.
        /// </summary>
        /// <param name="key">The key identifying the parameter array to retrieve the value from. Cannot be null.</param>
        /// <param name="index">The zero-based index of the value to retrieve from the parameter array. Must be non-negative.</param>
        /// <param name="defaultValue">The value to return if the key is not found or the index is out of range. If not specified, an empty string
        /// is used.</param>
        /// <returns>The parameter value at the specified index for the given key, or the specified default value if the key does
        /// not exist or the index is out of range.</returns>
        public string GetParameterOrDefault(string key, int index, string defaultValue = "")
        {
            if (!Parameters.TryGetValue(key, out var array))
                return defaultValue;

            if (index < array.Length)
                return array[index];

            return defaultValue;
        }

        /// <summary>
        /// Retrieves the value of a parameter at the specified index, or returns a default value if the parameter is
        /// not found or cannot be parsed.
        /// </summary>
        /// <remarks>If the parameter array does not contain an element at the specified index, or if
        /// parsing fails, this method returns the provided default value. This method is useful for safely retrieving
        /// and converting parameter values without throwing exceptions on missing or invalid data.</remarks>
        /// <typeparam name="T">The type to which the parameter value is to be parsed.</typeparam>
        /// <param name="key">The key identifying the parameter to retrieve.</param>
        /// <param name="index">The zero-based index of the value to retrieve from the parameter array.</param>
        /// <param name="tryParseDelegate">A delegate that attempts to parse the parameter value from a string to type <typeparamref name="T"/>.</param>
        /// <param name="defaultValue">The value to return if the parameter is not found, the index is out of range, or parsing fails. The default
        /// is <see langword="default"/> for type <typeparamref name="T"/>.</param>
        /// <returns>The parsed value of type <typeparamref name="T"/> if the parameter exists at the specified index and is
        /// successfully parsed; otherwise, the specified default value.</returns>
        public T? GetParameterOrDefault<T>(string key, int index, TryParseDelegate<T> tryParseDelegate, T? defaultValue = default)
        {
            if (!Parameters.TryGetValue(key, out var array))
                return defaultValue;

            if (index < array.Length && tryParseDelegate(array[index], out var result))
                return result;

            return defaultValue;
        }

        /// <summary>
        /// Retrieves a value of the specified type from memory by key, or returns a default value if the key is not
        /// found or the value is not of the expected type.
        /// </summary>
        /// <remarks>If the value stored under the specified key is not of the requested type, the method
        /// returns the default value rather than attempting a conversion.</remarks>
        /// <typeparam name="T">The type of the value to retrieve from memory.</typeparam>
        /// <param name="key">The key associated with the value to retrieve. Cannot be null.</param>
        /// <param name="defaultValue">The value to return if the key is not found or the stored value is not of type <typeparamref name="T"/>. The
        /// default is the default value of <typeparamref name="T"/>.</param>
        /// <returns>The value associated with the specified key if it exists and is of type <typeparamref name="T"/>; otherwise,
        /// the specified default value.</returns>
        public T? GetMemoryOrDefault<T>(string key, T? defaultValue = default)
        {
            if (Memory.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;

            return defaultValue;
        }

        /// <summary>
        /// Sets the value associated with the specified key in the memory store.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="key">The key with which the value will be associated. Cannot be null.</param>
        /// <param name="value">The value to store in memory. May be null for reference types.</param>
        public void SetMemory<T>(string key, T value)
        {
            Memory[key] = value!;
        }
    }
}