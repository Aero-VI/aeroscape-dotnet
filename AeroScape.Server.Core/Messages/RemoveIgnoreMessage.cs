namespace AeroScape.Server.Core.Messages;

/// <summary>Remove a player from the ignore list (opcode 2).</summary>
public record RemoveIgnoreMessage(long EncodedName);
