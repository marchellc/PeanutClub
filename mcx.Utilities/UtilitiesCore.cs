namespace mcx.Utilities;

using Items;

using LabApi.Features.Stores;

using LabExtended.API;
using LabExtended.Events;
using LabExtended.Commands;

using mcx.Utilities.Audio;
using mcx.Utilities.Features;
using mcx.Utilities.Features.NextBots;

/// <summary>
/// The main class of this library.
/// </summary>
public static class UtilitiesCore
{
    private static bool hasInitialized;
    
    /// <summary>
    /// Must be called at least once.
    /// </summary>
    public static void Initialize()
    {
        if (!hasInitialized)
        {
            CustomDataStoreManager.RegisterStore<PlaybackStore>();

            ItemHandler.Internal_Init();
            SnakeExplosion.Internal_Init();
            NextBotInstance.Internal_Init();
            PlayerInfoHealth.Internal_Init();
            PersistentOverwatch.Internal_Init();

            ExPlayerEvents.Joined += Internal_Joined;

            typeof(UtilitiesCore).Assembly
                .RegisterCommands();
            
            hasInitialized = true;
        }
    }

    private static void Internal_Joined(ExPlayer player)
        => player.GetDataStore<PlaybackStore>();
}