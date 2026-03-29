using AeroScape.Server.API.Models;

namespace AeroScape.Server.API;

/// <summary>
/// Inventory management API for plugins.
/// </summary>
public interface IInventoryAPI
{
    /// <summary>Add item to player inventory</summary>
    bool AddItem(PlayerInfo player, int itemId, int amount = 1);
    
    /// <summary>Add item to player inventory</summary>
    bool AddItem(int playerId, int itemId, int amount = 1);
    
    /// <summary>Remove item from player inventory</summary>
    bool RemoveItem(PlayerInfo player, int itemId, int amount = 1);
    
    /// <summary>Remove item from player inventory</summary>
    bool RemoveItem(int playerId, int itemId, int amount = 1);
    
    /// <summary>Check if player has item</summary>
    bool HasItem(PlayerInfo player, int itemId, int amount = 1);
    
    /// <summary>Check if player has item</summary>
    bool HasItem(int playerId, int itemId, int amount = 1);
    
    /// <summary>Check if player has item equipped</summary>
    bool HasItemEquipped(PlayerInfo player, int itemId);
    
    /// <summary>Check if player has item equipped</summary>
    bool HasItemEquipped(int playerId, int itemId);
    
    /// <summary>Check if player has either in inventory or equipped</summary>
    bool HasItemOrEquipped(PlayerInfo player, int itemId);
    
    /// <summary>Check if player has either in inventory or equipped</summary>
    bool HasItemOrEquipped(int playerId, int itemId);
    
    /// <summary>Get count of item in inventory</summary>
    int GetItemCount(PlayerInfo player, int itemId);
    
    /// <summary>Get count of item in inventory</summary>
    int GetItemCount(int playerId, int itemId);
    
    /// <summary>Get number of free inventory slots</summary>
    int GetFreeSlots(PlayerInfo player);
    
    /// <summary>Get number of free inventory slots</summary>
    int GetFreeSlots(int playerId);
}