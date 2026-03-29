using AeroScape.Server.API;
using AeroScape.Server.API.Models;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Services;

namespace AeroScape.Server.Core.Api;

/// <summary>
/// Implementation of IInventoryAPI that wraps core inventory services.
/// </summary>
public class InventoryAPIImpl : IInventoryAPI
{
    private readonly GameEngine _engine;
    private readonly InventoryService _inventoryService;

    public InventoryAPIImpl(GameEngine engine, InventoryService inventoryService)
    {
        _engine = engine;
        _inventoryService = inventoryService;
    }

    public bool AddItem(PlayerInfo player, int itemId, int amount = 1)
    {
        return AddItem(player.Id, itemId, amount);
    }

    public bool AddItem(int playerId, int itemId, int amount = 1)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null) return false;

        _inventoryService.AddItem(player, itemId, amount);
        return true;
    }

    public bool RemoveItem(PlayerInfo player, int itemId, int amount = 1)
    {
        return RemoveItem(player.Id, itemId, amount);
    }

    public bool RemoveItem(int playerId, int itemId, int amount = 1)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null) return false;

        _inventoryService.DeleteItem(player, itemId, amount);
        return true;
    }

    public bool HasItem(PlayerInfo player, int itemId, int amount = 1)
    {
        return HasItem(player.Id, itemId, amount);
    }

    public bool HasItem(int playerId, int itemId, int amount = 1)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null) return false;

        return _inventoryService.Count(player, itemId) >= amount;
    }

    public bool HasItemEquipped(PlayerInfo player, int itemId)
    {
        return HasItemEquipped(player.Id, itemId);
    }

    public bool HasItemEquipped(int playerId, int itemId)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null) return false;

        // Check equipment slots
        for (int i = 0; i < player.Equipment.Length; i++)
        {
            if (player.Equipment[i] == itemId)
                return true;
        }
        return false;
    }

    public bool HasItemOrEquipped(PlayerInfo player, int itemId)
    {
        return HasItemOrEquipped(player.Id, itemId);
    }

    public bool HasItemOrEquipped(int playerId, int itemId)
    {
        return HasItem(playerId, itemId, 1) || HasItemEquipped(playerId, itemId);
    }

    public int GetItemCount(PlayerInfo player, int itemId)
    {
        return GetItemCount(player.Id, itemId);
    }

    public int GetItemCount(int playerId, int itemId)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null) return 0;

        return _inventoryService.Count(player, itemId);
    }

    public int GetFreeSlots(PlayerInfo player)
    {
        return GetFreeSlots(player.Id);
    }

    public int GetFreeSlots(int playerId)
    {
        var player = _engine.OnlinePlayers.FirstOrDefault(p => p.PlayerId == playerId);
        if (player == null) return 0;

        int freeSlots = 0;
        for (int i = 0; i < 28; i++)
        {
            if (player.Items[i] <= 0)
                freeSlots++;
        }
        return freeSlots;
    }
}