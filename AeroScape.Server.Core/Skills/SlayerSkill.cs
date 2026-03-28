using AeroScape.Server.Core.Entities;
using AeroScape.Server.Core.Services;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Slayer skill implementation.
/// Basic implementation based on DavidScape references.
/// </summary>
public static class SlayerSkill
{
    public const int SKILL_ID = 18;
    public const int SLAYER_MASTER_DURADEL = 1599;

    public record SlayerTask(
        string MonsterName,
        int[] NpcIds,
        int RequiredLevel,
        int MinAmount,
        int MaxAmount,
        int Experience
    );

    /// <summary>
    /// Available slayer tasks
    /// </summary>
    private static readonly SlayerTask[] Tasks = 
    {
        new("Men", new[] { 1, 9 }, 1, 10, 20, 50),
        new("Guards", new[] { 9, 32 }, 10, 15, 30, 75),
        new("Hill Giants", new[] { 117 }, 20, 20, 40, 125),
        new("Moss Giants", new[] { 112 }, 30, 25, 45, 175),
        new("Lesser Demons", new[] { 82 }, 40, 30, 50, 225),
        new("Greater Demons", new[] { 83 }, 50, 20, 40, 275),
        new("Black Demons", new[] { 84 }, 60, 25, 45, 325),
        new("Dragons", new[] { 53, 54, 55, 50 }, 70, 10, 25, 400)
    };

    /// <summary>
    /// Assign a new slayer task to player
    /// </summary>
    public static void AssignTask(Player player, IClientUiService ui)
    {
        // Check if player already has a task
        if (player.SlayerTask > 0)
        {
            var currentTask = GetTaskById(player.SlayerTask);
            if (currentTask != null && player.SlayerAmount > 0)
            {
                ui.SendMessage(player, $"You still need to kill {player.SlayerAmount} {currentTask.MonsterName}.");
                return;
            }
        }

        // Find tasks appropriate for player level
        var availableTasks = Tasks.Where(t => player.SkillLvl[SKILL_ID] >= t.RequiredLevel).ToList();
        if (availableTasks.Count == 0)
        {
            ui.SendMessage(player, "You need a higher Slayer level for any tasks.");
            return;
        }

        // Assign random task from available
        var task = availableTasks[Random.Shared.Next(availableTasks.Count)];
        var amount = task.MinAmount + Random.Shared.Next(task.MaxAmount - task.MinAmount + 1);

        // Set task on player
        player.SlayerTask = Array.IndexOf(Tasks, task);
        player.SlayerAmount = amount;

        ui.SendMessage(player, $"Your new task is to kill {amount} {task.MonsterName}.");
    }

    /// <summary>
    /// Process a kill for slayer task
    /// </summary>
    public static void ProcessKill(Player player, int npcId, IClientUiService ui)
    {
        if (player.SlayerTask < 0 || player.SlayerTask >= Tasks.Length)
            return;

        var task = Tasks[player.SlayerTask];
        if (!task.NpcIds.Contains(npcId))
            return;

        if (player.SlayerAmount <= 0)
            return;

        player.SlayerAmount--;
        
        if (player.SlayerAmount == 0)
        {
            // Task complete - award XP
            int xp = task.Experience * player.SkillLvl[SKILL_ID];
            player.AddSkillXP(xp, SKILL_ID);
            
            ui.SendMessage(player, $"You have completed your slayer task! Gained {xp} XP.");
            player.SlayerTask = -1;
        }
        else
        {
            ui.SendMessage(player, $"{player.SlayerAmount} {task.MonsterName} left to kill.");
        }
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    private static SlayerTask? GetTaskById(int id)
    {
        if (id < 0 || id >= Tasks.Length)
            return null;
        return Tasks[id];
    }
}