using LabApi.Features.Wrappers;

using LabExtended.API;
using LabExtended.Events;

namespace mcx.Utilities.Audio
{
    /// <summary>
    /// Used for playback of player clips.
    /// </summary>
    public static class PlayerClips
    {
        /// <summary>
        /// Gets the configuration for player clips.
        /// </summary>
        public static ClipConfig<string> Config => UtilitiesCore.Config.PlayerClips;

        /// <summary>
        /// Gets all spawned clip managers for players.
        /// </summary>
        public static Dictionary<string, ClipManager<string>> Managers { get; } = new();

        /// <summary>
        /// Whether or not a player has a clip playing.
        /// </summary>
        public static bool IsPlayingClip(this Player player)
        {
            if (player?.ReferenceHub == null
                || player.UserId is null)
                return false;

            if (!Managers.TryGetValue(player.UserId, out var manager))
                return false;

            return manager.IsPlaying && !manager.IsPaused;
        }

        /// <summary>
        /// Stops a player's currently playing clip.
        /// </summary>
        public static bool StopClip(this Player player, out ClipDefinition? playedClip)
        {
            playedClip = null;

            if (player?.ReferenceHub == null
                || player.UserId is null)
                return false;

            if (!Managers.TryGetValue(player.UserId, out var manager))
                return false;

            playedClip = manager.CurrentClip;

            manager.Stop();
            return true;
        }

        /// <summary>
        /// Starts playing a random clip for a player.
        /// </summary>
        /// <param name="player">The player to play the clip for.</param>
        /// <param name="clipName">The name of the clip.</param>
        /// <param name="volume">The volue of the clip.</param>
        /// <param name="isPersonal">Whether or not the clip should be played only at the player's position.</param>
        /// <param name="sendToOthers">Whether or not the clip's audio should be sent to other players in proximity.</param>
        /// <returns>true if a clip started playing</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool PlayClip(this Player player, string clipName, float volume = 1f, bool isPersonal = true, bool sendToOthers = true)
        {
            if (string.IsNullOrWhiteSpace(clipName))
                throw new ArgumentNullException(nameof(clipName));

            if (player?.ReferenceHub == null
                || player.UserId is null)
                return false;

            var manager = player.GetManager();

            if (isPersonal)
                manager.SetPersonal(player, sendToOthers);
            else
                manager.SetGlobal();

            manager.Volume = volume;
            return manager.PlayRandomClip(clipName);
        }

        /// <summary>
        /// Gets or creates a clip manager for a player.
        /// </summary>
        public static ClipManager<string> GetManager(this Player player)
        {
            if (Managers.TryGetValue(player.UserId, out var manager))
                return manager;

            manager = new(Config, player.Camera);

            Managers[player.UserId] = manager;
            return manager;
        }

        private static void Left(ExPlayer player)
        {
            if (Managers.TryGetValue(player.UserId, out var manager))
            {
                manager.Destroy();

                Managers.Remove(player.UserId);
            }
        }

        private static void WaitingForPlayers()
        {
            if (Managers.Count > 0)
            {
                foreach (var manager in Managers.ToList())
                    manager.Value.Destroy();

                Managers.Clear();
            }
        }

        internal static void Initialize()
        {
            ExPlayerEvents.Left += Left;

            ExRoundEvents.WaitingForPlayers += WaitingForPlayers;
        }
    }
}