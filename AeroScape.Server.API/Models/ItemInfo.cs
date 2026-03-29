namespace AeroScape.Server.API.Models;

/// <summary>
/// Read-only information about an item definition.
/// </summary>
public class ItemInfo
{
    /// <summary>
    /// Item ID.
    /// </summary>
    public required int Id { get; init; }
    
    /// <summary>
    /// Item name.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// Item examine text.
    /// </summary>
    public required string Examine { get; init; }
    
    /// <summary>
    /// Whether the item is stackable.
    /// </summary>
    public required bool Stackable { get; init; }
    
    /// <summary>
    /// Item value in shops.
    /// </summary>
    public required int Value { get; init; }
    
    /// <summary>
    /// Whether the item is members-only.
    /// </summary>
    public required bool MembersOnly { get; init; }
    
    /// <summary>
    /// Item weight.
    /// </summary>
    public required double Weight { get; init; }
}