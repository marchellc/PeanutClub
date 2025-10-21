using LabExtended.API;
using LabExtended.Events;

using System.Text;

using UnityEngine;

namespace mcx.Utilities.Features
{
    /// <summary>
    /// Tracks player's health and updates their custom info display accordingly.
    /// </summary>
    public static class PlayerInfoHealth
    {
        private static void Internal_RefreshingCustomInfo(ExPlayer player, StringBuilder builder)
        {
            if (!player.Role.IsAlive)
                return;

            builder.AppendLine($"{Mathf.CeilToInt(player.Health)} HP / {Mathf.CeilToInt(player.MaxHealth)}");
        }

        internal static void Internal_Init()
        {
            ExPlayerEvents.RefreshingCustomInfo += Internal_RefreshingCustomInfo;
        }
    }
}