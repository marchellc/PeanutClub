using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;

using LabExtended.Core;
using LabExtended.Extensions;

using mcx.Loadouts;
using mcx.Overlays.Alerts;
using mcx.Utilities.Roles.Selection;

using PlayerRoles;

namespace mcx.Roles.Janitor;

/// <summary>
/// Handles logic of the Janitor role.
/// </summary>
public static class JanitorHandler
{
    /// <summary>
    /// Gets the role selector used to select Janitor players.
    /// </summary>
    public static RoleSelector Selector { get; private set; }

    /// <summary>
    /// Sets a player as the Janitor.
    /// </summary>
    /// <param name="player">The target player.</param>
    public static void SetJanitor(this ExPlayer player)
    {
        if (player?.ReferenceHub == null)
            throw new ArgumentNullException(nameof(player));
        
        player.Role.Set(RoleTypeId.ClassD, RoleChangeReason.RoundStart, RoleSpawnFlags.None);
        player.Position.Position = RoleTypeId.Scientist.GetSpawnPosition().position;

        player.Inventory.Clear();
        
        LoadoutPlugin.TryApply(player, "Janitor");

        player.CustomInfo = "Janitor";

        if ((player.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
            player.InfoArea |= PlayerInfoArea.CustomInfo;
        
        player.SendAlert(AlertType.Info, 10f, "<b>Tvoje role je</b>\n<size=30><color=yellow><b>UKLÍZEČ</b></color></size>!");
        
        ApiLog.Debug("Janitor Handler", $"Made player &3{player.Nickname}&r (&6{player.UserId}&r) the Janitor.");
    }

    private static void Internal_ChangedRole(PlayerChangedRoleEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Player.CustomInfo) && args.Player.CustomInfo == "Janitor")
        {
            args.Player.CustomInfo = string.Empty;
            args.Player.InfoArea &= ~PlayerInfoArea.CustomInfo;
        }
    }
    
    internal static void Internal_Init()
    {
        Selector = new(RolesCore.ConfigStatic.JanitorSpawns, SetJanitor, (_, role) => role is RoleTypeId.ClassD);
        
        LoadoutPlugin.Ensure("Janitor", new LoadoutDefinition()
            .WithItems(ItemType.Medkit, ItemType.KeycardJanitor));

        PlayerEvents.ChangedRole += Internal_ChangedRole;
    }
}