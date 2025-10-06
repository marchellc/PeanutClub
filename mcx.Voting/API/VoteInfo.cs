using LabExtended.API;

using LabExtended.Core.Pooling;
using LabExtended.Core.Pooling.Pools;

namespace mcx.Voting.API
{
    /// <summary>
    /// Represents an active vote.
    /// </summary>
    public class VoteInfo : PoolObject
    {
        /// <summary>
        /// Gets a list of voters and their choices.
        /// </summary>
        public Dictionary<ExPlayer, int> Votes { get; } = new();

        /// <summary>
        /// Returns this instance to the object pool for reuse.
        /// </summary>
        public void ReturnToPool()
        {
            Votes.Clear();

            ObjectPool<VoteInfo>.Shared.Return(this);
        }
    }
}