namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Second player option (e.g. Trade).
/// Parsed from the legacy PlayerOption2 packet – reads an unsigned word (player index).
/// </summary>
public record PlayerOption2Message(int TargetIndex);
