using System;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemSelectMessageHandler : IMessageHandler<ItemSelectMessage>
{
    public Task HandleAsync(PlayerSession session, ItemSelectMessage message, CancellationToken cancellationToken)
    {
        // TODO: Implement item select logic
        // Legacy behaviour (interfaceId == 149):
        //   - Validate slot bounds and item ownership
        //   - Check player is alive (hp > 0)
        //   - Large switch on itemId covering:
        //       * Bone burying (526, 528, 530, 532, 534, 536, etc.) → delete bone, add prayer XP, play anim 827
        //       * Food eating (391, 385, 397, etc.) → delete food, heal HP, play anim 829
        //       * Potion drinking (3024-3030, 2430-2444, 6685-6691, etc.) → stat boosts, dose replacement
        //       * Imp jar looting (11238, 11240, 11252, 11254, 11256) → random reward
        //       * Herb cleaning (199 → 249, 207 → 257) → herblore XP
        //       * Cannon setup (6), slayer gem (4155), summoning (4447), etc.
        Console.WriteLine($"[ItemSelect] Player {session.SessionId} selected item {message.ItemId} at slot {message.ItemSlot} on interface {message.InterfaceId}");
        return Task.CompletedTask;
    }
}
