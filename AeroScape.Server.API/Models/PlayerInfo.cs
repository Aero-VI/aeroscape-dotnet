namespace AeroScape.Server.API.Models;

/// <summary>
/// Read-only information about a player exposed to plugins.
/// </summary>
public class PlayerInfo
{
    /// <summary>
    /// Unique player ID in the database.
    /// </summary>
    public required int Id { get; init; }
    
    /// <summary>
    /// Player's username.
    /// </summary>
    public required string Username { get; init; }
    
    /// <summary>
    /// Player's combat level.
    /// </summary>
    public required int CombatLevel { get; init; }
    
    /// <summary>
    /// Player's current location.
    /// </summary>
    public required LocationInfo Location { get; init; }
    
    /// <summary>
    /// Player's current hitpoints.
    /// </summary>
    public required int CurrentHitpoints { get; init; }
    
    /// <summary>
    /// Player's maximum hitpoints.
    /// </summary>
    public required int MaxHitpoints { get; init; }
    
    /// <summary>
    /// Whether the player is currently in combat.
    /// </summary>
    public required bool InCombat { get; init; }
    
    /// <summary>
    /// Player's current animation ID (-1 if none).
    /// </summary>
    public required int AnimationId { get; init; }
}