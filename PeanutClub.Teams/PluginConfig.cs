using System.ComponentModel;

namespace PeanutClub.Teams;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfig
{
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
}