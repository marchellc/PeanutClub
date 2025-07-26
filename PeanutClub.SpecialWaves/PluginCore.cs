using LabApi.Loader.Features.Plugins;

using LabExtended.API.CustomTeams;

using PeanutClub.SpecialWaves.Weapons;

using PeanutClub.SpecialWaves.Waves.Archangels;
using PeanutClub.SpecialWaves.Waves.RedRightHand;
using PeanutClub.SpecialWaves.Waves.SerpentsHand;

using PeanutClub.SpecialWaves.Roles.Janitor;
using PeanutClub.SpecialWaves.Roles.GuardCommander;

namespace PeanutClub.SpecialWaves;

/// <summary>
/// The main plugin class.
/// </summary>
public class PluginCore : Plugin<PluginConfig>
{
    /// <summary>
    /// Gets the static singleton of the main plugin class.
    /// </summary>
    public static PluginCore Singleton { get; private set; }
    
    /// <summary>
    /// Gets the static instance of the plugin's config.
    /// </summary>
    public static PluginConfig StaticConfig { get; private set; }
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override string Name { get; } = "PeanutClub.SpecialWaves";

    /// <inheritdoc cref="Plugin.Name"/>
    public override string Author { get; } = "Peanut Club / marchellcx";

    /// <inheritdoc cref="Plugin.Name"/>
    public override string Description { get; } = "A plugin that adds special reinforcement waves.";
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override Version Version { get; } = new(1, 0, 0);

    /// <inheritdoc cref="Plugin.Name"/>
    public override Version RequiredApiVersion { get; } = null!;
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override void Enable()
    {
        Singleton = this;
        StaticConfig = Config!;
        
        SniperRifleHandler.Internal_Init();
        
        JanitorRole.Internal_Init();
        GuardCommanderRole.Internal_Init();

        CustomTeamRegistry.Register<ArchangelsTeam>();
        CustomTeamRegistry.Register<RedRightHandTeam>();
        CustomTeamRegistry.Register<SerpentsHandTeam>();
    }

    /// <inheritdoc cref="Plugin.Name"/>
    public override void Disable() { }

    /// <summary>
    /// Saves the plugin's config.
    /// </summary>
    public new static void SaveConfig()
    {
        (Singleton as Plugin<PluginConfig>).SaveConfig();
    }
}