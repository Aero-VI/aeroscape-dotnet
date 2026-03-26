using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class PrivateMessageMessageHandler : IMessageHandler<PrivateMessageMessage>
{
    public Task HandleAsync(PlayerSession session, PrivateMessageMessage message, CancellationToken cancellationToken)
    {
        // TODO: Look up target player by encoded name, send received private message frame.
        // If offline, send "Player is currently offline." to sender.
        Console.WriteLine($"[PM] Player {session.SessionId} → target {message.TargetName}: {message.Text}");
        return Task.CompletedTask;
    }
}
