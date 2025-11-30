using LabExtended.API;
using LabExtended.Utilities;

using Newtonsoft.Json;

using SecretLabAPI.Levels;
using SecretLabAPI.Utilities;

using YamlDotNet.Serialization;

using System.ComponentModel;

using LabExtended.Core;

namespace SecretLabAPI.Actions.API
{
    /// <summary>
    /// Provides functionality for managing and executing groups of actions based on configurable selection criteria,
    /// including weighted probabilities and player-specific multipliers.
    /// </summary>
    public class ActionTable
    {
        /// <summary>
        /// Represents a parsed configuration group, including its name, weight, optional multipliers, and associated
        /// actions.
        /// </summary>
        public class ParsedConfig
        {
            /// <summary>
            /// Represents the name of the table group.
            /// </summary>
            public string Name;

            /// <summary>
            /// The base weight of the group.
            /// </summary>
            public float Weight;

            /// <summary>
            /// Player-specific multipliers group.
            /// </summary>
            public WeightMultipliers? Multipliers;

            /// <summary>
            /// List of actions to invoke.
            /// </summary>
            public List<CompiledAction> Actions;
        }

        /// <summary>
        /// Represents the configuration settings for an action table, including group identifiers, selection weights,
        /// and associated actions.
        /// </summary>
        public class ActionConfig
        {
            /// <summary>
            /// Gets or sets the group identifier associated with this action table.
            /// </summary>
            [Description("Sets the group identifier for this action table.")]
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the name of the weight multiplier group used for selection purposes.
            /// </summary>
            [Description("Sets the name of the weight multiplier group of this action table for selection purposes.")]
            public string Multipliers { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the base weight used to influence the selection probability of this action table.
            /// </summary>
            [Description("Sets the base weight of this action table for selection purposes.")]
            public float Weight { get; set; } = 1f;

            /// <summary>
            /// Gets or sets the list of actions associated with this action table.
            /// </summary>
            [Description("Defines the list of actions associated with this action table.")]
            public List<string> Actions { get; set; } = new();
        }

        /// <summary>
        /// Gets the source action table.
        /// </summary>
        public List<ActionConfig> Source { get; set; } = new() { new(), new() };

        /// <summary>
        /// Gets the cached list of actions.
        /// </summary>
        [YamlIgnore]
        [JsonIgnore]
        public List<ParsedConfig> Parsed { get; } = new();

        /// <summary>
        /// Selects a table based on weighted criteria and executes its associated actions for the specified players.
        /// </summary>
        /// <remarks>If no valid tables are available or no actions are defined for the selected table,
        /// the method returns false. The selection process uses player-specific weighting and optional group filtering
        /// to determine the most appropriate table.</remarks>
        /// <param name="players">The list of players for whom the table selection and actions will be performed. Must contain at least one
        /// player.</param>
        /// <param name="groupPredicate">An optional predicate used to filter tables by group name. If specified, only tables whose group names
        /// satisfy the predicate are considered for selection.</param>
        /// <returns>true if a table was successfully selected and its actions executed; otherwise, false.</returns>
        public bool SelectAndExecuteTable(List<ExPlayer> players, Predicate<string>? groupPredicate = null)
        {
            if (players == null || players.Count < 1)
                return false;

            CacheTables();

            if (Parsed.Count < 1)
                return false;

            var table = Parsed.GetRandomWeighted(x =>
            {
                return players.Average(p =>
                {
                    var weight = x.Weight;

                    switch (weight)
                    {
                        case <= 0f:
                            return 0f;
                        
                        case >= 100f:
                            return 100f;
                    }

                    if (groupPredicate != null && !groupPredicate(x.Name))
                        return 0f;

                    if (x.Multipliers != null)
                        weight = x.Multipliers.GetWeight(weight, p.UserId, p.PermissionsGroupName, p.GetLevel());

                    return weight;
                });
            });

            if (table.Actions?.Count < 1)
                return false;

            table.Actions.ExecuteActions(players);
            return true;
        }

        /// <summary>
        /// Selects a random weighted table for the specified player and executes its associated actions.
        /// </summary>
        /// <remarks>If no eligible tables are available or the player is invalid, the method returns
        /// false. The selection process uses table weights and may apply additional multipliers based on the player's
        /// attributes.</remarks>
        /// <param name="player">The player for whom the table selection and actions will be performed. Cannot be null and must have a valid
        /// ReferenceHub.</param>
        /// <param name="groupPredicate">An optional predicate used to filter tables by their group name. Only tables for which the predicate returns
        /// <see langword="true"/> are considered for selection. If null, all groups are eligible.</param>
        /// <returns>true if a table was successfully selected and its actions executed; otherwise, false.</returns>
        public bool SelectAndExecuteTable(ExPlayer player, Predicate<string>? groupPredicate = null)
        {
            if (player?.ReferenceHub == null)
                return false;

            CacheTables();

            if (Parsed.Count < 1)
                return false;

            var table = Parsed.GetRandomWeighted(x =>
            {
                var weight = x.Weight;

                if (weight <= 0f)
                    return 0f;

                if (weight >= 100f)
                    return 100f;

                if (groupPredicate != null && !groupPredicate(x.Name))
                    return 0f;

                if (x.Multipliers != null)
                    weight = x.Multipliers.GetWeight(weight, player.UserId, player.PermissionsGroupName, player.GetLevel());

                return weight;
            });

            return !(table.Actions?.Count < 1) && table.Actions.ExecuteActions(player);
        }

        /// <summary>
        /// Populates the internal cache of parsed action tables based on the current source data. Subsequent calls have
        /// no effect if the cache is already populated or if there are no source tables.
        /// </summary>
        public void CacheTables()
        {
            if (Source.Count < 1)
                return;

            if (Parsed.Count > 0)
                return;
            
            foreach (var src in Source)
            {
                if (string.IsNullOrEmpty(src.Name)) 
                    continue;

                if (src.Actions.Count < 1) 
                    continue;

                var list = new List<CompiledAction>();

                if (src.Actions.ParseActions(list))
                {
                    Parsed.Add(new()
                    {
                        Weight = src.Weight,
                        Name = src.Name,

                        Actions = list,

                        Multipliers = WeightMultipliers.Groups.TryGetValue(src.Multipliers, out var mult) 
                                                                                                    ? mult 
                                                                                                    : null
                    });

                    ApiLog.Info("ActionManager", $"Loaded table &3{src.Name}&r with &6{list.Count}&r action(s)!");
                }
                else
                {
                    ApiLog.Error("ActionManager", $"Could not load table &3{src.Name}&r!");
                }
            }
        }
    }
}