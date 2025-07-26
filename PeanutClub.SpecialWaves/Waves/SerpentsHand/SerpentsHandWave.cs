using LabExtended.API;
using LabExtended.API.CustomTeams;

using LabExtended.Utilities;

using PeanutClub.LoadoutAPI;

namespace PeanutClub.SpecialWaves.Waves.SerpentsHand;

/// <summary>
/// An instance of a spawned Serpents Hand wave.
/// </summary>
public class SerpentsHandWave : CustomTeamInstance<SerpentsHandTeam>
{
    /// <inheritdoc cref="CustomTeamInstance.OnSpawned"/>
    public override void OnSpawned()
    {
        base.OnSpawned();

        TimingUtils.AfterSeconds(() =>
        {
            for (var i = 0; i < AlivePlayers.Count; i++)
            {
                SetupPlayer(AlivePlayers[i]);
            }
        }, 0.3f);
    }

    private void SetupPlayer(ExPlayer player)
    {
        LoadoutPlugin.TryApply(player, "SerpentsHand");
        
        player.CustomInfo = "Serpent's Hand";

        if ((player.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
            player.InfoArea |= PlayerInfoArea.CustomInfo;
    }
}