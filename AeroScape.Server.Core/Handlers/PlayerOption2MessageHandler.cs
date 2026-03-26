using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class PlayerOption2MessageHandler : IMessageHandler<PlayerOption2Message>
{
    private readonly ILogger<PlayerOption2MessageHandler> _logger;
    private readonly TradingService _trading;

    public PlayerOption2MessageHandler(ILogger<PlayerOption2MessageHandler> logger, TradingService trading)
    {
        _logger = logger;
        _trading = trading;
    }
    public Task HandleAsync(PlayerSession session, PlayerOption2Message message, CancellationToken cancellationToken)
    {
        if (session.Entity is { } player)
        {
            _trading.RequestTrade(player, message.TargetIndex);
        }

        _logger.LogInformation("[PlayerOption2] Player {SessionId} sent trade/option-2 to target index {TargetIndex}", session.SessionId, message.TargetIndex);
        return Task.CompletedTask;
    }
}
