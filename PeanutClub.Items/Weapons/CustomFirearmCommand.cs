using InventorySystem;

using LabExtended.API;

using LabExtended.Commands;
using LabExtended.Commands.Attributes;
using LabExtended.Commands.Interfaces;

using LabExtended.Extensions;

using MapGeneration;

using PeanutClub.Items.Weapons.AirsoftGun;
using PeanutClub.Items.Weapons.SniperRifle;

namespace PeanutClub.Items.Weapons;

/// <summary>
/// Commands targeting the Sniper Rifle.
/// </summary>
[Command("customfirearm", "Commands for custom firearms.", "cf")]
public class CustomFirearmCommand : CommandBase, IServerSideCommand
{
    /// <summary>
    /// Specifies the name of a custom firearm.
    /// </summary>
    public enum FirearmName
    {
        /// <summary>
        /// The sniper rifle.
        /// </summary>
        SniperRifle,

        /// <summary>
        /// The airsoft gun.
        /// </summary>
        AirsoftGun,
    }

    /// <summary>
    /// Lists active custom firearms.
    /// </summary>
    [CommandOverload("list", "Lists all active custom firearms.")]
    public void List()
    {
        if (CustomFirearmHandler.TrackedItems.Count == 0)
        {
            Fail("No active custom firearm instances.");
            return;
        }
        
        Ok(x =>
        {
            x.AppendLine($"Showing '{CustomFirearmHandler.TrackedItems.Count}' active custom firearm instance(s):");

            foreach (var pair in CustomFirearmHandler.TrackedItems)
            {
                x.AppendLine();
                
                if (InventoryExtensions.ServerTryGetItemWithSerial(pair.Key, out var item))
                {
                    x.AppendLine($"- [{pair.Value.GetType().Name}] Inventory {pair.Key}");

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
                    x.AppendLine($"- [{pair.Value.GetType().Name}] Pickup {pair.Key}");
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
                    x.AppendLine($"- [{pair.Value.GetType().Name}] Serial {pair.Key}");
                    x.AppendLine($"  >- Unknown Item");
                }
            }
        });
    }

    /// <summary>
    /// Adds a custom firearm.
    /// </summary>
    [CommandOverload("add", "Adds a custom firearm.")]
    public void Add(
        [CommandParameter("Target", "The player to add the firearm to.")] ExPlayer target,
        [CommandParameter("Item", "The type of the firearm's item.")] ItemType item,
        [CommandParameter("Type", "The type of the firearm to add.")] FirearmName type)
    {
        if (type is FirearmName.AirsoftGun)
        {
            target.GiveCustomFirearm(item, AirsoftGunHandler.DefaultProperties);

            Ok($"Added the Airsoft Gun ({item}) to {target.Nickname} ({target.UserId})!");
        }
        else if (type is FirearmName.SniperRifle)
        {
            target.GiveCustomFirearm(item, SniperRifleHandler.DefaultProperties);

            Ok($"Added the Sniper Rifle ({item}) to {target.Nickname} ({target.UserId})!");
        }
    }

    /// <summary>
    /// Removes a custom firearm.
    /// </summary>
    [CommandOverload("remove", "Removes a custom firearm.")]
    public void Remove(
        [CommandParameter("Serial", "The serial number of the custom firearm item.")] ushort sniperSerial, 
        [CommandParameter("Destroy", "Whether or not to destroy the item.")] bool destroyItem = true)
    {
        if (!CustomFirearmHandler.Remove(sniperSerial, destroyItem))
        {
            Fail($"Could not remove custom firearm of serial '{sniperSerial}'!");
            return;
        }
        
        Ok($"Custom Firearm with serial '{sniperSerial}' was removed!");
    }
}