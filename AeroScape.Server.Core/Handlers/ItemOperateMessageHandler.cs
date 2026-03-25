using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemOperateMessageHandler : IMessageHandler<ItemOperateMessage>
{
    public Task HandleAsync(PlayerSession session, ItemOperateMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[ItemOperate] Player {session.SessionId} operated item {message.ItemId} in slot {message.SlotId}");
        return Task.CompletedTask;
    }
}