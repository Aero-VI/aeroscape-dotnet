using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class RemoveIgnoreMessageHandler : IMessageHandler<RemoveIgnoreMessage>
{
    public Task HandleAsync(PlayerSession session, RemoveIgnoreMessage message, CancellationToken cancellationToken)
    {
        session.Entity?.Ignores.Remove(message.EncodedName);
        return Task.CompletedTask;
    }
}
