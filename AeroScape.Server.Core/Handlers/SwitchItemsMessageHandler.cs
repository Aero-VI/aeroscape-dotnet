using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class SwitchItemsMessageHandler : IMessageHandler<SwitchItemsMessage>
{
    private readonly ILogger<SwitchItemsMessageHandler> _logger;
    private readonly PlayerItemsService _items;

    public SwitchItemsMessageHandler(ILogger<SwitchItemsMessageHandler> logger, PlayerItemsService items)
    {
        _logger = logger;
        _items = items;
    }
    public Task HandleAsync(PlayerSession session, SwitchItemsMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
        {
            return Task.CompletedTask;
        }

        if (message.InterfaceId == 149)
        {
            _items.SwapInventoryItems(player, message.FromSlot, message.ToSlot);
        }

        _logger.LogInformation("[SwitchItems] Player {SessionId} swapped slot {FromSlot} -> {ToSlot} on interface {InterfaceId}", session.SessionId, message.FromSlot, message.ToSlot, message.InterfaceId);
        return Task.CompletedTask;
    }
}
