using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Services;

public sealed class ConstructionService
{
    public static readonly int[,] RoomInfo =
    {
        {1864, 5056, 0, 0},
        {1856, 5112, 1000, 1},
        {1856, 5064, 1000, 1},
        {1872, 5112, 5000, 5},
        {1890, 5112, 5000, 10},
        {1856, 5096, 10000, 15},
        {1904, 5112, 10000, 20},
        {1880, 5104, 15000, 25},
        {1896, 5088, 25000, 30},
        {1880, 5088, 25000, 32},
        {1912, 5104, 25000, 35},
        {1888, 5096, 50000, 40},
        {1904, 5064, 50000, 42},
        {1872, 5096, 50000, 45},
        {1864, 5088, 100000, 50},
        {1872, 5064, 75000, 55},
        {1904, 5096, 150000, 60},
        {1904, 5080, 150000, 65},
        {1888, 5080, 7500, 70},
        {1856, 5080, 7500, 70},
        {1872, 5080, 7500, 70},
        {1912, 5088, 250000, 75},
    };

    private readonly ConcurrentDictionary<int, HouseState> _houses = new();

    public bool HaveWateringCan(Player player)
        => Enumerable.Range(5333, 8).Any(can => CountItem(player, can) > 0);

    public void DecreaseCan(Player player)
    {
        for (var can = 5333; can <= 5340; can++)
        {
            if (CountItem(player, can) <= 0)
                continue;

            DeleteItem(player, can, 1);
            if (can > 5333)
                AddItem(player, can - 1, 1);
            return;
        }
    }

    public bool AddRoom(Player player, int roomId)
    {
        if (roomId < 0 || roomId + 1 >= RoomInfo.GetLength(0))
            return false;

        var requiredLevel = RoomInfo[roomId + 1, 3];
        var price = RoomInfo[roomId + 1, 2];
        if (player.SkillLvl[22] < requiredLevel || CountItem(player, 995) < price)
            return false;

        var house = _houses.GetOrAdd(player.PersistentId, _ => new HouseState());
        house.Rooms.Add(roomId + 1);
        DeleteItem(player, 995, price);
        return true;
    }

    public bool AddFurniture(Player player, int level, int[] items, int[] amounts, int spot, int objectId, bool needCan)
    {
        if (player.SkillLvl[22] < level)
            return false;

        for (var i = 0; i < items.Length; i++)
        {
            if (items[i] != 0 && CountItem(player, items[i]) < amounts[i])
                return false;
        }

        if (needCan && !HaveWateringCan(player))
            return false;

        for (var i = 0; i < items.Length; i++)
        {
            if (items[i] != 0)
                DeleteItem(player, items[i], amounts[i]);
        }

        if (needCan)
            DecreaseCan(player);

        var house = _houses.GetOrAdd(player.PersistentId, _ => new HouseState());
        house.Furniture[spot] = objectId;
        return true;
    }

    public void RemoveFurniture(Player player, int spot)
    {
        if (_houses.TryGetValue(player.PersistentId, out var house))
            house.Furniture.Remove(spot);
    }

    private static int CountItem(Player player, int itemId)
    {
        var count = 0;
        for (var i = 0; i < player.Items.Length; i++)
        {
            if (player.Items[i] != itemId)
                continue;

            count += player.ItemsN[i] > 0 ? player.ItemsN[i] : 1;
        }

        return count;
    }

    private static void DeleteItem(Player player, int itemId, int amount)
    {
        for (var i = 0; i < player.Items.Length && amount > 0; i++)
        {
            if (player.Items[i] != itemId)
                continue;

            var stack = player.ItemsN[i] > 0 ? player.ItemsN[i] : 1;
            if (stack > amount)
            {
                player.ItemsN[i] = stack - amount;
                return;
            }

            amount -= stack;
            player.Items[i] = -1;
            player.ItemsN[i] = 0;
        }
    }

    private static bool AddItem(Player player, int itemId, int amount)
    {
        for (var a = 0; a < amount; a++)
        {
            for (var i = 0; i < player.Items.Length; i++)
            {
                if (player.Items[i] != -1)
                    continue;

                player.Items[i] = itemId;
                player.ItemsN[i] = 1;
                goto next;
            }

            return false;
        next: ;
        }

        return true;
    }

    private sealed class HouseState
    {
        public List<int> Rooms { get; } = [];
        public Dictionary<int, int> Furniture { get; } = [];
    }
}
