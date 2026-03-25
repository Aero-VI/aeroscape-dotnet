using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemOnItemMessageHandler : IMessageHandler<ItemOnItemMessage>
{
    public Task HandleAsync(PlayerSession session, ItemOnItemMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement item on item logic (e.g., combining items, firemaking, fletching)
        Console.WriteLine($"[ItemOnItem] Player {session.SessionId} used item {message.ItemUsedId} on item {message.UsedWithId}");
        return Task.CompletedTask;
    }
}