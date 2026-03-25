namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Player selects/uses an item in their inventory (first click option).
/// Parsed from the legacy ItemSelect packet.
/// Fields read: Junk (byte), InterfaceId (word), Junk (byte), ItemId (wordBigEndian), ItemSlot (wordA).
/// </summary>
public record ItemSelectMessage(int InterfaceId, int ItemId, int ItemSlot);
