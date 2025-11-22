using HarmonyLib;

using LabApi.Loader.Features.Paths;

using LabExtended.API;

using LabExtended.Core;
using LabExtended.Utilities;
using LabExtended.Extensions;

using NorthwoodLib.Pools;

using SecretLabAPI.Levels;
using SecretLabAPI.Utilities;
using SecretLabAPI.Extensions;

namespace SecretLabAPI.Actions
{
    /// <summary>
    /// Helps with action executions.
    /// </summary>
    public static class ActionHelper
    {
        /// <summary>
        /// Gets a dictionary of all registered actions, keyed by their ID.
        /// </summary>
        public static Dictionary<string, ActionDelegate> Actions { get; } = new();

        /// <summary>
        /// Gets a collection of action definitions indexed by their table names.
        /// </summary>
        public static Dictionary<string, WeightedActionDefinition> Tables { get; private set; } = new()
        {
            ["example"] = new(),
            ["example2"] = new()
        };

        /// <summary>
        /// Registers an action delegate with the specified identifier for later invocation.
        /// </summary>
        /// <remarks>If an action delegate is already registered with the same identifier, it will be
        /// overwritten by the new delegate.</remarks>
        /// <param name="id">The unique identifier used to associate the action delegate. Cannot be null or empty.</param>
        /// <param name="actionDelegate">The delegate to be registered and invoked when the associated identifier is triggered. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null or empty, or if <paramref name="actionDelegate"/> is null.</exception>
        public static void Register(string id, ActionDelegate actionDelegate)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (actionDelegate is null)
                throw new ArgumentNullException(nameof(actionDelegate));

            Actions[id] = actionDelegate;
        }

        /// <summary>
        /// Attempts to unregister the action associated with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the action to unregister. Cannot be null.</param>
        /// <returns>true if the action was successfully unregistered; otherwise, false.</returns>
        public static bool Unregister(string id)
            => id != null && Actions.Remove(id);

        /// <summary>
        /// Attempts to select a weighted table for the specified player and execute its associated actions.
        /// </summary>
        /// <remarks>Table selection is based on weighted criteria, which may be influenced by
        /// player-specific multipliers and attributes. If no valid table is found or the player is invalid, the method
        /// returns false.</remarks>
        /// <param name="player">The player for whom the table selection and execution is performed. Cannot be null and must have a valid
        /// ReferenceHub.</param>
        /// <returns>true if a suitable table was selected and its actions executed for the player; otherwise, false.</returns>
        public static bool TrySelectAndExecuteTableForPlayer(ExPlayer player, Func<WeightedActionDefinition, string, bool>? predicate = null)
        {
            if (player?.ReferenceHub == null)
                return false;

            var table = Tables.GetRandomWeighted(p =>
            {
                if (p.Value is null)
                    return 0f;

                if (predicate != null && !predicate(p.Value, p.Key))
                    return 0f;

                if (p.Value.Weight <= 0f)
                    return 0f;

                if (p.Value.Weight >= 100f)
                    return 100f;

                var weight = p.Value.Weight;

                if (!string.IsNullOrEmpty(p.Value.Multipliers)
                    && WeightMultipliers.Groups.TryGetValue(p.Value.Multipliers, out var group))
                    weight = group.GetWeight(weight, player.UserId, player.PermissionsGroupName, player.GetLevel());

                return weight;
            }).Value;

            if (table == null)
                return false;

            return table.CachedActions.TryExecute(player);
        }

        /// <summary>
        /// Attempts to execute the table action associated with the specified name on the provided target object.
        /// </summary>
        /// <param name="name">The name of the table whose action should be executed. Cannot be null or empty.</param>
        /// <param name="target">The target object on which the table action will be performed.</param>
        /// <returns>true if the table action was found and executed successfully; otherwise, false.</returns>
        public static bool TryExecuteTable(string name, object target)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (!Tables.TryGetValue(name, out var definition))
                return false;

