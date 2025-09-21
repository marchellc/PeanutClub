namespace PeanutClub.Overlays.Alerts;

/// <summary>
/// A created alert.
/// </summary>
public class AlertInfo
{
    /// <summary>
    /// The content of the alert.
    /// </summary>
    public string Content { get; set; }
    
    /// <summary>
    /// The formatted content of the alert.
    /// </summary>
    public string FormattedContent { get; set; }

    /// <summary>
    /// The type of the alert.
    /// </summary>
    public AlertType Type { get; set; }
    
    /// <summary>
    /// The duration of the alert.
    /// </summary>
    public float Duration { get; set; }
}