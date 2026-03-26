using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class TradeAcceptMessageHandler : IMessageHandler<TradeAcceptMessage>
{
    private readonly ILogger<TradeAcceptMessageHandler> _logger;
    private readonly TradingService _trading;

    public TradeAcceptMessageHandler(ILogger<TradeAcceptMessageHandler> logger, TradingService trading)
    {
        _logger = logger;
        _trading = trading;
    }
    public Task HandleAsync(PlayerSession session, TradeAcceptMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
        {
            return Task.CompletedTask;
        }

        _trading.ConfirmTrade(player);
        _logger.LogInformation("[TradeAccept] Player {SessionId} accepted trade with partner {PartnerId}", session.SessionId, message.PartnerId);
        return Task.CompletedTask;
    }
}
