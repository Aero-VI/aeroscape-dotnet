using AeroScape.Server.API;
using AeroScape.Server.API.Models;
using AeroScape.Server.Core.Items;

namespace AeroScape.Server.Core.Api;

/// <summary>
/// Implementation of IItemAPI that wraps core item services.
/// </summary>
public class ItemAPIImpl : IItemAPI
{
    private readonly ItemDefinitionLoader _itemLoader;

    public ItemAPIImpl(ItemDefinitionLoader itemLoader)
    {
        _itemLoader = itemLoader;
    }

    public ItemInfo? GetItem(int itemId)
    {
        var def = _itemLoader.Get(itemId);
        if (def == null) return null;

        return new ItemInfo
        {
            Id = itemId,
            Name = def.Name ?? "Unknown",
            Examine = def.Description ?? "",
            Stackable = def.Stackable,
            Value = def.ShopValue,
            MembersOnly = false, // Not stored in current definitions
            Weight = 0.0 // Not stored in current definitions
        };
    }

    public ItemInfo? GetItem(string name)
    {
        // TODO: Implement name-based lookup
        return null;
    }

    public IReadOnlyList<ItemInfo> SearchItems(string partialName)
    {
        // TODO: Implement search
        return Array.Empty<ItemInfo>();
    }

    public IReadOnlyList<ItemInfo> GetAllItems()
    {
        var items = new List<ItemInfo>();
        for (int i = 0; i < 20000; i++)
        {
            var item = GetItem(i);
            if (item != null)
                items.Add(item);
        }
        return items;
    }

    public bool ItemExists(int itemId)
    {
        return _itemLoader.Get(itemId) != null;
    }
}