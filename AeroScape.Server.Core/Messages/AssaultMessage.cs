namespace AeroScape.Server.Core.Messages;

/// <summary>
/// Barbarian Assault minigame action message.
/// </summary>
public record AssaultMessage(AssaultAction Action, int Wave)
{
    /// <summary>
    /// Initializes a GoIn action for the specified wave.
    /// </summary>
    public static AssaultMessage GoIn(int wave) => new(AssaultAction.GoIn, wave);

    /// <summary>
    /// Initializes a StartWave action for the specified wave.
    /// </summary>
    public static AssaultMessage Start(int wave) => new(AssaultAction.StartWave, wave);
}

public enum AssaultAction
{
    GoIn,
    StartWave,
    EndGame,
    PlayerDied,
    NpcDied,
}
