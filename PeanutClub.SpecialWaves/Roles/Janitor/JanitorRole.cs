using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;
using LabExtended.API.Hints;

using LabExtended.Core;
using LabExtended.Extensions;

using PeanutClub.SpecialWaves.Loadouts;

using PlayerRoles;

namespace PeanutClub.SpecialWaves.Roles.Janitor;

/// <summary>
/// Manages the janitor role.
/// </summary>
public static class JanitorRole
{
    /// <summary>
    /// Gets the role selector used to select Janitor players.
    /// </summary>
    public static RoleSelector RoleSelector { get; private set; }

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
        player.ApplyLoadout("Janitor");

        player.CustomInfo = "Janitor";

        if ((player.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
            player.InfoArea |= PlayerInfoArea.CustomInfo;
        
        player.ShowHint("<b>Jsi <color=green>Uklízeč</color>!</b>", 10, true);
        
        ApiLog.Debug("Janitor Role", $"Made player &3{player.Nickname}&r (&6{player.UserId}&r) the Janitor.");
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
        RoleSelector = new(PluginCore.StaticConfig.JanitorSpawns, SetJanitor, (_, role) => role is RoleTypeId.ClassD);
        
        LoadoutManager.EnsureLoadout("Janitor", new LoadoutInfo()
            .WithGameItems(ItemType.Medkit, ItemType.KeycardJanitor));

        PlayerEvents.ChangedRole += Internal_ChangedRole;
    }
}