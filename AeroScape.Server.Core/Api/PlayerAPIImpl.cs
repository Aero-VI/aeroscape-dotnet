using AeroScape.Server.API;
using AeroScape.Server.API.Models;
using AeroScape.Server.Core.Entities;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Movement;

namespace AeroScape.Server.Core.Api;

/// <summary>
/// Implementation of IPlayerAPI that wraps core player services.
/// </summary>
public class PlayerAPIImpl : IPlayerAPI
{
    private readonly GameEngine _engine;
    private readonly IClientUiService _ui;
    private readonly WalkQueue _walkQueue;

    public PlayerAPIImpl(GameEngine engine, IClientUiService ui, WalkQueue walkQueue)
    {
        _engine = engine;
        _ui = ui;
        _walkQueue = walkQueue;
    }

    public PlayerInfo? GetPlayer(string username)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => 
            p.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return player != null ? MapToPlayerInfo(player) : null;
    }

    public PlayerInfo? GetPlayer(int playerId)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        return player != null ? MapToPlayerInfo(player) : null;
    }

    public IReadOnlyList<PlayerInfo> GetOnlinePlayers()
    {
        return _engine.OnlinePlayers.Select(MapToPlayerInfo).ToList();
    }

    public void SendMessage(PlayerInfo player, string message)
    {
        SendMessage(player.Id, message);
    }

    public void SendMessage(int playerId, string message)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            _ui.SendMessage(player, message);
        }
    }

    public void PlayAnimation(PlayerInfo player, int animationId, int delay = 0)
    {
        PlayAnimation(player.Id, animationId, delay);
    }

    public void PlayAnimation(int playerId, int animationId, int delay = 0)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            player.RequestAnim(animationId, delay);
        }
    }

    public void PlayGraphic(PlayerInfo player, int graphicId, int height = 0, int delay = 0)
    {
        PlayGraphic(player.Id, graphicId, height, delay);
    }

    public void PlayGraphic(int playerId, int graphicId, int height = 0, int delay = 0)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            player.RequestGfx(graphicId, delay);
        }
    }

    public void WalkTo(PlayerInfo player, LocationInfo location)
    {
        WalkTo(player.Id, location);
    }

    public void WalkTo(int playerId, LocationInfo location)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            _walkQueue.AddToWalkingQueue(player, location.X, location.Y);
        }
    }

    public void Teleport(PlayerInfo player, LocationInfo location)
    {
        Teleport(player.Id, location);
    }

    public void Teleport(int playerId, LocationInfo location)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            player.SetCoords(location.X, location.Y, location.HeightLevel);
        }
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
            InCombat = false, // TODO: Implement combat tracking
            AnimationId = player.AnimReq
        };
    }
}