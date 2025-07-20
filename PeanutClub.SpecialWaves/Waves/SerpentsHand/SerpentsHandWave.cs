using LabExtended.API;
using LabExtended.API.CustomTeams;

using PeanutClub.SpecialWaves.Loadouts;

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

        for (var i = 0; i < AlivePlayers.Count; i++)
        {
            SetupPlayer(AlivePlayers[i]);
        }
    }

    private void SetupPlayer(ExPlayer player)
    {
        player.ApplyLoadout("SerpentsHand");
        player.CustomInfo = "Serpent's Hand";

        if ((player.InfoArea & PlayerInfoArea.CustomInfo) != PlayerInfoArea.CustomInfo)
            player.InfoArea |= PlayerInfoArea.CustomInfo;
    }
}