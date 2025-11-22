using LabExtended.API;
using LabExtended.Extensions;

using LabExtended.Events;

using LabExtended.Utilities;

using NorthwoodLib.Pools;

using PlayerRoles;

namespace SecretLabAPI.Utilities.Roles;

/// <summary>
/// Selects custom roles.
/// </summary>
public class RoleSelector
{
    /// <summary>
    /// Gets the target list of ranges.
    /// </summary>
    public List<RoleRange> Ranges { get; }
    
    /// <summary>
    /// Gets the target list of players.
    /// </summary>
    public List<ExPlayer> Players { get; }
    
    /// <summary>
    /// Gets the delegate used to set the player's role.
    /// </summary>
    public Action<ExPlayer> SetRole { get; }
    
    /// <summary>
    /// Gets the filtering predicate.
    /// </summary>
    public Func<ExPlayer, RoleTypeId, bool>? Predicate { get; }

    /// <summary>
    /// Creates a new <see cref="RoleSelector"/> instance.
    /// </summary>
    /// <param name="ranges">The list of ranges.</param>
    /// <param name="setRole">The delegate used to set the player's custom role.</param>
    /// <param name="predicate">The predicate used to filter players.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public RoleSelector(List<RoleRange> ranges, Action<ExPlayer> setRole, Func<ExPlayer, RoleTypeId, bool>? predicate = null)
    {
        if (ranges is null)
            throw new ArgumentNullException(nameof(ranges));
        
        Ranges = ranges;
        SetRole = setRole;
        Predicate = predicate;
        
        Players = new();

        ExRoundEvents.Started += Internal_Started;
    }

    private void Internal_Started()
    {
        TimingUtils.AfterSeconds(() =>
        {
            Players.Clear();
        
            var range = SelectRange();

            if (range is null)
                return;
        
            var buffer = ListPool<ExPlayer>.Shared.Rent();

            foreach (var player in ExPlayer.Players)
            {
                if (Predicate != null && !Predicate(player, player.Role.Type))
                    continue;

                buffer.Add(player);
            }
        
            SelectPlayers(range, buffer);

            ListPool<ExPlayer>.Shared.Return(buffer);
            
            for (var x = 0; x < Players.Count; x++)
            {
                SetRole.InvokeSafe(Players[x]);
            }

            Players.Clear();
        }, 0.5f);
    }

    private void SelectPlayers(RoleRange range, List<ExPlayer> players)
    {
        if (players.Count == 0)
        {
            return;
        }

        var playerCount = 0;

        if (range.MinSpawnCount > 0 && range.MaxSpawnCount > 0)
        {
            if (range.MinSpawnCount == range.MaxSpawnCount)
            {
                playerCount = range.MinSpawnCount;
            }
            else
            {
                playerCount = UnityEngine.Random.Range(range.MinSpawnCount, range.MaxSpawnCount);
            }
        }
        else
        {
            if (range.MinSpawnCount > 0)
            {
                playerCount = players.Count;
            }
            else
            {
                playerCount = range.MaxSpawnCount;
            }
        }

        if (players.Count <= playerCount)
        {
            Players.AddRange(players);
            return;
        }

        while (Players.Count < playerCount)
        {
            var randomPlayer = players.RandomItem();
            
            Players.Add(randomPlayer);
            
            players.Remove(randomPlayer);
        }
    }
    
    private RoleRange? SelectRange()
    {
        for (var x = 0; x < Ranges.Count; x++)
        {
            var range = Ranges[x];
            
            if (range.MinPlayers > 0 && ExPlayer.Count < range.MinPlayers)
                continue;
            
            if (range.MaxPlayers > 0 && ExPlayer.Count > range.MaxPlayers)
                continue;
            
            if (range.OverallChance <= 0f || (range.OverallChance < 100f && !WeightUtils.GetBool(range.OverallChance)))
                continue;

            return range;
        }

        return null;
    }
}