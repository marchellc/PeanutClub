using System.ComponentModel;

using InventorySystem.Items.Firearms.Attachments;

using LabExtended.Core.Configs.Objects;

using PeanutClub.Utilities.Roles.Selection;

namespace PeanutClub.SpecialWaves;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfig
{
    /// <summary>
    /// The capacity of the sniper rifle.
    /// </summary>
    [Description("Sets the capacity of the sniper rifle's chamber.")]
    public int SniperRifleCapacity { get; set; } = 1;
    
    /// <summary>
    /// The damage of the sniper rifle.
    /// </summary>
    [Description("Sets the damage the sniper rifle deals.")]
    public float SniperRifleDamage { get; set; } = 250f;

    /// <summary>
    /// Whether or not players should be able to change sniper rifle attachments.
    /// </summary>
    [Description("Allows or prevents players from changing attachments on the sniper rifle.")]
    public bool SniperChangingAttachments { get; set; } = true;

    /// <summary>
    /// The list of default attachments for the sniper rifle.
    /// </summary>
    [Description("Sets the list of default attachments for the sniper rifle.")]
    public List<AttachmentName> SniperDefaultAttachments { get; set; } = new();

    /// <summary>
    /// The list of blacklisted attachments for the sniper rifle.
    /// </summary>
    [Description("Sets the list of attachments which cannot be applied on a sniper rifle.")]
    public List<AttachmentName> SniperBlacklistedAttachments { get; set; } = new()
    {
        AttachmentName.ExtendedMagAP,
        AttachmentName.ExtendedMagFMJ,
        AttachmentName.ExtendedMagJHP,
        
        AttachmentName.LowcapMagAP,
        AttachmentName.LowcapMagFMJ,
        AttachmentName.LowcapMagJHP,
        
        AttachmentName.StandardMagAP,
        AttachmentName.StandardMagFMJ,
        AttachmentName.StandardMagJHP,
        
        AttachmentName.CylinderMag5,
        AttachmentName.CylinderMag6,
        AttachmentName.CylinderMag7
    };

    [Description("Sets the minimum amount of players required to spawn a Serpent's Hand wave.")]
    public int SerpentsHandMinPlayers { get; set; } = 3;
    
    /// <summary>
    /// How many players to spawn when an SCP dies.
    /// </summary>
    [Description("Sets the maximum amount of players allowed to be spawned in a Serpent's Hand wave.")]
    public int SerpentsHandMaxPlayers { get; set; } = 3;

    /// <summary>
    /// Gets or sets the name of the Serpent's Hand hole schematic.
    /// </summary>
    [Description("Sets the name of the Serpent's Hand hole schematic.")]
    public string SerpentsHandHoleSchematicName { get; set; } = "SerpentsHandHole";
    
    /// <summary>
    /// Gets or sets the name of the Serpent's Hand hole position.
    /// </summary>
    [Description("Sets the name of the Serpent's Hand hole position.")]
    public string SerpentsHandHolePositionName { get; set; } = "SerpentsHandHole";
    
    /// <summary>
    /// Gets or sets the name of the Serpent's Hand spawn position.
    /// </summary>
    [Description("Sets the name of the Serpent's Hand hole position.")]
    public string SerpentsHandSpawnPositionName { get; set; } = "SerpentsHandCenter";

    /// <summary>
    /// The Serpent's Hand CASSIE announcement to play.
    /// </summary>
    [Description("Sets the CASSIE announcement to play once a Serpent's Hand wave spawns.")]
    public string SerpentsHandCassieMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Minimum amount of players in an Archangels wave.
    /// </summary>
    [Description("Sets the minimum amount of players required to spawn an Archangels wave.")]
    public int ArchangelsMinPlayers { get; set; } = 3;
    
    /// <summary>
    /// How many players to spawn when the radio is used.
    /// </summary>
    [Description("Sets the maximum amount of players allowed to spawn in an Archangels wave.")]
    public int ArchangelsMaxPlayers { get; set; } = 6;
    
