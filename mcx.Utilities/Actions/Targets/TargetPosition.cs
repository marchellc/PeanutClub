using mcx.Utilities.Actions.Interfaces;

using UnityEngine;

namespace mcx.Utilities.Actions.Targets
{
    /// <summary>
    /// Represents a target defined by a position in 3D space.
    /// </summary>
    public struct TargetPosition : IActionTarget
    {
        /// <summary>
        /// The targeted position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the TargetPosition class with the specified position.
        /// </summary>
        /// <param name="position">The position in 3D space to assign to the target.</param>
        public TargetPosition(Vector3 position)
        {
            Position = position;
        }
    }
}