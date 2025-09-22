using LabApi.Loader.Features.Plugins;

using PeanutClub.Items.Spawning;
using PeanutClub.Items.Stacking;
using PeanutClub.Items.Weapons;
using PeanutClub.Items.Weapons.AirsoftGun;
using PeanutClub.Items.Weapons.SniperRifle;

namespace PeanutClub.Items;

/// <summary>
/// The main class of the plugin.
/// </summary>
public class ItemsCore : Plugin<ItemsConfig>
{
    /// <summary>
    /// Gets the static instance of the plugin.
    /// </summary>
    public static ItemsCore PluginStatic { get; private set; }
    
    /// <summary>
    /// Gets the static instance of the plugin's config.
    /// </summary>
    public static ItemsConfig ConfigStatic { get; private set; }
    
    /// <inheritdoc cref="Plugin.Name"/>
    public override string Name { get; } = "PeanutClub.Items";

    /// <inheritdoc cref="Plugin.Author"/>
    public override string Author { get; } = "Peanut Club / marchellcx";

    /// <inheritdoc cref="Plugin.Description"/>
    public override string Description { get; } = "Plugin that adds custom items.";

    /// <inheritdoc cref="Plugin.Version"/>
    public override Version Version { get; } = null!;

    /// <inheritdoc cref="Plugin.RequiredApiVersion"/>
    public override Version RequiredApiVersion { get; } = null!;
    
    /// <inheritdoc cref="Plugin.Enable"/>
    public override void Enable()
    {
        PluginStatic = this;
        ConfigStatic = Config!;

        CustomFirearmHandler.Internal_Init();

        AirsoftGunHandler.Internal_Init();
        SniperRifleHandler.Internal_Init();

        ItemStacker.Internal_Init();

        SpawnPositions.Internal_Init();
        SpawnPrevention.Internal_Init();
    }

    /// <inheritdoc cref="Plugin.Disable"/>
    public override void Disable()
    {

    }
}