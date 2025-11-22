namespace SecretLabAPI.Utilities
{
    /// <summary>
    /// Represents a method that attempts to convert the specified string to an object of type T, returning a value that
    /// indicates whether the conversion succeeded.
    /// </summary>
    /// <remarks>This delegate follows the common TryParse pattern used throughout .NET for safe parsing
    /// operations that do not throw exceptions on failure.</remarks>
    /// <typeparam name="T">The type of the object to convert to.</typeparam>
    /// <param name="value">The string value to parse into an object of type T.</param>
    /// <param name="result">When this method returns, contains the parsed value of type T if the conversion succeeded; otherwise, the
    /// default value for type T.</param>
    /// <returns>true if the string was successfully converted to type T; otherwise, false.</returns>
    public delegate bool TryParseDelegate<T>(string value, out T result);
}