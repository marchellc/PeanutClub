using LabApi.Loader.Features.Plugins;

using PeanutClub.RandomPickup.API;

namespace PeanutClub.RandomPickup;

/// <summary>
/// The main class of the plugin.
/// </summary>
public class RandomPickupCore : Plugin<RandomPickupConfig>
{
    /// <summary>
    /// Gets the static instance of the plugin.
    /// </summary>
    public static RandomPickupCore PluginStatic { get; private set; }

    /// <summary>
    /// Gets the static instance of the plugin's config.
    /// </summary>
    public static RandomPickupConfig ConfigStatic { get; private set; }

    /// <inheritdoc cref="Plugin.Name"/>
    public override string Name { get; } = "PeanutClub.RandomPickup";

    /// <inheritdoc cref="Plugin.Author"/>
    public override string Author { get; } = "Peanut Club / marchellcx";

    /// <inheritdoc cref="Plugin.Description"/>
    public override string Description { get; } = "Plugin that adds a random pickup.";

    /// <inheritdoc cref="Plugin.Version"/>
    public override Version Version { get; } = null!;

    /// <inheritdoc cref="Plugin.RequiredApiVersion"/>
    public override Version RequiredApiVersion { get; } = null!;

    /// <inheritdoc cref="Plugin.Enable"/>
    public override void Enable()
    {
        PluginStatic = this;
        ConfigStatic = Config!;

        RandomPickupSpawner.Internal_Init();
    }

    /// <inheritdoc cref="Plugin.Disable"/>
    public override void Disable()
    {

    }
}