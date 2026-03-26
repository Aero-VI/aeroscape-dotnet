using System.Collections.Frozen;
using System.Collections.Generic;

namespace AeroScape.Server.Core.Combat;

/// <summary>
/// Magic spell definitions ported from MagicNPC.java.
/// Maps interface button IDs to internal spell IDs, and stores all per-spell data
/// (level requirements, base XP, rune costs, GFX).
/// </summary>
public static class MagicSpellData
{
    // ── Rune item IDs ──────────────────────────────────────────────────────
    public const int RuneFire = 554;
    public const int RuneWater = 555;
    public const int RuneAir = 556;
    public const int RuneEarth = 557;
    public const int RuneMind = 558;
    public const int RuneBody = 559;
    public const int RuneDeath = 560;
    public const int RuneNature = 561;
    public const int RuneChaos = 562;
    public const int RuneLaw = 563;
    public const int RuneCosmic = 564;
    public const int RuneBlood = 565;
    public const int RuneSoul = 566;

    // ── Elemental staff item IDs → which rune they provide ─────────────────
    public static readonly FrozenDictionary<int, int> StaffRuneMap =
        new Dictionary<int, int>
        {
            [1381] = RuneAir,   // Staff of Air
            [1383] = RuneWater, // Staff of Water
            [1385] = RuneEarth, // Staff of Earth
            [1387] = RuneFire,  // Staff of Fire
        }.ToFrozenDictionary();

    /// <summary>Staves that allow autocasting.</summary>
    private static readonly FrozenSet<int> _autocastStaves = new HashSet<int>
    {
        1379, 1381, 1383, 1385, 1387,
    }.ToFrozenSet();

    public static bool IsAutocastStaff(int weaponId) => _autocastStaves.Contains(weaponId);

    // ── Button → internal spell ID mapping ─────────────────────────────────
    /// <summary>
    /// Maps the interface button ID to an internal spell list index (1-16).
    /// Ported from MagicNPC.getSpellId.
    /// </summary>
    public static readonly FrozenDictionary<int, int> ButtonToSpellId =
        new Dictionary<int, int>
        {
            [129] = 1,  // Wind Strike
            [132] = 2,  // Water Strike
            [134] = 3,  // Earth Strike
            [136] = 4,  // Fire Strike
            [138] = 5,  // Wind Bolt
            [142] = 6,  // Water Bolt
            [145] = 7,  // Earth Bolt
            [148] = 8,  // Fire Bolt
            [152] = 9,  // Wind Blast
            [155] = 10, // Water Blast
            [161] = 11, // Earth Blast
            [166] = 12, // Fire Blast
            [173] = 13, // Wind Wave
            [176] = 14, // Water Wave
            [180] = 15, // Earth Wave
            [183] = 16, // Fire Wave
        }.ToFrozenDictionary();

    // ── Spell definitions (indexed by spell ID 1–16) ───────────────────────

    /// <summary>All 16 standard combat spells.</summary>
    public static readonly SpellDefinition[] Spells = BuildSpellTable();

