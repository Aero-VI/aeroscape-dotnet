using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

/// <summary>
/// Handles player commands (chat text starting with ::).
/// Translated from legacy DavidScape Commands.java.
///
/// The legacy code was a monolithic if/else chain covering player, mod, and admin commands.
/// This handler dispatches to focused sub-handlers via a clean pattern; individual command
/// implementations will be added as the rewrite progresses.
/// </summary>
public class CommandMessageHandler : IMessageHandler<CommandMessage>
{
    private readonly ILogger<CommandMessageHandler> _logger;

    public CommandMessageHandler(ILogger<CommandMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(PlayerSession session, CommandMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
            return Task.CompletedTask;

        var commandText = message.CommandText;
        if (string.IsNullOrWhiteSpace(commandText))
            return Task.CompletedTask;

        var parts = commandText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLowerInvariant();
        var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

        _logger.LogDebug("Player {Username} issued command: {Command} (args: {Args})",
            player.Username, command, string.Join(", ", args));

        // TODO: Check if player is jailed — jailed players can only ::yell
        // if (player.IsJailed && command != "yell")
        // {
        //     session.SendMessage("You have been jailed! You can't do anything except yell!");
        //     return Task.CompletedTask;
        // }

        // TODO: Check if player is in combat — some commands are blocked during combat
        // if (player.IsAttackingPlayer) return Task.CompletedTask;

        // Dispatch based on rights level:
        //   Rights >= 0 → Player commands
        //   Rights >= 1 → Moderator commands
        //   Rights >= 2 → Administrator commands

        switch (command)
        {
            // ── Player Commands (Rights >= 0) ──────────────────────────────────
            case "home":
                HandleHome(session, player);
                break;

            case "players":
                HandlePlayers(session, player);
                break;

            case "commands":
            case "help":
                HandleHelp(session, player);
                break;

            case "changepass":
                HandleChangePassword(session, player, args);
                break;

            case "yell":
                HandleYell(session, player, commandText);
                break;

            // ── Moderator Commands (Rights >= 1) ───────────────────────────────
            case "kick":
                if (player.Rights >= 1)
                    HandleKick(session, player, args);
                break;

            case "mute":
                if (player.Rights >= 1)
                    HandleMute(session, player, args);
                break;

            case "unmute":
                if (player.Rights >= 1)
                    HandleUnmute(session, player, args);
                break;

            case "ban":
                if (player.Rights >= 1)
                    HandleBan(session, player, args);
                break;

            case "jail":
                if (player.Rights >= 1)
                    HandleJail(session, player, args);
                break;

            // ── Admin Commands (Rights == 2) ───────────────────────────────────
            case "tele":
                if (player.Rights >= 2)
                    HandleTeleport(session, player, args);
                break;

            case "item":
                if (player.Rights >= 2)
                    HandleItem(session, player, args);
                break;

            case "coords":
                if (player.Rights >= 2)
                    HandleCoords(session, player);
                break;

            case "teleto":
                if (player.Rights >= 2)
                    HandleTeleTo(session, player, args);
                break;

            case "teletome":
                if (player.Rights >= 2)
                    HandleTeleToMe(session, player, args);
                break;

            default:
                _logger.LogDebug("Unknown command '{Command}' from {Username}", command, player.Username);
                break;
        }

        return Task.CompletedTask;
    }

    // ── Command Implementations (stubs — to be wired to game engine) ───────────

    private void HandleHome(PlayerSession session, Entities.Player player)
    {
        // TODO: Check duel arena
        // player.TeleportTo(3222, 3219, 0);
        _logger.LogInformation("Player {Username} teleporting home", player.Username);
    }

    private void HandlePlayers(PlayerSession session, Entities.Player player)
    {
        // TODO: Count online players and send list via interface 275
        // session.SendMessage($"Players Online: {count}");
        _logger.LogInformation("Player {Username} requested player list", player.Username);
    }

    private void HandleHelp(PlayerSession session, Entities.Player player)
    {
        // TODO: Show commands interface (255)
        _logger.LogInformation("Player {Username} requested help/commands", player.Username);
    }

    private void HandleChangePassword(PlayerSession session, Entities.Player player, string[] args)
    {
        if (args.Length < 1)
            return;

        player.Password = args[0];
        // TODO: session.SendMessage($"Your new password is {player.Password}");
        _logger.LogInformation("Player {Username} changed password", player.Username);
    }

    private void HandleYell(PlayerSession session, Entities.Player player, string fullCommand)
    {
        // TODO: Check muted status
        // if (player.IsMuted) { session.SendMessage("You can't yell because you are muted!"); return; }

        if (fullCommand.Length <= 5)
            return;

        var yellMessage = fullCommand[5..]; // strip "yell "

        var prefix = player.Rights switch
        {
            0 => "[Player]",
            1 => "[Moderator]",
            2 => "[Administrator]",
            _ => "[Player]"
        };

        _logger.LogInformation("YELL: {Prefix} {Username}: {Message}", prefix, player.Username, yellMessage);
        // TODO: Broadcast to all players
        // foreach (var p in Engine.Players)
        //     p.Session.SendMessage($"{prefix} {player.Username}: {yellMessage}");
    }

    private void HandleKick(PlayerSession session, Entities.Player player, string[] args)
    {
        // TODO: Look up target player and disconnect them
        _logger.LogInformation("Mod {Username} kicking player: {Target}", player.Username, string.Join(" ", args));
    }

    private void HandleMute(PlayerSession session, Entities.Player player, string[] args)
    {
        // TODO: Look up target player, set muted flag
        _logger.LogInformation("Mod {Username} muting player: {Target}", player.Username, string.Join(" ", args));
    }

    private void HandleUnmute(PlayerSession session, Entities.Player player, string[] args)
    {
        // TODO: Look up target player, clear muted flag
        _logger.LogInformation("Mod {Username} unmuting player: {Target}", player.Username, string.Join(" ", args));
    }

    private void HandleBan(PlayerSession session, Entities.Player player, string[] args)
    {
        // TODO: Look up target player, write ban record
        _logger.LogInformation("Mod {Username} banning player: {Target}", player.Username, string.Join(" ", args));
    }

    private void HandleJail(PlayerSession session, Entities.Player player, string[] args)
    {
        // TODO: Look up target player, set jailed flag, teleport to jail coords (2604, 3105)
        _logger.LogInformation("Mod {Username} jailing player: {Target}", player.Username, string.Join(" ", args));
    }

    private void HandleTeleport(PlayerSession session, Entities.Player player, string[] args)
    {
        if (args.Length < 3)
            return;

        if (int.TryParse(args[0], out var x) &&
            int.TryParse(args[1], out var y) &&
            int.TryParse(args[2], out var h))
        {
            // TODO: player.SetCoords(x, y, h);
            _logger.LogInformation("Admin {Username} teleporting to ({X}, {Y}, {H})", player.Username, x, y, h);
        }
    }

    private void HandleItem(PlayerSession session, Entities.Player player, string[] args)
    {
        if (args.Length < 2)
            return;

        if (int.TryParse(args[0], out var itemId) &&
            int.TryParse(args[1], out var amount))
        {
            // TODO: Engine.PlayerItems.AddItem(player, itemId, amount);
            _logger.LogInformation("Admin {Username} spawning item {ItemId} x{Amount}", player.Username, itemId, amount);
        }
    }

    private void HandleCoords(PlayerSession session, Entities.Player player)
    {
        // TODO: session.SendMessage($"x: {player.AbsX}, y: {player.AbsY}");
        _logger.LogInformation("Admin {Username} requested coords", player.Username);
    }

    private void HandleTeleTo(PlayerSession session, Entities.Player player, string[] args)
    {
        // TODO: Look up target player, teleport self to their coords
        _logger.LogInformation("Admin {Username} teleporting to player: {Target}", player.Username, string.Join(" ", args));
    }

    private void HandleTeleToMe(PlayerSession session, Entities.Player player, string[] args)
    {
        // TODO: Look up target player, teleport them to self
        _logger.LogInformation("Admin {Username} summoning player: {Target}", player.Username, string.Join(" ", args));
    }
}
