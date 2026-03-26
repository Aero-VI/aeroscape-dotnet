using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class NPCOption1MessageHandler : IMessageHandler<NPCOption1Message>
{
    private readonly ILogger<NPCOption1MessageHandler> _logger;
    private readonly GameEngine _engine;
    private readonly ShopService _shops;
    private readonly DialogueService _dialogues;
    private readonly IClientUiService _ui;

    public NPCOption1MessageHandler(ILogger<NPCOption1MessageHandler> logger, GameEngine engine, ShopService shops, DialogueService dialogues, IClientUiService ui)
    {
        _logger = logger;
        _engine = engine;
        _shops = shops;
        _dialogues = dialogues;
        _ui = ui;
    }

    public Task HandleAsync(PlayerSession session, NPCOption1Message message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null || message.NpcIndex <= 0 || message.NpcIndex >= _engine.Npcs.Length)
            return Task.CompletedTask;

        var npc = _engine.Npcs[message.NpcIndex];
        if (npc is null)
            return Task.CompletedTask;

        switch (npc.NpcType)
        {
            case 312:
            case 313:
            case 316:
                player.Fishing.StartFishing(npc.NpcType, 1);
                break;
            case 494:
            case 495:
            case 2619:
                _ui.ShowNpcDialogue(player, npc.NpcType, "Banker", "Hello there, you can bank by selecting the bank option.");
                break;
            case 2270:
                _ui.ShowNpcDialogue(player, 2270, "Martin Thwait", "What are you looking at noob?", 9827);
                break;
            case 682:
            case 6970:
                _shops.OpenShop(player, npc.NpcType switch
                {
                    682 => 3,
                    6970 => 11,
                    _ => 1
                });
                break;
            case 521:
                _ui.ShowNpcDialogue(player, 521, "Shop keeper", "Hey, I sell some good stuplies and weapons.");
                break;
            case 549:
                _ui.ShowNpcDialogue(player, 549, "Horvik", "I got a ton of armour if you want to buy some.");
                break;
            case 548:
                _ui.ShowNpcDialogue(player, 548, "Thessalia", "Don't forget you can get better items by killing monsters!");
                break;
            case 198:
                if (player.DragonSlayer == 0)
                    _dialogues.Start(player, 100);
                else if (player.DragonSlayer == 2)
                    _dialogues.Start(player, 102);
                else if (player.DragonSlayer == 4)
                {
                    player.Dialogue = 110;
                    _ui.ShowNpcDialogue(player, 198, "Guildmaster", "Wow you slayed Elvarg! Accept this reward.");
                }
                else if (player.DragonSlayer == 5)
                    _ui.ShowNpcDialogue(player, 198, "Guildmaster", "Hello...");
                else
                    _ui.ShowNpcDialogue(player, 198, "Guildmaster", "Go speak to Oziach in Edgeville...");
                break;
            case 747:
                if (player.DragonSlayer == 1)
                    _dialogues.Start(player, 105);
                else if (player.DragonSlayer == 2)
                    _ui.ShowNpcDialogue(player, 747, "Oziach", "Go speak to the guildmaster for help.");
                else if (player.DragonSlayer == 3)
                    _ui.ShowNpcDialogue(player, 747, "Oziach", "You haven't killed Elvarg yet noob!");
                else if (player.DragonSlayer == 4)
                    _ui.ShowNpcDialogue(player, 747, "Oziach", "Wow, you killed Elvarg? Go tell the Guildmaster!");
                else
                    _ui.ShowNpcDialogue(player, 747, "Oziach", "Good job killing that dragon.");
                break;
            case 746:
                if (player.DragonSlayer < 3)
                    _dialogues.Start(player, 108);
                else
                    _ui.ShowNpcDialogue(player, 746, "Oracle", "Take this anti-dragon shield...");
                break;
            case 2253:
                if (player.QuestPoints < 2)
                    _ui.ShowNpcDialogue(player, 2567, "Wise Old Man", "Ehh, you haven't completed all DavidScape Quests...");
                else
                    _dialogues.Start(player, 111);
                break;
        }

        _logger.LogInformation("[NPCOption1] Player {Username} npcType={NpcType}", player.Username, npc.NpcType);
        return Task.CompletedTask;
    }
}
