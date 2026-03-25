using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
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

    public WalkMessageHandler(ILogger<WalkMessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(PlayerSession session, WalkMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
            return Task.CompletedTask;

        _logger.LogDebug("Player {Username} walking to ({X}, {Y}), running={Running}",
            player.Username, message.X, message.Y, message.IsRunning);

        // TODO: Remove shown interface (p.frames.removeShownInterface)

        // TODO: Reset walking queue via movement engine
        // Engine.playerMovement.resetWalkingQueue(player);

        // TODO: Add first step to walking queue
        // Engine.playerMovement.addToWalkingQueue(player, firstX, firstY);
        // Then add remaining path steps (pathX[i] + firstX, pathY[i] + firstY)

        // Reset interaction flags
        // TODO: These flags need to be added to Player entity:
        // player.ItemPickup = false;
        // player.PlayerOption1 = false;
        // player.PlayerOption2 = false;
        // player.PlayerOption3 = false;
        // player.NpcOption1 = false;
        // player.NpcOption2 = false;
        // player.ObjectOption1 = false;
        // player.ObjectOption2 = false;
        // player.AttackingPlayer = false;
        // player.AttackingNpc = false;

        // TODO: Check freeze delay
        // if (player.FreezeDelay > 0)
        //     session.SendMessage("You can't move! You're frozen!");

        // TODO: Restore client state
        // session.RemoveShownInterface();
        // session.RestoreTabs();
        // session.RestoreInventory();
        // session.RemoveChatboxInterface();

        // TODO: Reset face-to target
        // if (player.FaceToRequest != 65535)
        //     player.RequestFaceTo(65535);

        return Task.CompletedTask;
    }
}
