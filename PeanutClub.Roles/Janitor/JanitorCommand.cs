using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

namespace PeanutClub.Roles.Janitor;

/// <summary>
/// Commands targeting the Janitor role.
/// </summary>
[Command("janitor", "Janitor Role commands.")]
public class JanitorCommand : CommandBase, IServerSideCommand
{
    /// <summary>
    /// Sets a player as the Janitor role.
    /// </summary>
    [CommandOverload("set", "Sets a player as the Janitor role.")]
    public void Set(
        [CommandParameter("Target", "The player to set as the Janitor role (defaults to you).")] ExPlayer? target = null)
    {
        target ??= Sender;
        target.SetJanitor();
        
        Ok($"Set player '{target.Nickname} ({target.UserId})' to the Janitor role!");
    }
}