namespace AeroScape.Server.Core.Combat;

/// <summary>
/// Represents a pending hit to be applied to an entity.
/// Used to queue hits that may be delayed (e.g. projectile travel time).
/// </summary>
public sealed class HitSplat
{
    /// <summary>Damage amount (capped to target's HP on application).</summary>
    public int Damage { get; set; }

    /// <summary>Hit type: 0 = normal, 1 = poison.</summary>
    public int HitType { get; set; }

    /// <summary>Ticks until this hit is applied (0 = immediate).</summary>
    public int Delay { get; set; }

    /// <summary>The source player index (for kill attribution).</summary>
    public int SourcePlayerId { get; set; }

    /// <summary>Combat type that caused this hit.</summary>
    public CombatType CombatType { get; set; }

    public HitSplat(int damage, int hitType, int delay = 0, int sourcePlayerId = 0,
        CombatType combatType = CombatType.Melee)
    {
        Damage = damage;
        HitType = hitType;
        Delay = delay;
        SourcePlayerId = sourcePlayerId;
        CombatType = combatType;
    }
}
