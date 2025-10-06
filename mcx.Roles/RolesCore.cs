using LabApi.Loader.Features.Plugins;

using mcx.Roles.GuardCommander;
using mcx.Roles.Janitor;

namespace mcx.Roles;

/// <summary>
/// The main class of the Roles plugin.
/// </summary>
public class RolesCore : Plugin<RolesConfig>
{
    /// <summary>
    /// Gets the static instance of the plugin.
    /// </summary>
    public static RolesCore PluginStatic { get; private set; }
    
    /// <summary>
    /// Gets the static instance of the plugin's config.
    /// </summary>
    public static RolesConfig ConfigStatic { get; private set; }
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override string Name { get; } = "mcx.Roles";

    /// <inheritdoc cref="Plugin.Author"/>
    public override string Author { get; } = "marchellcx";

    /// <inheritdoc cref="Plugin.Description"/>
    public override string Description { get; } = "Plugin that adds custom roles.";

    /// <inheritdoc cref="Plugin.Version"/>
    public override Version Version { get; } = null!;

    /// <inheritdoc cref="Plugin.RequiredApiVersion"/>
    public override Version RequiredApiVersion { get; } = null!;
    
    /// <inheritdoc cref="Plugin.Enable"/>
    public override void Enable()
    {
        PluginStatic = this;
        ConfigStatic = Config!;
        
        JanitorHandler.Internal_Init();
        GuardCommanderHandler.Internal_Init();
    }

    /// <inheritdoc cref="Plugin.Disable"/>
    public override void Disable()
    {

    }
}