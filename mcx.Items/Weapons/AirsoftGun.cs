﻿using LabExtended.API;

using LabExtended.API.Custom.Items;
using LabExtended.API.Custom.Items.Events;

using mcx.Overlays.Alerts;

using System.ComponentModel;

namespace mcx.Items.Weapons
{
    /// <summary>
    /// An airsoft gun.
    /// </summary>
    public class AirsoftGun : CustomFirearm
    {
        /// <inheritdoc/>
        public override string Id { get; } = "ptc.airsoftgun";

        /// <inheritdoc/>
        public override string Name { get; } = "Airsoft Gun";

        /// <inheritdoc/>
        public override ItemType PickupType { get; set; } = ItemType.GunCrossvec;

        /// <inheritdoc/>
        public override ItemType InventoryType { get; set; } = ItemType.GunCrossvec;

        /// <summary>
        /// Gets or sets the damage the firearm deals.
        /// </summary>
        [Description("Sets the damage the airsoft gun deals.")]
        public float Damage { get; set; } = 5f;

        /// <inheritdoc/>
        public override float ModifyDamage(ExPlayer target, float damage)
        {
            return Damage;
        }

        /// <inheritdoc/>
        public override void OnItemAdded(CustomItemAddedEventArgs args)
        {
            base.OnItemAdded(args);

            args.Player.SendAlert(AlertType.Info, 10f,
                $"Dostal si <color=red>Airsoft Gun</color>!\n" +
                $"Tato zbraň dává damage <color=yellow>5 HP</color> při <b>každé</b> ráně!");
        }
    }
}