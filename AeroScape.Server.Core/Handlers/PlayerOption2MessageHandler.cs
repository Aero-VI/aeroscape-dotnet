using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class PlayerOption2MessageHandler : IMessageHandler<PlayerOption2Message>
{
    public Task HandleAsync(PlayerSession session, PlayerOption2Message message, CancellationToken cancellationToken)
    {
        // TODO: Implement Player Option 2 logic (Trade request).
        // Legacy behaviour: validate target player, distance check, send trade request,
        // face target, and initiate the trade system via pTrade.tradePlayer().
        Console.WriteLine($"[PlayerOption2] Player {session.SessionId} sent trade/option-2 to target index {message.TargetIndex}");
        return Task.CompletedTask;
    }
}
