using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Skills;

/// <summary>
/// Agility skill implementation.
/// Translated from DavidScape's agility obstacle handling.
/// </summary>
public static class AgilitySkill
{
    public const int SKILL_ID = 16;

    /// <summary>
    /// Agility obstacle definitions
    /// </summary>
    public static class Obstacles
    {
        public const int ROPE_SWING = 2282;
        public const int LOG_BALANCE = 2294;
        public const int OBSTACLE_NET = 20211;
        public const int WALL_SLIDE = 2302;
        public const int LADDER_DOWN = 3205;
    }

    /// <summary>
    /// Handle rope swing obstacle
    /// </summary>
    public static bool HandleRopeSwing(Player player)
    {
        if (player.AbsX == 2551 && player.AbsY == 3554)
        {
            player.ChatText = "Lulz.";
            player.ChatTextUpdateReq = true;
            player.UpdateReq = true;
            player.SwingTimer1 = 2;
            player.IsRunning = true;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handle log balance obstacle
    /// </summary>
    public static bool HandleLogBalance(Player player)
    {
        if (player.AbsX == 2551 && player.AbsY == 3546)
        {
            player.LogTimer = 4;
            player.AgilityXP = 250;
            player.AgilityTimer = 24;
            player.JumpDelay = 24;
            player.NewEmote = 844;
            player.IsRunning = false;
            // Force player to move to destination
            player.SetCoords(2541, 3546, player.HeightLevel);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handle obstacle net climb
    /// </summary>
    public static bool HandleObstacleNet(Player player)
    {
        if (player.AbsX == 2539 && (player.AbsY == 3546 || player.AbsY == 3545))
        {
            player.AgilityXP = 400;
            player.AgilityTimer = 6;
            player.NetTimer = 4;
            player.RequestAnim(3063, 0);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handle wall slide obstacle
    /// </summary>
    public static bool HandleWallSlide(Player player)
    {
        if (player.AbsX == 2536 && player.AbsY == 3547)
        {
            player.AgilityXP = 200;
            player.AgilityTimer = 10;
            player.JumpDelay = 10;
            player.NewEmote = 756;
            player.IsRunning = false;
            // Force player to move to destination
            player.SetCoords(2532, 3547, player.HeightLevel);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handle ladder down to complete course
    /// </summary>
    public static bool HandleLadderDown(Player player)
    {
        player.SetCoords(2532, 3546, 0);
        return true;
    }

    /// <summary>
    /// Process agility obstacle interaction
    /// </summary>
    public static bool HandleObstacle(Player player, int objectId, int x, int y)
    {
        return objectId switch
        {
            Obstacles.ROPE_SWING => HandleRopeSwing(player),
            Obstacles.LOG_BALANCE => HandleLogBalance(player),
            Obstacles.OBSTACLE_NET => HandleObstacleNet(player),
            Obstacles.WALL_SLIDE => HandleWallSlide(player),
            Obstacles.LADDER_DOWN => HandleLadderDown(player),
            _ => false
        };
    }

    /// <summary>
    /// Give agility XP when timer expires (called from game engine tick)
    /// </summary>
    public static void ProcessAgilityTimer(Player player)
    {
        if (player.AgilityTimer > 0)
        {
            player.AgilityTimer--;
            if (player.AgilityTimer == 0 && player.AgilityXP > 0)
            {
                player.AddSkillXP(player.AgilityXP, SKILL_ID);
                player.AgilityXP = 0;
            }
        }
    }
}