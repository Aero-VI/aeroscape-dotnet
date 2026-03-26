using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemGiveMessageHandler : IMessageHandler<ItemGiveMessage>
{
    private readonly ILogger<ItemGiveMessageHandler> _logger;
    private readonly GameEngine _engine;
    private readonly PlayerItemsService _items;

    public ItemGiveMessageHandler(ILogger<ItemGiveMessageHandler> logger, GameEngine engine, PlayerItemsService items)
    {
        _logger = logger;
        _engine = engine;
        _items = items;
    }
    public Task HandleAsync(PlayerSession session, ItemGiveMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        var target = message.TargetPlayerIndex >= 0 && message.TargetPlayerIndex < _engine.Players.Length
            ? _engine.Players[message.TargetPlayerIndex]
            : null;

        if (player is not null && target is not null)
        {
            _items.TransferItem(player, target, message.ItemId, 1);
        }

        _logger.LogInformation("[ItemGive] Player {SessionId} giving item {ItemId} to player index {TargetPlayerIndex}", session.SessionId, message.ItemId, message.TargetPlayerIndex);
        return Task.CompletedTask;
    }
}
