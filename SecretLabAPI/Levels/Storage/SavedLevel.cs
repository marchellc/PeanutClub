using LabExtended.Core.Storage;

using Mirror;

namespace SecretLabAPI.Levels.Storage
{
    /// <summary>
    /// Represents a saved level, including the level number and experience points.
    /// </summary>
    public class SavedLevel : StorageValue
    {
        private int level = 1;
        private int experience = 0;

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
        public int Experience
        {
            get => experience;
            set => SetField(ref experience, value);
        }

        /// <summary>
        /// Gets or sets the experience required to reach the next level.
        /// </summary>
        public int RequiredExperience { get; set; }

        /// <inheritdoc/>
        public override void ReadValue(NetworkReader reader)
        {
            level = reader.ReadInt();
            experience = reader.ReadInt();
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
            Experience = 0;
        }
    }
}