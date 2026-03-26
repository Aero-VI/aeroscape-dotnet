using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

public sealed class StringInputMessageHandler : IMessageHandler<StringInputMessage>
{
    private readonly ILogger<StringInputMessageHandler> _logger;

    public StringInputMessageHandler(ILogger<StringInputMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(PlayerSession session, StringInputMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[StringInput] Player {SessionId} inputId={InputId} value={Value}", session.SessionId, session.Entity?.InputId, message.Value);
        return Task.CompletedTask;
    }
}
