using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class PickupItemMessageHandler : IMessageHandler<PickupItemMessage>
{
    public Task HandleAsync(PlayerSession session, PickupItemMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement pickup item logic
        // Legacy behaviour:
        //   - Check distance to ground item coords
        //   - If not adjacent, set itemPickup flag and wait for walking to complete
        //   - Look up ground item by (itemId, x, y, heightLevel) in world item list
        //   - If found, add item to player inventory and remove from ground
        Console.WriteLine($"[PickupItem] Player {session.SessionId} picking up item {message.ItemId} at ({message.ItemX}, {message.ItemY})");
        return Task.CompletedTask;
    }
}
