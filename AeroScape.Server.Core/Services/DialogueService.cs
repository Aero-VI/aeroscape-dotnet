using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public sealed class DialogueService
{
    private readonly InventoryService _inventory;
    private readonly IClientUiService _ui;

    public DialogueService(InventoryService inventory, IClientUiService ui)
    {
        _inventory = inventory;
        _ui = ui;
    }

    public bool Continue(Player player)
    {
        int nextDialogue = player.Dialogue;
        switch (player.Dialogue)
        {
            case 1: AcceptCape(player, 9779, 9777); return true;
            case 2:
            case 4:
            case 6:
            case 8:
            case 10:
            case 12:
            case 22:
            case 24:
            case 25:
            case 33:
            case 35:
            case 41:
            case 43:
            case 45:
            case 53:
            case 55:
                player.Dialogue = 0; return true;
            case 3: AcceptCape(player, 9764, 9762); return true;
            case 5: AcceptCape(player, 9755, 9753); return true;
            case 7: AcceptCape(player, 9749, 9747); return true;
            case 9: AcceptCape(player, 9752, 9750); return true;
            case 11: AcceptCape(player, 9770, 9768); return true;
            case 13: AcceptCape(player, 9800, 9798); return true;
            case 14: Give(player, 305, 307, 301, 311); return true;
            case 15: AcceptCape(player, 9809, 9807); return true;
            case 16: Give(player, 1359); return true;
            case 17: AcceptCape(player, 9794, 9792); return true;
            case 18: Give(player, 1275); return true;
            case 19: AcceptCape(player, 9761, 9759); return true;
            case 20: Give(player, player.SkillLvl[5] > 69 ? 536 : 526, 28); return true;
            case 21: AcceptCape(player, 9767, 9765); return true;
            case 23: AcceptCape(player, 9758, 9756); return true;
            case 26: AcceptCape(player, 9806, 9804); return true;
            case 27: Give(player, 590); return true;
            case 28: AcceptCape(player, 9785, 9783); return true;
            case 29: Give(player, 946); return true;
            case 30:
            case 31:
                player.Dialogue = 0; return true;
            case 32: AcceptCape(player, 9773, 9771); return true;
            case 34: AcceptCape(player, 9782, 9780); return true;
            case 36: AcceptCape(player, 9776, 9774); return true;
            case 37: Give(player, player.SkillLvl[15] > 69 ? 207 : 199, 28); return true;
            case 38: AcceptCape(player, 9950, 9948); return true;
            case 39: Give(player, 11259); return true;
            case 40: AcceptCape(player, 9803, 9801); return true;
            case 42: AcceptCape(player, 9797, 9795); return true;
            case 44: AcceptCape(player, 9788, 9786); return true;
            case 50: GiveFarmingReward(player); return true;
            case 51: AcceptCape(player, 9812, 9810); return true;
            case 52: AcceptCape(player, 9791, 9789); return true;
            case 54: AcceptCape(player, 12171, 12169); return true;
            case 100: player.Dialogue = 101; break;
            case 101: player.DragonSlayer = 1; player.Dialogue = 0; return true;
            case 102: player.Dialogue = 103; break;
            case 103: player.DragonSlayer = 3; player.Dialogue = 104; break;
            case 104: player.Dialogue = 0; player.Choice = 1; return true;
            case 105: player.Dialogue = 106; break;
            case 106: player.Dialogue = 107; break;
            case 107: player.DragonSlayer = 2; player.Dialogue = 0; return true;
            case 108: Give(player, 1538); return true;
            case 109: player.SetCoords(3048, 3208, 1); player.Dialogue = 0; return true;
            case 110:
                player.QuestPoints += 2;
                player.DragonSlayer = 5;
                player.AddSkillXP(180650, 0);
                player.AddSkillXP(180650, 2);
                player.Dialogue = 0;
                return true;
            case 111:
                Give(player, 9814, 1);
                _inventory.AddItem(player, 9813, 1);
                return true;
            default:
                player.Dialogue = 0;
                return false;
        }

        nextDialogue = player.Dialogue;
        if (nextDialogue > 0)
            Render(player);
        return true;
    }

    public void Start(Player player, int dialogue)
    {
        player.Dialogue = dialogue;
        Render(player);
    }

    private void AcceptCape(Player player, int hoodId, int capeId)
    {
        _inventory.AddItem(player, hoodId, 1);
        _inventory.AddItem(player, capeId, 1);
        player.Dialogue = 0;
    }

    private void Give(Player player, int itemId, int amount = 1)
    {
        _inventory.AddItem(player, itemId, amount);
        player.Dialogue = 0;
    }

    private void Give(Player player, int item1, int item2, int item3, int item4)
    {
        _inventory.AddItem(player, item1, 1);
        _inventory.AddItem(player, item2, 1);
        _inventory.AddItem(player, item3, 1);
        _inventory.AddItem(player, item4, 1);
        player.Dialogue = 0;
    }

    private void GiveFarmingReward(Player player)
    {
        if (player.SkillLvl[19] < 40)
            _inventory.AddItem(player, 5096, 1);
        else if (player.SkillLvl[19] < 60)
            _inventory.AddItem(player, 5283, 1);
        else if (player.SkillLvl[19] < 80)
            _inventory.AddItem(player, 5100, 1);
        else
            _inventory.AddItem(player, 5288, 1);

        player.Dialogue = 0;
    }

    private void Render(Player player)
    {
        switch (player.Dialogue)
        {
            case 100:
                _ui.ShowNpcDialogue(player, 198, "Guildmaster", "Looking for a quest are you?");
                break;
            case 101:
                _ui.ShowNpcDialogue(player, 198, "Guildmaster", "Well then speak to Oziach in Edgeville.");
                break;
            case 102:
                _ui.ShowNpcDialogue(player, 198, "Guildmaster", "Ah yes, the dragon of Crandor Island...");
                break;
            case 103:
                _ui.ShowNpcDialogue(player, 198, "Guildmaster", "You will need a map, ship, and somthing to");
                break;
            case 104:
                _ui.ShowOptionDialogue(player, "How can I find a route to Crandor?", "Where can I find the right ship?", "How can I protect myself from the dragon's breath?");
                break;
            case 105:
                _ui.ShowNpcDialogue(player, 747, "Oziach", "The guild master sent you right...");
                break;
            case 106:
                _ui.ShowNpcDialogue(player, 747, "Oziach", "There is somthing you could do...");
                break;
            case 107:
                _ui.ShowNpcDialogue(player, 747, "Oziach", "Kill Elvarg the dragon on crandor island.");
                break;
            case 108:
                _ui.ShowNpcDialogue(player, 746, "Oracle", "You are looking for a map to crandor?");
                break;
            case 111:
                _ui.ShowNpcDialogue(player, 2567, "Wise Old Man", "Looks like you completed all the quests...");
                break;
        }
    }
}
