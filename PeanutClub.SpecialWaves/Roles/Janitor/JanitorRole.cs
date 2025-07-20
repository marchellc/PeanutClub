using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;
using LabExtended.API.Hints;

using LabExtended.Core;
using LabExtended.Utilities;
using LabExtended.Extensions;

using LabExtended.Events;
using LabExtended.Events.Round;

using PeanutClub.SpecialWaves.Loadouts;

using PlayerRoles;

namespace PeanutClub.SpecialWaves.Roles.Janitor;

/// <summary>
/// Manages the janitor role.
/// </summary>
public static class JanitorRole
{
    /// <summary>
    /// Gets the spawned janitor.
    /// </summary>
    public static ExPlayer? Janitor { get; private set; }

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

    private static void Internal_RoundStarted()
    {
        TimingUtils.AfterSeconds(() =>
        {
            if (Janitor?.ReferenceHub != null)
            {
                SetJanitor(Janitor);
            }
        }, 0.2f);
    }

    private static void Internal_ChangedRole(PlayerChangedRoleEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Player.CustomInfo) && args.Player.CustomInfo == "Janitor")
        {
            args.Player.CustomInfo = string.Empty;
            args.Player.InfoArea &= ~PlayerInfoArea.CustomInfo;
        }
    }

    private static void Internal_AssigningRoles(AssigningRolesEventArgs args)
    {
        if (args.Roles.Count > 0)
        {
            if (args.Roles.Count == 1)
            {
                if (!WeightUtils.GetBool(50f, 50f))
                    return;

                Janitor = args.Roles.First().Key;
            }
            else
            {
                Janitor = args.Roles.GetRandomItem(x => x.Value is RoleTypeId.ClassD).Key;
            }
        }

        if (Janitor?.ReferenceHub != null)
        {
            args.Roles.Remove(Janitor);
        }
    }
    
    internal static void Internal_Init()
    {
        LoadoutManager.EnsureLoadout("Janitor", new LoadoutInfo()
            .WithGameItems(ItemType.Medkit, ItemType.KeycardJanitor));
        
        ExRoundEvents.Started += Internal_RoundStarted;
        ExRoundEvents.AssigningRoles += Internal_AssigningRoles;

        PlayerEvents.ChangedRole += Internal_ChangedRole;
    }
}