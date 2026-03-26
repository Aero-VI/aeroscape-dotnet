namespace AeroScape.Server.Core.Combat;

/// <summary>
/// Attack style chosen by the player (from the weapon interface).
/// Determines which skill receives XP.
/// </summary>
public enum CombatStyle
{
    /// <summary>Accurate — XP to Attack.</summary>
    Accurate = 0,

    /// <summary>Aggressive — XP to Strength.</summary>
    Aggressive = 1,

    /// <summary>Defensive — XP to Defence.</summary>
    Defensive = 2,

    /// <summary>Controlled — shared XP to Attack, Strength, Defence.</summary>
    Controlled = 3,
}

/// <summary>
/// The type of combat being used.
/// </summary>
public enum CombatType
{
    Melee,
    Ranged,
    Magic,
}
