using LabApi.Loader.Features.Paths;

using LabExtended.API;
using LabExtended.Core;
using LabExtended.Extensions;

using mcx.Utilities.Actions.Features.Audio;
using mcx.Utilities.Actions.Features.Configs;
using mcx.Utilities.Actions.Features.Functions;
using mcx.Utilities.Actions.Features.Items;
using mcx.Utilities.Actions.Features.Misc;

using mcx.Utilities.Actions.Interfaces;

using NorthwoodLib.Pools;

using System.Text;

namespace mcx.Utilities.Actions
{
    /// <summary>
    /// Represents a method that attempts to convert the specified string representation to a value of type T, returning
    /// a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <remarks>This delegate follows the common TryParse pattern used in .NET, where parsing failures do not
    /// throw exceptions but instead return false and set the out parameter to a default value.</remarks>
    /// <typeparam name="T">The type of the value to parse from the string.</typeparam>
    /// <param name="value">The string representation of the value to parse.</param>
    /// <param name="result">When this method returns, contains the parsed value of type T if the conversion succeeded, or the default value
    /// of T if the conversion failed.</param>
    /// <returns>true if the string was converted successfully; otherwise, false.</returns>
    public delegate bool TryParseDelegate<T>(string value, out T result);

    /// <summary>
    /// Represents a method that is called when an action is triggered, providing access to the action context by
    /// reference.
    /// </summary>
    /// <param name="context">A reference to the <see cref="ActionContext"/> that contains information about the triggered action.
    /// Modifications to this parameter will affect the original context.</param>
    public delegate void ActionTriggeredDelegate(ref ActionContext context);

    /// <summary>
    /// Provides static methods and a global registry for managing, registering, and triggering actions across multiple
    /// sources and targets.
    /// </summary>
    /// <remarks>The ActionManager class maintains a centralized collection of actions that can be registered,
    /// unregistered, and triggered in various contexts. All methods are thread-safe for typical usage scenarios, but
    /// callers should ensure thread safety when modifying shared action data concurrently. Actions are identified by
    /// unique string identifiers, and attempts to register duplicate or invalid actions will fail. Errors encountered
    /// during action execution are logged internally; methods return false to indicate failure rather than throwing
    /// exceptions.</remarks>
    public static class ActionManager
    {
        private static bool initialized = false;
        private static string debugPath;

        private static List<IAction> actions = new()
        {
            new PlayAudioAction(),
            new PlayAudioAction(),

            new AddItemAction(),
            new SpawnItemAction(),

            new DelayAction(),
            new ExplodeAction()
        };

        /// <summary>
        /// Gets a dictionary of registered actions.
        /// </summary>
        public static Dictionary<string, IAction> Actions { get; } = new();

        /// <summary>
        /// Gets called before an action is triggered.
        /// </summary>
        public static event ActionTriggeredDelegate? TriggeringAction;

        /// <summary>
        /// Gets called after an action is triggered.
        /// </summary>
        public static event ActionTriggeredDelegate? TriggeredAction;

        /// <summary>
        /// Appends debug information for all registered actions to the specified <see cref="StringBuilder"/>.
        /// </summary>
        /// <remarks>The appended debug information includes the ID, type, description, and any parameters
        /// associated with each action, excluding configuration actions. This method does not clear or modify the
        /// existing contents of the <paramref name="builder"/>.</remarks>
        /// <param name="builder">The <see cref="StringBuilder"/> to which the debug information will be appended. Must not be null.</param>
        public static void AppendDebug(StringBuilder builder)
        {
            var parameters = new Dictionary<string, string>();

            foreach (var action in actions)
            {
                if (action is ConfigAction)
                    continue;

                parameters.Clear();

                var description = action.DebugAction(parameters);

                builder.AppendLine($"ID: {action.Id}");
                builder.AppendLine($"Type: {action.GetType().Name}");
                builder.AppendLine($"Description: {description}");

                if (parameters.Count > 0)
                {
                    builder.AppendLine("Parameters:");

                    foreach (var pair in parameters)
                        builder.AppendLine($"- {pair.Key}: {pair.Value}");
                }

                builder.AppendLine();
            }

            parameters.Clear();
        }

