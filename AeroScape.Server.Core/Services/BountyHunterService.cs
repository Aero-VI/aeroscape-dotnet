using Microsoft.Extensions.DependencyInjection;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public sealed class BountyHunterService
{
    private readonly IServiceProvider _serviceProvider;
    private GameEngine? _engine;
    private GameEngine Engine => _engine ??= _serviceProvider.GetRequiredService<GameEngine>();

    public BountyHunterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool IsBountyArea(int x, int y)
        => x >= 3085 && x <= 3185 && y >= 3662 && y <= 3765;

    public void EnterBounty(Player player)
    {
        if (!player.Online)
            return;

        player.BountyOpponent = 0;
        player.PkIcon = 3;
        Teleport(player, 3174, 3710);
        AssignOpponent(player);
    }

    public void LeaveBounty(Player player)
    {
        if (!player.Online)
            return;

        var opponent = GetOpponent(player);
        if (opponent is not null)
        {
            opponent.BountyOpponent = 0;
            AssignOpponent(opponent);
            Teleport(opponent, 3180, 3685);
        }

        player.BountyOpponent = 0;
        Teleport(player, 3180, 3685);
    }

    public void AssignOpponent(Player player)
    {
        if (!player.Online || !IsBountyArea(player.AbsX, player.AbsY) || player.SkillLvl[3] <= 0)
            return;

        foreach (var candidate in Engine.Players)
        {
            if (candidate is null || !candidate.Online || candidate == player)
                continue;

            if (!IsBountyArea(candidate.AbsX, candidate.AbsY) || candidate.BountyOpponent > 0)
                continue;

            player.BountyOpponent = candidate.PlayerId;
            candidate.BountyOpponent = player.PlayerId;
            return;
        }

        player.BountyOpponent = 0;
    }

    public void UpdateTarget(Player player, int targetId)
    {
        if (targetId <= 0)
        {
            AssignOpponent(player);
            return;
        }

        var target = targetId >= Engine.Players.Length ? null : Engine.Players[targetId];
        if (target is null || !target.Online)
            return;

        player.BountyOpponent = target.PlayerId;
    }

    private Player? GetOpponent(Player player)
        => player.BountyOpponent > 0 && player.BountyOpponent < Engine.Players.Length
            ? Engine.Players[player.BountyOpponent]
            : null;

    private static void Teleport(Player player, int x, int y)
    {
        player.TeleX = x;
        player.TeleY = y;
        player.AbsX = x;
        player.AbsY = y;
        player.DidTeleport = true;
    }
}