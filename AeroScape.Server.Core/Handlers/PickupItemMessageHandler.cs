using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class PickupItemMessageHandler : IMessageHandler<PickupItemMessage>
{
    private readonly ILogger<PickupItemMessageHandler> _logger;
    private readonly GroundItemManager _groundItems;
    private readonly PlayerItemsService _items;

    public PickupItemMessageHandler(ILogger<PickupItemMessageHandler> logger, GroundItemManager groundItems, PlayerItemsService items)
    {
        _logger = logger;
        _groundItems = groundItems;
        _items = items;
    }
    public Task HandleAsync(PlayerSession session, PickupItemMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
        {
            return Task.CompletedTask;
        }

        var groundItem = _groundItems.GetPickupCandidate(player, message.ItemId, message.ItemX, message.ItemY);
        var pickedUp = groundItem is not null
            && _items.AddItem(player, groundItem.ItemId, groundItem.ItemAmt);

        if (pickedUp && groundItem is not null)
        {
            _groundItems.Remove(groundItem.Index);
        }

        _logger.LogInformation("[PickupItem] Player {SessionId} pickup item {ItemId} at ({ItemX}, {ItemY}) success={Success}", session.SessionId, message.ItemId, message.ItemX, message.ItemY, pickedUp);
        return Task.CompletedTask;
    }
}
