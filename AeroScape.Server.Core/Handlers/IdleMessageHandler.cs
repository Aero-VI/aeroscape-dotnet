using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class IdleMessageHandler : IMessageHandler<IdleMessage>
{
    public Task HandleAsync(PlayerSession session, IdleMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null) return Task.CompletedTask;

        player.Idle++;
        if (player.Idle >= 5)
            player.Disconnected[0] = true;

        return Task.CompletedTask;
    }
}
