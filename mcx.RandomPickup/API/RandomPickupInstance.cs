using AdminToys;

using InventorySystem.Items.Pickups;

using LabExtended.API;
using LabExtended.API.Toys;
using LabExtended.Extensions;

using Mirror;

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
        public LightToy Light { get; private set; }

        /// <summary>
        /// Gets the interactable toy.
        /// </summary>
        public InteractableToy Interactable { get; private set; }

        /// <summary>
        /// Gets the loot contained in this random pickup instance.
        /// </summary>
        public List<ItemPickupBase> Loot { get; } = new();

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

            Light = new(Schematic.Position, Schematic.Rotation)
            {
                Color = Color.red.FixPrimitiveColor(),
                Range = 1.5f,
                Intensity = 0.6f
            };

            Interactable = new(Schematic.Position, Schematic.Rotation)
            {
                Shape = InvisibleInteractableToy.ColliderShape.Box,
                Scale = new Vector3(1f, 1f, 1f),
                InteractionDuration = 0.5f,
                IsLocked = false
            };

            Rotation.Initialize();

            Status = RandomPickupStatus.Waiting;
        }

        /// <summary>
        /// Destroys the current instance and releases all associated resources.
        /// </summary>
        public void Destroy()
        {
            if (Status is RandomPickupStatus.Destroyed)
                return;

            Rotation?.Destroy();
            Rotation = null!;

            if (Status is RandomPickupStatus.Waiting)
                Loot.ForEach(x => x.DestroySelf());

            Interactable?.Delete();
            Interactable = null!;

            Light?.Delete();
            Light = null!;

            Schematic?.Destroy();
            Schematic = null!;

            Loot.Clear();

            Status = RandomPickupStatus.Destroyed;

            RandomPickupSpawner.Internal_Destroyed(this);
        }

        internal void Internal_Interacted(ExPlayer player)
        {
            if (Status is RandomPickupStatus.Waiting)
            {
                foreach (var pickup in Loot)
                {
                    pickup.Position = player.Position;
                    pickup.Rotation = player.Rotation;

                    NetworkServer.Spawn(pickup.gameObject);
                }
            }

            Loot.Clear();

            Rotation?.Destroy();
            Rotation = null!;

            Interactable?.Delete();
            Interactable = null!;

            Light?.Delete();
            Light = null!;

            Schematic?.Destroy();
            Schematic = null!;

            Status = RandomPickupStatus.Opened;
        }
    }
}