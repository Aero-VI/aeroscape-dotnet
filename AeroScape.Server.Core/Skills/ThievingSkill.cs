using AeroScape.Server.Core.Entities;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Items;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Thieving skill implementation.
/// Translated from DavidScape's pickpocket system.
/// </summary>
public static class ThievingSkill
{
    public const int SKILL_ID = 17;
    public const int PICKPOCKET_ANIMATION = 881;

    public record ThievingTarget(
        int NpcId,
        int RequiredLevel,
        int Experience,
        int MinCoins,
        int MaxCoins,
        string Name
    );

    /// <summary>
    /// Thieving targets with their requirements and rewards
    /// </summary>
    private static readonly ThievingTarget[] Targets = 
    {
        new(1, 1, 30, 1, 3, "Man"),
        new(9, 1, 30, 1, 3, "Man"),
        new(2234, 40, 75, 1, 5, "Farmer"),
        new(21, 65, 150, 1, 7, "Hero"),
        new(20, 83, 300, 1, 11, "Paladin")
    };

    /// <summary>
    /// Attempt to pickpocket an NPC
    /// </summary>
    public static bool Pickpocket(Player player, NPC npc, IClientUiService ui, PlayerItemsService items)
    {
        if (player.ActionTimer > 0)
            return false;

        var target = FindTarget(npc.NpcType);
        if (target == null)
            return false;

        // Check level requirement
        if (player.SkillLvl[SKILL_ID] < target.RequiredLevel)
        {
            ui.SendMessage(player, $"You need {target.RequiredLevel} Thieving to steal from {target.Name}.");
            return true;
        }

        // Set action timer
        player.ActionTimer = 3;
        
        // Play pickpocket animation
        player.RequestAnim(PICKPOCKET_ANIMATION, 0);
        
        // Calculate coins reward
        int coins = target.MinCoins + Random.Shared.Next(target.MaxCoins - target.MinCoins + 1);
        
        // Add coins to inventory
        items.AddItem(player, 995, coins);
        
        // Send success message
        ui.SendMessage(player, $"You pickpocket the {target.Name}.");
        
        // Calculate and award XP
        int xp = target.Experience * player.SkillLvl[SKILL_ID];
        player.AddSkillXP(xp, SKILL_ID);

        return true;
    }

    /// <summary>
    /// Find thieving target data by NPC ID
    /// </summary>
    private static ThievingTarget? FindTarget(int npcId)
    {
        foreach (var target in Targets)
        {
            if (target.NpcId == npcId)
                return target;
        }
        return null;
    }
}