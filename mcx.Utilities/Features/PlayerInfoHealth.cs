using LabExtended.API;
using LabExtended.API.Collections.Unsafe;

using LabExtended.Events;

using LabExtended.Utilities.Update;

namespace mcx.Utilities.Features
{
    /// <summary>
    /// Tracks player's health and updates their custom info display accordingly.
    /// </summary>
    public static class PlayerInfoHealth
    {
        private static PlayerUpdateComponent updateComponent;

        /// <summary>
        /// An internal class that tracks a player's health and updates their custom info display.
        /// </summary>
        public class PlayerInfoHealthTracker
        {
            /// <summary>
            /// Gets the tracked player.
            /// </summary>
            public ExPlayer Target { get; }

            /// <summary>
            /// Gets or sets the target's health in the previous frame.
            /// </summary>
            public float PreviousHealth { get; set; }

            /// <summary>
            /// Gets or sets the previous line added to the player's custom info.
            /// </summary>
            public string? PreviousLine { get; set; } = null;

            /// <summary>
            /// Initializes a new instance of the PlayerInfoHealthTracker class for the specified player.
            /// </summary>
            /// <param name="target">The player whose health information will be tracked. Cannot be null.</param>
            public PlayerInfoHealthTracker(ExPlayer target)
            {
                Target = target;
            }

            /// <inheritdoc/>
            public void OnUpdate()
            {
                if (Target?.ReferenceHub == null)
                    return;

                if (Target.Role.IsAlive && !Target.Role.IsScp)
                {
                    if (Target.Stats.CurHealth == PreviousHealth)
                        return;

                    PreviousHealth = Target.Stats.CurHealth;

                    var line = $"{PreviousHealth} HP / {Target.Stats.MaxHealth} HP";

                    if (string.IsNullOrEmpty(Target.CustomInfo))
                    {
                        Target.CustomInfo = $"\n{line}";

                        PreviousLine = line;
                    }
                    else
                    {
                        if (PreviousLine != null)
                        {
                            Target.CustomInfo = Target.CustomInfo.Replace($"\n{PreviousLine}", string.Empty)
                                                                 .Replace(PreviousLine, string.Empty);
                        }

                        Target.CustomInfo += $"\n{line}";

                        PreviousLine = line;
                    }

                    if ((Target.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
                    {
                        Target.InfoArea |= PlayerInfoArea.CustomInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of active trackers.
        /// </summary>
        public static UnsafeList<PlayerInfoHealthTracker> Trackers { get; } = new();

        private static void Internal_Joined(ExPlayer player)
        {
            Trackers.Add(new(player));
        }

        private static void Internal_Left(ExPlayer player)
        {
            Trackers.RemoveAll(x => x.Target == player);
        }

        private static void Internal_Waiting()
        {
            Trackers.Clear();
        }

        private static void Internal_Update()
        {
            for (var x = 0; x < Trackers.Count; x++)
            {
                Trackers[x]?.OnUpdate();
            }
        }

        internal static void Internal_Init()
        {
            updateComponent = PlayerUpdateComponent.Create();
            updateComponent.OnUpdate += Internal_Update;

            ExPlayerEvents.Left += Internal_Left;

            ExPlayerEvents.Verified += Internal_Joined;
            ExPlayerEvents.NpcJoined += Internal_Joined;

            ExRoundEvents.WaitingForPlayers += Internal_Waiting;
        }
    }
}