        /// <summary>
        /// Attempts to register the specified action in the global action registry.
        /// </summary>
        /// <remarks>Registration will fail if the action is null, its Id is null or whitespace, or if an
        /// action with the same Id is already registered.</remarks>
        /// <param name="action">The action to register. Must not be null and must have a non-empty, unique identifier.</param>
        /// <returns>true if the action was successfully registered; otherwise, false.</returns>
        public static bool Register(this IAction action)
        {
            if (action is null)
                return false;

            if (string.IsNullOrWhiteSpace(action.Id))
                return false;

            if (Actions.ContainsKey(action.Id))
                return false;

            Actions.Add(action.Id, action);

            if (initialized)
                WriteDebugFile();

            return true;
        }

        /// <summary>
        /// Unregisters the specified action from the global action registry.
        /// </summary>
        /// <remarks>If the action is null or its identifier is null, empty, or consists only of
        /// white-space characters, the method returns false and no action is unregistered.</remarks>
        /// <param name="action">The action to unregister. Must not be null and must have a non-empty identifier.</param>
        /// <returns>true if the action was successfully unregistered; otherwise, false.</returns>
        public static bool Unregister(this IAction action)
        {
            if (action is null)
                return false;

            if (string.IsNullOrWhiteSpace(action.Id))
                return false;

            if (Actions.Remove(action.Id))
            {
                if (initialized)
                    WriteDebugFile();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Triggers a sequence of actions for the specified targets using the provided action source.
        /// </summary>
        /// <remarks>If any action in the sequence is null, contains a null action, or throws an exception
        /// during execution, the method stops processing and returns false. The method also returns false if the
        /// actions or targets collections are null, or if there are no targets provided.</remarks>
        /// <param name="actions">A collection of action descriptors to be triggered. Each descriptor must contain a valid action and any
        /// associated parameters.</param>
        /// <param name="source">The source that initiates the actions. This provides context for the actions being triggered.</param>
        /// <param name="targets">A collection of targets on which the actions will be performed. Must contain at least one target.</param>
        /// <returns>true if all actions are successfully triggered for all targets; otherwise, false.</returns>
        public static bool TriggerMany(this IEnumerable<ActionInfo> actions, IActionSource source, IEnumerable<IActionTarget> targets)
        {
            if (actions is null)
                return false;

            if (targets is null)
                return false;

            var context = new ActionContext(source, null!);

            context.Actions.AddRange(actions);
            context.Targets.AddRange(targets);

            return TriggerContext(ref context);
        }

        /// <summary>
        /// Triggers a sequence of actions on the specified target using the provided action source.
        /// </summary>
        /// <remarks>If any action in the sequence is null or fails to trigger, the method stops
        /// processing further actions and returns false. The method disposes the action context after
        /// execution.</remarks>
        /// <param name="actions">A collection of <see cref="ActionInfo"/> objects representing the actions to trigger. The collection must
        /// not be null, and each item must have a non-null <see cref="ActionInfo.Action"/>.</param>
        /// <param name="source">The source that provides context for the actions being triggered. May be null if the actions do not require
        /// a source.</param>
        /// <param name="target">The target on which the actions will be triggered. Cannot be null.</param>
        /// <returns>true if all actions are triggered successfully; otherwise, false.</returns>
        public static bool TriggerMany(this IEnumerable<ActionInfo> actions, IActionSource source, IActionTarget target)
        {
            if (actions is null)
                return false;

            if (target is null)
                return false;

            var context = new ActionContext(source, null!);

            context.Targets.Add(target);
            context.Actions.AddRange(actions);

            return TriggerContext(ref context);
        }

        /// <summary>
        /// Attempts to trigger the specified action for a single set of targets using the provided source and
        /// parameters.
        /// </summary>
        /// <remarks>Returns false if the action information is invalid, the targets collection is null or
        /// empty, or if an exception occurs during action execution. The method disposes of the action context after
        /// execution.</remarks>
        /// <param name="actionInfo">The action information containing the action to trigger and its associated parameters. Cannot be null and
        /// must contain a valid action.</param>
        /// <param name="source">The source that initiates the action. Used to provide context for the action execution.</param>
        /// <param name="targets">A collection of targets on which the action will be performed. Cannot be null and must contain at least one
        /// target.</param>
        /// <returns>true if the action was successfully triggered for the specified targets; otherwise, false.</returns>
        public static bool TriggerSingle(this ActionInfo actionInfo, IActionSource source, IEnumerable<IActionTarget> targets)
        {
            if (actionInfo?.Action is null)
                return false;

            if (targets is null)
                return false;

            var context = new ActionContext(source, actionInfo.Parameters);

            context.Targets.AddRange(targets);
            return TriggerContext(ref context);
        }

        /// <summary>
        /// Attempts to trigger the specified action using the provided source and target.
        /// </summary>
        /// <remarks>If the action in the provided ActionInfo is null, or if an exception occurs during
        /// triggering, the method returns false. The action context is disposed after the action is triggered or if an
        /// error occurs.</remarks>
        /// <param name="actionInfo">The action information containing the action to trigger and its parameters. Cannot be null, and must contain
        /// a valid action.</param>
        /// <param name="source">The source object that initiates the action. Used to construct the action context.</param>
        /// <param name="target">The target object that the action is intended to affect.</param>
        /// <returns>true if the action was successfully triggered; otherwise, false.</returns>
        public static bool TriggerSingle(this ActionInfo actionInfo, IActionSource source, IActionTarget target)
        {
            if (actionInfo?.Action is null)
                return false;

            var context = new ActionContext(source, actionInfo.Parameters);

            context.Targets.Add(target);
            return TriggerContext(ref context);
        }

        private static bool TriggerContext(ref ActionContext context)
        {
            try
            {
                if (context.Actions.Count < 1)
                {
                    context.Dispose();
                    return false;
                }

                for (var i = 0; i < context.Actions.Count; i++)
                {
                    var actionInfo = context.Actions[i];

                    if (actionInfo?.Action is null)
                    {
                        ApiLog.Error("ActionManager", $"Unknown action ID!");

                        context.Dispose();
                        return false;
                    }

                    context.Parameters = actionInfo.Parameters;

                    context.CurrentAction = actionInfo.Action;
                    context.CurrentIndex = i;

                    if (i + 1 < context.Actions.Count)
                        context.NextAction = context.Actions[i + 1]?.Action;
                    else
                        context.NextAction = null;

                    if (i - 1 >= 0)
                        context.PreviousAction = context.Actions[i - 1]?.Action;
                    else
                        context.PreviousAction = null;

                    TriggeringAction?.Invoke(ref context);

                    try
                    {
                        var result = actionInfo.Action.Trigger(ref context);

                        if (result is ActionResult.StopAndDispose)
                        {
                            context.Dispose();
                            return true;
                        }

                        if (result is ActionResult.Stop)
                            return true;
                    }
                    catch (Exception ex)
                    {
                        ApiLog.Error("ActionManager", $"Could not trigger action &3{actionInfo.Id}&r:\n{ex}");

                        context.Dispose();
                        return false;
                    }

                    TriggeredAction?.Invoke(ref context);
                }

                context.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                ApiLog.Error("ActionManager", $"Could not trigger action context:\n{ex}");

                context.Dispose();
                return false;
            }
        }

        private static void WriteDebugFile()
            => File.WriteAllText(debugPath, StringBuilderPool.Shared.BuildString(AppendDebug));

        internal static void Initialize()
        {
            debugPath = Path.Combine(PathManager.SecretLab.FullName, $"actions_{ExServer.Port}.txt");

            actions.ForEach(action => action.Register());

            foreach (var pair in UtilitiesCore.Config.Actions)
            {
                var action = new ConfigAction(pair.Key);

                foreach (var actionInfo in pair.Value)
                {
                    if (actionInfo?.Action == null)
                        continue;

                    action.Actions.Add(actionInfo);
                }

                action.Register();
            }

            WriteDebugFile();

            initialized = true;
        }
    }
}