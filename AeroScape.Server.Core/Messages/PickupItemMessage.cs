namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Player picks up a ground item.
/// Parsed from the legacy PickupItem packet.
/// Fields read: ItemY (wordA), ItemX (word), ItemId (wordBigEndianA).
/// </summary>
public record PickupItemMessage(int ItemId, int ItemX, int ItemY);
