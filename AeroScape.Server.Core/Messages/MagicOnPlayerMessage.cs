namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Cast a spell on another player (PvP magic).
/// Parsed from the legacy MagicOnPlayer packet.
/// Fields: TargetIndex (attacked player slot), PlayerId (target player id),
///         InterfaceId (spellbook: 388 = modern PvP), ButtonId (spell button).
/// </summary>
public record MagicOnPlayerMessage(int TargetIndex, int PlayerId, int InterfaceId, int ButtonId);
