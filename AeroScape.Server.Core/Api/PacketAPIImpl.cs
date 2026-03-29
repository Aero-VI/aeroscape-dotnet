using AeroScape.Server.API;

namespace AeroScape.Server.Core.Api;

/// <summary>
/// Implementation of IPacketAPI - stub for now.
/// </summary>
public class PacketAPIImpl : IPacketAPI
{
    public void RegisterIncomingHandler<TPacket>(Func<int, TPacket, bool> handler) where TPacket : class
    {
        // TODO: Implement packet handler registration
    }

    public void UnregisterIncomingHandler<TPacket>(Func<int, TPacket, bool> handler) where TPacket : class
    {
        // TODO: Implement packet handler unregistration
    }

    public void SendPacket(int playerId, object packet)
    {
        // TODO: Implement packet sending
    }

    public void SendPacket(IEnumerable<int> playerIds, object packet)
    {
        foreach (var playerId in playerIds)
        {
            SendPacket(playerId, packet);
        }
    }
}