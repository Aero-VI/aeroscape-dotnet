using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class AddIgnoreMessageHandler : IMessageHandler<AddIgnoreMessage>
{
    public Task HandleAsync(PlayerSession session, AddIgnoreMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null) return Task.CompletedTask;

        if (player.Ignores.Count >= 100)
        {
            // TODO: session.SendMessage("Your ignore list is full.");
            return Task.CompletedTask;
        }
        if (player.Ignores.Contains(message.EncodedName))
        {
            // TODO: session.SendMessage("Already on your ignore list.");
            return Task.CompletedTask;
        }

        player.Ignores.Add(message.EncodedName);
        return Task.CompletedTask;
    }
}
