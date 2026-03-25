namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Third NPC option (e.g. shop browse, teleport, tutor dialogue).
/// Parsed from the legacy NPCOption3 packet – reads an unsigned word big-endian (NPC index).
/// </summary>
public record NPCOption3Message(int NpcIndex);
