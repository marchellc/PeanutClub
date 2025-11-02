using LabExtended.API;
using LabExtended.Core;
using LabExtended.Utilities;

using mcx.Utilities.Actions.Interfaces;
using mcx.Utilities.Actions.Targets;

using System.ComponentModel;

namespace mcx.Utilities.Actions
{
    /// <summary>
    /// Represents the method that will handle an event when the weight of a collecting group is being calculated.
    /// </summary>
    /// <param name="group">The group of items being collected.</param>
    /// <param name="source">The source of the action that triggered the collection.</param>
    /// <param name="player">The player associated with the collection action.</param>
    /// <param name="weight">The weight of the group, which can be modified by the event handler.</param>
    public delegate void CollectingGroupWeightEventHandler(ActionLoot.Group group, IActionSource source, ExPlayer player, ref float weight);

    /// <summary>
    /// Actions acting as loot.
    /// </summary>
    public static class ActionLoot
    {
        /// <summary>
        /// Occurs when the system begins collecting the weight of a group.
        /// </summary>
        /// <remarks>This event is triggered during the process of collecting and calculating the weight
        /// of a group.  Subscribers can use this event to perform custom actions or monitor the weight collection
        /// process.</remarks>
        public static event CollectingGroupWeightEventHandler? CollectingGroupWeight;

        /// <summary>
        /// A group of loot actions.
        /// </summary>
        public class Group
        {
            /// <summary>
            /// Gets or sets the multipliers applied to the group's weight per-player.
            /// </summary>
            [Description("The multipliers applied to the group's weight per-player.")]
            public Dictionary<string, float> Multipliers { get; set; } = new();

            /// <summary>
            /// Gets or sets the multipliers applied to the group's weight per level.
            /// </summary>
            [Description("The multipliers applied to the group's weight per level.")]
            public Dictionary<int, float> LevelMultipliers { get; set; } = new();

            /// <summary>
            /// Gets or sets the base weight of the group.
            /// </summary>
            [Description("The base weight of the group.")]
            public float Weight { get; set; } = 0f;

            /// <summary>
            /// Gets or sets the list of actions to trigger if this group is selected.
            /// </summary>
            [Description("The list of actions to trigger if this group is selected.")]
            public List<ActionInfo> Actions { get; set; } = new();

            /// <summary>
            /// Calculates the weight for a user based on their unique identifier and optional rank.
            /// </summary>
            /// <remarks>If no multipliers are found for the provided user ID or rank, the base weight
            /// remains unchanged.</remarks>
            /// <param name="userId">The unique identifier of the user. This is used to retrieve the user's specific multiplier.</param>
            /// <param name="userRank">The optional rank of the user. If provided, it is used to retrieve an additional multiplier.</param>
            /// <returns>The calculated weight as a <see cref="float"/>. The base weight is adjusted by any applicable
            /// multipliers associated with the user ID and rank.</returns>
            public float GetWeight(string userId, string? userRank)
            {
                var weight = Weight;

                if (Multipliers.TryGetValue(userId, out var userMultiplier))
                    weight *= userMultiplier;

                if (userRank != null && Multipliers.TryGetValue(userRank, out var rankMultiplier))
                    weight *= rankMultiplier;

                return weight;
            }
        }

        /// <summary>
        /// Attempts to trigger a weighted action for the specified target player based on the provided groups and
        /// action source.
        /// </summary>
        /// <param name="groups">A dictionary mapping action source identifiers to lists of groups. Cannot be <see langword="null"/>.</param>
        /// <param name="source">The action source used to identify the relevant group. Cannot be <see langword="null"/>.</param>
        /// <param name="target">The target player for whom the action is triggered. The player's <c>ReferenceHub</c> must not be <see
        /// langword="null"/>.</param>
        /// <returns><see langword="true"/> if a weighted action was successfully triggered for the target player; otherwise,
        /// <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="groups"/> is <see langword="null"/>, or if <paramref name="target"/> is <see
        /// langword="null"/> or has a <c>ReferenceHub</c> that is <see langword="null"/>.</exception>
        public static bool TriggerWeighted(this Dictionary<string, List<Group>> groups, IActionSource source, ExPlayer target)
        {
            if (groups is null)
                throw new ArgumentNullException(nameof(groups));
            if (target?.ReferenceHub == null)
                throw new ArgumentNullException(nameof(target), "Target player cannot be null.");

            if (!groups.TryGetValue(source.Id, out var groupList))
                return false;

            return groupList.TriggerWeighted(target);
        }

