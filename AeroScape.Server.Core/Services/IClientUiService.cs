using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public interface IClientUiService
{
    void SendMessage(Player player, string message);
    void OpenBank(Player player);
    void OpenShop(Player player, string title);
    void RefreshShop(Player player);
    void ShowNpcDialogue(Player player, int npcId, string name, string line, int animationId = 9850);
    void ShowOptionDialogue(Player player, string option1, string option2, string option3);
    void ShowInterface(Player player, int interfaceId);
    void UpdateCastleWarsCounters(Player player);
    void ResetClanChatList(Player player);
    void SendClanChat(Player recipient, Player sender, string clanName, string message);
}
