using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Crafting skill — ported from gem cutting logic in ItemOnItem.java,
/// godsword assembly, and tanning via NPC 804.
///
/// Crafting actions:
///   1. Gem cutting: chisel (1755) + uncut gem → cut gem + XP
///   2. Godsword assembly: blade + hilt → godsword
///   3. Tanning: NPC interaction (not item-on-item, handled via NPC dialogue)
/// </summary>
public class CraftingSkill
{
    private readonly Player _player;

    public CraftingSkill(Player player)
    {
        _player = player;
    }

    /// <summary>Chisel item ID.</summary>
    public const int ChiselItemId = 1755;

    // ── Gem definitions ─────────────────────────────────────────────────────

    /// <param name="UncutItemId">Item ID of the uncut gem.</param>
    /// <param name="CutItemId">Item ID of the cut gem.</param>
    /// <param name="LevelRequired">Minimum Crafting level.</param>
    /// <param name="Xp">XP per gem cut.</param>
    /// <param name="Name">Display name.</param>
    public record GemDefinition(int UncutItemId, int CutItemId, int LevelRequired, double Xp, string Name);

    public static readonly GemDefinition[] Gems =
    [
        new(1623, 1607, 1,  50.0,  "Sapphire"),
        new(1621, 1605, 30, 67.5,  "Emerald"),
        new(1619, 1603, 50, 85.0,  "Ruby"),
        new(1617, 1601, 60, 107.5, "Diamond"),
        new(1631, 1615, 75, 137.5, "Dragonstone"),
        new(6571, 6573, 85, 167.5, "Onyx"),
    ];

    // ── Godsword assembly definitions ───────────────────────────────────────

    /// <param name="BladeItemId">Godsword blade item ID.</param>
    /// <param name="HiltItemId">Hilt item ID.</param>
    /// <param name="GodswordItemId">Resulting godsword item ID.</param>
    /// <param name="Name">Display name.</param>
    public record GodswordDefinition(int BladeItemId, int HiltItemId, int GodswordItemId, string Name);

    public static readonly GodswordDefinition[] Godswords =
    [
        new(11690, 11702, 11694, "Armadyl godsword"),
        new(11690, 11704, 11696, "Bandos godsword"),
        new(11690, 11706, 11698, "Saradomin godsword"),
        new(11690, 11708, 11700, "Zamorak godsword"),
    ];

    // ── Tanning definitions (NPC 804) ───────────────────────────────────────

    /// <param name="HideItemId">Raw hide item ID.</param>
    /// <param name="LeatherItemId">Tanned leather item ID.</param>
    /// <param name="Cost">Gold cost per tan.</param>
    /// <param name="Name">Display name.</param>
    public record TanDefinition(int HideItemId, int LeatherItemId, int Cost, string Name);

    public static readonly TanDefinition[] Tans =
    [
        new(1739, 1741, 1,   "Leather"),
        new(1753, 1745, 20,  "Green dragonhide"),
        new(1751, 2505, 20,  "Blue dragonhide"),
        new(1749, 2507, 20,  "Red dragonhide"),
        new(1747, 2509, 20,  "Black dragonhide"),
    ];

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Attempt to cut a gem using chisel + uncut gem.
    /// Called from ItemOnItem handler.
    /// </summary>
    /// <param name="itemUsed">One of the two items used.</param>
    /// <param name="usedWith">The other item.</param>
    /// <returns>True if a gem was cut.</returns>
    public bool TryCutGem(int itemUsed, int usedWith)
    {
        // One of the items must be a chisel
        int gemItemId;
        if (itemUsed == ChiselItemId)
            gemItemId = usedWith;
        else if (usedWith == ChiselItemId)
            gemItemId = itemUsed;
        else
            return false;

        var gem = FindGem(gemItemId);
        if (gem == null)
            return false;

        int craftLevel = _player.SkillLvl[SkillConstants.Crafting];
        if (craftLevel < gem.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need level {gem.LevelRequired} crafting to cut this gem.");
            return false;
        }

        if (!DeleteItem(gem.UncutItemId))
            return false;

        AddItem(gem.CutItemId);
        _player.AddSkillXP(gem.Xp, SkillConstants.Crafting);

        // TODO: p.frames.sendMessage(p, $"You cut the {gem.Name.ToLower()}.");

        return true;
    }

    /// <summary>
    /// Attempt to assemble a godsword from blade + hilt.
    /// Called from ItemOnItem handler.
    /// </summary>
    public bool TryAssembleGodsword(int itemUsed, int usedWith)
    {
        foreach (var gs in Godswords)
        {
            if ((itemUsed == gs.BladeItemId && usedWith == gs.HiltItemId) ||
                (itemUsed == gs.HiltItemId && usedWith == gs.BladeItemId))
            {
                if (!DeleteItem(gs.BladeItemId) || !DeleteItem(gs.HiltItemId))
                    return false;

                AddItem(gs.GodswordItemId);

                // TODO: p.frames.sendMessage(p, "You attach the Godsword Blade and Hilt together...");
                // TODO: p.frames.sendMessage(p, $"...and get a {gs.Name}!");

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Tan a hide at the tanner NPC.
    /// Called from NPC dialogue handler.
    /// </summary>
    /// <param name="hideItemId">The hide to tan.</param>
    /// <param name="amount">How many to tan.</param>
    public bool TanHide(int hideItemId, int amount = 1)
    {
        var tan = FindTan(hideItemId);
        if (tan == null)
            return false;

        int tanned = 0;
        for (int i = 0; i < amount; i++)
        {
            if (!HasItem(tan.HideItemId))
                break;

            // Check gold (item 995)
            if (ItemCount(995) < tan.Cost)
            {
                // TODO: p.frames.sendMessage(p, "You don't have enough coins.");
                break;
            }

            DeleteItem(tan.HideItemId);
            DeleteCoins(tan.Cost);
            AddItem(tan.LeatherItemId);
            tanned++;
        }

        return tanned > 0;
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    public static GemDefinition? FindGem(int uncutItemId)
    {
        foreach (var gem in Gems)
            if (gem.UncutItemId == uncutItemId) return gem;
        return null;
    }

    public static TanDefinition? FindTan(int hideItemId)
    {
        foreach (var tan in Tans)
            if (tan.HideItemId == hideItemId) return tan;
        return null;
    }

    // ── Inventory helpers ───────────────────────────────────────────────────

    private bool HasItem(int itemId)
    {
        for (int i = 0; i < _player.Items.Length; i++)
            if (_player.Items[i] == itemId) return true;
        return false;
    }

    private int ItemCount(int itemId)
    {
        int count = 0;
        for (int i = 0; i < _player.Items.Length; i++)
            if (_player.Items[i] == itemId) count += _player.ItemsN[i];
        return count;
    }

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

    /// <summary>Delete a specific amount of coins (item 995, stackable).</summary>
    private bool DeleteCoins(int amount)
    {
        // TODO: Proper stackable item handling
        // For now, find coins and reduce amount
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == 995)
            {
                if (_player.ItemsN[i] >= amount)
                {
                    _player.ItemsN[i] -= amount;
                    if (_player.ItemsN[i] <= 0)
                    {
                        _player.Items[i] = -1;
                        _player.ItemsN[i] = 0;
                    }
                    return true;
                }
            }
        }
        return false;
    }
}
