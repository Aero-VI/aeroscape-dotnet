using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Fletching skill — ported from the fletching logic in ItemOnItem.java and Player.java.
///
/// Fletching is initiated by using a knife (item 946) on logs.
/// Each log type produces a different arrow shaft/headless arrow/unstrung bow,
/// with a level requirement and XP reward.
///
/// The Java code processes fletching via FletchTimer countdown in Player.process().
/// </summary>
public class FletchingSkill
{
    private readonly Player _player;

    public FletchingSkill(Player player)
    {
        _player = player;
    }

    /// <summary>Knife item ID.</summary>
    public const int KnifeItemId = 946;

    // ── Fletchable definitions ──────────────────────────────────────────────

    /// <param name="LogItemId">Item ID of the logs to cut.</param>
    /// <param name="ProductItemId">Item ID of the product.</param>
    /// <param name="LevelRequired">Minimum Fletching level.</param>
    /// <param name="BaseXp">XP per set crafted.</param>
    /// <param name="Name">Display name.</param>
    public record FletchableDefinition(int LogItemId, int ProductItemId, int LevelRequired, int BaseXp, string Name);

    public static readonly FletchableDefinition[] Fletchables =
    [
        new(1511, 882,  1,  15, "Normal arrows"),    // Normal logs → arrow shafts
        new(1521, 884,  15, 45, "Oak arrows"),        // Oak logs
        new(1519, 886,  30, 60, "Willow arrows"),     // Willow logs
        new(1517, 888,  45, 70, "Maple arrows"),      // Maple logs
        new(1515, 890,  65, 85, "Yew arrows"),        // Yew logs
        new(1513, 892,  75, 100, "Magic arrows"),     // Magic logs
    ];

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Start fletching logs with a knife.
    /// Called from ItemOnItem handler when knife + logs are used together.
    /// </summary>
    /// <param name="logItemId">The log item ID.</param>
    public void StartFletching(int logItemId)
    {
        var fletchable = FindFletchable(logItemId);
        if (fletchable == null)
            return;

        int fletchLevel = _player.SkillLvl[SkillConstants.Fletching];
        if (fletchLevel < fletchable.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need level {fletchable.LevelRequired} fletching to cut this log.");
            return;
        }

        // Set up fletching state (matches Java pattern)
        _player.FletchId = fletchable.LogItemId;
        _player.FletchGet = fletchable.ProductItemId;
        _player.FletchXP = fletchable.BaseXp;
        _player.FletchAmount = 28; // Fletch the whole inventory by default
        _player.FletchTimer = 3;

        // Fletch the first one
        FletchOne(fletchable);
    }

    /// <summary>
    /// Called every game tick from player processing loop.
    /// Handles the FletchTimer countdown.
    /// </summary>
    public void Process()
    {
        if (_player.FletchTimer > 0)
            _player.FletchTimer--;

        if (_player.FletchTimer == 0 && _player.FletchAmount > 0)
        {
            var fletchable = FindFletchable(_player.FletchId);
            if (fletchable != null)
                FletchOne(fletchable);
            else
                StopFletching();
        }
    }

    public void StopFletching()
    {
        _player.FletchTimer = -1;
        _player.FletchAmount = 0;
        _player.FletchXP = 0;
        _player.FletchGet = 0;
        _player.FletchId = 0;
    }

    // ── Internal ────────────────────────────────────────────────────────────

    private void FletchOne(FletchableDefinition fletchable)
    {
        if (_player.FletchAmount <= 0)
        {
            StopFletching();
            return;
        }

        if (!HasItem(fletchable.LogItemId))
        {
            // TODO: p.frames.sendMessage(p, "You have run out of logs.");
            StopFletching();
            return;
        }

        if (FreeSlotCount() < 1)
        {
            StopFletching();
            return;
        }

        // Consume log
        DeleteItem(fletchable.LogItemId);

        // Grant product (random amount 1-10 for arrow shafts, matches Java)
        int amount = 1 + Random.Shared.Next(10);
        AddItem(fletchable.ProductItemId, amount);

        // Grant XP
        _player.AddSkillXP(fletchable.BaseXp, SkillConstants.Fletching);

        // TODO: p.requestAnim(fletching animation, 0);
        // TODO: p.frames.sendMessage(p, "You carefully cut the wood into arrow shafts.");

        _player.FletchAmount--;
        _player.FletchTimer = 3;

        if (_player.FletchAmount <= 0 || !HasItem(fletchable.LogItemId))
            StopFletching();
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    public static FletchableDefinition? FindFletchable(int logItemId)
    {
        foreach (var f in Fletchables)
            if (f.LogItemId == logItemId) return f;
        return null;
    }

    // ── Inventory helpers ───────────────────────────────────────────────────

    private int FreeSlotCount()
    {
        int count = 0;
        for (int i = 0; i < _player.Items.Length; i++)
            if (_player.Items[i] == -1) count++;
        return count;
    }

    private bool HasItem(int itemId)
    {
        for (int i = 0; i < _player.Items.Length; i++)
            if (_player.Items[i] == itemId) return true;
        return false;
    }

    private bool AddItem(int itemId, int amount = 1)
    {
        for (int a = 0; a < amount; a++)
        {
            bool added = false;
            for (int i = 0; i < _player.Items.Length; i++)
            {
                if (_player.Items[i] == -1)
                {
                    _player.Items[i] = itemId;
                    _player.ItemsN[i] = 1;
                    added = true;
                    break;
                }
            }
            if (!added) return false;
        }
        return true;
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
