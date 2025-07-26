namespace PeanutClub.Utilities;

using Items;

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
            ItemTags.Internal_Init();
            
            hasInitialized = true;
        }
    }
}