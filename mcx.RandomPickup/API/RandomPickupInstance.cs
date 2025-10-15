using AdminToys;

using LabExtended.API;
using LabExtended.API.Toys;

using LabExtended.Core;
using LabExtended.Extensions;

using mcx.Utilities.Audio;
using mcx.Utilities.Items;

using NorthwoodLib.Pools;

using ProjectMER.Features.Objects;

using UnityEngine;

namespace mcx.RandomPickup.API
{
    /// <summary>
    /// A spawned instance of a random pickup.
    /// </summary>
    public class RandomPickupInstance
    {
        private PlaybackHandle? playbackHandle;

        /// <summary>
        /// Gets the status of this random pickup instance.
        /// </summary>
        public RandomPickupStatus Status { get; private set; } = RandomPickupStatus.NotInitialized;

        /// <summary>
        /// Gets the reason of this instance spawning.
        /// </summary>
        public RandomPickupSpawnReason SpawnReason { get; internal set; }

        /// <summary>
        /// Gets the scenario that spawned this random pickup instance, if any.
        /// </summary>
        public RandomPickupScenario? SpawnScenario { get; internal set; }

        /// <summary>
        /// Gets the player who triggered this random pickup instance, if any.
        /// </summary>
        public ExPlayer? TriggerPlayer { get; internal set; }

        /// <summary>
        /// Gets the component rotating this random pickup instance.
        /// </summary>
        public RandomPickupRotation Rotation { get; private set; }

        /// <summary>
        /// Gets the target spawned schematic.
        /// </summary>
        public SchematicObject Schematic { get; private set; }

        /// <summary>
        /// Gets the light toy.
        /// </summary>
        public LightToy? Light { get; private set; }

        /// <summary>
        /// Gets the interactable toy.
        /// </summary>
        public InteractableToy Interactable { get; private set; }

        /// <summary>
        /// Gets or sets the loot table used by this random pickup instance.
        /// </summary>
        public ItemLoot Loot { get; set; }

        /// <summary>
        /// Gets or sets custom data of the scenario that spawned this random pickup instance, if any.
        /// </summary>
        public object ScenarioData { get; set; }

        /// <summary>
        /// Initializes a new instance of the RandomPickupInstance class.
        /// </summary>
        public RandomPickupInstance(SchematicObject schematic)
        {
            if (schematic is null)
                throw new ArgumentNullException(nameof(schematic));

            Schematic = schematic;
            Rotation = new RandomPickupRotation(this);
        }

        /// <summary>
        /// Initializes the random pickup instance.
        /// </summary>
        public void Initialize()
        {
            if (Status is not RandomPickupStatus.NotInitialized)
                return;

            if (RandomPickupCore.ConfigStatic.SpawnLight)
            {
                Light = new(Schematic.Position, Schematic.Rotation)
                {
                    Color = RandomPickupCore.ConfigStatic.LightColor.Color.FixPrimitiveColor(),
                    Range = RandomPickupCore.ConfigStatic.LightRange,
                    Intensity = RandomPickupCore.ConfigStatic.LightIntensity,
                };

                Light.Transform.parent = Schematic.transform;
            }

            Interactable = new(Schematic.Position, Schematic.Rotation)
            {
                Shape = InvisibleInteractableToy.ColliderShape.Box,
                Scale = new Vector3(1f, 1f, 1f),
                InteractionDuration = 0.5f,
                IsLocked = false
            };

            Interactable.Transform.parent = Schematic.transform;

            Rotation.Initialize();

            Status = RandomPickupStatus.Waiting;

            playbackHandle = PlaybackUtils.PlayAt(RandomPickupCore.ConfigStatic.WaitingAudioClip, Schematic.Position, null, true, () => playbackHandle = null);
        }

        /// <summary>
        /// Destroys the current instance and releases all associated resources.
        /// </summary>
        public void Destroy()
        {
            if (Status is RandomPickupStatus.Destroyed)
                return;

            playbackHandle?.Destroy();
            playbackHandle = null;

            Rotation?.Destroy();
            Rotation = null!;

            Interactable?.Delete();
            Interactable = null!;

            Light?.Delete();
            Light = null!;

            Schematic?.Destroy();
            Schematic = null!;

            Status = RandomPickupStatus.Destroyed;

            RandomPickupSpawner.Internal_Destroyed(this);
        }

        internal void Internal_Interacted(ExPlayer player)
        {
            playbackHandle?.Destroy();

            if (Status is RandomPickupStatus.Waiting)
            {
                playbackHandle = PlaybackUtils.PlayAt(RandomPickupCore.ConfigStatic.OpenedAudioClip, Schematic.Position, null, false, () => playbackHandle = null);

                var lootItems = ListPool<string>.Shared.Rent();

                if (SpawnReason is RandomPickupSpawnReason.DefinedLocation)
                {
                    Loot.GetLoot(player, RandomPickupCore.ConfigStatic.DefinedItemCount.GetRandom(), lootItems.Add);
                }
                else if (SpawnReason is RandomPickupSpawnReason.RandomPlayer)
                {
                    Loot.GetLoot(player, RandomPickupCore.ConfigStatic.ItemCount.GetRandom(), lootItems.Add);
                }
                else if (SpawnReason is RandomPickupSpawnReason.Scenario)
                {
                    SpawnScenario?.FillLoot(player, ScenarioData, lootItems);
                }

                foreach (var lootItem in lootItems)
                {
                    if (!ItemHandler.TryApplyOrSpawnItemFromString(player, Schematic.Position, Schematic.Rotation, true, lootItem, out _, out var entry))
                    {
                        ApiLog.Warn("Random Pickup", $"Unrecognized loot item: &1{lootItem}&r!");
                    }
                    else
                    {
                        ApiLog.Debug("Random Pickup", $"Added item &3{lootItem}&r (&6{entry.Name}&r / &6{entry.GetType().Name}&r)");
                    }
                }

                ListPool<string>.Shared.Return(lootItems);

                Status = RandomPickupStatus.Opened;
            }

            Rotation?.Destroy();
            Rotation = null!;

            Interactable?.Delete();
            Interactable = null!;

            Light?.Delete();
            Light = null!;

            Schematic?.Destroy();
            Schematic = null!;
        }
    }
}