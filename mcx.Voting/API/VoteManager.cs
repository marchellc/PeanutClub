using LabExtended.Core.Pooling.Pools;

namespace mcx.Voting.API
{
    /// <summary>
    /// Manages active votes.
    /// </summary>
    public class VoteManager
    {
        /// <summary>
        /// Gets the current active vote, or null if there is no active vote.
        /// </summary>
        public VoteInfo? CurrentVote { get; private set; }

        private static VoteInfo GetVoteInstance()
            => ObjectPool<VoteInfo>.Shared.Rent(null!, () => new());
    }
}