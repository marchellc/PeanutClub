using System.ComponentModel;

using PeanutClub.Utilities.Roles.Selection;

namespace PeanutClub.Roles;

/// <summary>
/// The config file of the Roles plugin.
/// </summary>
public class RolesConfig
{
    /// <summary>
    /// Janitor spawn conditions.
    /// </summary>
    [Description("Sets the Janitor spawn conditions.")]
    public List<RoleRange> JanitorSpawns { get; set; } = new()
    {
        new()
        {
            MinPlayers = 1,
            MaxPlayers = 6,
            OverallChance = 20,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 7,
            MaxPlayers = 11,
            OverallChance = 50,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 12,
            MaxPlayers = 18,
            OverallChance = 80,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 19,
            MaxPlayers = -1,
            OverallChance = 100,
            MaxSpawnCount = 1
        }
    };

    /// <summary>
    /// Guard Commander spawn conditions.
    /// </summary>
    [Description("Sets the Guard Commander spawn conditions.")]
    public List<RoleRange> GuardCommanderSpawns { get; set; } = new()
    {
        new()
        {
            MinPlayers = 3,
            MaxPlayers = 5,
            OverallChance = 20,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 6,
            MaxPlayers = 12,
            OverallChance = 40,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 13,
            MaxPlayers = 16,
            OverallChance = 60,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 17,
            MaxPlayers = 26,
            OverallChance = 80,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 27,
            MaxPlayers = -1,
            OverallChance = 100,
            MaxSpawnCount = 1
        }
    };
}