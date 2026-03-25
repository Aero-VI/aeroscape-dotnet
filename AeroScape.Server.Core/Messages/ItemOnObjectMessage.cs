namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Use an inventory item on a world object (furnace, anvil, range, farming patch, etc.).
/// Parsed from the legacy ItemOnObject packet.
/// Fields: ObjectId (the object interacted with), ItemId (the item used).
/// </summary>
public record ItemOnObjectMessage(int ObjectId, int ItemId);
