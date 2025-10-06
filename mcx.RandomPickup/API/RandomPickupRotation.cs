using LabExtended.Utilities.Update;

using UnityEngine;

namespace mcx.RandomPickup.API
{
    /// <summary>
    /// Manages rotation of random pickups.
    /// </summary>
    public class RandomPickupRotation
    {
        private float initialY = 0f;

        /// <summary>
        /// Gets or sets the rotation angle in degrees per second.
        /// </summary>
        public float RotationAngle { get; set; } = 45f;

        /// <summary>
        /// Gets or sets the speed at which the object floats.
        /// </summary>
        public float FloatSpeed { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the amplitude value as a floating-point number.
        /// </summary>
        public float FloatAmplitude { get; set; } = 0.25f;

        /// <summary>
        /// Whether or not the rotation is paused.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets the target pickup instance.
        /// </summary>
        public RandomPickupInstance TargetInstance { get; }

        /// <summary>
        /// Initializes a new instance of the RandomPickupRotation class with the specified random pickup instance.
        /// </summary>
        /// <param name="randomPickupInstance">The RandomPickupInstance to associate with this rotation. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if randomPickupInstance is null.</exception>
        public RandomPickupRotation(RandomPickupInstance randomPickupInstance)
        {
            if (randomPickupInstance is null)
                throw new ArgumentNullException(nameof(randomPickupInstance));

            TargetInstance = randomPickupInstance;
        }

        public void Initialize()
        {
            initialY = TargetInstance.Schematic.Position.y;

            PlayerUpdateHelper.OnUpdate += Internal_Update;
        }

        public void Destroy()
        {
            PlayerUpdateHelper.OnUpdate -= Internal_Update;
        }

        private void Internal_Update()
        {
            if (TargetInstance is null || TargetInstance.Schematic == null)
            {
                Destroy();
                return;
            }

            var pos = TargetInstance.Schematic.Position;

            pos.y = initialY + Mathf.Sin(Time.time * FloatSpeed) * FloatAmplitude;

            TargetInstance.Schematic.Position = pos;
            TargetInstance.Schematic.Rotation *= 
                Quaternion.Inverse(TargetInstance.Schematic.Rotation)
                * Quaternion.Euler(0f, RotationAngle * Time.deltaTime, 0f)
                * TargetInstance.Schematic.Rotation;
        }
    }
}
