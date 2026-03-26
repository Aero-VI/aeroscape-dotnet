namespace AeroScape.Server.Core.Messages;

/// <summary>Player examines an item (opcode 38).</summary>
public record ItemExamineMessage(int ItemId);
