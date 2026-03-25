using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class MagicOnNPCMessageHandler : IMessageHandler<MagicOnNPCMessage>
{
    public Task HandleAsync(PlayerSession session, MagicOnNPCMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement magic-on-NPC combat logic.
        // Legacy supported two spellbooks:
        //   InterfaceId 192 = Modern (Wind/Water/Earth/Fire Strike/Bolt/Blast/Wave/Surge)
        //   InterfaceId 193 = Ancient Magicks (Smoke/Shadow/Blood/Ice Rush/Burst/Blitz/Barrage)
        // Each spell was identified by ButtonId and required specific runes + magic level.
        Console.WriteLine($"[MagicOnNPC] Player {session.SessionId} cast spell (button {message.ButtonId}, interface {message.InterfaceId}) on NPC index {message.NpcIndex}");
        return Task.CompletedTask;
    }
}
