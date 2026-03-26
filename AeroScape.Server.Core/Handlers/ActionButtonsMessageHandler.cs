using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Session;
namespace AeroScape.Server.Core.Handlers;

public class ActionButtonsMessageHandler : IMessageHandler<ActionButtonsMessage>
{
    private readonly ILogger<ActionButtonsMessageHandler> _logger;
    private readonly PlayerBankService _bank;
    private readonly TradingService _trading;
    private readonly PlayerItemsService _items;
    private readonly MagicService _magic;
    private readonly ShopService _shops;
    private readonly PrayerService _prayers;
    private readonly ClanChatService _clanChat;
    private readonly ConstructionService _construction;
    private readonly IClientUiService _ui;

    public ActionButtonsMessageHandler(ILogger<ActionButtonsMessageHandler> logger, PlayerBankService bank, TradingService trading, PlayerItemsService items, MagicService magic, ShopService shops, PrayerService prayers, ClanChatService clanChat, ConstructionService construction, IClientUiService ui)
    {
        _logger = logger;
        _bank = bank;
        _trading = trading;
        _items = items;
        _magic = magic;
        _shops = shops;
        _prayers = prayers;
        _clanChat = clanChat;
        _construction = construction;
        _ui = ui;
    }
    public Task HandleAsync(PlayerSession session, ActionButtonsMessage message, CancellationToken cancellationToken)
    {
        var player = session.Entity;
        if (player is null)
        {
            return Task.CompletedTask;
        }

        switch (message.InterfaceId)
        {
            case 192:
                _magic.TryCastModernAction(player, message.ButtonId);
                break;
            case 193:
                _magic.TryCastAncientAction(player, message.ButtonId);
                break;
            case 430:
                _magic.TryCastLunarAction(player, message.ButtonId);
                break;
            case 589:
                if (message.ButtonId == 9)
                {
                    _ui.ShowInterface(player, 590);
                }
                else if (message.ButtonId == 14)
                {
                    _clanChat.SetLootShare(player, !_clanChat.LootShareOn(player));
                }
                break;
            case 590:
                if (message.ButtonId == 22)
                {
                    if (message.PacketOpcode == 233)
                        _ui.ShowLongTextInput(player, 0, "Enter clan prefix:");
                    else if (message.PacketOpcode == 21)
                        _clanChat.CreateOrRenameChat(player, string.Empty);
                }
                break;
            case 402:
                _construction.AddRoom(player, message.ButtonId - 160);
                break;
            case 300:
                player.Smithing.SmithItem(message.ButtonId);
                break;
            case 271:
                _prayers.Toggle(player, message.ButtonId);
                break;
            case 763:
                if (message.ButtonId == 0)
                {
                    var depositAmount = message.PacketOpcode switch
                    {
                        233 => 1,
                        21 => 5,
                        169 => 10,
                        214 => player.BankX,
                        232 => message.SlotId >= 0 && message.SlotId < player.Items.Length ? _items.InvItemCount(player, player.Items[message.SlotId]) : 0,
                        _ => 0
                    };

                    if (depositAmount > 0)
                    {
                        _bank.Deposit(player, message.SlotId, depositAmount);
                    }
                }
                break;
            case 762:
                if (message.ButtonId == 73)
                {
                    var withdrawAmount = message.PacketOpcode switch
                    {
                        233 => 1,
                        21 => 5,
                        169 => 10,
                        214 => player.BankX,
                        232 => message.SlotId >= 0 && message.SlotId < player.BankItems.Length ? player.BankItemsN[message.SlotId] : 0,
                        133 => message.SlotId >= 0 && message.SlotId < player.BankItems.Length ? Math.Max(0, player.BankItemsN[message.SlotId] - 1) : 0,
                        _ => 0
                    };

                    if (withdrawAmount > 0)
                    {
                        _bank.Withdraw(player, message.SlotId, withdrawAmount);
                    }
                }
                else if (message.ButtonId == 16)
                {
                    player.WithdrawNote = !player.WithdrawNote;
                }
                else if (message.ButtonId == 14)
                {
                    player.InsertMode = !player.InsertMode;
                }
                else if (message.ButtonId is 41 or 39 or 37 or 35 or 33 or 31 or 29 or 27 or 25)
                {
                    if (message.PacketOpcode == 21)
                    {
                        _bank.CollapseTab(player, _bank.GetArrayIndex(message.ButtonId));
                    }
                    else if (message.PacketOpcode == 233)
                    {
                        player.ViewingBankTab = _bank.GetArrayIndex(message.ButtonId);
                    }
                }
                break;
            case 620:
                if (message.ButtonId == 24 && message.SlotId >= 0 && message.SlotId < player.ShopItems.Length)
                {
                    var itemId = player.ShopItems[message.SlotId];
                    if (itemId >= 0)
                    {
                        switch (message.PacketOpcode)
                        {
                            case 21:
                                _shops.Buy(player, itemId, 1);
                                break;
                            case 169:
                                _shops.Buy(player, itemId, 5);
                                break;
                            case 214:
                                _shops.Buy(player, itemId, 10);
                                break;
                        }
                    }
                }
                break;
            case 621:
                if (message.ButtonId == 0 && message.SlotId >= 0 && message.SlotId < player.Items.Length)
                {
                    var itemId = player.Items[message.SlotId];
                    if (itemId >= 0)
                    {
                        switch (message.PacketOpcode)
                        {
                            case 21:
                                _shops.Sell(player, itemId, 1);
                                break;
                            case 169:
                                _shops.Sell(player, itemId, 5);
                                break;
                            case 214:
                                _shops.Sell(player, itemId, 10);
                                break;
                        }
                    }
                }
                break;
        }

        _trading.HandleActionButton(player, message.InterfaceId, message.PacketOpcode, message.ButtonId, message.SlotId);
        _logger.LogInformation("[ActionButtons] Player {SessionId} opcode {Opcode} interface {InterfaceId} button {ButtonId} item {ItemId} slot {SlotId}", session.SessionId, message.PacketOpcode, message.InterfaceId, message.ButtonId, message.ItemId, message.SlotId);
        return Task.CompletedTask;
    }
}
