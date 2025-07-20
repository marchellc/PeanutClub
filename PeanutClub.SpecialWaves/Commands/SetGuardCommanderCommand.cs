using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using PeanutClub.SpecialWaves.Roles.GuardCommander;

namespace PeanutClub.SpecialWaves.Commands;

[Command("setguardcomm", "Sets a player as the Guard Commander.")]
public class SetGuardCommanderCommand : CommandBase, IServerSideCommand
{
    [CommandOverload]
    public void Invoke(
        [CommandParameter("Target", "The target player.")] ExPlayer target)
    {
        GuardCommanderRole.SetCommander(target);
        
        Ok($"Set '{target.Nickname} ({target.UserId})' to the Guard Commander role.");
    }
}