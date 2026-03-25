using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemOnNPCMessageHandler : IMessageHandler<ItemOnNPCMessage>
{
    public Task HandleAsync(PlayerSession session, ItemOnNPCMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement item-on-NPC logic (e.g. feeding, quest items, using bones on altars, etc.)
        // Legacy checked InterfaceId == 33152 for a specific interaction.
        Console.WriteLine($"[ItemOnNPC] Player {session.SessionId} used item {message.ItemId} on NPC index {message.NpcIndex} (interface {message.InterfaceId})");
        return Task.CompletedTask;
    }
}
