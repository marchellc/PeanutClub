using LabApi.Loader.Features.Plugins;

namespace mcx.Voting
{
    /// <summary>
    /// Main class of the plugin.
    /// </summary>
    public class VotingCore : Plugin<VotingConfig>
    {
        /// <summary>
        /// Gets the static instance of the plugin.
        /// </summary>
        public static VotingCore PluginStatic { get; private set; }

        /// <summary>
        /// Gets the static instance of the plugin's config.
        /// </summary>
        public static VotingConfig ConfigStatic { get; private set; }

        /// <inheritdoc cref="Plugin.Name"/>
        public override string Name { get; } = "mcx.Voting";

        /// <inheritdoc cref="Plugin.Author"/>
        public override string Author { get; } = "marchellcx";

        /// <inheritdoc cref="Plugin.Description"/>
        public override string Description { get; } = "Adds the ability for server administrators to start a vote.";

        /// <inheritdoc cref="Plugin.Version"/>
        public override Version Version { get; } = null!;

        /// <inheritdoc cref="Plugin.RequiredApiVersion"/>
        public override Version RequiredApiVersion { get; } = null!;

        /// <inheritdoc cref="Plugin.Enable"/>
        public override void Enable()
        {
            PluginStatic = this;
            ConfigStatic = Config!;
        }

        /// <inheritdoc cref="Plugin.Disable"/>
        public override void Disable()
        {

        }
    }
}