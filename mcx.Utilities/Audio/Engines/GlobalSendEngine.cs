using LabApi.Features.Wrappers;

using LabExtended.API;

using Mirror;

using SecretLabNAudio.Core.SendEngines;

using VoiceChat.Networking;

using SpeakerToy = AdminToys.SpeakerToy;

namespace mcx.Utilities.Audio.Engines
{
    public class GlobalSendEngine : SendEngine, IDisposable
    {
        public static int RpcHash { get; } = MirrorMethods.TryGetRpcHash(typeof(SpeakerToy), nameof(SpeakerToy.RpcChangeParent), out var hash)
            ? hash
            : 0;

        public HashSet<string> SyncedPlayers { get; } = new();

        public SpeakerToy Speaker { get; }

        public GlobalSendEngine(SpeakerToy speaker)
        {
            Speaker = speaker;
        }

        protected override bool Broadcast(Player player, AudioMessage message)
        {
            if (SyncedPlayers.Add(player.UserId))
                player.ReferenceHub.connectionToClient.Send(Speaker.GetRpcMessage(RpcHash, writer => writer.WriteUInt(player.NetworkId)));

            return base.Broadcast(player, message);
        }

        public void Dispose()
        {
            foreach (var player in ExPlayer.Players)
            {
                if (player?.ReferenceHub == null)
                    continue;

                if (SyncedPlayers.Remove(player.UserId))
                    player.ReferenceHub.connectionToClient.Send(Speaker.GetRpcMessage(RpcHash, writer => writer.WriteUInt(0)));
            }

            SyncedPlayers.Clear();
        }
    }
}