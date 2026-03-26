using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class TradeAcceptMessageHandler : IMessageHandler<TradeAcceptMessage>
{
    public Task HandleAsync(PlayerSession session, TradeAcceptMessage message, CancellationToken cancellationToken)
    {
        // TODO: Validate partner, confirm trade, swap items.
        // Legacy decoded: playerId = readUnsignedWord() - 33024, /256, +1
        Console.WriteLine($"[TradeAccept] Player {session.SessionId} accepted trade with partner {message.PartnerId}");
        return Task.CompletedTask;
    }
}
