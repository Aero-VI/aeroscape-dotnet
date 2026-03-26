using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class ItemOnObjectMessageHandler : IMessageHandler<ItemOnObjectMessage>
{
    private readonly ILogger<ItemOnObjectMessageHandler> _logger;

    public ItemOnObjectMessageHandler(ILogger<ItemOnObjectMessageHandler> logger)
    {
        _logger = logger;
    }
    public Task HandleAsync(PlayerSession session, ItemOnObjectMessage message, CancellationToken cancellationToken)
    {
        if (session.Entity is { } player)
        {
            if (AeroScape.Server.Core.Skills.SmithingSkill.IsFurnaceObject(message.ObjectId))
                player.Smithing.SmeltOre(message.ItemId);
            else if (AeroScape.Server.Core.Skills.SmithingSkill.IsAnvilObject(message.ObjectId))
                player.Smithing.OpenSmithingInterface(message.ItemId);
        }

        _logger.LogInformation("[ItemOnObject] Player {SessionId} used item {ItemId} on object {ObjectId}", session.SessionId, message.ItemId, message.ObjectId);
        return Task.CompletedTask;
    }
}
