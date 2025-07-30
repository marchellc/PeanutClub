using System.ComponentModel;

using InventorySystem.Items.Firearms.Attachments;

using PeanutClub.Utilities.Randomness;

namespace PeanutClub.Items.Weapons.SniperRifle;

/// <summary>
/// Defines properties of a sniper rifle.
/// </summary>
public class SniperRifleProperties
{
    /// <summary>
    /// The maximum capacity of a Sniper Rifle.
    /// </summary>
    [Description("Sets the maximum ammo capacity of the Sniper Rifle.")]
    public int MaxAmmo { get; set; } = 1;

    /// <summary>
    /// Whether or not players can change the attachments of the sniper rifle.
    /// </summary>
    [Description("Enables / disables changing attachments of the Sniper Rifle.")]
    public bool AllowAttachmentsChanging { get; set; } = false;
    
    /// <summary>
    /// The Sniper Rifle hitbox damage matrix.
    /// </summary>
    [Description("Sets the Sniper Rifle's damage range per hitbox.")]
    public Dictionary<HitboxType, FloatRange> DamagePerHitbox { get; set; } = new()
    {
        [HitboxType.Headshot] = new()
        {
            MinValue = 250f,
            MaxValue = 250f
        },
        
        [HitboxType.Body] = new()
        {
            MinValue = 250f,
            MaxValue = 250f
        },
        
        [HitboxType.Limb] = new()
        {
            MinValue = 250f,
            MaxValue = 250f
        }
    };

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