using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class DialogueContinueMessageHandler : IMessageHandler<DialogueContinueMessage>
{
    public Task HandleAsync(PlayerSession session, DialogueContinueMessage message, CancellationToken cancellationToken)
    {
        // TODO: Advance the player's current dialogue state.
        // The legacy code had a massive switch on p.Dialogue (0-111+) handling skill capes,
        // quests (Dragon Slayer), destroy confirmations, and more.
        // This will be refactored into a proper DialogueService in future phases.
        Console.WriteLine($"[DialogueContinue] Player {session.SessionId}");
        return Task.CompletedTask;
    }
}
