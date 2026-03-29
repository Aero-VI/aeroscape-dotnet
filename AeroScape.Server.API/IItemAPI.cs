using AeroScape.Server.API.Models;

namespace AeroScape.Server.API;

/// <summary>
/// Item definition API for plugins.
/// </summary>
public interface IItemAPI
{
    /// <summary>Get item definition by ID</summary>
    ItemInfo? GetItem(int itemId);
    
    /// <summary>Get item definition by name (case-insensitive)</summary>
    ItemInfo? GetItem(string name);
    
    /// <summary>Search for items by partial name</summary>
    IReadOnlyList<ItemInfo> SearchItems(string partialName);
    
    /// <summary>Get all item definitions</summary>
    IReadOnlyList<ItemInfo> GetAllItems();
    
    /// <summary>Check if item ID exists</summary>
    bool ItemExists(int itemId);
}