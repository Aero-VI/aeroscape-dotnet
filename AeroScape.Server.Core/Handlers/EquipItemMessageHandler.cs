using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class EquipItemMessageHandler : IMessageHandler<EquipItemMessage>
{
    private readonly ILogger<EquipItemMessageHandler> _logger;
    private readonly PlayerEquipmentService _equipment;

    public EquipItemMessageHandler(ILogger<EquipItemMessageHandler> logger, PlayerEquipmentService equipment)
    {
        _logger = logger;
        _equipment = equipment;
    }
    public Task HandleAsync(PlayerSession session, EquipItemMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
        {
            return Task.CompletedTask;
        }

        var equipped = _equipment.Equip(player, message.ItemId, message.Slot, message.InterfaceId);
        _logger.LogInformation("[EquipItem] Player {SessionId} equip item {ItemId} slot {Slot} success={Success}", session.SessionId, message.ItemId, message.Slot, equipped);
        return Task.CompletedTask;
    }
}
