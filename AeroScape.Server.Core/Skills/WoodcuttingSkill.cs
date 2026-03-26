using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Woodcutting skill — ported from DavidScape/Skills/Woodcutting.java.
/// 
/// Uses a data-driven approach instead of the Java switch-case spaghetti.
/// Tree definitions and axe definitions are stored as static arrays of records,
/// making it trivial to add new tree types or axes.
/// </summary>
public class WoodcuttingSkill : GatheringSkillBase
{
    // ── Tree definitions ────────────────────────────────────────────────────
    // Maps object IDs to tree data. A tree type groups multiple object variants.

    /// <param name="ObjectIds">Game object IDs that map to this tree type.</param>
    /// <param name="LogItemId">Item ID of the log produced.</param>
    /// <param name="LevelRequired">Minimum Woodcutting level to chop.</param>
    /// <param name="BaseXp">Base XP per log (scaled by level).</param>
    /// <param name="MaxLogs">Maximum logs before the tree is depleted.</param>
    /// <param name="Name">Display name for messages.</param>
    public record TreeDefinition(
        int[] ObjectIds,
        int LogItemId,
        int LevelRequired,
        int BaseXp,
        int MaxLogs,
        string Name);

    public static readonly TreeDefinition[] Trees =
    [
        new([1276, 1277, 1278],               1511, 1,  50,  1, "Normal tree"),
        new([1281],                            1521, 15, 75,  2, "Oak tree"),
        new([1308],                            1519, 30, 100, 3, "Willow tree"),
        new([9036],                            6333, 35, 150, 4, "Teak tree"),
        new([1307, 4674],                      1517, 45, 175, 5, "Maple tree"),
        new([9034],                            6332, 50, 250, 5, "Mahogany tree"),
        new([1309],                            1515, 60, 300, 4, "Yew tree"),
        new([1306],                            1513, 75, 500, 8, "Magic tree"),
    ];

    // ── Axe definitions ─────────────────────────────────────────────────────
    // Ordered from worst to best; detection tries best first.

    /// <param name="ItemId">Item ID of the axe.</param>
    /// <param name="LevelRequired">Minimum Woodcutting level to use this axe.</param>
    /// <param name="Animation">Animation ID when using this axe.</param>
    /// <param name="Name">Display name.</param>
    public record AxeDefinition(int ItemId, int LevelRequired, int Animation, string Name);

    /// <summary>Ordered best → worst for detection priority.</summary>
    public static readonly AxeDefinition[] Axes =
    [
        new(6739, 61, 2846, "Dragon axe"),
        new(1359, 41, 867,  "Rune axe"),
        new(1357, 31, 869,  "Adamant axe"),
        new(1355, 21, 871,  "Mithril axe"),
        new(1361, 10, 873,  "Black axe"),
        new(1353, 5,  875,  "Steel axe"),
        new(1349, 1,  877,  "Iron axe"),
        new(1351, 1,  879,  "Bronze axe"),
    ];

    // ── Instance state ──────────────────────────────────────────────────────
    private TreeDefinition? _currentTree;
    private AxeDefinition? _currentAxe;
    private int _logsChopped;
    private int _maxLogs;

    /// <summary>Ticks per log (derived from axe/tree but simplified to 4 like the Java code).</summary>
    private int _ticksPerLog = 4;

    public WoodcuttingSkill(Player player) : base(player) { }

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Begin chopping a tree. Called from ObjectOption1 handler.
    /// </summary>
    /// <param name="objectId">The game object ID that was clicked.</param>
    public void StartCutting(int objectId)
    {
        // 1. Find which tree was clicked
        _currentTree = FindTree(objectId);
        if (_currentTree == null)
            return; // Unknown object — silently ignore

        // 2. Find the best axe the player can use
        _currentAxe = FindBestAxe();
        if (_currentAxe == null)
        {
            // TODO: Send message via frames when network layer is wired
            // p.frames.sendMessage(p, "You don't have an axe which you have the woodcutting level to use.");
            return;
        }

        // 3. Check level requirement for the tree
        int wcLevel = _player.SkillLvl[SkillConstants.Woodcutting];
        if (wcLevel < _currentTree.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need level {_currentTree.LevelRequired} to chop down this tree.");
            return;
        }

        // 4. Start the action
        // TODO: p.frames.sendMessage(p, "You swing your axe at the tree.");
        _logsChopped = 0;
        _maxLogs = _currentTree.MaxLogs > 0 ? _currentTree.MaxLogs : 1;
        _ticksPerLog = 4; // Java always calculates to 4 (the speed formula is effectively constant)
        CurrentAnimation = _currentAxe.Animation;
        GatherTimer = _ticksPerLog;
        AnimationTimer = 2;
        IsActive = true;

        _player.RequestAnim(CurrentAnimation, 0);
    }

    public override void Reset()
    {
        base.Reset();
        _currentTree = null;
        _currentAxe = null;
        _logsChopped = 0;
    }

    // ── Core logic ──────────────────────────────────────────────────────────

    protected override void OnGatherComplete()
    {
        if (_currentTree == null || _currentAxe == null)
        {
            Reset();
            return;
        }

        // Check inventory space
        if (FreeSlotCount() < 1)
        {
            // TODO: p.frames.sendMessage(p, "Not enough inventory space to cut any more logs!");
            Reset();
            return;
        }

        // Grant the log
        AddItem(_currentTree.LogItemId);
        _logsChopped++;

        // Grant XP: Java formula is (BaseXp * wcLevel) / 3
        int wcLevel = _player.SkillLvl[SkillConstants.Woodcutting];
        double xp = (_currentTree.BaseXp * wcLevel) / 3.0;
        _player.AddSkillXP(xp, SkillConstants.Woodcutting);

        // TODO: p.frames.sendMessage(p, "You cut some logs.");

        // Check if tree is depleted (maxLogs reached)
        if (_logsChopped >= _maxLogs)
        {
            Reset();
            return;
        }

        // Continue chopping — reset the timer
        GatherTimer = _ticksPerLog;
        _player.RequestAnim(CurrentAnimation, 0);
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    /// <summary>Find the tree definition for a given object ID.</summary>
    public static TreeDefinition? FindTree(int objectId)
    {
        foreach (var tree in Trees)
        {
            foreach (var id in tree.ObjectIds)
            {
                if (id == objectId)
                    return tree;
            }
        }
        return null;
    }

    /// <summary>
    /// Find the best axe the player can use (checks inventory then equipment).
    /// Returns null if no usable axe is found.
    /// </summary>
    private AxeDefinition? FindBestAxe()
    {
        int wcLevel = _player.SkillLvl[SkillConstants.Woodcutting];

        // Axes are ordered best → worst; return first usable one
        foreach (var axe in Axes)
        {
            if (wcLevel >= axe.LevelRequired && HasItemOrEquipped(axe.ItemId))
                return axe;
        }
        return null;
    }
}
