namespace AeroScape.Server.Core.Combat;

/// <summary>
/// Global combat constants ported from the legacy Java codebase.
/// </summary>
public static class CombatConstants
{
    /// <summary>XP multiplier for combat hits.</summary>
    public const int CombatXpRate = 25;

    /// <summary>XP multiplier for magic spell casts.</summary>
    public const int MagicXpRate = 5;

    /// <summary>Default magic cast cooldown in ticks.</summary>
    public const int MagicCastDelay = 3;

    /// <summary>Maximum ranged attack distance (tiles).</summary>
    public const int MaxRangeDistance = 10;

    /// <summary>Skull duration in ticks when initiating PvP combat (180 ticks ≈ 108 seconds).</summary>
    public const int SkullDuration = 180;

    // ── Skill indices ──────────────────────────────────────────────────────
    public const int SkillAttack = 0;
    public const int SkillDefence = 1;
    public const int SkillStrength = 2;
    public const int SkillHitpoints = 3;
    public const int SkillRanged = 4;
    public const int SkillPrayer = 5;
    public const int SkillMagic = 6;
    public const int SkillSlayer = 18;

    // ── Equipment slot indices ─────────────────────────────────────────────
    public const int SlotHead = 0;
    public const int SlotCape = 1;
    public const int SlotAmulet = 2;
    public const int SlotWeapon = 3;
    public const int SlotChest = 4;
    public const int SlotShield = 5;
    public const int SlotLegs = 7;
    public const int SlotHands = 9;
    public const int SlotFeet = 10;
    public const int SlotRing = 12;
    public const int SlotAmmo = 13;

    // ── Equipment bonus indices ────────────────────────────────────────────
    public const int BonusStabAttack = 0;
    public const int BonusSlashAttack = 1;
    public const int BonusCrushAttack = 2;
    public const int BonusMagicAttack = 3;
    public const int BonusRangeAttack = 4;
    public const int BonusStabDefence = 5;
    public const int BonusSlashDefence = 6;
    public const int BonusCrushDefence = 7;
    public const int BonusMagicDefence = 8;
    public const int BonusRangeDefence = 9;
    public const int BonusStrength = 10;
    public const int BonusPrayer = 11;
}
