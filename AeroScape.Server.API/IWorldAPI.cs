using AeroScape.Server.API.Models;

namespace AeroScape.Server.API;

/// <summary>
/// World and object interaction API for plugins.
/// </summary>
public interface IWorldAPI
{
    /// <summary>Check if an object exists at a location</summary>
    bool HasObjectAt(int objectId, LocationInfo location);
    
    /// <summary>Check if any object exists at a location</summary>
    bool HasObjectAt(LocationInfo location);
    
    /// <summary>Spawn a temporary object in the world</summary>
    void SpawnObject(int objectId, LocationInfo location, int type = 10, int rotation = 0, int durationTicks = 500);
    
    /// <summary>Remove an object from the world</summary>
    void RemoveObject(int objectId, LocationInfo location);
    
    /// <summary>Spawn a ground item</summary>
    void SpawnGroundItem(int itemId, int amount, LocationInfo location, string? ownerUsername = null, int durationTicks = 300);
    
    /// <summary>Remove a ground item</summary>
    void RemoveGroundItem(int itemId, LocationInfo location);
    
    /// <summary>Get all players at a location</summary>
    IReadOnlyList<PlayerInfo> GetPlayersAt(LocationInfo location, int radius = 0);
    
    /// <summary>Get all players in a region</summary>
    IReadOnlyList<PlayerInfo> GetPlayersInRegion(int regionX, int regionY);
    
    /// <summary>Play a sound effect at a location</summary>
    void PlaySound(int soundId, LocationInfo location, int radius = 10);
    
    /// <summary>Create a projectile</summary>
    void CreateProjectile(LocationInfo from, LocationInfo to, int projectileId, int startHeight, int endHeight, int speed, int angle, int startDistanceOffset);
}