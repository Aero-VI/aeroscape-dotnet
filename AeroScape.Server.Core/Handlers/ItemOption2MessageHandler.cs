using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemOption2MessageHandler : IMessageHandler<ItemOption2Message>
{
    private readonly ILogger<ItemOption2MessageHandler> _logger;
    private readonly PlayerItemsService _items;
    private readonly PlayerEquipmentService _equipment;

    public ItemOption2MessageHandler(ILogger<ItemOption2MessageHandler> logger, PlayerItemsService items, PlayerEquipmentService equipment)
    {
        _logger = logger;
        _items = items;
        _equipment = equipment;
    }
    public Task HandleAsync(PlayerSession session, ItemOption2Message message, CancellationToken cancellationToken)
    {
        if (session.Entity is { } player &&
            message.InterfaceId == 387 &&
            message.ItemSlot >= 0 &&
            message.ItemSlot < player.Equipment.Length &&
            player.Equipment[message.ItemSlot] == message.ItemId)
        {
            int amount = player.EquipmentN[message.ItemSlot] > 0 ? player.EquipmentN[message.ItemSlot] : 1;
            if (_items.AddItem(player, message.ItemId, amount))
            {
                player.Equipment[message.ItemSlot] = -1;
                player.EquipmentN[message.ItemSlot] = 0;
                _equipment.ApplyWeaponState(player);
                _equipment.RecalculateBonuses(player);
                player.AppearanceUpdateReq = true;
                player.UpdateReq = true;
            }
        }

        _logger.LogInformation("[ItemOption2] Player {SessionId} used item option 2: ItemId={ItemId}, Slot={ItemSlot}, Interface={InterfaceId}", session.SessionId, message.ItemId, message.ItemSlot, message.InterfaceId);
        return Task.CompletedTask;
    }
}
