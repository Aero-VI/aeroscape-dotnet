using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Herblore skill — ported from ItemOption1.java herb cleaning logic.
///
/// The Java implementation is minimal: clicking a grimy herb cleans it,
/// consuming the grimy version and producing the clean version + XP.
///
/// The NPC Kaqemeex (455) also provides herb exchange services and
/// skillcape rewards, but those are handled by the NPC dialogue system.
/// </summary>
public class HerbloreSkill
{
    private readonly Player _player;

    public HerbloreSkill(Player player)
    {
        _player = player;
    }

    // ── Herb definitions ────────────────────────────────────────────────────

    /// <param name="GrimyItemId">Item ID of the grimy herb.</param>
    /// <param name="CleanItemId">Item ID of the clean herb.</param>
    /// <param name="LevelRequired">Minimum Herblore level.</param>
    /// <param name="BaseXp">Base XP per herb cleaned (scaled by level in Java).</param>
    /// <param name="Name">Display name.</param>
    public record HerbDefinition(int GrimyItemId, int CleanItemId, int LevelRequired, int BaseXp, string Name);

    public static readonly HerbDefinition[] Herbs =
    [
        new(199, 249, 1,  200, "Guam leaf"),
        new(207, 257, 25, 450, "Ranarr weed"),
        // Additional herbs can be added as the Java codebase reveals more
        // Common RS herbs for reference:
        new(201, 251, 5,  250, "Marrentill"),
        new(203, 253, 11, 300, "Tarromin"),
        new(205, 255, 20, 350, "Harralander"),
        new(209, 259, 30, 500, "Irit leaf"),
        new(211, 261, 35, 550, "Avantoe"),
        new(213, 263, 40, 600, "Kwuarm"),
        new(215, 265, 48, 650, "Cadantine"),
        new(217, 267, 54, 700, "Dwarf weed"),
        new(219, 269, 59, 750, "Torstol"),
        new(2485, 2481, 65, 800, "Lantadyme"),
    ];

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Attempt to clean a grimy herb.
    /// Called from ItemOption1 handler (clicking a grimy herb in inventory).
    /// </summary>
    /// <param name="grimyItemId">The grimy herb item ID.</param>
    /// <returns>True if the herb was cleaned.</returns>
    public bool CleanHerb(int grimyItemId)
    {
        var herb = FindHerb(grimyItemId);
        if (herb == null)
            return false;

        int herbloreLevel = _player.SkillLvl[SkillConstants.Herblore];
        if (herbloreLevel < herb.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need level {herb.LevelRequired} Herblore to clean this herb.");
            return false;
        }

        if (!DeleteItem(herb.GrimyItemId))
            return false;

        AddItem(herb.CleanItemId);

        // Java formula: BaseXp * herbloreLevel (then /1 since addSkillXP handles it)
        double xp = herb.BaseXp * herbloreLevel;
        _player.AddSkillXP(xp, SkillConstants.Herblore);

        // TODO: p.frames.sendMessage(p, $"You clean the {herb.Name.ToLower()}.");

        return true;
    }

    /// <summary>
    /// Check if an item ID is a grimy herb (for routing in ItemOption1 handler).
    /// </summary>
    public static bool IsGrimyHerb(int itemId)
    {
        foreach (var herb in Herbs)
            if (herb.GrimyItemId == itemId) return true;
        return false;
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    public static HerbDefinition? FindHerb(int grimyItemId)
    {
        foreach (var herb in Herbs)
            if (herb.GrimyItemId == grimyItemId) return herb;
        return null;
    }

    // ── Inventory helpers ───────────────────────────────────────────────────

    private bool AddItem(int itemId)
    {
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == -1)
            {
                _player.Items[i] = itemId;
                _player.ItemsN[i] = 1;
                return true;
            }
        }
        return false;
    }

    private bool DeleteItem(int itemId)
    {
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == itemId)
            {
                _player.Items[i] = -1;
                _player.ItemsN[i] = 0;
                return true;
            }
        }
        return false;
    }
}
