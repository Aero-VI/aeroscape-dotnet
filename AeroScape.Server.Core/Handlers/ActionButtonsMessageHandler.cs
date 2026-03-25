using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ActionButtonsMessageHandler : IMessageHandler<ActionButtonsMessage>
{
    public Task HandleAsync(PlayerSession session, ActionButtonsMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement ActionButtons logic
        Console.WriteLine($"[ActionButtons] Player {session.SessionId} interface {message.InterfaceId} button {message.ButtonId} item {message.ItemId} slot {message.SlotId}");
        return Task.CompletedTask;
    }
}