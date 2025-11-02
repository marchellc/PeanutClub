namespace mcx.Utilities;

using mcx.Utilities.Audio;
using mcx.Utilities.Textures;
using mcx.Utilities.Features;

using LabApi.Loader.Features.Plugins;

using System;
using mcx.Utilities.Actions;

/// <summary>
/// The main class of this library.
/// </summary>
public class UtilitiesCore : Plugin<UtilitiesConfig>
{
    /// <summary>
    /// Gets an instance of this plugin.
    /// </summary>
    public static UtilitiesCore Plugin { get; private set; }

    /// <summary>
    /// Gets an instance of the configuration of this plugin.
    /// </summary>
    public static new UtilitiesConfig Config { get; private set; }

    /// <inheritdoc/>
    public override string Name { get; } = "mcx.Utilities";

    /// <inheritdoc/>
    public override string Author { get; } = "marchellcx";

    /// <inheritdoc/>
    public override string Description { get; } = "Utilities for plugins in the mcx project.";

    /// <inheritdoc/>
    public override Version Version { get; } = new(1, 0, 0);

    /// <inheritdoc/>
    public override Version RequiredApiVersion { get; }

    /// <inheritdoc/>
    public override void Enable()
    {
        Config = base.Config!;
        Plugin = this;

        ActionManager.Initialize();
        TextureManager.Initialize();
        PlaybackUtils.Initialize();

        PlayerClips.Initialize();
        SnakeExplosion.Internal_Init();
        PlayerInfoHealth.Internal_Init();
        PersistentOverwatch.Internal_Init();
    }

    /// <inheritdoc/>
    public override void Disable()
    {

    }
}