using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class NPCAttackMessageHandler : IMessageHandler<NPCAttackMessage>
{
    public Task HandleAsync(PlayerSession session, NPCAttackMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[NPCAttack] Player {session.SessionId} attacked NPC {message.NpcIndex}");
        return Task.CompletedTask;
    }
}