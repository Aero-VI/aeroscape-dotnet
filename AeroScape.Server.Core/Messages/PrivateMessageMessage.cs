namespace AeroScape.Server.Core.Messages;

/// <summary>Private message sent from one player to another (opcode 178).</summary>
public record PrivateMessageMessage(long TargetName, string Text);
