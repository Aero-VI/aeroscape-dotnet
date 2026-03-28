using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Entities;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

/// <summary>
/// Handles public chat messages from players.
/// Translated from legacy DavidScape PublicChat.java.
/// </summary>
public class PublicChatMessageHandler : IMessageHandler<PublicChatMessage>
{
    private readonly ILogger<PublicChatMessageHandler> _logger;
    private readonly ClanChatService _clanChat;

    /// <summary>
    /// Words to censor from public chat. In production this should be loaded from config.
    /// </summary>
    private static readonly string[] CensoredWords =
    {
        "fuck", "damn", "shit", "fuk", "4uck", "bitch",
        "sex", "pussy", "vagina", "dick", "blow", "bastard"
    };

    public PublicChatMessageHandler(ILogger<PublicChatMessageHandler> logger, ClanChatService clanChat)
    {
        _logger = logger;
        _clanChat = clanChat;
    }

    public Task HandleAsync(PlayerSession session, PublicChatMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
            return Task.CompletedTask;

        var chatText = message.Text;
        if (string.IsNullOrEmpty(chatText))
            return Task.CompletedTask;

        _logger.LogDebug("Player {Username} public chat: effects={Effects}, color={Color}",
            player.Username, message.Effects, message.Color);

        // Apply word filter
        chatText = ApplyCensorFilter(chatText);

        // Check for clan chat prefix (messages starting with "/")
        if (chatText.StartsWith('/'))
        {
            var clanMessage = chatText[1..].TrimStart();
            HandleClanChat(session, player, clanMessage);
            return Task.CompletedTask;
        }

        // Regular public chat
        // TODO: Check muted status
        // if (player.IsMuted)
        // {
        //     session.SendMessage("You cannot talk because you are muted. Follow the rules!");
        //     return Task.CompletedTask;
        // }

        // Set chat update flags on player entity for the update processor
        player.ChatText = chatText;
        player.ChatTextEffects = message.Effects;
        player.ChatTextUpdateReq = true;
        player.UpdateReq = true;

        _logger.LogDebug("Player {Username} says: {Text}", player.Username, chatText);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Replaces censored words with asterisks (case-insensitive).
    /// </summary>
    private static string ApplyCensorFilter(string text)
    {
        foreach (var word in CensoredWords)
        {
            var index = text.IndexOf(word, StringComparison.OrdinalIgnoreCase);
            while (index >= 0)
            {
                var replacement = new string('*', word.Length);
                text = string.Concat(text.AsSpan(0, index), replacement, text.AsSpan(index + word.Length));
                index = text.IndexOf(word, index + replacement.Length, StringComparison.OrdinalIgnoreCase);
            }
        }

        return text;
    }

    private void HandleClanChat(PlayerSession session, Player player, string message)
    {
        _clanChat.SendMessage(player, message);
        _logger.LogDebug("Player {Username} clan chat: {Message}", player.Username, message);
    }
}
