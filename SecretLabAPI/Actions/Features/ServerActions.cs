using LabApi.Features.Wrappers;

using LabExtended.API;

namespace SecretLabAPI.Actions.Features
{
    /// <summary>
    /// Actions related to server operations.
    /// </summary>
    public static class ServerActions
    {
        private static bool Command(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            var command = info.GetValue(0);

            if (string.IsNullOrEmpty(command))
                return true;

            ExServer.ExecuteCommand(command);
            return true;
        }

        private static bool Kick(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            var reason = info.GetValue(0, "Kicked by an action.");

            if (target is ExPlayer player)
                player.Kick(reason);

            return true;
        }

        private static bool Ban(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            var duration = info.GetValue(0, long.TryParse, (long)30);
            var reason = info.GetValue(1, "Banned by an action.");

            if (target is ExPlayer player)
                player.Ban(reason, duration);

            return true;
        }

        private static bool Unban(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            var userId = info.GetValue(0);
            var ipAddress = info.GetValue(1);

            if (string.IsNullOrEmpty(userId))
                return true;

            if (!string.IsNullOrEmpty(userId))
                Server.UnbanUserId(userId);

            if (!string.IsNullOrEmpty(ipAddress))
                Server.UnbanIpAddress(ipAddress);

            return true;
        }

        private static bool Restart(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            Server.Restart();
            return true;
        }

        private static bool GlobalBroadcast(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            var message = info.GetValue(0);
            var duration = info.GetValue(1, ushort.TryParse, (ushort)5);
            var truncated = info.GetValue(2, bool.TryParse, false);

            if (string.IsNullOrEmpty(message))
                return true;

            Server.SendBroadcast(message, duration, truncated ? Broadcast.BroadcastFlags.Truncated : Broadcast.BroadcastFlags.Normal);
            return true;
        }

        private static bool GlobalHint(ref object target, ActionInfo info, int index, List<ActionInfo> list)
        {
            var message = info.GetValue(0);
            var duration = info.GetValue(1, ushort.TryParse, (ushort)5);
            var priority = info.GetValue(2, bool.TryParse, false);

            if (string.IsNullOrEmpty(message))
                return true;

            ExPlayer.Players.ForEach(x => x.SendHint(message, duration, priority));
            return true;
        }
    }
}