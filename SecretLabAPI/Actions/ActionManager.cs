using LabApi.Loader.Features.Paths;

using LabExtended.API;

using LabExtended.Core;
using LabExtended.Utilities;
using LabExtended.Extensions;

using SecretLabAPI.Levels;
using SecretLabAPI.Utilities;
using SecretLabAPI.Extensions;

using SecretLabAPI.Actions.API;
using SecretLabAPI.Actions.Attributes;
using SecretLabAPI.Actions.Extensions;

using NorthwoodLib.Pools;

using System.Reflection;

using LabApi.Loader;

using Utils.NonAllocLINQ;

namespace SecretLabAPI.Actions
{
    /// <summary>
    /// Helps with action executions.
    /// </summary>
    public static class ActionManager
    {
        /// <summary>
        /// Gets a collection of action definitions indexed by their table names.
        /// </summary>
        public static Dictionary<string, WeightedActionDefinition> Tables { get; private set; } = new()
        {
            ["example"] = new(),
            ["example2"] = new()
        };

        /// <summary>
        /// Gets a dictionary of all compiled actions, keyed by their ID.
        /// </summary>
        public static Dictionary<string, ActionMethod> Actions { get; } = new();

        /// <summary>
        /// Registers all eligible actions defined in the specified assembly and returns the total number of actions
        /// registered.
        /// </summary>
        /// <param name="assembly">The assembly containing types whose actions will be registered. Cannot be null.</param>
        /// <returns>The total number of actions successfully registered from all types in the assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public static int RegisterActions(Assembly assembly)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            var count = 0;

            foreach (var type in assembly.GetTypes())
                count += RegisterActions(type);

            return count;
        }

        /// <summary>
        /// Registers all eligible action methods defined on the specified type and returns the number of actions
        /// successfully registered.
        /// </summary>
        /// <param name="type">The type whose methods will be scanned and registered as actions. Cannot be null.</param>
        /// <returns>The number of methods on the specified type that were successfully registered as actions.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public static int RegisterActions(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var count = 0;

            foreach (var method in type.GetAllMethods())
            {
                if (!RegisterAction(method))
                    continue;

                count++;
            }

