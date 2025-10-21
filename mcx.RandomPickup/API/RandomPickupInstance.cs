using AdminToys;

using LabExtended.API;
using LabExtended.API.Toys;

using LabExtended.Extensions;

using mcx.Utilities.Audio;
using mcx.Utilities.Items.Loot;

using ProjectMER.Features.Objects;

using UnityEngine;

namespace mcx.RandomPickup.API
{
    /// <summary>
    /// A spawned instance of a random pickup.
    /// </summary>
    public class RandomPickupInstance
    {
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
        public LootConfig Loot { get; set; }

        /// <summary>
        /// Gets the clip manager.
        /// </summary>
        public ClipManager<RandomPickupClipType> Clips { get; private set; }

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
            Clips = new(RandomPickupCore.ConfigStatic.Clips, Schematic.transform);
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

            Clips.PlayRandomClip(RandomPickupClipType.Waiting);

            Status = RandomPickupStatus.Waiting;
        }

        /// <summary>
        /// Destroys the current instance and releases all associated resources.
        /// </summary>
        public void Destroy()
        {
            if (Status is RandomPickupStatus.Destroyed)
                return;

            Clips?.Destroy();
            Clips = null!;

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
            try
            {
                Clips.Stop();
            }
            catch
            {

            }

            if (Status is RandomPickupStatus.Waiting)
            {
                Clips.PlayRandomClip(RandomPickupClipType.Opened);

                var lootGroup = SpawnReason switch
                {
                    RandomPickupSpawnReason.DefinedLocation => Loot?.GetGroup(player),
                    RandomPickupSpawnReason.RandomPlayer => Loot?.GetGroup(player),
                    RandomPickupSpawnReason.Scenario => SpawnScenario?.GetLoot(player, ScenarioData) ?? Loot?.GetGroup(player),

                    _ => null
                };

                lootGroup?.ApplyGroup(player);

                Status = RandomPickupStatus.Opened;
            }

            try
            {
                Rotation?.Destroy();
                Rotation = null!;
            }
            catch
            {

            }

            try
            {
                Interactable?.Delete();
                Interactable = null!;
            }
            catch
            {

            }

            try
            {
                Light?.Delete();
                Light = null!;
            }
            catch
            {

            }

            try
            {
                Schematic?.Destroy();
                Schematic = null!;
            }
            catch
            {

            }
        }
    }
}