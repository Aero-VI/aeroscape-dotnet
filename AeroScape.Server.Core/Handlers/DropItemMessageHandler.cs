using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class DropItemMessageHandler : IMessageHandler<DropItemMessage>
{
    private readonly ILogger<DropItemMessageHandler> _logger;
    private readonly PlayerItemsService _items;
    private readonly GroundItemManager _groundItems;
    private readonly ItemDefinitionLoader _definitions;

    public DropItemMessageHandler(ILogger<DropItemMessageHandler> logger, PlayerItemsService items, GroundItemManager groundItems, ItemDefinitionLoader definitions)
    {
        _logger = logger;
        _items = items;
        _groundItems = groundItems;
        _definitions = definitions;
    }
    public Task HandleAsync(PlayerSession session, DropItemMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null || message.Slot < 0 || message.Slot >= player.Items.Length || player.Items[message.Slot] != message.ItemId)
        {
            return Task.CompletedTask;
        }

        if (_definitions.IsUntradable(message.ItemId))
        {
            return Task.CompletedTask;
        }

        var amount = player.ItemsN[message.Slot];
        if (_groundItems.CreateGroundItem(message.ItemId, amount, player.AbsX, player.AbsY, player.HeightLevel) &&
            _items.DeleteItem(player, message.ItemId, message.Slot, amount))
        {
            _logger.LogInformation("[DropItem] Player {SessionId} dropped item {ItemId} x{Amount} from slot {Slot}", session.SessionId, message.ItemId, amount, message.Slot);
        }

        return Task.CompletedTask;
    }
}
