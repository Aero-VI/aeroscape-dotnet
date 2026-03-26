using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Entities;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Core.Handlers;

public class SwitchItems2MessageHandler : IMessageHandler<SwitchItems2Message>
{
    private readonly ILogger<SwitchItems2MessageHandler> _logger;
    private readonly PlayerBankService _bank;
    private readonly PlayerItemsService _items;

    public SwitchItems2MessageHandler(ILogger<SwitchItems2MessageHandler> logger, PlayerBankService bank, PlayerItemsService items)
    {
        _logger = logger;
        _bank = bank;
        _items = items;
    }
    public Task HandleAsync(PlayerSession session, SwitchItems2Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[SwitchItems2] Player {SessionId} interface={InterfaceId} tab={TabId} from={FromSlot} to={ToSlot}", session.SessionId, message.InterfaceId, message.TabId, message.FromSlot, message.ToSlot);
        var player = session.Entity;
        if (player is null)
        {
            return Task.CompletedTask;
        }

        if ((message.FromInterface == 50003968 || message.ToInterface == 50003968) &&
            message.FromInterface != message.ToInterface)
        {
            return Task.CompletedTask;
        }

        switch (message.InterfaceId)
        {
            case 762: // Bank interface
                HandleBankSwitch(player, message);
                break;

            case 763: // Inventory (while bank is open)
                _items.SwapInventoryItems(player, message.FromSlot, message.ToSlot);
                break;
        }

        return Task.CompletedTask;
    }

    private void HandleBankSwitch(Player player, SwitchItems2Message message)
    {
        switch (message.TabId)
        {
            case 73: // Swap/insert within bank
                _bank.HandleBankSwitch(player, message.FromSlot, message.ToSlot);
                break;

            // Tab icon drags (move item to specific tab)
            case 41: // Main tab (10)
            case 39: // Tab 2
            case 37: // Tab 3
            case 35: // Tab 4
            case 33: // Tab 5
            case 31: // Tab 6
            case 29: // Tab 7
            case 27: // Tab 8
            case 25: // Tab 9
            // Direct tab drags
            case 51: // Main tab (10)
            case 52: // Tab 2
            case 53: // Tab 3
            case 54: // Tab 4
            case 55: // Tab 5
            case 56: // Tab 6
            case 57: // Tab 7
            case 58: // Tab 8
            case 59: // Tab 9
                _bank.MoveToBankTab(player, message.FromSlot, message.TabId);
                break;
        }
    }
}
