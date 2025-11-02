using mcx.Overlays.Levels;

using System.ComponentModel;

namespace mcx.Overlays;

/// <summary>
/// The config file class.
/// </summary>
public class OverlayConfig
{
    /// <summary>
    /// Gets or sets the server name to display in the overlay.
    /// </summary>
    [Description("The name of the server to display in the overlay.")]
    public string ServerName { get; set; } = "My Server";

    public bool ServerNameBasicOverlays { get; set; }

    /// <summary>
    /// Gets or sets the collection of static overlay elements and their configuration options.
    /// </summary>
    [Description("Configures a list of static string elements.")]
    public Dictionary<string, OverlayOptions> StaticOverlays { get; set; } = new()
    {
        { "ServerName", new() }
    };

    /// <summary>
    /// Gets or sets the level overlay settings.
    /// </summary>
    [Description("Manages options for the level overlay.")]
    public LevelSettings LevelOverlay { get; set; } = new LevelSettings();
}