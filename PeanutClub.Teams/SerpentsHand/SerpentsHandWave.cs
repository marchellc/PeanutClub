using LabExtended.API;
using LabExtended.API.CustomTeams;

using LabExtended.Core;
using LabExtended.Utilities;

using PeanutClub.LoadoutAPI;
using PeanutClub.OverlayAPI.Alerts;
using PlayerRoles;

namespace PeanutClub.Teams.SerpentsHand;

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
            player.Role.Set(role, RoleChangeReason.Respawn, RoleSpawnFlags.UseSpawnpoint);

            TimingUtils.AfterSeconds(() =>
            {
                player.IsGodModeEnabled = false;
                player.CustomInfo = "Serpent's Hand";
                player.Position.Position = SerpentsHandTeam.SpawnPosition;

                if ((player.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
                    player.InfoArea |= PlayerInfoArea.CustomInfo;

                LoadoutPlugin.TryApply(player, "SerpentsHand");
                
                player.SendAlert(AlertType.Info, 10f, 
                    $"<b>Jsi člen týmu</b>\n" +
                    $"<b><size=30><color=red>Serpent's Hand</color></size></b>!\n" +
                    $"Tvým objektivem je <b>pomáhat <color=red>SCP</color></b> v přežití, všechny <b>ostatní týmy jsou <color=red>nepřátelské</color></b>.");
            }, 0.2f);
        }
        catch (Exception ex)
        {
            ApiLog.Error("Serpent's Hand", $"Error while setting up player &3{player.Nickname}&r (&6{player.UserId}&r):\n{ex}");
        }
    }
}