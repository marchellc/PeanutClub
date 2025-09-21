using LabApi.Loader.Features.Plugins;

using PeanutClub.Overlays.Alerts;

namespace PeanutClub.Overlays
{
    /// <summary>
    /// Main class of the plugin.
    /// </summary>
    public class OverlayCore : Plugin<OverlayConfig>
    {
        /// <summary>
        /// Gets the static instance of the plugin.
        /// </summary>
        public static OverlayCore PluginStatic { get; private set; }

        /// <summary>
        /// Gets the static instance of the plugin's config.
        /// </summary>
        public static OverlayConfig ConfigStatic { get; private set; }

        /// <inheritdoc cref="Plugin.Name"/>
        public override string Name { get; } = "PeanutClub.Overlays";

        /// <inheritdoc cref="Plugin.Author"/>
        public override string Author { get; } = "Peanut Club / marchellcx";

        /// <inheritdoc cref="Plugin.Description"/>
        public override string Description { get; } = "Custom hint overlays.";

        /// <inheritdoc cref="Plugin.Version"/>
        public override Version Version { get; } = null!;

        /// <inheritdoc cref="Plugin.RequiredApiVersion"/>
        public override Version RequiredApiVersion { get; } = null!;

        /// <inheritdoc cref="Plugin.Enable"/>
        public override void Enable()
        {
            PluginStatic = this;
            ConfigStatic = Config!;

            AlertElement.Internal_Init();
        }

        /// <inheritdoc cref="Plugin.Disable"/>
        public override void Disable()
        {

        }
    }
}