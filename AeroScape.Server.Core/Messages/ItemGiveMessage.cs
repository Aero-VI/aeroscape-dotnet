namespace AeroScape.Server.Core.Messages;

/// <summary>
/// A player attempts to give (use) an item on another player.
/// Parsed from the legacy ItemGive packet.
/// Fields: TargetPlayerIndex (readSignedWordA), ItemId (readSignedWordBigEndian).
/// </summary>
public record ItemGiveMessage(int TargetPlayerIndex, int ItemId);
