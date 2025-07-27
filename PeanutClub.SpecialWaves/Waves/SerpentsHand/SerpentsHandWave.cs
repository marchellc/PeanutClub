using LabExtended.API;
using LabExtended.API.CustomTeams;

using LabExtended.Core;
using LabExtended.Utilities;

using PeanutClub.LoadoutAPI;
using PlayerRoles;

namespace PeanutClub.SpecialWaves.Waves.SerpentsHand;

/// <summary>
/// An instance of a spawned Serpents Hand wave.
/// </summary>
public class SerpentsHandWave : CustomTeamInstance<SerpentsHandTeam>
{
    /// <inheritdoc cref="CustomTeamInstance.SpawnPlayer"/>
    public override void SpawnPlayer(ExPlayer player, RoleTypeId role)
    {
        try
        {
            ApiLog.Debug("Serpent's Hand", $"Processing player &3{player.Nickname}&r (&6{player.UserId}&r)");
            
            player.Role.Set(role, RoleChangeReason.Respawn, RoleSpawnFlags.None);

            TimingUtils.AfterSeconds(() =>
            {
                player.IsGodModeEnabled = false;
                player.CustomInfo = "Serpent's Hand";
                player.Position.Set(SerpentsHandTeam.SpawnPosition);

                if ((player.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
                    player.InfoArea |= PlayerInfoArea.CustomInfo;

                LoadoutPlugin.TryApply(player, "SerpentsHand");

                ApiLog.Debug("Serpent's Hand",
                    $"Finished processing player &3{player.Nickname}&r (&6{player.UserId}&r)");
            }, 0.2f);
        }
        catch (Exception ex)
        {
            ApiLog.Error("Serpent's Hand", $"Error while setting up player &3{player.Nickname}&r (&6{player.UserId}&r):\n{ex}");
        }
    }
}