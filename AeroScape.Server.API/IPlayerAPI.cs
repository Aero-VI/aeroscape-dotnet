using AeroScape.Server.API.Models;

namespace AeroScape.Server.API;

/// <summary>
/// Player management API for plugins.
/// </summary>
public interface IPlayerAPI
{
    /// <summary>Get player by username</summary>
    PlayerInfo? GetPlayer(string username);
    
    /// <summary>Get player by ID</summary>
    PlayerInfo? GetPlayer(int playerId);
    
    /// <summary>Get all online players</summary>
    IReadOnlyList<PlayerInfo> GetOnlinePlayers();
    
    /// <summary>Send message to player</summary>
    void SendMessage(PlayerInfo player, string message);
    
    /// <summary>Send message to player</summary>
    void SendMessage(int playerId, string message);
    
    /// <summary>Play animation for player</summary>
    void PlayAnimation(PlayerInfo player, int animationId, int delay = 0);
    
    /// <summary>Play animation for player</summary>
    void PlayAnimation(int playerId, int animationId, int delay = 0);
    
    /// <summary>Play graphic effect for player</summary>
    void PlayGraphic(PlayerInfo player, int graphicId, int height = 0, int delay = 0);
    
    /// <summary>Play graphic effect for player</summary>
    void PlayGraphic(int playerId, int graphicId, int height = 0, int delay = 0);
    
    /// <summary>Force player to walk to a location</summary>
    void WalkTo(PlayerInfo player, LocationInfo location);
    
    /// <summary>Force player to walk to a location</summary>  
    void WalkTo(int playerId, LocationInfo location);
    
    /// <summary>Teleport player to a location</summary>
    void Teleport(PlayerInfo player, LocationInfo location);
    
    /// <summary>Teleport player to a location</summary>
    void Teleport(int playerId, LocationInfo location);
}