        /// <summary>
        /// Triggers a weighted action for the specified groups based on the provided source and targets.
        /// </summary>
        /// <param name="groups">A dictionary where the key is a string identifier and the value is a list of <see cref="Group"/> objects.</param>
        /// <param name="source">The source of the action, used to identify the relevant group.</param>
        /// <param name="targets">A collection of <see cref="ExPlayer"/> objects representing the targets of the action.</param>
        /// <returns><see langword="true"/> if the action was successfully triggered for the relevant group; otherwise, <see
        /// langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="groups"/> or <paramref name="targets"/> is <see langword="null"/>.</exception>
        public static bool TriggerWeighted(this Dictionary<string, List<Group>> groups, IActionSource source, IEnumerable<ExPlayer> targets)
        {
            if (groups is null)
                throw new ArgumentNullException(nameof(groups));

            if (targets is null)
                throw new ArgumentNullException(nameof(targets));

            if (!groups.TryGetValue(source.Id, out var groupList))
                return false;

            return groupList.TriggerWeighted(targets);
        }

        /// <summary>
        /// Selects a group from the provided collection based on weighted probabilities and triggers its actions for
        /// the specified target player.
        /// </summary>
        /// <remarks>The method uses a weighted random selection to determine which group to trigger. The
        /// weight for each group is calculated using the target player's user ID and permissions group name.</remarks>
        /// <param name="groups">The collection of groups to evaluate. Each group's weight is determined dynamically based on the target
        /// player's attributes.</param>
        /// <param name="target">The target player for whom the group's actions will be triggered. Cannot be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a group was successfully selected and its actions triggered; otherwise, <see
        /// langword="false"/> if no group was selected.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="groups"/> is <see langword="null"/> or if <paramref name="target"/> is <see
        /// langword="null"/> or has a <see langword="null"/> <c>ReferenceHub</c>.</exception>
        public static bool TriggerWeighted(this IEnumerable<Group> groups, ExPlayer target)
        {
            if (groups is null)
                throw new ArgumentNullException(nameof(groups));

            if (target?.ReferenceHub == null)
                throw new ArgumentNullException(nameof(target), "Target player cannot be null.");

            ApiLog.Debug("ActionLoot", $"Selecting loot for player {target.ToLogString()}");

            var targetGroup = groups.GetRandomWeighted(group =>
            {
                var weight = group.GetWeight(target.UserId, target.PermissionsGroupName);

                ApiLog.Debug("ActionLoot", $"Base group weight &3{group.Weight}&r");

                CollectingGroupWeight?.Invoke(group, null!, target, ref weight);

                ApiLog.Debug("ActionLoot", $"Group weight &3{weight}&r");
                return weight;
            });

            if (targetGroup == null)
            {
                ApiLog.Debug("ActionLoot", $"Group is null");
                return false;
            }

            ApiLog.Debug("ActionLoot", $"Triggering group");
            return targetGroup.Actions.TriggerMany(null!, new TargetPlayer(target));
        }

        /// <summary>
        /// Selects a random group based on weighted criteria and triggers its actions for the specified targets.
        /// </summary>
        /// <remarks>This method uses a weighted random selection to determine the target group. The
        /// weight for each group is calculated using the group's <see cref="Group.GetWeight(string, object?)"/> method.
        /// If no group is selected, the method returns <see langword="false"/>.</remarks>
        /// <param name="groups">The collection of groups to evaluate. Cannot be <see langword="null"/>.</param>
        /// <param name="source">The source of the action being triggered.</param>
        /// <param name="targets">The collection of players to target. Cannot be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if a group was successfully selected and its actions triggered; otherwise, <see
        /// langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="groups"/> or <paramref name="targets"/> is <see langword="null"/>.</exception>
        public static bool TriggerWeighted(this IEnumerable<Group> groups, IEnumerable<ExPlayer> targets)
        {
            if (groups is null)
                throw new ArgumentNullException(nameof(groups));

            if (targets is null)
                throw new ArgumentNullException(nameof(targets));

            var targetGroup = groups.GetRandomWeighted(group => group.GetWeight(string.Empty, null));

            if (targetGroup == null)
                return false;

            var targetPlayers = targets.Select(target => (IActionTarget)new TargetPlayer(target));
            return targetGroup.Actions.TriggerMany(null!, targetPlayers);
        }
    }
}