            return count;
        }

        /// <summary>
        /// Attempts to register a static method as an action if it meets the required signature and attribute
        /// constraints.
        /// </summary>
        /// <remarks>The method will only be registered if it matches the expected signature and is
        /// decorated with <see cref="ActionAttribute"/>. If registration fails due to signature mismatch or other
        /// constraints, the method returns false without throwing an exception.</remarks>
        /// <param name="method">The method to register as an action. Must be static, decorated with <see cref="ActionAttribute"/>, return
        /// <see cref="ActionResultFlags"/>, and accept parameters of type <see cref="ActionContext"/> by reference and
        /// <see cref="CompiledParameter[]"/>.</param>
        /// <returns>true if the method was successfully registered as an action; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null.</exception>
        public static bool RegisterAction(MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            if (!method.IsStatic)
                return false;

            if (!method.HasAttribute<ActionAttribute>(out var actionAttribute))
                return false;

            if (method.ReturnType != typeof(ActionResultFlags))
                return false;

            var parameters = method.GetParameters();

            if (parameters.Length != 1)
                return false;

            if (parameters[0].ParameterType != typeof(ActionContext).MakeByRefType())
                return false;

            try
            {
                var actionDelegate = (ActionDelegate)method.CreateDelegate(typeof(ActionDelegate));

                if (actionDelegate is null)
                    return false;

                var actionParameters = GetParameters(method);
                var actionMethod = new ActionMethod(actionAttribute.Id, actionDelegate, actionParameters);

                Actions[actionAttribute.Id] = actionMethod;

                ApiLog.Info("ActionManager", $"Registered action &3{method.Name}&r with ID &6{actionAttribute.Id}&r");
                return true;
            }
            catch (Exception ex)
            {
                ApiLog.Error("ActionManager", $"Error registering action &3{method.Name}&r:\n{ex}");
                return false;
            }
        }

        /// <summary>
        /// Executes the specified compiled action for the given player and returns a value indicating whether the
        /// execution was successful.
        /// </summary>
        /// <param name="action">The compiled action to execute. Cannot be null.</param>
        /// <param name="player">The player for whom the action is executed. Cannot be null.</param>
        /// <returns>true if the action was executed successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="player"/> is null.</exception>
        public static bool ExecuteAction(this CompiledAction action, ExPlayer player)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            if (player is null)
                throw new ArgumentNullException(nameof(player));

            var actions = ListPool<CompiledAction>.Shared.Rent(1);
            var players = ListPool<ExPlayer>.Shared.Rent(1);

            players.Add(player);
            actions.Add(action);

            var result = actions.ExecuteActions(players);

            ListPool<CompiledAction>.Shared.Return(actions);
            ListPool<ExPlayer>.Shared.Return(players);

            return result;
        }

        /// <summary>
        /// Executes the specified compiled action for the provided list of players.
        /// </summary>
        /// <param name="action">The compiled action to execute. Cannot be null.</param>
        /// <param name="players">The list of players on which to execute the action. Cannot be null.</param>
        /// <returns>true if the action was successfully executed for all players; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> or <paramref name="players"/> is null.</exception>
        public static bool ExecuteAction(this CompiledAction action, List<ExPlayer> players)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            if (players is null)
                throw new ArgumentNullException(nameof(players));

            var list = ListPool<CompiledAction>.Shared.Rent(1);

            list.Add(action);

            var result = list.ExecuteActions(players);

            ListPool<CompiledAction>.Shared.Return(list);
            return result;
        }

        /// <summary>
        /// Executes the specified compiled actions for the given player and returns a value indicating whether all
        /// actions completed successfully.
        /// </summary>
        /// <param name="actions">The list of compiled actions to execute. Cannot be null.</param>
        /// <param name="player">The player for whom the actions will be executed. Cannot be null.</param>
        /// <returns>true if all actions executed successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actions"/> or <paramref name="player"/> is null.</exception>
        public static bool ExecuteActions(this List<CompiledAction> actions, ExPlayer player)
        {
            if (actions is null)
                throw new ArgumentNullException(nameof(actions));

            if (player is null)
                throw new ArgumentNullException(nameof(player));

            var list = ListPool<ExPlayer>.Shared.Rent(1);

            list.Add(player);

            var result = actions.ExecuteActions(list);

            ListPool<ExPlayer>.Shared.Return(list);
            return result;
        }

        /// <summary>
        /// Executes a sequence of compiled actions for the specified players and returns a value indicating whether all
        /// actions completed successfully.
        /// </summary>
        /// <remarks>If either the actions or players list is empty, the method returns false without
        /// executing any actions. If an action signals to stop or an error occurs during execution, the method disposes
        /// the context and returns false or the result as indicated by the action flags. Logging is performed for
        /// unsuccessful or error cases.</remarks>
        /// <param name="actions">The list of compiled actions to execute. Cannot be null or empty.</param>
        /// <param name="players">The list of players for whom the actions are executed. Cannot be null or empty.</param>
        /// <returns>true if all actions were executed and completed successfully; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either the actions or players parameter is null.</exception>
        public static bool ExecuteActions(this List<CompiledAction> actions, List<ExPlayer> players)
        {
            if (actions is null)
                throw new ArgumentNullException(nameof(actions));

            if (players is null)
                throw new ArgumentNullException(nameof(players));

            if (actions.Count < 1)
                return false;

            if (players.Count < 1)
                return false;

            var context = new ActionContext(actions, players);

            for (context.IteratorIndex = 0; context.IteratorIndex < actions.Count; context.IteratorIndex++)
            {
                var current = actions[context.IteratorIndex];

                context.Index = context.IteratorIndex;
                context.Previous = context.Current;
                context.Current = current;
                context.Next = context.IteratorIndex + 1 < actions.Count ? actions[context.IteratorIndex + 1] : null;

                try
                {
                    var flags = current.Action.Delegate(ref context);

                    if (flags.ShouldStop())
                    {
                        if (flags.ShouldDispose())
                            context.Dispose();

                        return flags.IsSuccess();
                    }
                    else if (!flags.IsSuccess())
                    {
                        context.Dispose();

                        ApiLog.Warn("ActionManager", $"Action &r{current.Action.Delegate.Method}&r returned unsuccessful result.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ApiLog.Error("ActionManager", $"Error executing compiled action &r{current.Action.Delegate.Method}&r:\n{ex}");

                    context.Dispose();
                    return false;
                }
            }

            context.Dispose();
            return true;
        }

        /// <summary>
        /// Attempts to select a weighted table for the specified player and execute its associated actions.
        /// </summary>
        /// <remarks>Table selection is based on weighted criteria, which may be influenced by
        /// player-specific multipliers and attributes. If no valid table is found or the player is invalid, the method
        /// returns false.</remarks>
        /// <param name="player">The player for whom the table selection and execution is performed. Cannot be null and must have a valid
        /// ReferenceHub.</param>
        /// <returns>true if a suitable table was selected and its actions executed for the player; otherwise, false.</returns>
        public static bool SelectAndExecuteTable(this ExPlayer player, Func<WeightedActionDefinition, string, bool>? predicate = null)
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

            return table.CachedActions.ExecuteActions(player);
        }

        /// <summary>
        /// Executes the set of actions associated with the specified table for the given player.
        /// </summary>
        /// <remarks>Returns false if the player is invalid or if the specified table does not
        /// exist.</remarks>
        /// <param name="player">The player for whom the table actions will be executed. Must not be null and must have a valid reference
        /// hub.</param>
        /// <param name="tableName">The name of the table whose actions are to be executed. Must correspond to an existing table.</param>
        /// <returns>true if the table exists and its actions were executed for the player; otherwise, false.</returns>
        public static bool ExecuteTable(this ExPlayer player, string tableName)
        {
            if (player?.ReferenceHub == null)
                return false;

            if (!Tables.TryGetValue(tableName, out var table))
                return false;

            return table.CachedActions.ExecuteActions(player);
        }

        /// <summary>
        /// Executes the actions associated with the specified table for the provided list of players.
        /// </summary>
        /// <remarks>Returns false if the players list is null or empty, or if the specified table does
        /// not exist.</remarks>
        /// <param name="players">The list of players to execute table actions for. Must not be null or empty.</param>
        /// <param name="tableName">The name of the table whose actions will be executed. Must correspond to an existing table.</param>
        /// <returns>true if the table actions were successfully executed for the players; otherwise, false.</returns>
        public static bool ExecuteTable(this List<ExPlayer> players, string tableName)
        {
            if (players is null || players.Count < 1)
                return false;

            if (!Tables.TryGetValue(tableName, out var table))
                return false;

            return table.CachedActions.ExecuteActions(players);
        }

        /// <summary>
        /// Attempts to parse a list of string representations into compiled actions.
        /// </summary>
        /// <remarks>Parsing will fail if the input list is empty, if the actions list is null, or if no
        /// valid actions are found in the input. Each string in the input list may contain multiple actions separated
        /// by semicolons. Invalid or unknown action identifiers are skipped.</remarks>
        /// <param name="value">A list of strings containing action definitions to be parsed. Each string should follow the expected format
        /// for action specification.</param>
        /// <param name="actions">A list to which successfully parsed and compiled actions will be added. Must not be null.</param>
        /// <returns>true if at least one action was successfully parsed and added to the actions list; otherwise, false.</returns>
        public static bool ParseActions(this List<string> value, List<CompiledAction> actions)
        {
            if (Actions.Count < 1)
                return false;

            if (actions is null)
                return false;

            if (value.Count < 1)
                return false;

            for (var i = 0; i < value.Count; i++)
            {
                var trimmed = value[i].Trim();

                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // ActionArg; ActionAndArgs; ActionAndArgs;
                var parts = trimmed.SplitEscaped(';');

                if (parts.Length < 1)
                    continue;

                // - ActionID: Arg, Arg, Arg; ActionID: Arg, Arg; ActionID: Arg;
                // OR
                // - ActionID
                // - ActionID: Arg
                // - ActionID: ArgKey=ArgValue
                // OR
                // ActionID: Arg=Value, Arg=Value; ActionID: Arg=Value; ActionID: Arg=Value, Arg=Value, Arg=Value;
                for (var x = 0; x < parts.Length; x++)
                {
                    var part = parts[x].Trim();

                    if (string.IsNullOrEmpty(part))
                        continue;

                    var actionParts = part.SplitEscaped(':');

                    var actionId = actionParts[0].Trim();
                    var actionArgs = string.Join(":", actionParts.Skip(1));

                    if (!Actions.TryGetValue(actionId, out var action))
                    {
                        ApiLog.Error("ActionManager", $"Failed to compile action &3{part}&r: unknown action ID (&6{actionId}&r)");
                        continue;
                    }

                    var argsList = actionArgs.SplitEscaped(',');
                    var resultAction = CompileAction(action, argsList);

                    if (resultAction is null)
                    {
                        ApiLog.Error("ActionManager", $"Failed to compile action: &3{part}&r");
                        continue;
                    }

                    actions.Add(resultAction);
                }
            }

            return actions.Count > 0;
        }

        // Arguments can either be assigned by their position or by their name (using a key=value format).
        private static CompiledAction? CompileAction(ActionMethod method, string[] args)
        {
            var array = new CompiledParameter[method.Parameters.Length];
            var output = default(string);

            for (var i = 0; i < args.Length; i++)
            {
                if (i >= method.Parameters.Length)
                {
                    ApiLog.Warn("ActionManager", $"Error while compiling action (&3{method.Id}&r): Too many arguments were provided.");
                    return null;
                }

                var compiledArg = new CompiledParameter();
                var arg = args[i].Trim();

                if (string.IsNullOrEmpty(arg))
                {
                    ApiLog.Error("ActionManager", $"Error while compiling action (&3{method.Id}&r): Argument &6{method.Parameters[i].Name}&r was provided as empty.");
                    return null;
                }

                if (output is null && i == 0 && arg[0] == '$')
                {
                    output = arg.Substring(1).Trim();
                    args = args.Skip(1).ToArray();

                    i--;
                    continue;
                }

                var splitArg = arg.SplitEscaped('=');

                if (splitArg.Length == 2)
                {
                    var argKey = splitArg[0].Trim();
                    var argValue = splitArg[1].Trim();
                    var argIndex = method.Parameters.FindIndex(x => string.Equals(x.Name, argKey, StringComparison.InvariantCultureIgnoreCase));

                    if (argIndex == -1)
                    {
                        ApiLog.Error("ActionManager", $"Error while compiling action (&3{method.Id}&r): No argument labeled &6{argKey}&r was found.");
                        return null;
                    }

                    if (array[argIndex] != null)
                    {
                        ApiLog.Error("ActionManager", $"Error while compiling action (&3{method.Id}&r): Duplicate argument &6{argKey}&r was provided.");
                        return null;
                    }

                    compiledArg.Source = argValue;

                    array[argIndex] = compiledArg;
                }
                else
                {
                    compiledArg.Source = arg;

                    if (array[i] != null)
                    {
                        ApiLog.Error("ActionManager", $"Error while compiling action (&3{method.Id}&r): Duplicate argument &6{method.Parameters[i].Name}&r was provided.");
                        return null;
                    }

                    array[i] = compiledArg;
                }
            }

            return new(method, array, output);
        }

        private static ActionParameter[] GetParameters(MethodInfo method)
        {
            var attributes = method.GetCustomAttributes<ActionParameterAttribute>();
            var count = attributes.Count();

            if (count < 1)
                return Array.Empty<ActionParameter>();

            var parameters = new ActionParameter[count];
            var index = 0;

            foreach (var attribute in attributes)
            {
                parameters[index] = new ActionParameter(index, attribute.Name, attribute.DefaultValue);

                index++;
            }

            return parameters;
        }

        internal static void Initialize()
        {
            PluginLoader.Plugins.ForEachValue(asm => RegisterActions(asm));

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

                var id = Path.GetFileNameWithoutExtension(file);

                var action = new ActionDelegate((ref context) =>
                {
                    definition.CachedActions.ExecuteActions(context.Players.ToList());
                    return ActionResultFlags.Success | ActionResultFlags.Dispose;
                });

                var method = new ActionMethod(id, action, Array.Empty<ActionParameter>());

                Actions[Path.GetFileNameWithoutExtension(file)] = method;

                ApiLog.Info("ActionHelper", $"Loaded custom action &3{Path.GetFileName(file)}&r");
            }

            Tables = SecretLab.LoadConfig(false, "tables", () => Tables);

            ApiLog.Info("ActionHelper", $"Loaded &6{Tables.Count}&r action table(s)");
        }
    }
}