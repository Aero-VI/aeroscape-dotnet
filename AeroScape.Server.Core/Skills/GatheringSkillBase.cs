using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Base class for tick-driven gathering skills (Woodcutting, Mining, Fishing).
/// 
/// Each gathering skill follows the same pattern from the legacy Java code:
///   1. Player initiates the action (e.g., clicks a tree / rock / fishing spot).
///   2. Validation: tool check, level check.
///   3. Animation plays; a tick-based countdown begins.
///   4. Every N ticks, an item is produced and XP is awarded.
///   5. The action resets when the player's inventory is full or the resource is exhausted.
/// 
/// This base class encapsulates that shared rhythm. Subclasses define the
/// data (tools, resources, animations) and any skill-specific quirks.
/// </summary>
public abstract class GatheringSkillBase
{
    protected readonly Player _player;

    /// <summary>Whether the player is currently performing this gathering action.</summary>
    public bool IsActive { get; protected set; }

    /// <summary>Ticks remaining until the next resource is gathered.</summary>
    protected int GatherTimer { get; set; } = -1;

    /// <summary>Sub-tick counter for animation replay (animations replay every 2 ticks).</summary>
    protected int AnimationTimer { get; set; } = 2;

    /// <summary>Current animation ID being played.</summary>
    protected int CurrentAnimation { get; set; }

    protected GatheringSkillBase(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Called every game tick (600ms) from the engine's player processing loop.
    /// Drives the gather → reward → repeat cycle.
    /// </summary>
    public void Process()
    {
        if (!IsActive)
            return;

        // Replay the gathering animation every 2 ticks (matches Java secondtimer pattern)
        AnimationTimer--;
        if (AnimationTimer <= 0)
        {
            AnimationTimer = 2;
            GatherTimer--;
            _player.RequestAnim(CurrentAnimation, 0);
        }

        if (GatherTimer <= 0)
        {
            OnGatherComplete();
        }
    }

    /// <summary>
    /// Reset the skill to idle state. Called when the action ends
    /// (inventory full, resource depleted, player moves, etc.).
    /// </summary>
    public virtual void Reset()
    {
        IsActive = false;
        GatherTimer = -1;
        AnimationTimer = 2;
    }

    /// <summary>
    /// Called when a gathering cycle completes (timer hits zero).
    /// Implementations should grant items, XP, and decide whether to continue.
    /// </summary>
    protected abstract void OnGatherComplete();

    // ── Inventory helpers ────────────────────────────────────────────────
    // These wrap the Player's raw arrays until the full PlayerItems service is built.
    // They mirror the logic from Java's PlayerItems.java.

    /// <summary>Count how many free inventory slots the player has.</summary>
    protected int FreeSlotCount()
    {
        int count = 0;
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == -1)
                count++;
        }
        return count;
    }

    /// <summary>Check if the player has at least one of the given item (inventory).</summary>
    protected bool HasItem(int itemId)
    {
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == itemId)
                return true;
        }
        return false;
    }

    /// <summary>Check if the player has the item equipped in the given slot.</summary>
    protected bool HasEquipped(int slot, int itemId)
        => _player.Equipment[slot] == itemId;

    /// <summary>Check if the player has the item in inventory OR equipped in weapon slot (3).</summary>
    protected bool HasItemOrEquipped(int itemId)
        => HasItem(itemId) || HasEquipped(3, itemId);

    /// <summary>
    /// Add an item to the player's inventory.
    /// Returns true if successful, false if inventory is full.
    /// </summary>
    protected bool AddItem(int itemId, int amount = 1)
    {
        // TODO: Stackable item handling (runes, coins, etc.)
        for (int a = 0; a < amount; a++)
        {
            int slot = -1;
            for (int i = 0; i < _player.Items.Length; i++)
            {
                if (_player.Items[i] == -1)
                {
                    slot = i;
                    break;
                }
            }

            if (slot == -1)
                return false;

            _player.Items[slot] = itemId;
            _player.ItemsN[slot] = 1;
        }
        return true;
    }

    /// <summary>
    /// Delete one instance of the given item from inventory.
    /// Returns true if found and removed.
    /// </summary>
    protected bool DeleteItem(int itemId, int amount = 1)
    {
        int removed = 0;
        for (int i = 0; i < _player.Items.Length && removed < amount; i++)
        {
            if (_player.Items[i] == itemId)
            {
                _player.Items[i] = -1;
                _player.ItemsN[i] = 0;
                removed++;
            }
        }
        return removed == amount;
    }

    /// <summary>Count how many of a given item the player has in inventory.</summary>
    protected int ItemCount(int itemId)
    {
        int count = 0;
        for (int i = 0; i < _player.Items.Length; i++)
        {
            if (_player.Items[i] == itemId)
                count += _player.ItemsN[i];
        }
        return count;
    }
}
