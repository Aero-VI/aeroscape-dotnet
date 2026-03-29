using AeroScape.Server.API.Models;

namespace AeroScape.Server.API.Events;

/// <summary>
/// Fired when a player logs in.
/// </summary>
public class PlayerLoginEventArgs : EventArgs
{
    public required PlayerInfo Player { get; init; }
}

/// <summary>
/// Fired when a player logs out.
/// </summary>
public class PlayerLogoutEventArgs : EventArgs
{
    public required PlayerInfo Player { get; init; }
}

/// <summary>
/// Fired when a player walks/moves.
/// </summary>
public class PlayerWalkEventArgs : CancellableEventArgs
{
    public required PlayerInfo Player { get; init; }
    public required LocationInfo From { get; init; }
    public required LocationInfo To { get; init; }
}

/// <summary>
/// Fired when a player interacts with a game object.
/// </summary>
public class PlayerInteractObjectEventArgs : CancellableEventArgs
{
    public required PlayerInfo Player { get; init; }
    public required int ObjectId { get; init; }
    public required LocationInfo Location { get; init; }
    public required int Option { get; init; } // 1 = first option, 2 = second option, etc.
}

/// <summary>
/// Fired on each player tick/update cycle.
/// </summary>
public class PlayerTickEventArgs : EventArgs
{
    public required PlayerInfo Player { get; init; }
}

/// <summary>
/// Fired when a player gains skill experience.
/// </summary>
public class PlayerSkillXpEventArgs : CancellableEventArgs
{
    public required PlayerInfo Player { get; init; }
    public required int SkillId { get; init; }
    public double XpAmount { get; set; }  // Modifiable by event handlers
}

/// <summary>
/// Fired when a player receives an item.
/// </summary>
public class PlayerReceiveItemEventArgs : CancellableEventArgs
{
    public required PlayerInfo Player { get; init; }
    public required int ItemId { get; init; }
    public int Amount { get; set; }  // Modifiable by event handlers
}