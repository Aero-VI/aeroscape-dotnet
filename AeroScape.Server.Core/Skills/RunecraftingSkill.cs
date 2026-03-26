using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Runecrafting skill — ported from ObjectOption1.java altar interactions.
///
/// Runecrafting works by clicking on a runecrafting altar (object IDs 2478-2489).
/// All rune essence (item 1436) in inventory is consumed and converted to runes.
/// A multiplier formula gives bonus runes at higher levels:
///   runesPerEssence = floor((rcLevel - reqLevel) / 20) + 1
/// </summary>
public class RunecraftingSkill
{
    private readonly Player _player;

    public RunecraftingSkill(Player player)
    {
        _player = player;
    }

    /// <summary>Rune Essence item ID.</summary>
    public const int RuneEssenceItemId = 1436;

    /// <summary>Pure Essence item ID (used for higher-level runes).</summary>
    public const int PureEssenceItemId = 7936;

    // ── Altar definitions ───────────────────────────────────────────────────

    /// <param name="AltarObjectId">Object ID of the runecrafting altar.</param>
    /// <param name="RuneItemId">Item ID of the rune produced.</param>
    /// <param name="LevelRequired">Minimum Runecrafting level.</param>
    /// <param name="BaseXpPerEssence">XP per essence crafted (before level scaling).</param>
    /// <param name="Name">Display name of the rune.</param>
    public record AltarDefinition(
        int AltarObjectId,
        int RuneItemId,
        int LevelRequired,
        int BaseXpPerEssence,
        string Name);

    public static readonly AltarDefinition[] Altars =
    [
        new(2478, 556, 1,  15, "Air rune"),
        new(2479, 558, 2,  15, "Mind rune"),
        new(2480, 555, 5,  15, "Water rune"),
        new(2481, 557, 9,  15, "Earth rune"),
        new(2482, 554, 14, 15, "Fire rune"),
        new(2483, 559, 20, 15, "Body rune"),
        new(2484, 564, 27, 15, "Cosmic rune"),
        new(2487, 562, 35, 15, "Chaos rune"),
        new(2486, 561, 44, 15, "Nature rune"),
        new(2485, 563, 54, 15, "Law rune"),
        new(2488, 560, 65, 15, "Death rune"),
        new(2489, 565, 78, 15, "Blood rune"),
    ];

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Craft runes at an altar. Consumes all rune essence in inventory.
    /// Called from ObjectOption1 handler.
    /// </summary>
    /// <param name="altarObjectId">The altar object ID clicked.</param>
    /// <returns>True if any runes were crafted.</returns>
    public bool CraftRunes(int altarObjectId)
    {
        var altar = FindAltar(altarObjectId);
        if (altar == null)
            return false;

        int rcLevel = _player.GetLevelForXP(SkillConstants.Runecrafting);
        if (rcLevel < altar.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need a RuneCrafting level of {altar.LevelRequired} to craft {altar.Name}s!");
            return false;
        }

        // Count rune essence in inventory
        int essenceCount = ItemCount(RuneEssenceItemId);
        if (essenceCount <= 0)
        {
            // TODO: p.frames.sendMessage(p, $"You need Rune Essence to craft {altar.Name}s!");
            return false;
        }

        // Remove all essence
        DeleteAllOfItem(RuneEssenceItemId);

        // Calculate rune multiplier: floor((rcLevel - reqLevel) / 20) + 1
        int multiplier = ((rcLevel - altar.LevelRequired) / 20) + 1;
        int runesProduced = essenceCount * multiplier;

        // Add runes (stackable — add to existing stack or create new)
        AddStackableItem(altar.RuneItemId, runesProduced);

        // Grant XP: Java formula = BaseXp * currentLevel * essenceCount
        double xp = altar.BaseXpPerEssence * _player.SkillLvl[SkillConstants.Runecrafting] * essenceCount;
        _player.AddSkillXP(xp, SkillConstants.Runecrafting);

        // TODO: p.requestAnim(runecrafting animation, 0);
        // TODO: p.requestGfx(runecrafting gfx, 0);

        return true;
    }

    /// <summary>
    /// Check if an object ID is a runecrafting altar.
    /// </summary>
    public static bool IsRunecraftingAltar(int objectId)
    {
        foreach (var altar in Altars)
            if (altar.AltarObjectId == objectId) return true;
        return false;
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    public static AltarDefinition? FindAltar(int altarObjectId)
    {
        foreach (var altar in Altars)
            if (altar.AltarObjectId == altarObjectId) return altar;
        return null;
    }

    // ── Inventory helpers ───────────────────────────────────────────────────

    private int ItemCount(int itemId)
    {
        int count = 0;
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == itemId)
                count += _player.ItemsN[i];
        }
        return count;
    }

    /// <summary>Delete all instances of an item from inventory.</summary>
    private void DeleteAllOfItem(int itemId)
    {
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == itemId)
            {
                _player.Items[i] = -1;
                _player.ItemsN[i] = 0;
            }
        }
    }

    /// <summary>
    /// Add a stackable item. If the item already exists in a slot,
    /// increase its count. Otherwise find an empty slot.
    /// </summary>
    private bool AddStackableItem(int itemId, int amount)
    {
        // Check if item already exists in inventory (stackable)
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == itemId)
            {
                _player.ItemsN[i] += amount;
                return true;
            }
        }

        // Find empty slot
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == -1)
            {
                _player.Items[i] = itemId;
                _player.ItemsN[i] = amount;
                return true;
            }
        }
        return false;
    }
}
