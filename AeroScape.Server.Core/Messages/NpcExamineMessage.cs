namespace AeroScape.Server.Core.Messages;

/// <summary>Player examines an NPC (opcode 88).</summary>
public record NpcExamineMessage(int NpcId);
