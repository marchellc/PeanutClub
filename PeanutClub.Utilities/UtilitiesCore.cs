namespace PeanutClub.Utilities;

using CentralAuth;

using Items;

using LabExtended.API;
using LabExtended.API.Containers;

using Mirror;

using NetworkManagerUtils.Dummies;

using PeanutClub.Utilities.Features;

/// <summary>
/// The main class of this library.
/// </summary>
public static class UtilitiesCore
{
    private static bool hasInitialized;
    
    /// <summary>
    /// Must be called at least once.
    /// </summary>
    public static void Initialize()
    {
        if (!hasInitialized)
        {
            ItemTags.Internal_Init();
            SnakeExplosion.Internal_Init();
            
            hasInitialized = true;
        }
    }

    /// <summary>
    /// Spawns a NPC hidden from the spectator list.
    /// </summary>
    /// <param name="npcNick">The nickname to assign to the spawned NPC. If not specified, defaults to "Dummy".</param>
    /// <returns>An ExPlayer instance representing the newly spawned hidden NPC.</returns>
    public static ExPlayer SpawnHiddenNpc(string npcNick = "Dummy")
    {
        var hubGo = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
        var hub = hubGo.GetComponent<ReferenceHub>();

        NetworkServer.AddPlayerForConnection(new DummyNetworkConnection(), hubGo);

        hub.nicknameSync.MyNick = "Dealer";

        hub.authManager.NetworkSyncedUserId = "ID_Dedicated";
        hub.authManager.syncMode = (SyncMode)ClientInstanceMode.DedicatedServer;

        var npc = ExPlayer.Get(hub) ?? new ExPlayer(hub, SwitchContainer.GetNewNpcToggles(true));

        npc.Toggles.ShouldSendPosition = true;
        npc.Toggles.IsVisibleInRemoteAdmin = true;

        return npc;
    }
}