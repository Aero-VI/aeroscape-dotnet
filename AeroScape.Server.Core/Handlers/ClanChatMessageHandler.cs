using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ClanChatMessageHandler : IMessageHandler<ClanChatMessage>
{
    public Task HandleAsync(PlayerSession session, ClanChatMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement clan chat logic
        Console.WriteLine($"[ClanChat] Player {session.SessionId} ({message.PlayerName}) sent message to clan {message.ClanName}: {message.Message}");
        return Task.CompletedTask;
    }
}