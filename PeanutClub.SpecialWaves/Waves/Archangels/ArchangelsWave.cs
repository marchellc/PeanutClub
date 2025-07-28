using LabExtended.API;
using LabExtended.API.CustomTeams;

using LabExtended.Extensions;
using LabExtended.Utilities;

using PeanutClub.LoadoutAPI;

using PlayerRoles;

namespace PeanutClub.SpecialWaves.Waves.Archangels;

/// <summary>
/// An instance of Zeta-3 Archangels wave.
/// </summary>
public class ArchangelsWave : CustomTeamInstance<ArchangelsTeam>
{   
    /// <inheritdoc cref="CustomTeamInstance.SpawnPlayer"/>
    public override void SpawnPlayer(ExPlayer player, RoleTypeId role)
    {
        player.Role.Set(role, RoleChangeReason.Respawn, RoleSpawnFlags.UseSpawnpoint);

        TimingUtils.AfterSeconds(() =>
        {
            var loadout = string.Empty;

            switch (player.Role.Type)
            {
                // Archangels 1
                case RoleTypeId.ChaosRepressor:
                    loadout = "Archangels1";
                    break;

                // Archangels2
                case RoleTypeId.ChaosRifleman:
                    loadout = "Archangels2";
                    break;

                // Archangels 3
                case RoleTypeId.ChaosMarauder:
                    loadout = "Archangels3";
                    break;
            }

            if (!string.IsNullOrEmpty(loadout))
            {
                LoadoutPlugin.TryApply(player, loadout);
            }
        }, 0.2f);
    }
}