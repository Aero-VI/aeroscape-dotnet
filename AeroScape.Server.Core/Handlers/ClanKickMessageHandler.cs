using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

public sealed class ClanKickMessageHandler : IMessageHandler<ClanKickMessage>
{
    private readonly ILogger<ClanKickMessageHandler> _logger;
    private readonly ClanChatService _clanChat;

    public ClanKickMessageHandler(ILogger<ClanKickMessageHandler> logger, ClanChatService clanChat)
    {
        _logger = logger;
        _clanChat = clanChat;
    }

    public Task HandleAsync(PlayerSession session, ClanKickMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is null)
            return Task.CompletedTask;

        _clanChat.Kick(session.Entity, message.PlayerName);
        _logger.LogInformation("[ClanKick] Player {Username} kick={Target}", session.Entity.Username, message.PlayerName);
        return Task.CompletedTask;
    }
}
