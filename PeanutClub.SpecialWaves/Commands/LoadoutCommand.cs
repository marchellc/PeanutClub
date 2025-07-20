using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using PeanutClub.SpecialWaves.Loadouts;

namespace PeanutClub.SpecialWaves.Commands;

[Command("loadout", "Applies a loadout to a player.")]
public class LoadoutCommand : CommandBase, IServerSideCommand
{
    [CommandOverload]
    public void Invoke(
        [CommandParameter("Target", "The target player.")] ExPlayer target,
        [CommandParameter("Loadout", "The name of the loadout.")] string loadout)
    {
        target.ApplyLoadout(loadout);
        
        Ok($"Applied loadout '{loadout}' to player '{target.Nickname} ({target.UserId})'");
    }
}