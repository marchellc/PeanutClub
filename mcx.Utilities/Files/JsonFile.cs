using LabExtended.Extensions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace mcx.Utilities.Files
{
    /// <summary>
    /// Utilities for parsing JSON files.
    /// </summary>
    public static class JsonFile
    {
        static JsonFile()
        {
            Settings = new();
            Settings.Formatting = Formatting.Indented;

            if (Settings.Converters.TryGetFirst(x => x is StringEnumConverter, out var targetConv))
                Settings.Converters.Remove(targetConv);

            Settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(false, false), false));

        }

        /// <summary>
        /// Gets the global serializer settings.
        /// </summary>
        public static JsonSerializerSettings Settings { get; }

        /// <summary>
        /// Writes the specified data to a JSON file.
        /// </summary>
        public static void WriteFile<T>(string filePath, T data)
        {
            var serialized = JsonConvert.SerializeObject(data, Settings);

            File.WriteAllText(filePath, serialized);
        }

        /// <summary>
        /// Reads JSON data from a specified file.
        /// </summary>
        public static T ReadFile<T>(string filePath, T? defaultValue = default)
        {
            if (!File.Exists(filePath))
            {
                var serialized = JsonConvert.SerializeObject(defaultValue, Settings);

                File.WriteAllText(filePath, serialized);
                return defaultValue!;
            }
            else
            {
                try
                {
                    var content = File.ReadAllText(filePath);
                    var deserialized = JsonConvert.DeserializeObject<T>(content, Settings);

                    return deserialized!;
                }
                catch
                {
                    try { File.Delete(filePath + ".error"); } catch { }
                    try { File.Move(filePath, filePath + ".error"); } catch { }

                    var serialized = JsonConvert.SerializeObject(defaultValue, Settings);

                    File.WriteAllText(filePath, serialized);
                    return defaultValue!;
                }
            }
        }
    }
}