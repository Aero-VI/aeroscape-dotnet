using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Movement;
using AeroScape.Server.Core.Session;
using Microsoft.Extensions.Logging;

namespace AeroScape.Server.Core.Handlers;

/// <summary>
/// Handles walking packets (regular walk, mini-map walk, etc.).
/// Translated from legacy DavidScape Walking.java.
/// </summary>
public class WalkMessageHandler : IMessageHandler<WalkMessage>
{
    private readonly ILogger<WalkMessageHandler> _logger;
    private readonly WalkQueue _walkQueue;

    public WalkMessageHandler(ILogger<WalkMessageHandler> logger, WalkQueue walkQueue)
    {
        _logger = logger;
        _walkQueue = walkQueue;
    }

    public Task HandleAsync(PlayerSession session, WalkMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
            return Task.CompletedTask;

        _logger.LogDebug(
            "Player {Username} walking: first=({X}, {Y}) steps={Steps} running={Running} opcode={Opcode}",
            player.Username,
            message.FirstX,
            message.FirstY,
            message.PathX.Length + 1,
            message.IsRunning,
            message.PacketId);

        _walkQueue.HandleWalk(player, message);

        return Task.CompletedTask;
    }
}
