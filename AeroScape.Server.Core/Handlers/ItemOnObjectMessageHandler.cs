using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemOnObjectMessageHandler : IMessageHandler<ItemOnObjectMessage>
{
    public Task HandleAsync(PlayerSession session, ItemOnObjectMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement item-on-object logic (smelting, smithing, cooking, farming, cannon placement, etc.)
        // Legacy handled furnace (56332), anvil (54540), range (58124/28173), farming patch (34573), and more.
        Console.WriteLine($"[ItemOnObject] Player {session.SessionId} used item {message.ItemId} on object {message.ObjectId}");
        return Task.CompletedTask;
    }
}
