using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemGiveMessageHandler : IMessageHandler<ItemGiveMessage>
{
    public Task HandleAsync(PlayerSession session, ItemGiveMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement item give logic
        // Legacy behaviour:
        //   - Reset woodcutting & mining
        //   - Look up target player by index
        //   - Transfer item from sender to target (add to target, delete from sender)
        Console.WriteLine($"[ItemGive] Player {session.SessionId} giving item {message.ItemId} to player index {message.TargetPlayerIndex}");
        return Task.CompletedTask;
    }
}
