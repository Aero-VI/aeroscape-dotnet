namespace AeroScape.Server.Core.Messages;

/// <summary>Add a player to the ignore list (opcode 61).</summary>
public record AddIgnoreMessage(long EncodedName);
