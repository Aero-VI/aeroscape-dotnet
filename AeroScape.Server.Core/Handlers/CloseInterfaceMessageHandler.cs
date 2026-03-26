using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class CloseInterfaceMessageHandler : IMessageHandler<CloseInterfaceMessage>
{
    public Task HandleAsync(PlayerSession session, CloseInterfaceMessage message, CancellationToken cancellationToken)
    {
        // TODO: Close open interface, restore tabs/inventory.
        // Legacy also showed update notes on first two closes.
        Console.WriteLine($"[CloseInterface] Player {session.SessionId}");
        return Task.CompletedTask;
    }
}
