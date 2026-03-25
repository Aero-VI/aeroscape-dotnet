using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class PlayerOption1MessageHandler : IMessageHandler<PlayerOption1Message>
{
    public Task HandleAsync(PlayerSession session, PlayerOption1Message message, CancellationToken cancellationToken)
    {
        // TODO: Implement Player Option 1 logic (e.g. Attack, Trade, Follow)
        Console.WriteLine($"[PlayerOption1] Player {session.SessionId} interacted with target {message.TargetIndex}");
        return Task.CompletedTask;
    }
}