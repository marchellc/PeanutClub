using LabExtended.API;
using LabExtended.API.CustomTeams;

using LabExtended.Extensions;
using LabExtended.Utilities;

using PeanutClub.LoadoutAPI;

using PlayerRoles;

namespace PeanutClub.SpecialWaves.Waves.RedRightHand;

/// <summary>
/// An instance of a Red Right Hand team.
/// </summary>
public class RedRightHandWave : CustomTeamInstance<RedRightHandTeam>
{
    /// <inheritdoc cref="CustomTeamInstance.SpawnPlayer"/>
    public override void SpawnPlayer(ExPlayer player, RoleTypeId role)
    {
        player.Role.Set(role, RoleChangeReason.Respawn, RoleSpawnFlags.None);

        TimingUtils.AfterSeconds(() =>
        {
            player.Position.Set(RedRightHandTeam.NtfRoles.GetRandomItem().GetSpawnPosition().position);

            var loadout = string.Empty;

            switch (player.Role.Type)
            {
                case RoleTypeId.NtfCaptain:
                    loadout = "Hand1";
                    break;

                case RoleTypeId.NtfSpecialist:
                    loadout = "Hand2";
                    break;

                case RoleTypeId.NtfSergeant:
                    loadout = "Hand3";
                    break;
            }

            if (!string.IsNullOrEmpty(loadout))
            {
                LoadoutPlugin.TryApply(player, loadout);
            }
        }, 0.2f);
    }
}