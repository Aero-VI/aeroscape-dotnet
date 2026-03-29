using AeroScape.Server.API;
using AeroScape.Server.API.Models;
using AeroScape.Server.Core.Engine;

namespace AeroScape.Server.Core.Api;

/// <summary>
/// Implementation of ISkillAPI that wraps core skill services.
/// </summary>
public class SkillAPIImpl : ISkillAPI
{
    private readonly GameEngine _engine;

    public SkillAPIImpl(GameEngine engine)
    {
        _engine = engine;
    }

    public int GetLevel(PlayerInfo player, int skillId)
    {
        return GetLevel(player.Id, skillId);
    }

    public int GetLevel(int playerId, int skillId)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        return player?.SkillLvl[skillId] ?? 1;
    }

    public double GetExperience(PlayerInfo player, int skillId)
    {
        return GetExperience(player.Id, skillId);
    }

    public double GetExperience(int playerId, int skillId)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        return player?.SkillXP[skillId] ?? 0;
    }

    public void AddExperience(PlayerInfo player, int skillId, double experience)
    {
        AddExperience(player.Id, skillId, experience);
    }

    public void AddExperience(int playerId, int skillId, double experience)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            player.AddSkillXP(experience, skillId);
        }
    }

    public void SetTemporaryLevel(PlayerInfo player, int skillId, int level)
    {
        SetTemporaryLevel(player.Id, skillId, level);
    }

    public void SetTemporaryLevel(int playerId, int skillId, int level)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            player.SkillLvl[skillId] = level;
        }
    }

    public void ResetLevel(PlayerInfo player, int skillId)
    {
        ResetLevel(player.Id, skillId);
    }

    public void ResetLevel(int playerId, int skillId)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player != null)
        {
            player.SkillLvl[skillId] = player.GetLevelForXP(skillId);
        }
    }
}