using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Cooking skill — ported from the cooking logic in Player.java's CookThat()
/// and ItemOnObject.java (raw fish on cooking range/fire).
///
/// Cooking is initiated by using a raw food item on a cooking range (58124) or
/// fire (28173, 2732). A dialogue asks how many to cook (1/5/All).
/// Each cook tick: consume raw food, produce cooked food, grant XP.
/// </summary>
public class CookingSkill
{
    private readonly Player _player;

    public CookingSkill(Player player)
    {
        _player = player;
    }

    // ── Cookable definitions ────────────────────────────────────────────────

    /// <param name="RawItemId">Item ID of the raw food.</param>
    /// <param name="CookedItemId">Item ID of the cooked food.</param>
    /// <param name="LevelRequired">Minimum Cooking level.</param>
    /// <param name="BaseXp">XP per successful cook.</param>
    /// <param name="Name">Display name.</param>
    public record CookableDefinition(
        int RawItemId,
        int CookedItemId,
        int LevelRequired,
        int BaseXp,
        string Name);

    public static readonly CookableDefinition[] Cookables =
    [
        new(317,  315,  1,  30,  "Shrimps"),
        new(335,  329,  15, 70,  "Trout"),
        new(363,  365,  30, 100, "Bass"),
        new(377,  379,  40, 120, "Lobster"),
        new(383,  385,  80, 210, "Shark"),
        new(389,  391,  90, 250, "Manta ray"),
        new(3142, 3144, 1,  30,  "Karambwan"),  // bonus entry
    ];

    /// <summary>Cooking range and fire object IDs.</summary>
    public static readonly int[] CookingObjects = [58124, 28173, 2732];

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Attempt to start cooking a raw food item on a cooking object.
    /// Called from ItemOnObject handler.
    /// </summary>
    /// <param name="rawItemId">The raw food item ID.</param>
    /// <param name="objectId">The cooking range/fire object ID.</param>
    /// <param name="amount">How many to cook (1, 5, or 28 for all).</param>
    public void StartCooking(int rawItemId, int objectId, int amount = 1)
    {
        if (!IsCookingObject(objectId))
            return;

        var cookable = FindCookable(rawItemId);
        if (cookable == null)
        {
            // TODO: p.frames.sendMessage(p, "You can't cook that.");
            return;
        }

        int cookLevel = _player.SkillLvl[SkillConstants.Cooking];
        if (cookLevel < cookable.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need level {cookable.LevelRequired} Cooking to cook this.");
            return;
        }

        // Set up cooking state
        _player.CookXP = cookable.BaseXp;
        _player.CookId = cookable.RawItemId;
        _player.CookGet = cookable.CookedItemId;
        _player.CookAmount = Math.Min(amount, ItemCount(rawItemId));
        _player.CookTimer = 3;

        // Cook the first one immediately (matches Java pattern)
        CookOne(cookable);
    }

    /// <summary>
    /// Called every game tick from player processing loop.
    /// Handles the CookTimer countdown.
    /// </summary>
    public void Process()
    {
        if (_player.CookTimer > 0)
            _player.CookTimer--;

        if (_player.CookTimer == 0 && _player.CookAmount > 0)
        {
            var cookable = FindCookable(_player.CookId);
            if (cookable != null)
                CookOne(cookable);
            else
                StopCooking();
        }
    }

    public void StopCooking()
    {
        _player.CookTimer = -1;
        _player.CookAmount = 0;
        _player.CookXP = 0;
        _player.CookGet = 0;
        _player.CookId = 0;
    }

    // ── Internal ────────────────────────────────────────────────────────────

    private void CookOne(CookableDefinition cookable)
    {
        if (_player.CookAmount <= 0)
        {
            StopCooking();
            return;
        }

        if (!HasItem(cookable.RawItemId))
        {
            // TODO: p.frames.sendMessage(p, "You ran out of fish to cook.");
            StopCooking();
            return;
        }

        // Remove raw, add cooked
        DeleteItem(cookable.RawItemId);
        AddItem(cookable.CookedItemId);

        // TODO: Burn chance based on level (not in original Java code, but could be added)

        // Grant XP
        _player.AddSkillXP(cookable.BaseXp, SkillConstants.Cooking);

        // TODO: p.frames.sendMessage(p, "You cook the food.");
        // TODO: p.requestAnim(cooking animation, 0);

        _player.CookAmount--;
        _player.CookTimer = 3;

        if (_player.CookAmount <= 0)
            StopCooking();
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    public static CookableDefinition? FindCookable(int rawItemId)
    {
        foreach (var c in Cookables)
        {
            if (c.RawItemId == rawItemId)
                return c;
        }
        return null;
    }

    public static bool IsCookingObject(int objectId)
    {
        foreach (var id in CookingObjects)
            if (id == objectId) return true;
        return false;
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
}
