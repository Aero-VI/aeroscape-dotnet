namespace AeroScape.Plugin.Woodcutting;

/// <summary>
/// Tree definitions for the woodcutting plugin.
/// </summary>
public static class TreeDefinitions
{
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
}

/// <summary>
/// Axe definitions for the woodcutting plugin.
/// </summary>
public static class AxeDefinitions
{
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
}