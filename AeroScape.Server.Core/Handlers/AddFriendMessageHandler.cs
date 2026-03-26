using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class AddFriendMessageHandler : IMessageHandler<AddFriendMessage>
{
    public Task HandleAsync(PlayerSession session, AddFriendMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null) return Task.CompletedTask;

        if (player.Friends.Count >= 200)
        {
            // TODO: session.SendMessage("Your friends list is full.");
            return Task.CompletedTask;
        }
        if (player.Friends.Contains(message.EncodedName))
        {
            // TODO: session.SendMessage("Already on your friends list.");
            return Task.CompletedTask;
        }

        player.Friends.Add(message.EncodedName);
        // TODO: session.SendFriend(message.EncodedName, player.GetWorld(message.EncodedName));

        return Task.CompletedTask;
    }
}
