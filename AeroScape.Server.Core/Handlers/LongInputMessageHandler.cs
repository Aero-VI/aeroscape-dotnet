using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;
using AeroScape.Server.Core.Util;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

public sealed class LongInputMessageHandler : IMessageHandler<LongInputMessage>
{
    private readonly ILogger<LongInputMessageHandler> _logger;
    public LongInputMessageHandler(ILogger<LongInputMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(PlayerSession session, LongInputMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is null)
            return Task.CompletedTask;

        string value = NameUtil.LongToString(message.Value).Replace('_', ' ');
        // Clan chat removed - minimal server

        _logger.LogInformation("[LongInput] Player {Username} inputId={InputId} value={Value}", session.Entity.Username, session.Entity.InputId, value);
        return Task.CompletedTask;
    }
}
