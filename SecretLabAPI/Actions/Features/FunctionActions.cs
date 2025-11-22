using LabExtended.Core;
using LabExtended.Utilities;

using MEC;

using NorthwoodLib.Pools;

namespace SecretLabAPI.Actions.Features
{
    /// <summary>
    /// Actions that can be used to manipulate function execution.
    /// </summary>
    public static class FunctionActions
    {
        private static bool Repeat(ref object target, ActionInfo info, int index, List<ActionInfo> actions)
        {
            var count = info.GetValue(0, int.TryParse, 1);

            if (count <= 0 || index + 1 >= actions.Count)
                return true;

            var action = actions[index + 1];

            for (var i = 0; i < count; i++)
            {
                try
                {
                    action.Action?.Invoke(ref target, action, index + 1, actions);
                }
                catch (Exception ex)
                {
                    ApiLog.Error("ActionHelper", $"Error occurred while repeating action:\n{ex}");
                }
            }

            actions.RemoveAt(index + 1);
            return true;
        }

        private static bool RepeatDelayed(ref object target, ActionInfo info, int index, List<ActionInfo> actions)
        {
            var count = info.GetValue(0, int.TryParse, 1);
            var delay = info.GetValue(1, float.TryParse, 0f);

            if (count <= 0 || delay <= 0f || index + 1 >= actions.Count)
                return true;

            var action = actions[index + 1];
            var obj = target;

            IEnumerator<float> _Coroutine()
            {
                for (var i = 0; i < count; i++)
                {
                    try
                    {
                        action.Action?.Invoke(ref obj, action, index + 1, actions);
                    }
                    catch (Exception ex)
                    {
                        ApiLog.Error("ActionHelper", $"Error occurred while repeating action:\n{ex}");
                    }

                    yield return Timing.WaitForSeconds(delay);
                }

                actions.RemoveRange(0, index + 1);
            }

            Timing.RunCoroutine(_Coroutine());
            return false;
        }

        private static bool Delay(ref object target, ActionInfo info, int index, List<ActionInfo> actions)
        {
            if (index + 1 >= actions.Count)
                return true;

            var delay = info.GetValue(0, float.TryParse, 0f);

            if (delay <= 0f)
                return true;

            var copy = ListPool<ActionInfo>.Shared.Rent(actions.Count - index);
            var obj = target;

            for (var i = index + 1; i < actions.Count; i++)
                copy.Add(actions[i]);

            TimingUtils.AfterSeconds(() =>
            {
                copy.TryExecute(obj);

                ListPool<ActionInfo>.Shared.Return(copy);
            }, delay);

            return false;
        }
    }
}