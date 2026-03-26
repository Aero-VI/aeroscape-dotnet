using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Fishing skill — ported from DavidScape/Skills/Fishing.java and
/// the fishing logic spread across NPCOption1.java / NPCOption2.java / Player.java.
///
/// Unlike Woodcutting/Mining which use object clicks, Fishing is initiated by
/// clicking on fishing spot NPCs (NPC types 312, 313, 316). Each NPC type
/// supports two fishing methods (Option 1 and Option 2).
///
/// The tick processing is simpler: the player's FishTimer counts down, and
/// when it hits 0 the player catches a fish. The Java code handles this in
/// Player.java's process() method via FishThat().
/// </summary>
public class FishingSkill
{
    private readonly Player _player;

    public FishingSkill(Player player)
    {
        _player = player;
    }

    // ── Fishing spot definitions ────────────────────────────────────────────
    // Each fishing spot NPC has two options (Option1 and Option2).

    /// <param name="NpcType">The fishing spot NPC type ID.</param>
    /// <param name="FishItemId">Item ID of the fish caught.</param>
    /// <param name="LevelRequired">Minimum Fishing level.</param>
    /// <param name="BaseXp">XP awarded per catch.</param>
    /// <param name="ToolItemId">Required tool item ID.</param>
    /// <param name="Animation">Fishing animation ID.</param>
    /// <param name="BaitItemId">Required bait item ID (0 = no bait needed).</param>
    /// <param name="Name">Display name for messages.</param>
    public record FishingSpot(
        int NpcType,
        int OptionNumber,
        int FishItemId,
        int LevelRequired,
        int BaseXp,
        int ToolItemId,
        int Animation,
        int BaitItemId,
        string ToolName,
        string Name);

    public static readonly FishingSpot[] Spots =
    [
        // NPC 316 — Small fishing net spot
        // Option 1: Net (Shrimps)
        new(316, 1, 317, 1,  25,  305, 620, 0,   "small fishing net", "Shrimps"),
        // Option 2: Bait (Trout)
        new(316, 2, 335, 15, 62,  307, 622, 313, "fishing rod",       "Trout"),

        // NPC 313 — Big fishing net spot
        // Option 1: Big Net (Bass)
        new(313, 1, 363, 30, 45,  305, 620, 0,   "big fishing net",   "Bass"),
        // Option 2: Harpoon (Manta ray)
        new(313, 2, 389, 90, 200, 311, 618, 0,   "harpoon",           "Manta ray"),

        // NPC 312 — Cage/Harpoon spot
        // Option 1: Cage (Lobster)
        new(312, 1, 377, 40, 90,  301, 619, 0,   "lobster pot",       "Lobster"),
        // Option 2: Harpoon (Shark)
        new(312, 2, 383, 75, 150, 311, 618, 0,   "harpoon",           "Shark"),
    ];

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Attempt to start fishing at a fishing spot.
    /// Called from NPCOption1/NPCOption2 handlers.
    /// </summary>
    /// <param name="npcType">The NPC type of the fishing spot.</param>
    /// <param name="optionNumber">1 for first click, 2 for second click.</param>
    public void StartFishing(int npcType, int optionNumber)
    {
        var spot = FindSpot(npcType, optionNumber);
        if (spot == null)
            return;

        int fishingLevel = _player.SkillLvl[SkillConstants.Fishing];

        // Level check
        if (fishingLevel < spot.LevelRequired)
        {
            // TODO: p.frames.sendMessage(p, $"You need level {spot.LevelRequired} fishing to fish here.");
            return;
        }

        // Tool check
        if (!HasItem(spot.ToolItemId))
        {
            // TODO: p.frames.sendMessage(p, $"You need a {spot.ToolName} to fish here.");
            return;
        }

        // Bait check
        if (spot.BaitItemId != 0 && !HasItem(spot.BaitItemId))
        {
            // TODO: p.frames.sendMessage(p, "You need more fishing bait!");
            return;
        }

        // Set up the fishing state on the player (matches Java pattern)
        _player.FishXP = spot.BaseXp;
        _player.FishGet = spot.FishItemId;
        _player.FishEmote = spot.Animation;
        _player.FishTimer = 4 + Random.Shared.Next(7); // 4-10 ticks
        _player.ActionTimer = 3;

        // Do the first catch immediately (matches Java FishThat call)
        CatchFish(spot);
    }

    /// <summary>
    /// Called every game tick from the player processing loop.
    /// Handles the FishTimer countdown and catching fish.
    /// </summary>
    public void Process()
    {
        if (_player.FishTimer > 0)
            _player.FishTimer--;

        if (_player.FishTimer == 0 && _player.FishGet > 0)
        {
            // Find the matching spot to get bait info
            var spot = FindSpotByFish(_player.FishGet);
            if (spot != null)
                CatchFish(spot);
            else
                StopFishing();
        }
    }

    /// <summary>Stop the current fishing action.</summary>
    public void StopFishing()
    {
        _player.FishTimer = -1;
        _player.FishGet = 0;
        _player.FishXP = 0;
        _player.FishEmote = 0;
    }

    // ── Internal ────────────────────────────────────────────────────────────

    private void CatchFish(FishingSpot spot)
    {
        if (FreeSlotCount() < 1)
        {
            // TODO: p.frames.sendMessage(p, "Your inventory is too full to hold any more fish.");
            StopFishing();
            return;
        }

        // Consume bait if required
        if (spot.BaitItemId != 0)
        {
            if (!HasItem(spot.BaitItemId))
            {
                // TODO: p.frames.sendMessage(p, "You need more fishing bait!");
                StopFishing();
                return;
            }
            DeleteItem(spot.BaitItemId);
        }

        // Play animation
        _player.RequestAnim(spot.Animation, 0);

        // Grant fish
        AddItem(spot.FishItemId);

        // Grant XP: Java formula is FishXP * fishLevel / 3
        int fishLevel = _player.SkillLvl[SkillConstants.Fishing];
        double xp = (spot.BaseXp * fishLevel) / 3.0;
        _player.AddSkillXP(xp, SkillConstants.Fishing);

        // TODO: p.frames.sendMessage(p, "You catch a fish.");

        // Set next catch timer
        _player.FishTimer = 4 + Random.Shared.Next(7);
    }

    // ── Lookup helpers ──────────────────────────────────────────────────────

    public static FishingSpot? FindSpot(int npcType, int optionNumber)
    {
        foreach (var spot in Spots)
        {
            if (spot.NpcType == npcType && spot.OptionNumber == optionNumber)
                return spot;
        }
        return null;
    }

    private static FishingSpot? FindSpotByFish(int fishItemId)
    {
        foreach (var spot in Spots)
        {
            if (spot.FishItemId == fishItemId)
                return spot;
        }
        return null;
    }

    // ── Inventory helpers (same as GatheringSkillBase) ───────────────────────
    // Duplicated here since FishingSkill doesn't extend GatheringSkillBase
    // (its tick pattern is different — timer-based rather than countdown-based).

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
