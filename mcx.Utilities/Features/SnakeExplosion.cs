using LabExtended.Events;
using LabExtended.Events.Player.Snake;

namespace mcx.Utilities.Features
{
    /// <summary>
    /// Makes the player playing the Snake minigame explode upon death.
    /// </summary>
    public static class SnakeExplosion
    {
        private static void Internal_SnakeGameOver(PlayerSnakeGameOverEventArgs args)
        {
            ExplosionEffects.Explode(args.Player, ItemType.GrenadeHE, "Game Over", true);
        }

        internal static void Internal_Init()
        {
            ExPlayerEvents.SnakeGameOver += Internal_SnakeGameOver;
        }
    }
}