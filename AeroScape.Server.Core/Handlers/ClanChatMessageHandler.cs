using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ClanChatMessageHandler : IMessageHandler<ClanChatMessage>
{
    private readonly ILogger<ClanChatMessageHandler> _logger;
    private readonly ClanChatService _clanChat;

    public ClanChatMessageHandler(ILogger<ClanChatMessageHandler> logger, ClanChatService clanChat)
    {
        _logger = logger;
        _clanChat = clanChat;
    }
    public Task HandleAsync(PlayerSession session, ClanChatMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is { } player)
        {
            if (!string.IsNullOrWhiteSpace(message.ClanName))
                _clanChat.JoinChat(player, message.ClanName);

            if (!string.IsNullOrWhiteSpace(message.Message))
                _clanChat.SendMessage(player, message.Message);
        }

        _logger.LogInformation("[ClanChat] Player {SessionId} ({PlayerName}) sent message to clan {ClanName}: {Message}", session.SessionId, message.PlayerName, message.ClanName, message.Message);
        return Task.CompletedTask;
    }
}
