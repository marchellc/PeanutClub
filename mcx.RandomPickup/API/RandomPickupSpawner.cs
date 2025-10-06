using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

using LabExtended.API;
using LabExtended.Events;
using LabExtended.Extensions;
using LabExtended.Utilities.Update;

using ProjectMER.Features;

using System.Diagnostics;

using UnityEngine;

namespace mcx.RandomPickup.API
{
    public static class RandomPickupSpawner
    {
        private static float spawnDelay = 0f;

        private static int spawnCount = 0;
        private static Stopwatch spawnStopwatch = new();

        /// <summary>
        /// Gets the name of the schematic used by the RandomPickup system.
        /// </summary>
        public static string SchematicName => RandomPickupCore.ConfigStatic.SchematicName;

        /// <summary>
        /// Gets or sets a value indicating whether the timer is paused.
        /// </summary>
        /// <remarks>Setting this property to <see langword="true"/> pauses the timer; setting it to <see
        /// langword="false"/> restarts the timer from zero. This property is static and affects the global timer
        /// state.</remarks>
        public static bool TimerPaused
        {
            get => !spawnStopwatch.IsRunning;
            set
            {
                if (value)
                    spawnStopwatch.Stop();
                else
                    spawnStopwatch.Restart();
            }
        }

        /// <summary>
        /// A list of all instances spawned this round.
        /// </summary>
        public static List<RandomPickupInstance> SpawnedInstances { get; } = new();

        /// <summary>
        /// A list of all who have been targeted by a random pickup along with the round number it happened in.
        /// </summary>
        public static Dictionary<string, int> PlayerSpawnCache { get; } = new();

        /// <summary>
        /// Gets called once a new instance is spawned.
        /// </summary>
        public static event Action<RandomPickupInstance>? Spawned;

        /// <summary>
        /// Gets called once an instance is destroyed.
        /// </summary>
        public static event Action<RandomPickupInstance>? Destroyed;

        /// <summary>
        /// Spawns a new instance of a random pickup at the specified position and rotation, optionally using a provided
        /// scenario.
        /// </summary>
        /// <param name="position">The world position where the random pickup instance will be spawned.</param>
        /// <param name="rotation">The rotation to apply to the spawned random pickup instance.</param>
        /// <param name="scenario">An optional scenario that defines the behavior or configuration of the spawned pickup. If null, the default
        /// scenario is used.</param>
        /// <returns>A RandomPickupInstance representing the newly spawned pickup.</returns>
        /// <exception cref="Exception">Thrown if the schematic could not be spawned at the specified position and rotation.</exception>
        public static RandomPickupInstance SpawnInstance(Vector3 position, Quaternion rotation, ExPlayer? triggerPlayer = null, RandomPickupScenario? scenario = null)
        {
            if (!ExRound.IsRunning)
                throw new Exception($"Pickups can be spawned only while the round is in progress!");

            if (!ObjectSpawner.TrySpawnSchematic(SchematicName, position, rotation, out var schematic))
                throw new Exception($"Could not spawn the schematic!");

            var instance = new RandomPickupInstance(schematic);

            instance.SpawnScenario = scenario;
            instance.TriggerPlayer = triggerPlayer;

            instance.Initialize();

            SpawnedInstances.Add(instance);
            Spawned?.Invoke(instance);

            if (scenario != null && spawnCount > 0)
            {
                spawnCount--;
                spawnDelay = RandomPickupCore.ConfigStatic.SpawnDelay.GetRandom();
            }

            spawnStopwatch.Restart();
            return instance;
        }

        private static void Internal_RoundStarted()
        {
            spawnCount = RandomPickupCore.ConfigStatic.SpawnCount.GetRandom();
            spawnDelay = RandomPickupCore.ConfigStatic.SpawnDelay.GetRandom();

            TimerPaused = false;
        }

        private static void Internal_RoundEnding()
        {
            TimerPaused = true;

            foreach (var instance in SpawnedInstances.ToArray())
                instance.Destroy();

            SpawnedInstances.Clear();
        }

        private static void Internal_SearchedToy(PlayerSearchedToyEventArgs args)
        {
            if (args.Player is not ExPlayer player)
                return;

            foreach (var instance in SpawnedInstances)
            {
                if (instance.Status is not RandomPickupStatus.Waiting)
                    continue;

                if (instance.Interactable?.Base == null)
                    continue;

                if (instance.Interactable.Base != args.Interactable.Base)
                    continue;

                instance.Internal_Interacted(player);
                break;
            }
        }

        private static void Internal_Update()
        {
            if (!ExRound.IsRunning)
                return;

            RandomPickupScenario.Internal_UpdateScenarios();

            if (!spawnStopwatch.IsRunning || spawnCount < 1)
                return;

            if (spawnStopwatch.Elapsed.TotalSeconds < spawnDelay)
                return;

            var targetPlayer = ExPlayer.Players.GetRandomItem(x =>
            {
                if (!x.Role.IsAlive)
                    return false;

                if (x.Role.IsScp && !RandomPickupCore.ConfigStatic.TargetScps)
                    return false;

                if (x.Role.IsTutorial)
                    return false;

                if (RandomPickupCore.ConfigStatic.MinimumRoundCount > 0
                    && PlayerSpawnCache.TryGetValue(x.UserId, out var roundNumber)
                    && (ExRound.RoundNumber - roundNumber) < RandomPickupCore.ConfigStatic.MinimumRoundCount)
                    return false;

                return true;
            });

            if (targetPlayer?.ReferenceHub == null)
            {
                spawnDelay = RandomPickupCore.ConfigStatic.SpawnDelay.GetRandom();
                return;
            }

            spawnCount--;
            spawnDelay = RandomPickupCore.ConfigStatic.SpawnDelay.GetRandom();

            SpawnInstance(targetPlayer.Position, targetPlayer.Rotation, targetPlayer);

            PlayerSpawnCache[targetPlayer.UserId] = ExRound.RoundNumber;
        }

        internal static void Internal_Destroyed(RandomPickupInstance instance)
        {
            Destroyed?.Invoke(instance);
        }

        internal static void Internal_Init()
        {
            PlayerUpdateHelper.OnUpdate += Internal_Update;

            ExRoundEvents.Ending += Internal_RoundEnding;
            ExRoundEvents.Started += Internal_RoundStarted;

            PlayerEvents.SearchedToy += Internal_SearchedToy;
        }
    }
}