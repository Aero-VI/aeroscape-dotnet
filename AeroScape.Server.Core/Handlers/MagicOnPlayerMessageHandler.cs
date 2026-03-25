using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class MagicOnPlayerMessageHandler : IMessageHandler<MagicOnPlayerMessage>
{
    public Task HandleAsync(PlayerSession session, MagicOnPlayerMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement magic-on-player PvP logic.
        // Legacy checked wilderness boundaries, duel arena partner, clan field teammates,
        // and jail/lobby restrictions before allowing spell casting.
        // InterfaceId 388 handled modern PvP spells (e.g. Ice Barrage with ButtonId 3).
        Console.WriteLine($"[MagicOnPlayer] Player {session.SessionId} cast spell (button {message.ButtonId}, interface {message.InterfaceId}) on player {message.PlayerId}");
        return Task.CompletedTask;
    }
}
