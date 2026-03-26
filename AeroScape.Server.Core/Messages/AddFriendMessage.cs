namespace AeroScape.Server.Core.Messages;

/// <summary>Add a friend to the friends list (opcode 30). Name is a long-encoded RS name.</summary>
public record AddFriendMessage(long EncodedName);
