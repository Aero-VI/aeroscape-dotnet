using AeroScape.Server.API.Models;

namespace AeroScape.Server.API.Events;

/// <summary>
/// Fired when a game object spawns in the world.
/// </summary>
public class ObjectSpawnEventArgs : CancellableEventArgs
{
    public required int ObjectId { get; init; }
    public required LocationInfo Location { get; init; }
    public required int Type { get; init; }
    public required int Rotation { get; init; }
}

/// <summary>
/// Fired when a game object is removed from the world.
/// </summary>
public class ObjectDespawnEventArgs : CancellableEventArgs
{
    public required int ObjectId { get; init; }
    public required LocationInfo Location { get; init; }
}

/// <summary>
/// Fired when a ground item is spawned.
/// </summary>
public class GroundItemSpawnEventArgs : CancellableEventArgs
{
    public required int ItemId { get; init; }
    public required int Amount { get; init; }
    public required LocationInfo Location { get; init; }
    public required string? OwnerUsername { get; init; } // null = visible to all
}

/// <summary>
/// Fired when a ground item is removed.
/// </summary>
public class GroundItemDespawnEventArgs : CancellableEventArgs
{
    public required int ItemId { get; init; }
    public required LocationInfo Location { get; init; }
}