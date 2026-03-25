using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class EquipItemMessageHandler : IMessageHandler<EquipItemMessage>
{
    public Task HandleAsync(PlayerSession session, EquipItemMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement item equip logic
        Console.WriteLine($"[EquipItem] Player {session.SessionId} equipped item {message.ItemId} to slot {message.Slot}");
        return Task.CompletedTask;
    }
}