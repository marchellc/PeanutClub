using LabExtended.API;
using LabExtended.API.CustomTeams;

using LabExtended.Core;
using LabExtended.Attributes;

using PeanutClub.LoadoutAPI;

using PlayerRoles;
using PlayerRoles.Spectating;

namespace PeanutClub.Teams.Archangels;

/// <summary>
/// Zeta-3 Archangels team handler.
/// </summary>
[LoaderIgnore]
public class ArchangelsTeam : CustomTeamHandler<ArchangelsWave>
{
    /// <summary>
    /// Gets the singleton instance of the team handler.
    /// </summary>
    public static ArchangelsTeam Singleton { get; private set; }

    /// <summary>
    /// Gets the CASSIE announcement.
    /// </summary>
    public static string CassieMessage => PluginCore.StaticConfig.ArchangelsCassieMessage;

    /// <inheritdoc cref="CustomTeamHandler.Name"/>
    public override string? Name { get; } = "Zeta-3 \"Archangels\"";

    /// <inheritdoc cref="CustomTeamHandler.IsSpawnable"/>
    public override bool IsSpawnable(ExPlayer player)
        => player?.ReferenceHub != null && player.Role.Role is SpectatorRole spectatorRole && spectatorRole.ReadyToRespawn;

    // Archangels 1 - Repressor
    // Archangels 2 - Rifleman (1)
    // Archangels 3 - Marauder (1)
    /// <inheritdoc cref="CustomTeamHandler.SelectRole"/>
    public override RoleTypeId SelectRole(ExPlayer player, Dictionary<ExPlayer, RoleTypeId> selectedRoles)
    {
        if (!selectedRoles.Any(x => x.Value is RoleTypeId.ChaosRifleman))
            return RoleTypeId.ChaosRifleman;

        if (!selectedRoles.Any(x => x.Value is RoleTypeId.ChaosMarauder))
            return RoleTypeId.ChaosMarauder;

        return RoleTypeId.ChaosRepressor;
    }
    
    /// <inheritdoc cref="CustomTeamHandler.OnRegistered"/>
    public override void OnRegistered()
    {
        base.OnRegistered();
        
        ArchangelsRadio.Internal_Init();

        Singleton = this;
        
        LoadoutPlugin.Ensure("Archangels1", new LoadoutDefinition()
            .WithHealth(250f, 250f)
            .WithAmmo(ItemType.Ammo762x39, 200)
            .WithItems(ItemType.GunLogicer, ItemType.KeycardChaosInsurgency, ItemType.GrenadeHE, ItemType.Medkit, ItemType.Adrenaline, ItemType.ArmorHeavy));
        
        LoadoutPlugin.Ensure("Archangels2", new LoadoutDefinition()
            .WithHealth(250f, 250f)
            .WithAmmo(ItemType.Ammo762x39, 200)
            .WithItems(ItemType.GunLogicer, ItemType.Jailbird, ItemType.KeycardChaosInsurgency, ItemType.GrenadeHE, ItemType.Medkit, ItemType.Adrenaline, ItemType.ArmorHeavy));
        
        LoadoutPlugin.Ensure("Archangels3",  new LoadoutDefinition()
            .WithHealth(250f, 250f)
            .WithAmmo(ItemType.Ammo556x45, 120)
            .WithAmmo(ItemType.GunCOM18, 120)
            .WithItem(ItemType.GunE11SR, "SniperRifle")
            .WithItems(ItemType.GunCOM18, ItemType.KeycardChaosInsurgency, ItemType.Medkit, ItemType.Adrenaline, ItemType.ArmorHeavy));
        
        ApiLog.Debug("Archangels Team", "Registered");
    }
}