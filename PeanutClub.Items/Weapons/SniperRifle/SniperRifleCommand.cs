using InventorySystem;

using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using LabExtended.Extensions;

using MapGeneration;

namespace PeanutClub.Items.Weapons.SniperRifle;

/// <summary>
/// Commands targeting the Sniper Rifle.
/// </summary>
[Command("sniperrifle", "Commands for the Sniper Rifle.", "sniper")]
public class SniperRifleCommand : CommandBase, IServerSideCommand
{
    /// <summary>
    /// Lists active Sniper Rifles.
    /// </summary>
    [CommandOverload("list", "Lists all active Sniper Rifles.")]
    public void List()
    {
        if (SniperRifleHandler.TrackedItems.Count == 0)
        {
            Fail("No active Sniper Rifle instances.");
            return;
        }
        
        Ok(x =>
        {
            x.AppendLine($"Showing '{SniperRifleHandler.TrackedItems.Count}' active Sniper Rifle instance(s):");

            foreach (var pair in SniperRifleHandler.TrackedItems)
            {
                x.AppendLine();
                
                if (InventoryExtensions.ServerTryGetItemWithSerial(pair.Key, out var item))
                {
                    x.AppendLine($"- Inventory {pair.Key}");

                    if (ExPlayer.TryGet(item.Owner, out var player))
                    {
                        x.AppendLine($"  >- Owned By: [{player.PlayerId} - {player.Role.Name}] {player.Nickname} ({player.UserId})");
                        x.AppendLine($"  >- Inventory Slot: {player.Inventory.UserInventory.Items.FindKeyIndex(pair.Key)}");
                    }
                    else
                    {
                        x.AppendLine($"  >- Owned By: (null)");
                    }
                }
                else if (ExMap.Pickups.TryGetFirst(y => y != null && y.Info.Serial == pair.Key, out var pickup))
                {
                    x.AppendLine($"- Pickup {pair.Key}");
                    x.AppendLine($"  >- Position: {pickup.Position.ToPreciseString()}");

                    if (ExPlayer.TryGet(pickup.PreviousOwner.Hub, out var player))
                    {
                        x.AppendLine($"  >- Dropped By: [{player.PlayerId} - {player.Role.Name}] {player.Nickname} ({player.UserId})");
                    }
                    else
                    {
                        x.AppendLine($"  >- Dropped By: (null)");
                    }

                    if (pickup.Position.TryGetRoom(out var room))
                    {
                        x.AppendLine($"  >- Room: {room.Name} ({room.Zone}; {room.Shape})");
                    }
                    else
                    {
                        x.AppendLine("  >- Room: (null)");
                    }
                }
                else
                {
                    x.AppendLine($"- Serial {pair.Key}");
                    x.AppendLine($"  >- Unknown Item");
                }
            }
        });
    }
    
    /// <summary>
    /// Adds the Sniper Rifle.
    /// </summary>
    [CommandOverload("add", "Adds the Sniper Rifle to a player's inventory.")]
    public void Add(
        [CommandParameter("Target", "The player to add the Sniper Rifle to.")] ExPlayer? target = null)
    {
        target ??= Sender;
        target.GiveSniperRifle();
        
        Ok($"Added the Sniper Rifle to player '{target.Nickname} ({target.UserId})'!");
    }

    /// <summary>
    /// Removes the Sniper Rifle.
    /// </summary>
    [CommandOverload("remove", "Removes the Sniper Rifle.")]
    public void Remove(
        [CommandParameter("Serial", "The serial number of the Sniper Rifle item.")] ushort sniperSerial, 
        [CommandParameter("Destroy", "Whether or not to destroy the item.")] bool destroyItem = true)
    {
        if (!SniperRifleHandler.Remove(sniperSerial, destroyItem))
        {
            Fail($"Could not remove Sniper Rifle of serial '{sniperSerial}'!");
            return;
        }
        
        Ok($"Sniper Rifle with serial '{sniperSerial}' was removed!");
    }
}