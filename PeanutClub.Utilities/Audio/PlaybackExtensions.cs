using LabApi.Features.Wrappers;

using LabExtended.Extensions;

namespace PeanutClub.Utilities.Audio
{
    /// <summary>
    /// Extensions for audio playback.
    /// </summary>
    public static class PlaybackExtensions
    {
        /// <summary>
        /// Pauses or resumes audio playback for the specified player.
        /// </summary>
        /// <param name="player">The player instance whose audio playback state will be modified. Cannot be null.</param>
        /// <param name="paused">A value indicating whether audio playback should be paused. Specify <see langword="true"/> to pause audio;
        /// <see langword="false"/> to resume playback.</param>
        public static void SetAudioPaused(this Player player, bool paused)
            => player.AccessPlayback(store => store.IsPaused = paused);

        /// <summary>
        /// Sets the playback volume for the specified player instance.
        /// </summary>
        /// <param name="player">The player whose audio volume will be adjusted. Cannot be null.</param>
        /// <param name="volume">The desired audio volume level. Must be between 0.0 (muted) and 1.0 (maximum volume). Values outside this
        /// range may be clamped.</param>
        public static void SetAudioVolume(this Player player, float volume)
            => player.AccessPlayback(store => store.Volume = volume);

        /// <summary>
        /// Stops audio playback for the specified player.
        /// </summary>
        /// <param name="player">The player instance whose audio playback will be stopped. Cannot be null.</param>
        public static void StopAudio(this Player player)
            => player.AccessPlayback(store => store.Stop());

        /// <summary>
        /// Plays the specified audio clip for the given player, optionally looping the clip until stopped.
        /// </summary>
        /// <param name="player">The player instance on which to play the audio clip. Cannot be null.</param>
        /// <param name="clipName">The name of the audio clip to play. Must correspond to a valid, loaded clip.</param>
        /// <param name="loopClip">Indicates whether the audio clip should loop continuously until stopped. Specify <see langword="true"/> to
        /// loop the clip; otherwise, <see langword="false"/>.</param>
        public static void PlayAudioClip(this Player player, string clipName, bool loopClip = false)
            => player.AccessPlayback(store => store.Play(clipName, loopClip));

        /// <summary>
        /// Invokes the specified action, providing access to the player's playback data store.
        /// </summary>
        /// <remarks>Use this method to safely interact with a player's playback data store within the
        /// provided action. The action receives the current <see cref="PlaybackStore"/> associated with the
        /// player.</remarks>
        /// <param name="player">The player instance whose playback data store will be accessed. Cannot be null.</param>
        /// <param name="action">An action to perform on the player's playback data store. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="player"/> or <paramref name="action"/> is null.</exception>
        public static void AccessPlayback(this Player player, Action<PlaybackStore> action)
        {
            if (player == null) 
                throw new ArgumentNullException(nameof(player));

            if (action == null) 
                throw new ArgumentNullException(nameof(action));

            action.InvokeSafe(player.GetDataStore<PlaybackStore>());
        }
    }
}