using mcx.Overlays.Levels;

using System.ComponentModel;

namespace mcx.Overlays;

/// <summary>
/// The config file class.
/// </summary>
public class OverlayConfig
{
    /// <summary>
    /// Gets or sets the level overlay settings.
    /// </summary>
    [Description("Manages options for the level overlay.")]
    public LevelSettings LevelOverlay { get; set; } = new LevelSettings();
}