using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

namespace mcx.Loadouts;

/// <summary>
/// Loadout commands.
/// </summary>
[Command("loadout", "Loadout management commands.")]
public class LoadoutCommand : CommandBase, IServerSideCommand
{
    [CommandOverload("list", "Lists all loaded loadouts.")]
    public void List()
    {
        if (LoadoutPlugin.Loadouts.Count == 0)
        {
            Fail("No loadouts were loaded.");
            return;
        }
        
        Ok(x =>
        {
            x.AppendLine();

            foreach (var loadout in LoadoutPlugin.Loadouts)
            {
                x.AppendLine($"- {loadout.Name} ({loadout.Items.Count} items; {loadout.Ammo.Count} ammo items)");
            }
        });
    }

    [CommandOverload("apply", "Applies a loadout.")]
    public void Apply(
        [CommandParameter("Name", "Name of the loadout.")] string name, 
        [CommandParameter("Player", "The target player (defaults to you if not specified.)")] ExPlayer? player = null)
    {
        player ??= Sender;
        
        if (LoadoutPlugin.TryApply(player, name))
        {
            Ok($"Loadout '{name}' applied to '{player.Nickname} ({player.UserId})'");
        }
        else
        {
            Fail($"Loadout '{name}' could not be applied - check the console for more details.");
        }
    }
}