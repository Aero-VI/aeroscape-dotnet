using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Firemaking skill — ported from the firemaking logic in ItemOnItem.java.
///
/// Firemaking is initiated by using a tinderbox (item 590) on logs.
/// The logs are consumed, a fire object (2732) is created at the player's location,
/// the fire despawns after 50 ticks leaving ashes.
///
/// Coloured fires are supported via firelighters (red, green, blue, purple, white).
/// </summary>
public class FiremakingSkill
{
    private readonly Player _player;

    public FiremakingSkill(Player player)
    {
        _player = player;
    }

    // ── Log definitions ─────────────────────────────────────────────────────

    /// <param name="LogItemId">Item ID of the logs.</param>
    /// <param name="LevelRequired">Minimum Firemaking level.</param>
    /// <param name="BaseXp">XP per fire lit.</param>
    /// <param name="FireObjectId">Object ID of the fire created (2732 = normal).</param>
    /// <param name="Name">Display name.</param>
    public record LogDefinition(int LogItemId, int LevelRequired, double BaseXp, int FireObjectId, string Name);

    public static readonly LogDefinition[] Logs =
    [
        new(1511, 1,  40.0,  2732, "Normal logs"),
        new(1521, 15, 60.0,  2732, "Oak logs"),
        new(1519, 30, 90.0,  2732, "Willow logs"),
        new(1517, 45, 135.0, 2732, "Maple logs"),
        new(1515, 60, 202.5, 2732, "Yew logs"),
        new(1513, 75, 303.8, 2732, "Magic logs"),
    ];

    /// <summary>Tinderbox item ID.</summary>
    public const int TinderboxItemId = 590;

    /// <summary>Fire-lighting animation.</summary>
    public const int FireLightAnimation = 733;

    /// <summary>Ticks before a fire object despawns.</summary>
    public const int FireDespawnTicks = 50;

    // ── Coloured firelighter definitions ────────────────────────────────────

    /// <param name="FirelighterItemId">Item ID of the firelighter.</param>
    /// <param name="ColouredLogItemId">Item ID of the coloured log variant.</param>
    /// <param name="FireObjectId">Object ID of the coloured fire.</param>
    /// <param name="Name">Colour name.</param>
    public record FirelighterDefinition(int FirelighterItemId, int ColouredLogItemId, int FireObjectId, string Name);

    public static readonly FirelighterDefinition[] Firelighters =
    [
        new(7329, 7404, 11404, "Red"),
        new(7330, 7405, 11405, "Green"),
        new(7331, 7406, 11406, "Blue"),
        new(10326, 10327, 20000, "Purple"),
        new(10328, 10329, 20001, "White"),
    ];

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Attempt to light logs with a tinderbox.
    /// Called from ItemOnItem handler when tinderbox + logs are used together.
    /// </summary>
    /// <param name="logItemId">The log item ID.</param>
    /// <returns>True if the fire was successfully lit.</returns>
    public bool LightFire(int logItemId)
    {
        var log = FindLog(logItemId);
        if (log == null)
            return false;

        int fmLevel = _player.SkillLvl[SkillConstants.Firemaking];
        if (fmLevel < log.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need level {log.LevelRequired} Firemaking to light these logs.");
            return false;
        }

        if (_player.FireDelay > 0)
        {
            // TODO: p.frames.sendMessage(p, "You're already tending to a fire.");
            return false;
        }

        // Consume the logs
        if (!DeleteItem(logItemId))
            return false;

        // Play animation
        _player.RequestAnim(FireLightAnimation, 0);

        // Grant XP
        _player.AddSkillXP(log.BaseXp, SkillConstants.Firemaking);

        // Set fire delay so the player can't spam fires
        _player.FireDelay = FireDespawnTicks;

        // TODO: When frame/object system is implemented:
        // p.frames.createGlobalObject(log.FireObjectId, p.HeightLevel, p.AbsX, p.AbsY, 0, 10);
        // p.frames.sendMessage(p, "You set the logs on fire.");

        return true;
    }

    /// <summary>
    /// Apply a firelighter to normal logs to create coloured logs.
    /// Called from ItemOnItem handler.
    /// </summary>
    /// <param name="firelighterItemId">The firelighter item ID.</param>
    /// <param name="logItemId">The log item ID (must be normal logs 1511).</param>
    /// <returns>True if coloured logs were created.</returns>
    public bool ApplyFirelighter(int firelighterItemId, int logItemId)
    {
        if (logItemId != 1511) // Only normal logs can be coloured
            return false;

        var fl = FindFirelighter(firelighterItemId);
        if (fl == null)
            return false;

        if (!DeleteItem(logItemId))
            return false;
        if (!DeleteItem(firelighterItemId))
        {
            AddItem(logItemId); // Restore logs if firelighter removal fails
            return false;
        }

        AddItem(fl.ColouredLogItemId);
        // TODO: p.frames.sendMessage(p, $"You rub the firelighter on the logs to make {fl.Name.ToLower()} logs.");

        return true;
    }

    /// <summary>
    /// Light coloured logs (tinderbox + coloured logs).
    /// </summary>
    public bool LightColouredFire(int colouredLogItemId)
    {
        var fl = FindFirelighterByLog(colouredLogItemId);
        if (fl == null)
            return false;

        int fmLevel = _player.SkillLvl[SkillConstants.Firemaking];
        if (fmLevel < 1) // Coloured logs use normal log level requirement
            return false;

        if (_player.FireDelay > 0)
            return false;

        if (!DeleteItem(colouredLogItemId))
            return false;

        _player.RequestAnim(FireLightAnimation, 0);
        _player.AddSkillXP(40.0, SkillConstants.Firemaking); // Same as normal logs
        _player.FireDelay = FireDespawnTicks;

        // TODO: p.frames.createGlobalObject(fl.FireObjectId, p.HeightLevel, p.AbsX, p.AbsY, 0, 10);

        return true;
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    public static LogDefinition? FindLog(int logItemId)
    {
        foreach (var log in Logs)
            if (log.LogItemId == logItemId) return log;
        return null;
    }

    public static FirelighterDefinition? FindFirelighter(int firelighterItemId)
    {
        foreach (var fl in Firelighters)
            if (fl.FirelighterItemId == firelighterItemId) return fl;
        return null;
    }

    public static FirelighterDefinition? FindFirelighterByLog(int colouredLogItemId)
    {
        foreach (var fl in Firelighters)
            if (fl.ColouredLogItemId == colouredLogItemId) return fl;
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
