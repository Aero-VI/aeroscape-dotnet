using Microsoft.Extensions.DependencyInjection;
using AeroScape.Server.Core.Entities;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.API.Events;
using AeroScape.Server.API.Models;
using AeroScape.Server.PluginHost;

namespace AeroScape.Server.Core.Services;

public sealed class ObjectInteractionService
{
    private readonly IServiceProvider _serviceProvider;
    
    private GameEngine? _engine;
    private GameEngine Engine => _engine ??= _serviceProvider.GetRequiredService<GameEngine>();
    
    private PluginManager? _pluginManager;
    private PluginManager? PluginManager => _pluginManager ??= _serviceProvider.GetService<PluginManager>();

    public ObjectInteractionService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool HandleOption1(Player player, int objectId, int x, int y)
    {
        player.ClickId = objectId;
        player.ClickX = x;
        player.ClickY = y;

        if (!HasObjectAt(objectId, x, y))
            return false;

        // Fire plugin event for object interaction
        if (PluginManager != null)
        {
            var playerInfo = MapToPlayerInfo(player);
            var location = new LocationInfo { X = x, Y = y, HeightLevel = player.HeightLevel };
            var eventArgs = new PlayerInteractObjectEventArgs
            {
                Player = playerInfo,
                ObjectId = objectId,
                Location = location,
                Option = 1
            };
            
            PluginManager.GlobalEventManager.FireEvent(this, eventArgs);
            
            if (eventArgs.IsCancelled)
                return true;
        }

        // Remove direct woodcutting handling - now handled by plugin
        // if (AeroScape.Server.Core.Skills.WoodcuttingSkill.FindTree(objectId) is not null)
        // {
        //     player.Woodcutting.StartCutting(objectId);
        //     return true;
        // }

        // DISABLED - Mining
        // if (AeroScape.Server.Core.Skills.MiningSkill.FindRock(objectId) is not null)
        // {
        //     player.Mining.StartMining(objectId);
        //     return true;
        // }

        // DISABLED - Agility
        // if (AeroScape.Server.Core.Skills.AgilitySkill.HandleObstacle(player, objectId, x, y))
        // {
        //     return true;
        // }

        switch (objectId)
        {
            // DISABLED - Banking
            // case 2213:
            // case 2672:
            // case 280:
            // case 4483:
            // case 25808:
            // case 26972:
            //     player.InterfaceId = 762;
            //     return true;
            // DISABLED - Prayer altars
            // case 409:
            // case 34616:
            // case 19145:
            // case 26286:
            // case 26288:
            // case 26289:
            //     player.SkillLvl[5] = player.GetLevelForXP(5);
            //     _prayer.Reset(player);
            //     return true;
            // DISABLED - Magic altars
            // case 6552:
            //     player.IsAncients = player.IsAncients == 1 ? 0 : 1;
            //     player.IsLunar = 0;
            //     return true;
            // case 17010:
            //     player.IsLunar = player.IsLunar == 1 ? 0 : 1;
            //     player.IsAncients = 0;
            //     return true;
            case 1738:
            case 1740:
                player.SetCoords(player.AbsX, player.AbsY, player.HeightLevel == 0 ? 1 : 0);
                return true;
            case 15482:
                player.InterfaceId = 399;
                return true;
            // DISABLED - Bounty Hunter
            // case 28120:
            // case 28121:
            //     _bountyHunter.EnterBounty(player);
            //     return true;
            case 23271:
                player.JumpDelay = 3;
                return true;
            default:
                return false;
        }
    }

    public bool HandleOption2(Player player, int objectId, int x, int y)
    {
        if (!HasObjectAt(objectId, x, y))
            return false;

        if (objectId == 28089)
        {
            player.InterfaceId = 762;
            return true;
        }

        return false;
    }

    private bool HasObjectAt(int objectId, int x, int y)
    {
        foreach (var obj in Engine.LoadedObjects)
        {
            if (obj.ObjectId == objectId && obj.X == x && obj.Y == y)
                return true;
        }

        return false;
    }
    
    private static PlayerInfo MapToPlayerInfo(Player player)
    {
        return new PlayerInfo
        {
            Id = player.PlayerId,
            Username = player.Username,
            CombatLevel = player.CombatLevel,
            Location = new LocationInfo
            {
                X = player.AbsX,
                Y = player.AbsY,
                HeightLevel = player.HeightLevel
            },
            CurrentHitpoints = player.SkillLvl[3], // HP skill ID
            MaxHitpoints = player.GetLevelForXP(3),
            InCombat = false,
            AnimationId = player.AnimReq
        };
    }
}
