using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemExamineMessageHandler : IMessageHandler<ItemExamineMessage>
{
    public Task HandleAsync(PlayerSession session, ItemExamineMessage message, CancellationToken cancellationToken)
    {
        // TODO: Look up item description from item definition provider and send to player.
        Console.WriteLine($"[ItemExamine] Player {session.SessionId} examined item {message.ItemId}");
        return Task.CompletedTask;
    }
}
