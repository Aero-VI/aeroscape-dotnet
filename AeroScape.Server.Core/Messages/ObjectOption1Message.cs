namespace AeroScape.Server.Core.Messages;

/// <summary>
/// First option on a game object (e.g. Open, Climb, Chop, Mine, Enter).
/// Parsed from the legacy ObjectOption1 packet.
/// Fields read: ObjectX (wordBigEndian), ObjectId (word), ObjectY (wordBigEndianA).
/// </summary>
public record ObjectOption1Message(int ObjectId, int ObjectX, int ObjectY);
