using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

public sealed class ClanJoinMessageHandler : IMessageHandler<ClanJoinMessage>
{
    private readonly ILogger<ClanJoinMessageHandler> _logger;
    private readonly ClanChatService _clanChat;

    public ClanJoinMessageHandler(ILogger<ClanJoinMessageHandler> logger, ClanChatService clanChat)
    {
        _logger = logger;
        _clanChat = clanChat;
    }

    public Task HandleAsync(PlayerSession session, ClanJoinMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is null)
            return Task.CompletedTask;

        if (message.ClanName == "invalid name")
            _clanChat.LeaveChat(session.Entity);
        else
            _clanChat.JoinChat(session.Entity, message.ClanName);

        _logger.LogInformation("[ClanJoin] Player {Username} join={ClanName}", session.Entity.Username, message.ClanName);
        return Task.CompletedTask;
    }
}
