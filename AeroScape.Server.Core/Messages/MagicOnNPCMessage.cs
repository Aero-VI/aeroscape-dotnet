namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Cast a spell on an NPC (modern or ancient magicks).
/// Parsed from the legacy MagicOnNPC packet.
/// Fields: NpcIndex (target NPC slot), ButtonId (spell button within the spellbook),
///         InterfaceId (192 = modern, 193 = ancients).
/// </summary>
public record MagicOnNPCMessage(int NpcIndex, int ButtonId, int InterfaceId);
