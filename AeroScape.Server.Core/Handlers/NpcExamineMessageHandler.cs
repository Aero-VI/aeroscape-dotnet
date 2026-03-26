using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class NpcExamineMessageHandler : IMessageHandler<NpcExamineMessage>
{
    public Task HandleAsync(PlayerSession session, NpcExamineMessage message, CancellationToken cancellationToken)
    {
        // TODO: Look up NPC description and send to player.
        Console.WriteLine($"[NpcExamine] Player {session.SessionId} examined NPC {message.NpcId}");
        return Task.CompletedTask;
    }
}
