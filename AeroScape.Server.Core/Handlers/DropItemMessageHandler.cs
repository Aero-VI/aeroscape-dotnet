using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class DropItemMessageHandler : IMessageHandler<DropItemMessage>
{
    public Task HandleAsync(PlayerSession session, DropItemMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement drop item logic
        // E.g., remove item from inventory, spawn ground item
        Console.WriteLine($"[DropItem] Player {session.SessionId} dropped item {message.ItemId} from slot {message.Slot}");
        return Task.CompletedTask;
    }
}