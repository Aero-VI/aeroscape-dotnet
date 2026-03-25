namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Use an inventory item on an NPC.
/// Parsed from the legacy ItemOnNPC packet.
/// Fields: ItemId, NpcIndex, InterfaceId (the component the item was used from).
/// </summary>
public record ItemOnNPCMessage(int ItemId, int NpcIndex, int InterfaceId);
