using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ObjectExamineMessageHandler : IMessageHandler<ObjectExamineMessage>
{
    public Task HandleAsync(PlayerSession session, ObjectExamineMessage message, CancellationToken cancellationToken)
    {
        // TODO: Send object description (or ID if player is mod/admin).
        Console.WriteLine($"[ObjectExamine] Player {session.SessionId} examined object {message.ObjectId}");
        return Task.CompletedTask;
    }
}
