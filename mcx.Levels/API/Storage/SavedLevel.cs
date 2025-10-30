using LabExtended.Core.Storage;

using Mirror;

namespace mcx.Levels.API.Storage
{
    /// <summary>
    /// Represents a saved level, including the level number and experience points.
    /// </summary>
    public class SavedLevel : StorageValue
    {
        private int level = 1;
        private float experience = 0f;

        /// <summary>
        /// Gets or sets the current level value.
        /// </summary>
        public int Level
        {
            get => level;
            set => SetField(ref level, value);
        }

        /// <summary>
        /// Gets or sets the experience points.
        /// </summary>
        public float Experience
        {
            get => experience;
            set => SetField(ref experience, value);
        }

        /// <summary>
        /// Gets or sets the experience required to reach the next level.
        /// </summary>
        public float RequiredExperience { get; set; }

        /// <inheritdoc/>
        public override void ReadValue(NetworkReader reader)
        {
            level = reader.ReadInt();
            experience = reader.ReadFloat();
        }

        /// <inheritdoc/>
        public override void WriteValue(NetworkWriter writer)
        {
            writer.WriteInt(level);
            writer.WriteFloat(experience);
        }

        /// <inheritdoc/>
        public override void ApplyDefault()
        {
            Level = 1;
            Experience = 0f;
        }
    }
}