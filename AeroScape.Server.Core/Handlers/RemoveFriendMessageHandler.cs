using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class RemoveFriendMessageHandler : IMessageHandler<RemoveFriendMessage>
{
    public Task HandleAsync(PlayerSession session, RemoveFriendMessage message, CancellationToken cancellationToken)
    {
        session.Entity?.Friends.Remove(message.EncodedName);
        return Task.CompletedTask;
    }
}
