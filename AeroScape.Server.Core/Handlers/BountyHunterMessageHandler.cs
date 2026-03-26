using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class BountyHunterMessageHandler : IMessageHandler<BountyHunterMessage>
{
    private readonly ILogger<BountyHunterMessageHandler> _logger;
    private readonly BountyHunterService _bountyHunter;

    public BountyHunterMessageHandler(ILogger<BountyHunterMessageHandler> logger, BountyHunterService bountyHunter)
    {
        _logger = logger;
        _bountyHunter = bountyHunter;
    }
    public Task HandleAsync(PlayerSession session, BountyHunterMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is { } player)
            _bountyHunter.UpdateTarget(player, message.TargetId);

        _logger.LogInformation("[BountyHunter] Player {SessionId} updated target to {TargetId}", session.SessionId, message.TargetId);
        return Task.CompletedTask;
    }
}
