namespace mcx.Utilities;

using Items;

using mcx.Utilities.Features;
using mcx.Utilities.Features.NextBots;

using LabApi.Loader.Features.Plugins;

using System;

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

        ItemHandler.Internal_Init();
        SnakeExplosion.Internal_Init();
        NextBotInstance.Internal_Init();
        PlayerInfoHealth.Internal_Init();
        PersistentOverwatch.Internal_Init();
    }

    /// <inheritdoc/>
    public override void Disable()
    {

    }
}