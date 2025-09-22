using InventorySystem.Items.Firearms.Attachments;

using PlayerRoles;

using System.ComponentModel;

namespace PeanutClub.Items.Weapons
{
    /// <summary>
    /// Represents configurable properties for a custom sniper rifle, including ammunition capacity, damage values,
    /// attachment options, and damage multipliers.
    /// </summary>
    /// <remarks>Use this class to define gameplay characteristics and restrictions for a custom sniper rifle.
    /// Properties allow customization of damage per hitbox, per-role and per-team multipliers, and control over
    /// available attachments. All properties should be set before the sniper rifle is used in gameplay to ensure
    /// consistent behavior.</remarks>
    public class CustomFirearmProperties
    {
        /// <summary>
        /// The maximum capacity of a Sniper Rifle.
        /// </summary>
        [Description("Sets the maximum ammo capacity of the Sniper Rifle.")]
        public int? MaxAmmo { get; set; } = null;

        /// <summary>
        /// Whether or not players can change the attachments of the sniper rifle.
        /// </summary>
        [Description("Enables / disables changing attachments of the Sniper Rifle.")]
        public bool AllowAttachmentsChanging { get; set; } = false;

        /// <summary>
        /// The base damage of the Sniper Rifle, per hitbox.
        /// </summary>
        [Description("Sets the base damage of the Sniper Rifle.")]
        public Dictionary<HitboxType, float> BaseDamage { get; set; } = new()
        {
            [HitboxType.Body] = 250f,
            [HitboxType.Limb] = 250f,
            [HitboxType.Headshot] = 250f
        };

        /// <summary>
        /// The Sniper Rifle role type damage multipliers.
        /// </summary>
        [Description("Sets the per-role damage multipliers (overrides team multipliers).")]
        public Dictionary<RoleTypeId, float> RoleMultipliers { get; set; } = new();

        /// <summary>
        /// The Sniper Rifle team type damage multipliers.
        /// </summary>
        [Description("Sets the per-team damage multipliers (overriden by role_multipliers).")]
        public Dictionary<Team, float> TeamMultipliers { get; set; } = new();

        /// <summary>
        /// A list of the sniper rifle's default attachments.
        /// </summary>
        [Description("Sets the default attachments of the Sniper Rifle.")]
        public List<AttachmentName> DefaultAttachments { get; set; } = new()
    {
        AttachmentName.ScopeSight,
        AttachmentName.LightweightStock,
        AttachmentName.SoundSuppressor,
        AttachmentName.StandardMagFMJ,
        AttachmentName.RifleBody
    };

        /// <summary>
        /// A list of the sniper rifle's blacklisted attachments.
        /// </summary>
        [Description("Sets the blacklisted attachments of the Sniper Rifle.")]
        public List<AttachmentName> BlacklistedAttachments { get; set; } = new();
    }
}