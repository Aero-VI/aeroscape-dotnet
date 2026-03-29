using AeroScape.Server.API;
using AeroScape.Server.API.Models;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Api;

/// <summary>
/// Implementation of IWorldAPI that wraps core world services.
/// </summary>
public class WorldAPIImpl : IWorldAPI
{
    private readonly GameEngine _engine;
    private readonly GroundItemManager _groundItems;

    public WorldAPIImpl(GameEngine engine, GroundItemManager groundItems)
    {
        _engine = engine;
        _groundItems = groundItems;
    }

    public bool HasObjectAt(int objectId, LocationInfo location)
    {
        return _engine.LoadedObjects.Any(o => 
            o.ObjectId == objectId && 
            o.X == location.X && 
            o.Y == location.Y);
    }

    public bool HasObjectAt(LocationInfo location)
    {
        return _engine.LoadedObjects.Any(o => 
            o.X == location.X && 
            o.Y == location.Y);
    }

    public void SpawnObject(int objectId, LocationInfo location, int type = 10, int rotation = 0, int durationTicks = 500)
    {
        // TODO: Implement temporary object spawning
    }

    public void RemoveObject(int objectId, LocationInfo location)
    {
        // TODO: Implement object removal
    }

    public void SpawnGroundItem(int itemId, int amount, LocationInfo location, string? ownerUsername = null, int durationTicks = 300)
    {
        Player? owner = null;
        if (!string.IsNullOrEmpty(ownerUsername))
        {
            owner = _engine.OnlinePlayers.FirstOrDefault(p => 
                p.Username.Equals(ownerUsername, StringComparison.OrdinalIgnoreCase));
        }
        
        _groundItems.CreateGroundItem(itemId, amount, location.X, location.Y, location.HeightLevel, ownerUsername ?? "");
    }

    public void RemoveGroundItem(int itemId, LocationInfo location)
    {
        // TODO: Implement ground item removal by location
    }

    public IReadOnlyList<PlayerInfo> GetPlayersAt(LocationInfo location, int radius = 0)
    {
        return _engine.OnlinePlayers
            .Where(p => Math.Abs(p.AbsX - location.X) <= radius && 
                       Math.Abs(p.AbsY - location.Y) <= radius)
            .Select(MapToPlayerInfo)
            .ToList();
    }

    public IReadOnlyList<PlayerInfo> GetPlayersInRegion(int regionX, int regionY)
    {
        // Calculate absolute coordinates from region
        int baseX = regionX * 64;
        int baseY = regionY * 64;
        
        return _engine.OnlinePlayers
            .Where(p => p.AbsX >= baseX && p.AbsX < baseX + 64 &&
                       p.AbsY >= baseY && p.AbsY < baseY + 64)
            .Select(MapToPlayerInfo)
            .ToList();
    }

    public void PlaySound(int soundId, LocationInfo location, int radius = 10)
    {
        // TODO: Implement sound effects
    }

    public void CreateProjectile(LocationInfo from, LocationInfo to, int projectileId, int startHeight, int endHeight, int speed, int angle, int startDistanceOffset)
    {
        // TODO: Implement projectiles
    }
    
    private static PlayerInfo MapToPlayerInfo(Player player)
    {
        return new PlayerInfo
        {
            Id = player.PlayerId,
            Username = player.Username,
            CombatLevel = player.CombatLevel,
            Location = new LocationInfo
            {
                X = player.AbsX,
                Y = player.AbsY,
                HeightLevel = player.HeightLevel
            },
            CurrentHitpoints = player.SkillLvl[3],
            MaxHitpoints = player.GetLevelForXP(3),
            InCombat = false,
            AnimationId = player.AnimReq
        };
    }
}