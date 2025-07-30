using LabApi.Loader.Features.Plugins;

using LabExtended.API.CustomTeams;
using LabExtended.Attributes;

using PeanutClub.Teams.Archangels;
using PeanutClub.Teams.RedRightHand;
using PeanutClub.Teams.SerpentsHand;

namespace PeanutClub.Teams;

/// <summary>
/// The main plugin class.
/// </summary>
[LoaderPatch]
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
    public override string Name { get; } = "PeanutClub.Teams";

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

        CustomTeamRegistry.Register<ArchangelsTeam>();
        CustomTeamRegistry.Register<RedRightHandTeam>();
        CustomTeamRegistry.Register<SerpentsHandTeam>();
    }

    /// <inheritdoc cref="Plugin.Name"/>
    public override void Disable() { }
}