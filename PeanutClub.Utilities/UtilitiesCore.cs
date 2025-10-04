namespace PeanutClub.Utilities;

using Items;

using LabApi.Features.Stores;

using LabExtended.API;
using LabExtended.Events;

using PeanutClub.Utilities.Audio;
using PeanutClub.Utilities.Features;

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

            ItemTags.Internal_Init();
            SnakeExplosion.Internal_Init();

            ExPlayerEvents.Joined += Internal_Joined;
            
            hasInitialized = true;
        }
    }

    private static void Internal_Joined(ExPlayer player)
        => player.GetDataStore<PlaybackStore>();
}