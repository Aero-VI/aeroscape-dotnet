namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Second NPC option (e.g. Pickpocket, second Trade, Harpoon, Bank).
/// Parsed from the legacy NPCOption2 packet – reads an unsigned word big-endian + A (NPC index).
/// </summary>
public record NPCOption2Message(int NpcIndex);
