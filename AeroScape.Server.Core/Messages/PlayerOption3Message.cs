namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Third player option (e.g. Duel / Clan challenge).
/// Parsed from the legacy PlayerOption3 packet – reads an unsigned word big-endian + A (player index).
/// </summary>
public record PlayerOption3Message(int TargetIndex);