            return definition.CachedActions.TryExecute(target);
        }

        /// <summary>
        /// Attempts to execute the specified list of actions on the provided target, optionally parsing actions from
        /// the given string values if the list is empty.
        /// </summary>
        /// <remarks>If <paramref name="actions"/> is null, the method returns <see langword="false"/>. If
        /// <paramref name="actions"/> is empty and <paramref name="value"/> is provided, the method attempts to parse
        /// actions from <paramref name="value"/> before execution. This method does not throw exceptions for invalid
        /// input; it returns <see langword="false"/> if execution cannot proceed.</remarks>
        /// <param name="actions">The list of actions to execute. If empty, actions may be parsed from <paramref name="value"/>.</param>
        /// <param name="value">A list of string representations of actions to parse and execute if <paramref name="actions"/> is empty. Can
        /// be null or empty.</param>
        /// <param name="target">The target object on which the actions are executed. The meaning of the target depends on the actions being
        /// performed.</param>
        /// <returns>true if one or more actions were successfully executed on the target; otherwise, false.</returns>
        public static bool TryExecuteCached(this List<ActionInfo> actions, List<string> value, object target)
        {
            if (actions == null)
                return false;

            if (actions.Count < 1 && (value?.Count < 1 || !TryParseString(value, actions)))
                return false;

            return actions.TryExecute(target);
        }

        /// <summary>
        /// Attempts to parse and execute a sequence of string actions on the specified target object.
        /// </summary>
        /// <remarks>This method does not throw exceptions for invalid or empty input; instead, it returns
        /// false if the action list is empty or parsing/execution fails. The method is intended for scenarios where
        /// failure is expected and should be handled gracefully.</remarks>
        /// <param name="value">The list of strings representing actions to be parsed and executed. The list must contain at least one
        /// element.</param>
        /// <param name="target">The target object on which the parsed actions will be executed.</param>
        /// <returns>true if all actions are successfully parsed and executed on the target; otherwise, false.</returns>
        public static bool TryExecute(this List<string> value, object target)
        {
            if (value?.Count < 1)
                return false;

            var list = ListPool<ActionInfo>.Shared.Rent();
            var result = TryParseString(value, list) && list.TryExecute(target);

            ListPool<ActionInfo>.Shared.Return(list);
            return result;
        }

        /// <summary>
        /// Attempts to execute each action in the specified collection using the provided target object. Returns a
        /// value indicating whether all actions were executed successfully.
        /// </summary>
        /// <remarks>If any action throws an exception during execution, the method logs the error and
        /// returns false. No further actions are executed after an error occurs.</remarks>
        /// <param name="actions">The collection of actions to execute. Must not be null and must contain at least one element.</param>
        /// <param name="target">The target object to pass to each action. Must not be null.</param>
        /// <returns>true if all actions were executed without error; otherwise, false.</returns>
        public static bool TryExecute(this List<ActionInfo> actions, object target)
        {
            if (actions?.Count < 1)
                return false;

            if (target == null)
                return false;

            for (var i = 0; i < actions!.Count; i++)
            {
                var actionInfo = actions[i];

                try
                {
                    var result = actionInfo.Action?.Invoke(ref target, actionInfo, i, actions) ?? true;

                    if (!result)
                        return true;
                }
                catch (Exception ex)
                {
                    ApiLog.Error("ActionHelper", $"Error executing action '{actionInfo.Id}':\n{ex}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to parse the specified string into a collection of action information objects.
        /// </summary>
        /// <remarks>Parsing will fail if the input string is empty, if no actions are defined in the
        /// system, or if <paramref name="actions"/> is <see langword="null"/>. Unknown action IDs are ignored and do
        /// not prevent successful parsing of other valid actions.</remarks>
        /// <param name="value">The input string containing one or more action definitions, separated by commas. Each action should be
        /// formatted as an action ID followed by arguments, separated by semicolons.</param>
        /// <param name="actions">A list to which parsed <see cref="ActionInfo"/> objects will be added if parsing succeeds. Must not be <see
        /// langword="null"/>.</param>
        /// <returns>true if at least one valid action was parsed and added to <paramref name="actions"/>; otherwise, false.</returns>
        public static bool TryParseString(List<string> value, List<ActionInfo> actions)
        {
            if (Actions.Count < 1)
                return false;

            if (actions is null)
                return false;

            if (value.Count < 1)
                return false;

            foreach (var str in value)
            {
                var parts = str.SplitEscaped(',');

                if (parts.Length < 1)
                    continue;

                // ID; Arg; Arg2; Arg;
                for (var i = 0; i < parts.Length; i++)
                {
                    var sub = parts[i].Trim();
                    var subParts = sub.SplitOutsideQuotes(';');

                    if (subParts.Length < 1)
                        continue;

                    var actionId = subParts[0].Trim();

                    if (!Actions.TryGetValue(actionId, out var action))
                    {
                        ApiLog.Warn("ActionHelper", $"Unknown action ID: &3{actionId}&r");
                        continue;
                    }

                    subParts = subParts.Skip(1).Select(x => x.Trim()).ToArray();

                    actions.Add(new ActionInfo(actionId, subParts, action));
                }
            }

            return actions.Count > 0;
        }

        private static void OnDiscovered(Type type)
        {
            foreach (var method in type.GetDeclaredMethods())
            {
                if (!method.IsStatic)
                    continue;

                if (method.ReturnType != typeof(bool))
                    continue;

                var parameters = method.GetAllParameters();

                if (parameters.Length != 4 ||
                    parameters[0].ParameterType != typeof(object) ||
                    parameters[1].ParameterType != typeof(ActionInfo) ||
                    parameters[2].ParameterType != typeof(int) ||
                    parameters[3].ParameterType != typeof(List<ActionInfo>))
                    continue;

                try
                {
                    var action = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), method);

                    Actions[method.Name] = action;

                    ApiLog.Debug("ActionHelper", $"Registered action &3{method.Name}&r (&6{method}&r)");
                }
                catch (Exception ex)
                {
                    ApiLog.Error("ActionHelper", $"Failed to register action from method &3{method.Name}&r in type &3{type.FullName}&r:\n{ex}");
                }
            }
        }

        internal static void Initialize()
        {
            ReflectionUtils.Discovered += OnDiscovered;

            var path = Path.Combine(PathManager.SecretLab.FullName, "actions");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var file in Directory.GetFiles(path, "*.yml"))
            {
                if (!FileUtils.TryLoadYamlFile<ActionDefinition>(file, out var definition))
                {
                    ApiLog.Error("ActionHelper", $"Failed to load action definition file: &3{Path.GetFileName(file)}&r");
                    continue;
                }
                
                Actions[Path.GetFileNameWithoutExtension(file)] = (ref target, info, index, list) =>
                {
                    return definition.CachedActions.TryExecute(target);
                };
            }

            Tables = SecretLab.LoadConfig(false, "tables", () => Tables);
        }
    }
}