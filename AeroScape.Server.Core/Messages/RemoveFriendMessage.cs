namespace AeroScape.Server.Core.Messages;

/// <summary>Remove a friend from the friends list (opcode 132).</summary>
public record RemoveFriendMessage(long EncodedName);
