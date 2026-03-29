namespace AeroScape.Server.API.Models;

/// <summary>
/// Represents a location in the game world.
/// </summary>
public class LocationInfo
{
    /// <summary>
    /// X coordinate (absolute).
    /// </summary>
    public required int X { get; init; }
    
    /// <summary>
    /// Y coordinate (absolute).
    /// </summary>
    public required int Y { get; init; }
    
    /// <summary>
    /// Height level (0 = ground floor).
    /// </summary>
    public required int HeightLevel { get; init; }
    
    /// <summary>
    /// Calculate distance to another location (ignoring height).
    /// </summary>
    public double DistanceTo(LocationInfo other)
    {
        int dx = X - other.X;
        int dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
    
    /// <summary>
    /// Check if this location is within a certain distance of another.
    /// </summary>
    public bool IsWithinDistance(LocationInfo other, int distance)
    {
        return Math.Abs(X - other.X) <= distance && 
               Math.Abs(Y - other.Y) <= distance;
    }
    
    public override string ToString() => $"({X}, {Y}, {HeightLevel})";
}