    private static SpellDefinition[] BuildSpellTable()
    {
        // Index 0 is unused (spell IDs are 1-based)
        var spells = new SpellDefinition[17];

        // Wind Strike (1)
        spells[1] = new SpellDefinition(1, "Wind Strike", 1, 25, 90,
            new RuneRequirement(RuneAir, 1), new RuneRequirement(RuneMind, 1));

        // Water Strike (2)
        spells[2] = new SpellDefinition(2, "Water Strike", 5, 35, 93,
            new RuneRequirement(RuneWater, 1), new RuneRequirement(RuneAir, 1), new RuneRequirement(RuneMind, 1));

        // Earth Strike (3)
        spells[3] = new SpellDefinition(3, "Earth Strike", 9, 60, 96,
            new RuneRequirement(RuneEarth, 2), new RuneRequirement(RuneAir, 1), new RuneRequirement(RuneMind, 1));

        // Fire Strike (4)
        spells[4] = new SpellDefinition(4, "Fire Strike", 13, 80, 99,
            new RuneRequirement(RuneFire, 3), new RuneRequirement(RuneAir, 2), new RuneRequirement(RuneMind, 1));

        // Wind Bolt (5)
        spells[5] = new SpellDefinition(5, "Wind Bolt", 17, 110, 117,
            new RuneRequirement(RuneAir, 2), new RuneRequirement(RuneChaos, 1));

        // Water Bolt (6)
        spells[6] = new SpellDefinition(6, "Water Bolt", 23, 140, 120,
            new RuneRequirement(RuneWater, 2), new RuneRequirement(RuneAir, 2), new RuneRequirement(RuneChaos, 1));

        // Earth Bolt (7)
        spells[7] = new SpellDefinition(7, "Earth Bolt", 29, 170, 123,
            new RuneRequirement(RuneEarth, 3), new RuneRequirement(RuneAir, 2), new RuneRequirement(RuneChaos, 1));

        // Fire Bolt (8)
        spells[8] = new SpellDefinition(8, "Fire Bolt", 35, 200, 126,
            new RuneRequirement(RuneFire, 4), new RuneRequirement(RuneAir, 3), new RuneRequirement(RuneChaos, 1));

        // Wind Blast (9)
        spells[9] = new SpellDefinition(9, "Wind Blast", 41, 215, 132,
            new RuneRequirement(RuneAir, 3), new RuneRequirement(RuneDeath, 1));

        // Water Blast (10)
        spells[10] = new SpellDefinition(10, "Water Blast", 47, 220, 135,
            new RuneRequirement(RuneWater, 3), new RuneRequirement(RuneAir, 3), new RuneRequirement(RuneDeath, 1));

        // Earth Blast (11)
        spells[11] = new SpellDefinition(11, "Earth Blast", 53, 235, 138,
            new RuneRequirement(RuneEarth, 4), new RuneRequirement(RuneAir, 3), new RuneRequirement(RuneDeath, 1));

        // Fire Blast (12)
        spells[12] = new SpellDefinition(12, "Fire Blast", 59, 250, 129,
            new RuneRequirement(RuneFire, 5), new RuneRequirement(RuneAir, 4), new RuneRequirement(RuneDeath, 1));

        // Wind Wave (13)
        spells[13] = new SpellDefinition(13, "Wind Wave", 62, 350, 158,
            new RuneRequirement(RuneAir, 5), new RuneRequirement(RuneBlood, 1));

        // Water Wave (14)
        spells[14] = new SpellDefinition(14, "Water Wave", 65, 360, 161,
            new RuneRequirement(RuneWater, 7), new RuneRequirement(RuneAir, 5), new RuneRequirement(RuneBlood, 1));

        // Earth Wave (15)
        spells[15] = new SpellDefinition(15, "Earth Wave", 70, 380, 164,
            new RuneRequirement(RuneEarth, 7), new RuneRequirement(RuneAir, 5), new RuneRequirement(RuneBlood, 1));

        // Fire Wave (16)
        spells[16] = new SpellDefinition(16, "Fire Wave", 75, 450, 155,
            new RuneRequirement(RuneFire, 7), new RuneRequirement(RuneAir, 5), new RuneRequirement(RuneBlood, 1));

        return spells;
    }
}

/// <summary>
/// A single rune requirement for a spell.
/// </summary>
public readonly record struct RuneRequirement(int RuneId, int Amount);

/// <summary>
/// Full definition of a standard combat spell.
/// </summary>
public sealed class SpellDefinition
{
    public int SpellId { get; }
    public string Name { get; }
    public int LevelRequired { get; }
    public double BaseXp { get; }
    public int CasterGfx { get; }
    public int VictimGfx => CasterGfx + 2;
    public RuneRequirement[] RuneRequirements { get; }

    public SpellDefinition(int spellId, string name, int level, double baseXp, int casterGfx,
        params RuneRequirement[] runes)
    {
        SpellId = spellId;
        Name = name;
        LevelRequired = level;
        BaseXp = baseXp;
        CasterGfx = casterGfx;
        RuneRequirements = runes;
    }

    /// <summary>
    /// Calculate total XP for a spell hit.
    /// Ported from MagicNPC.getExpByHit.
    /// </summary>
    public double GetXpForHit(int hitDamage) =>
        (BaseXp + hitDamage) * CombatConstants.MagicXpRate;
}
