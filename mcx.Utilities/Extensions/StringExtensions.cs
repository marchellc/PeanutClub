using UnityEngine;

namespace mcx.Utilities.Extensions
{
    /// <summary>
    /// String extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets a dictionary that maps emoji names to their corresponding Unicode characters.
        /// </summary>
        public static Dictionary<string, string> Emojis { get; } = new()
        {
            { "$EmojiChart", "📊" }
        };

        /// <summary>
        /// Attempts to parse a string representation of a 2D vector into a <see cref="Vector2"/> instance.
        /// </summary>
        /// <remarks>If the input string is null, empty, or contains invalid components, the corresponding
        /// values in <paramref name="result"/> are set to zero. Only the first two comma-separated values are
        /// considered as the X and Y components, respectively.</remarks>
        /// <param name="str">The string containing the vector components, separated by a comma (e.g., "1.0,2.0").</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Vector2"/> value if parsing succeeded; otherwise,
        /// contains <see cref="Vector2.zero"/>.</param>
        /// <returns><see langword="true"/> if at least one component is successfully parsed; otherwise, <see langword="false"/>.</returns>
        public static bool TryParseVector2(this string str, out Vector2 result)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                result = Vector2.zero;
                return false;
            }

            var parts = str.Split(',');

            var x = 0f;
            var y = 0f;
            var z = 0f;

            var successX = parts.Length > 0 && float.TryParse(parts[0], out x);
            var successY = parts.Length > 1 && float.TryParse(parts[1], out y);

            result = new(
                successX ? x : 0f,
                successY ? y : 0f);

            return successX || successY;
        }

        /// <summary>
        /// Attempts to parse a string containing three comma-separated values into a <see cref="Vector3"/> instance.
        /// </summary>
        /// <remarks>Missing or invalid components are set to 0. Components are parsed in order: X, Y,
        /// then Z. Extra components beyond the third are ignored. Leading and trailing whitespace in each component is
        /// allowed.</remarks>
        /// <param name="str">The input string representing the vector components, with values separated by commas (e.g., "1.0,2.0,3.0").</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Vector3"/> value if parsing succeeded; otherwise,
        /// contains <see cref="Vector3.zero"/>.</param>
        /// <returns>true if at least one component is successfully parsed; otherwise, false.</returns>
        public static bool TryParseVector3(this string str, out Vector3 result)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                result = Vector3.zero;
                return false;
            }

            var parts = str.Split(',');

            var x = 0f;
            var y = 0f;
            var z = 0f;

            var successX = parts.Length > 0 && float.TryParse(parts[0], out x);
            var successY = parts.Length > 1 && float.TryParse(parts[1], out y);
            var successZ = parts.Length > 2 && float.TryParse(parts[2], out z);

            result = new(
                successX ? x : 0f,
                successY ? y : 0f,
                successZ ? z : 0f);

            return successX || successY || successZ;
        }

        /// <summary>
        /// Attempts to parse a string representation of a quaternion into a <see cref="Quaternion"/> value.
        /// </summary>
        /// <remarks>Missing or invalid components are set to 0. If the input string is null, empty, or
        /// whitespace, the result is set to <see cref="Quaternion.identity"/> and the method returns false.</remarks>
        /// <param name="str">The string containing the quaternion components, separated by commas. Components are expected in the order:
        /// x, y, z, w.</param>
        /// <param name="result">When this method returns, contains the parsed <see cref="Quaternion"/> value if parsing succeeded;
        /// otherwise, contains <see cref="Quaternion.identity"/>.</param>
        /// <returns>true if at least one component is successfully parsed; otherwise, false.</returns>
        public static bool TryParseQuaternion(this string str, out Quaternion result)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                result = Quaternion.identity;
                return false;
            }

            var parts = str.Split(',');

            var x = 0f;
            var y = 0f;
            var z = 0f;
            var w = 0f;

            var successX = parts.Length > 0 && float.TryParse(parts[0], out x);
            var successY = parts.Length > 1 && float.TryParse(parts[1], out y);
            var successZ = parts.Length > 2 && float.TryParse(parts[2], out z);
            var successW = parts.Length > 3 && float.TryParse(parts[3], out w);

            result = new(
                successX ? x : 0f,
                successY ? y : 0f,
                successZ ? z : 0f,
                successW ? w : 0f);

            return successX || successY || successZ || successW;
        }

        /// <summary>
        /// Replaces all emoji codes in the string with their corresponding Unicode emoji characters.
        /// </summary>
        /// <remarks>Only emoji codes present in the predefined mapping will be replaced. Unrecognized
        /// codes are left unchanged.</remarks>
        /// <param name="str">The input string in which emoji codes will be replaced. Cannot be null.</param>
        /// <returns>A new string with all recognized emoji codes replaced by their Unicode emoji equivalents. If no emoji codes
        /// are found, the original string is returned.</returns>
        public static string ReplaceEmojis(this string str)
        {
            foreach (var pair in Emojis)
                str = str.Replace(pair.Key, pair.Value);

            return str;
        }
    }
}