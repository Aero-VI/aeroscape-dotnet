using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class NPCOption3MessageHandler : IMessageHandler<NPCOption3Message>
{
    private readonly ILogger<NPCOption3MessageHandler> _logger;
    private readonly GameEngine _engine;
    private readonly IClientUiService _ui;
    private readonly ShopService _shops;

    public NPCOption3MessageHandler(ILogger<NPCOption3MessageHandler> logger, GameEngine engine, IClientUiService ui, ShopService shops)
    {
        _logger = logger;
        _engine = engine;
        _ui = ui;
        _shops = shops;
    }

    public Task HandleAsync(PlayerSession session, NPCOption3Message message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null || message.NpcIndex <= 0 || message.NpcIndex >= _engine.Npcs.Length)
            return Task.CompletedTask;

        var npc = _engine.Npcs[message.NpcIndex];
        if (npc is null)
            return Task.CompletedTask;

        if (Combat.CombatFormulas.GetDistance(player.AbsX, player.AbsY, npc.AbsX, npc.AbsY) > 1)
            return Task.CompletedTask;

        switch (npc.NpcType)
        {
            case 548:
                _ui.ShowInterface(player, 591);
                break;
            case 553:
                player.SetCoords(3504, 3575, 0);
                break;
            case 1599:
                _shops.OpenShop(player, 8);
                break;
            case 4906:
                _ui.ShowNpcDialogue(player, 4906, "Woodcutting Tutor", "I'll pay you 8 coins per log you bring me.", 9827);
                break;
            case 1861:
                _ui.ShowNpcDialogue(player, 1861, "Range Tutor", "Sorry, I have no work for you today...", 9827);
                break;
        }

        _logger.LogInformation("[NPCOption3] Player {Username} npcType={NpcType}", player.Username, npc.NpcType);
        return Task.CompletedTask;
    }
}
