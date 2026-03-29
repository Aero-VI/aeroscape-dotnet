namespace AeroScape.Server.API;

/// <summary>
/// Low-level packet handling API for plugins.
/// </summary>
public interface IPacketAPI
{
    /// <summary>
    /// Register a handler for incoming packets of a specific type.
    /// Return true from handler to prevent default processing.
    /// </summary>
    void RegisterIncomingHandler<TPacket>(Func<int, TPacket, bool> handler) where TPacket : class;
    
    /// <summary>
    /// Unregister an incoming packet handler.
    /// </summary>
    void UnregisterIncomingHandler<TPacket>(Func<int, TPacket, bool> handler) where TPacket : class;
    
    /// <summary>
    /// Send a packet to a specific player.
    /// </summary>
    void SendPacket(int playerId, object packet);
    
    /// <summary>
    /// Send a packet to multiple players.
    /// </summary>
    void SendPacket(IEnumerable<int> playerIds, object packet);
}