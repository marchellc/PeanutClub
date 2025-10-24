using LabApi.Loader.Features.Plugins;

using mcx.Levels.API;

namespace mcx.Levels;

/// <summary>
/// The main class of the plugin.
/// </summary>
public class LevelsPlugin : Plugin<LevelsConfig>
{
    /// <summary>
    /// Gets the static singleton of the main plugin class.
    /// </summary>
    public static LevelsPlugin Singleton { get; private set; }
    
    /// <summary>
    /// Gets the static instance of the plugin's config.
    /// </summary>
    public static LevelsConfig StaticConfig { get; private set; }

    /// <inheritdoc cref="Plugin.Name"/>
    public override string Name { get; } = "mcx.Levels";

    /// <inheritdoc cref="Plugin.Name"/>
    public override string Author { get; } = "marchellcx";

    /// <inheritdoc cref="Plugin.Name"/>
    public override string Description { get; } = "A plugin that adds a level system.";
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override Version Version { get; } = new(1, 0, 0);

    /// <inheritdoc cref="Plugin.Name"/>
    public override Version RequiredApiVersion { get; } = null!;
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override void Enable()
    {
        Singleton = this;
        StaticConfig = Config!;

        LevelManager.Initialize();
    }

    /// <inheritdoc cref="Plugin.Name"/>
    public override void Disable() { }

    /// <summary>
    /// Saves the plugin's config.
    /// </summary>
    public new static void SaveConfig()
        => (Singleton as Plugin<LevelsConfig>)?.SaveConfig();
}