    /// <summary>
    /// Name of the schematic for the Archangels Radio.
    /// </summary>
    [Description("Sets the name of the Archangels Radio schematic.")]
    public string ArchangelsSchematicName { get; set; } = "ArchangelsRadio";

    /// <summary>
    /// Name of the position for the Archangels Radio.
    /// </summary>
    [Description("Sets the name of the Red Right Hand Button position.")]
    public string ArchangelsPositionName { get; set; } = "ArchangelsRadio";

    /// <summary>
    /// The Archangels CASSIE announcement.
    /// </summary>
    [Description("Sets the CASSIE announcement to play once an Archangels wave spawns.")]
    public string ArchangelsCassieMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// The minimum amount of players required to summon Red Right Hand.
    /// </summary>
    [Description("Sets the minimum amount of players required to spawn a Red Right Hand wave.")]
    public int RedRightHandMinPlayers { get; set; } = 3;
    
    /// <summary>
    /// The maximum amount of players allowed to spawn in a Red Right Hand wave.
    /// </summary>
    [Description("Sets the maximum amount of players allowed to spawn in a Red Right Hand wave.")]
    public int RedRightHandMaxPlayers { get; set; } = 6;

    /// <summary>
    /// The adjusted angle of the button.
    /// </summary>
    [Description("Sets the adjusted angle of the Red Right Hand Button.")]
    public float RedRightHandButtonAngle { get; set; } = 180f;
    
    /// <summary>
    /// Name of the schematic for the Red Right Hand Button.
    /// </summary>
    [Description("Sets the name of the Red Right Hand Button schematic.")]
    public string RedRightHandButtonSchematicName { get; set; } = "RedRightHandButton";

    /// <summary>
    /// Name of the position for the Red Right Hand Button.
    /// </summary>
    [Description("Sets the name of the Red Right Hand Button position.")]
    public string RedRightHandButtonPositionName { get; set; } = "RedRightHandButton";
    
    /// <summary>
    /// Name of the button animator.
    /// </summary>
    [Description("Sets the name of the Red Right Hand Button animator.")]
    public string RedRightHandButtonAnimatorName { get; set; } = "Button_press";

    /// <summary>
    /// Name of the button press animation.
    /// </summary>
    [Description("Sets the name of the Red Right Hand Button press animation.")]
    public string RedRightHandButtonPressAnimationName { get; set; } = "Press";
    
    /// <summary>
    /// Name of the button idle animation.
    /// </summary>
    [Description("Sets the name of the Red Right Hand Button idle animation.")]
    public string RedRightHandButtonIdleAnimationName { get; set; } = "Idle";

    /// <summary>
    /// The Red Right Hand CASSIE announcement.
    /// </summary>
    [Description("Sets the Red Right Hand CASSIE message to play once a wave spawns.")]
    public string RedRightHandCassieMessage { get; set; } = string.Empty;

    /// <summary>
    /// Janitor spawn conditions.
    /// </summary>
    [Description("Sets the Janitor spawn conditions.")]
    public List<RoleRange> JanitorSpawns { get; set; } = new()
    {
        new()
        {
            MinPlayers = 1,
            MaxPlayers = 6,
            OverallChance = 20,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 7,
            MaxPlayers = 11,
            OverallChance = 50,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 12,
            MaxPlayers = 18,
            OverallChance = 80,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 19,
            MaxPlayers = -1,
            OverallChance = 100,
            MaxSpawnCount = 1
        }
    };

    /// <summary>
    /// Guard Commander spawn conditions.
    /// </summary>
    [Description("Sets the Guard Commander spawn conditions.")]
    public List<RoleRange> GuardCommanderSpawns { get; set; } = new()
    {
        new()
        {
            MinPlayers = 3,
            MaxPlayers = 5,
            OverallChance = 20,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 6,
            MaxPlayers = 12,
            OverallChance = 40,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 13,
            MaxPlayers = 16,
            OverallChance = 60,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 17,
            MaxPlayers = 26,
            OverallChance = 80,
            MaxSpawnCount = 1
        },
        
        new()
        {
            MinPlayers = 27,
            MaxPlayers = -1,
            OverallChance = 100,
            MaxSpawnCount = 1
        }
    };
}