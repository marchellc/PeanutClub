using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using PeanutClub.SpecialWaves.Roles.Janitor;

namespace PeanutClub.SpecialWaves.Commands;

[Command("setjanitor", "Sets a player as the Janitor.")]
public class SetJanitorCommand : CommandBase, IServerSideCommand
{
    [CommandOverload]
    public void Invoke(
        [CommandParameter("Target", "The target player.")] ExPlayer target)
    {
        JanitorRole.SetJanitor(target);
        
        Ok($"Set player '{target.Nickname} ({target.UserId})' to the Janitor role.");
    }
}