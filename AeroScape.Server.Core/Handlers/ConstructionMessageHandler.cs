using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

public sealed class ConstructionMessageHandler : IMessageHandler<ConstructionMessage>
{
    private readonly ILogger<ConstructionMessageHandler> _logger;
    private readonly ConstructionService _construction;

    public ConstructionMessageHandler(ILogger<ConstructionMessageHandler> logger, ConstructionService construction)
    {
        _logger = logger;
        _construction = construction;
    }

    public Task HandleAsync(PlayerSession session, ConstructionMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is null)
            return Task.CompletedTask;

        _construction.HandleBuildClick(session.Entity, message.X, message.Y, message.ObjectId);
        _logger.LogInformation("[Construction] Player {Username} object={ObjectId} at {X},{Y}", session.Entity.Username, message.ObjectId, message.X, message.Y);
        return Task.CompletedTask;
    }
}
