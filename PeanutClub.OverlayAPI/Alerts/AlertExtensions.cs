using LabExtended.API;
using LabExtended.API.Hints;

using LabExtended.Extensions;

using NorthwoodLib.Pools;

namespace PeanutClub.OverlayAPI.Alerts;

/// <summary>
/// Extensions targeting alerts.
/// </summary>
public static class AlertExtensions
{
    /// <summary>
    /// The color of the info emoji.
    /// </summary>
    public const string InfoColor = "047a08";
    
    /// <summary>
    /// The color of the warning emoji.
    /// </summary>
    public const string WarnColor = "fc8403";

    /// <summary>
    /// The start of a message line.
    /// </summary>
    public const string TextStart = "»";

    /// <summary>
    /// The end of a message line.
    /// </summary>
    public const string TextEnd = "«";

    /// <summary>
    /// Gets the prefix for alert messages.
    /// </summary>
    public const string MessagePrefix = "<size=30><b>﹝ Ｓ E R VＥ R • Z P R Á V A ﹞</b></size>";
    
    /// <summary>
    /// Sends a new alert to a player.
    /// </summary>
    /// <param name="player">The player receiving the alert.</param>
    /// <param name="type">The type of the alert.</param>
    /// <param name="duration">The duration of the alert (in seconds) - must be at least one.</param>
    /// <param name="content">The content of the alert.</param>
    public static void SendAlert(this ExPlayer player, AlertType type, float duration, string content)
    {
        if (player?.ReferenceHub == null)
            return;

        if (string.IsNullOrWhiteSpace(content))
            return;

        if (duration < 1f)
            return;

        if (!player.TryGetHintElement<AlertElement>(out var alertElement))
            return;
        
        alertElement.Alerts.Add(new()
        {
            Type = type,
            Content = content,
            Duration = duration
        });
    }
    
    /// <summary>
    /// Formats an alert's content.
    /// </summary>
    /// <param name="alert">The alert to format.</param>
    /// <returns>the formatted content of the alert</returns>
    public static string FormatAlert(this AlertInfo alert)
    {
        return StringBuilderPool.Shared.BuildString(x =>
        {
            var content = alert.Content.Replace("\n", $" {TextEnd}\n{TextStart} ");
            
            x.Append("<color=#");
            x.Append(alert.Type is AlertType.Info ? InfoColor : WarnColor);
            x.Append(">");
            x.Append(MessagePrefix);
            x.Append("</color>\n<size=25>");
            x.Append(TextStart);
            x.Append(" ");
            x.Append(content);
            x.Append(" ");
            x.Append(TextEnd);
            x.Append("</size>");
        });
    }
}