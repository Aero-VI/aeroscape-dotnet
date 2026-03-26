namespace AeroScape.Server.Core.Messages;

/// <summary>Player examines an object (opcode 84).</summary>
public record ObjectExamineMessage(int ObjectId);
