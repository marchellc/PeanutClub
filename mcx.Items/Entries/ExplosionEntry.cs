using LabExtended.API;

using mcx.Utilities.Items;
using mcx.Utilities.Items.Entries;

namespace mcx.Items.Entries
{
    /// <summary>
    /// Implements explosion effects.
    /// </summary>
    public class ExplosionEntry : EffectItemEntry
    {
        /// <inheritdoc/>
        public override string Name { get; } = "Explosion";

        /// <inheritdoc/>
        public override void ApplyEffect(ExPlayer player, string[]? effectString)
        {
            if (ItemHandler.TryProcessEffectString(effectString, out var parameters))
            {
                var type = parameters.TryParseEffectParameter<ItemType>("Type", Enum.TryParse, out var itemType)
                    ? itemType
                    : ItemType.GrenadeHE;

                var allowDamage = parameters.TryParseEffectParameter<bool>("AllowDamage", bool.TryParse, out var damageAllowed)
                    ? damageAllowed
                    : true;

                var deathReason = parameters.TryGetValue("DeathReason", out var reason)
                    ? reason
                    : "Explosion";

                player.Explode(type, deathReason, !allowDamage);
            }
            else
            {
                player.Explode(ItemType.GrenadeHE, "Explosion", false);
            }
        }
    }
}