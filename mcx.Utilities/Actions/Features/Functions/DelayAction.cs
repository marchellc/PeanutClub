using LabExtended.Utilities;

using mcx.Utilities.Actions.Interfaces;

namespace mcx.Utilities.Actions.Features.Functions
{
    /// <summary>
    /// Delays the execution of subsequent actions by a specified amount of time.
    /// </summary>
    public class DelayAction : IAction
    {
        /// <inheritdoc/>
        public string Id { get; } = "Delay";

        /// <inheritdoc/>
        public string DebugAction(Dictionary<string, string> parameters)
        {
            parameters["Time"] = "The time (in seconds) to delay the execution of subsequent actions.";
            return "Delays the execution of subsequent actions by a specified amount of time.";
        }

        /// <inheritdoc/>
        public ActionResult Trigger(ref ActionContext context)
        {
            var time = context.GetParameterOrDefault("Time", 0, float.TryParse, 0f);

            if (time <= 0f)
                return ActionResult.Success;

            var ctx = context;

            TimingUtils.AfterSeconds(() =>
            {
                ctx.Actions.RemoveRange(0, ctx.CurrentIndex + 1);
                ctx.Actions.TriggerMany(ctx.Source, ctx.Targets);

                ctx.Dispose();
            }, time);

            return ActionResult.Stop;
        }
    }
}