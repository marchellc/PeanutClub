using LabApi.Loader.Features.Plugins;

using PeanutClub.Dealer.API;

namespace PeanutClub.Dealer
{
    /// <summary>
    /// Main class of the plugin.
    /// </summary>
    public class DealerCore : Plugin<DealerConfig>
    {
        /// <summary>
        /// Gets the static instance of the plugin.
        /// </summary>
        public static DealerCore PluginStatic { get; private set; }

        /// <summary>
        /// Gets the static instance of the plugin's config.
        /// </summary>
        public static DealerConfig ConfigStatic { get; private set; }

        /// <inheritdoc cref="Plugin.Name"/>
        public override string Name { get; } = "PeanutClub.Dealer";

        /// <inheritdoc cref="Plugin.Author"/>
        public override string Author { get; } = "Peanut Club / marchellcx";

        /// <inheritdoc cref="Plugin.Description"/>
        public override string Description { get; } = "Plugin that adds a dealer.";

        /// <inheritdoc cref="Plugin.Version"/>
        public override Version Version { get; } = null!;

        /// <inheritdoc cref="Plugin.RequiredApiVersion"/>
        public override Version RequiredApiVersion { get; } = null!;

        /// <inheritdoc cref="Plugin.Enable"/>
        public override void Enable()
        {
            PluginStatic = this;
            ConfigStatic = Config!;

            DealerManager.Internal_Init();
        }

        /// <inheritdoc cref="Plugin.Disable"/>
        public override void Disable()
        {

        }
    }
}