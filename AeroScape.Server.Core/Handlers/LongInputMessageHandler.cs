using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;
using AeroScape.Server.Core.Util;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

public sealed class LongInputMessageHandler : IMessageHandler<LongInputMessage>
{
    private readonly ILogger<LongInputMessageHandler> _logger;
    private readonly ClanChatService _clanChat;

    public LongInputMessageHandler(ILogger<LongInputMessageHandler> logger, ClanChatService clanChat)
    {
        _logger = logger;
        _clanChat = clanChat;
    }

    public Task HandleAsync(PlayerSession session, LongInputMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is null)
            return Task.CompletedTask;

        string value = NameUtil.LongToString(message.Value).Replace('_', ' ');
        if (session.Entity.InputId == 0)
            _clanChat.CreateOrRenameChat(session.Entity, value);

        _logger.LogInformation("[LongInput] Player {Username} inputId={InputId} value={Value}", session.Entity.Username, session.Entity.InputId, value);
        return Task.CompletedTask;
    }
}
