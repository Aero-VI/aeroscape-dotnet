using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class BountyHunterMessageHandler : IMessageHandler<BountyHunterMessage>
{
    public Task HandleAsync(PlayerSession session, BountyHunterMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement bounty hunter target logic
        Console.WriteLine($"[BountyHunter] Player {session.SessionId} updated target to {message.TargetId}");
        return Task.CompletedTask;
    }
}