using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

namespace mcx.Roles.GuardCommander;

/// <summary>
/// Command to manage the Guard Commander role.
/// </summary>
[Command("guardcommander", "Guard Commander role commands.", "guardcomm", "gc")]
public class GuardCommanderCommand : CommandBase, IServerSideCommand
{
    /// <summary>
    /// Sets a player as the Guard Commander.
    /// </summary>
    [CommandOverload("set", "Sets a player as the Guard Commander.")]
    public void Set(
        [CommandParameter("Target", "The player to set as the Guard Commander (defaults to you).")] ExPlayer? target = null)
    {
        target ??= Sender;
        target.SetCommander();
        
        Ok($"Set player '{target.Nickname} ({target.UserId})' as the Guard Commander!");
    }
}