using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Items;

public sealed class PlayerEquipmentService(ItemDefinitionLoader items, PlayerItemsService playerItems)
{
    private static readonly string[] Capes = ["cape", "Cape", "cloak", "Cloak"];
    private static readonly string[] Hats =
    [
        "helm", "hood", "Helm", "coif", "Coif", "hat", "mitre", "partyhat", "Hat", "helmet", "mask",
        "Helm of neitiznot", "Mime mask", "Sleep", "sleep", "bandana", "Bandana", "eyepatch", "Eyepatch", "bunny ears", "Bunny Ears", "bunny", "Bunny"
    ];
    private static readonly string[] Boots = ["boots", "Boots"];
    private static readonly string[] Gloves = ["gloves", "gauntlets", "Gloves", "vambraces", "vamb", "bracers"];
    private static readonly string[] Shields = ["kiteshield", "sq shield", "Toktz-ket", "books", "book", "defender", "shield", "Spirit Shield", "Book"];
    private static readonly string[] Amulets = ["amulet", "necklace", "stole", "Amulet of", "scarf", "Scarf"];
    private static readonly string[] Arrows = ["arrow", "arrows", "arrow(p)", "arrow(+)", "arrow(s)", "bolt", "Bolt rack", "Opal bolts", "Dragon bolts"];
    private static readonly string[] Rings = ["ring"];
    private static readonly string[] Body =
    [
        "platebody", "chainbody", "blouse", "robetop", "leathertop", "platemail", "top", "brassard", "Robe top", "body",
        "chestplate", "torso", "shirt", "Varrock armour", "Prince tunic", "Runecrafter robe"
    ];
    private static readonly string[] Legs =
    [
        "platelegs", "knight robe", "plateskirt", "skirt", "bottoms", "chaps", "bottom", "tassets", "legs", "trousers", "shorts", "Shorts", "Bottom", "Bottoms"
    ];
    private static readonly string[] Weapons =
    [
        "secateurs", "scimitar", "Rubber chicken", "longsword", "sword", "crozier", "longbow", "shortbow", "dagger", "mace", "halberd", "spear",
        "Abyssal whip", "axe", "flail", "crossbow", "Torags hammers", "maul", "dart", "javelin", "knife", "Crossbow", "Toktz-xil", "Toktz-mej",
        "Tzhaar-ket", "staff", "Staff", "Scythe", "scythe", "sickle", "godsword", "c'bow", "Crystal bow", "Dark bow", "claws", "banner", "Warhammer", "warhammer", "wand", "Wand"
    ];

    public bool Equip(Player player, int itemId, int slot, int interfaceId)
    {
        if (interfaceId != 149 || slot < 0 || slot >= player.Items.Length || player.Items[slot] != itemId)
        {
            return false;
        }

        var targetSlot = GetItemType(itemId);
        if (targetSlot < 0)
        {
            return false;
        }

        if (targetSlot == 3 && IsTwoHanded(itemId) && player.Equipment[5] != -1 && playerItems.FreeSlotCount(player) < 1)
        {
            return false;
        }

        var amount = items.IsStackable(itemId) ? player.ItemsN[slot] : 1;
        var previousItem = player.Equipment[targetSlot];
        var previousAmount = player.EquipmentN[targetSlot];

        if (targetSlot == 3 && IsTwoHanded(itemId) && player.Equipment[5] != -1)
        {
            if (!playerItems.AddItem(player, player.Equipment[5], player.EquipmentN[5]))
            {
                return false;
            }

            player.Equipment[5] = -1;
            player.EquipmentN[5] = 0;
        }

        if (targetSlot == 5 && player.Equipment[3] != -1 && IsTwoHanded(player.Equipment[3]))
        {
            if (!playerItems.AddItem(player, player.Equipment[3], player.EquipmentN[3]))
            {
                return false;
            }

            player.Equipment[3] = -1;
            player.EquipmentN[3] = 0;
        }

        player.Equipment[targetSlot] = itemId;
        player.EquipmentN[targetSlot] = amount;
        playerItems.DeleteItem(player, itemId, slot, amount);

        if (previousItem != -1)
        {
            playerItems.AddItem(player, previousItem, previousAmount > 0 ? previousAmount : 1);
        }

        ApplyWeaponState(player);
        RecalculateBonuses(player);
        player.AppearanceUpdateReq = true;
        player.UpdateReq = true;
        return true;
    }

    public void RecalculateBonuses(Player player)
    {
        Array.Clear(player.EquipmentBonus);
        for (var slot = 0; slot < player.Equipment.Length; slot++)
        {
            var itemId = player.Equipment[slot];
            if (itemId < 0)
            {
                continue;
            }

            var bonuses = items.GetBonuses(itemId);
            for (var i = 0; i < player.EquipmentBonus.Length && i < bonuses.Length; i++)
            {
                player.EquipmentBonus[i] += bonuses[i];
            }
        }
    }

    public void ApplyWeaponState(Player player)
    {
        var weaponId = player.Equipment[3];
        player.WalkEmote = GetWalkEmote(weaponId);
        player.RunEmote = GetRunEmote(weaponId);
        player.StandEmote = GetStandEmote(weaponId);
        player.AttackEmote = GetAttackEmote(weaponId);
        player.AttackDelay = GetAttackDelay(weaponId);
    }

    private int GetItemType(int itemId)
    {
        var name = items.GetItemName(itemId);
        if (ContainsAny(name, Capes)) return 1;
        if (ContainsAny(name, Hats)) return 0;
        if (StartsOrEndsWithAny(name, Boots)) return 10;
        if (StartsOrEndsWithAny(name, Gloves)) return 9;
        if (ContainsAny(name, Shields)) return 5;
        if (StartsOrEndsWithAny(name, Amulets)) return 2;
        if (StartsOrEndsWithAny(name, Arrows)) return 13;
        if (StartsOrEndsWithAny(name, Rings)) return 12;
        if (ContainsAny(name, Body)) return 4;
        if (ContainsAny(name, Legs)) return 7;
        if (StartsOrEndsWithAny(name, Weapons)) return 3;
        return -1;
    }

    private bool IsTwoHanded(int itemId)
    {
        var weapon = items.GetItemName(itemId);
        return itemId is 4212 or 1231 or 4214 or 12842
            || weapon.EndsWith("2h sword", StringComparison.Ordinal)
            || weapon.EndsWith("Staff of Light", StringComparison.Ordinal)
            || weapon.EndsWith("net", StringComparison.Ordinal)
            || weapon.EndsWith("longbow", StringComparison.Ordinal)
            || weapon.EndsWith("shortbow", StringComparison.Ordinal)
            || weapon.EndsWith("Longbow", StringComparison.Ordinal)
            || weapon.EndsWith("Shortbow", StringComparison.Ordinal)
            || weapon.EndsWith("bow full", StringComparison.Ordinal)
            || weapon.EndsWith("halberd", StringComparison.Ordinal)
            || weapon.EndsWith("godsword", StringComparison.Ordinal)
            || weapon.Equals("Seercull", StringComparison.Ordinal)
            || weapon.Equals("Granite maul", StringComparison.Ordinal)
            || weapon.Equals("Karils crossbow", StringComparison.Ordinal)
            || weapon.Equals("Torags hammers", StringComparison.Ordinal)
            || weapon.Equals("Veracs flail", StringComparison.Ordinal)
            || weapon.Equals("Dharoks greataxe", StringComparison.Ordinal)
            || weapon.Equals("Guthans warspear", StringComparison.Ordinal)
            || weapon.Equals("Tzhaar-ket-om", StringComparison.Ordinal)
            || weapon.Equals("Saradomin sword", StringComparison.Ordinal)
            || weapon.Contains("claws", StringComparison.OrdinalIgnoreCase)
            || weapon.Contains("warhammer", StringComparison.OrdinalIgnoreCase);
    }

    private int GetRunEmote(int id)
    {
        var weapon = items.GetItemName(id);
        if (id == 4718 || weapon.EndsWith("2h sword", StringComparison.Ordinal) || id == 6528 || weapon.EndsWith("godsword", StringComparison.Ordinal) || weapon.StartsWith("Anger", StringComparison.Ordinal) || weapon.Equals("Saradomin sword", StringComparison.Ordinal))
            return 7039;
        if (weapon is "Saradomin staff" or "Guthix staff" or "Zamorak staff") return 0x338;
        if (id == 12842) return 8961;
        if (id == 4755) return 1831;
        if (id == 11259) return 0x680;
        if (id == 4734) return 2077;
        if (id == 4726 || weapon.Contains("Spear", StringComparison.Ordinal) || weapon.EndsWith("halberd", StringComparison.Ordinal) || weapon.Contains("Staff", StringComparison.Ordinal) || weapon.Contains("staff", StringComparison.Ordinal)) return 1210;
        if (weapon.Equals("Abyssal whip", StringComparison.Ordinal)) return 1661;
        if (id == 4153) return 1664;
        return 0x338;
    }

    private int GetWalkEmote(int id)
    {
        var weapon = items.GetItemName(id);
        if (weapon is "Saradomin staff" or "Guthix staff" or "Zamorak staff") return 0x333;
        if (id == 4755) return 2060;
        if (id == 11259) return 0x67F;
        if (id == 4734) return 2076;
        if (id == 4153) return 1663;
        if (id == 12842) return 8961;
        if (weapon.Equals("Abyssal whip", StringComparison.Ordinal)) return 1660;
        if (id == 4718 || weapon.EndsWith("2h sword", StringComparison.Ordinal) || id == 6528 || weapon.EndsWith("godsword", StringComparison.Ordinal) || weapon.Equals("Saradomin sword", StringComparison.Ordinal)) return 7046;
        if (id == 4726 || weapon.Contains("spear", StringComparison.Ordinal) || weapon.EndsWith("halberd", StringComparison.Ordinal) || weapon.Contains("Staff", StringComparison.Ordinal) || weapon.Contains("staff", StringComparison.Ordinal)) return 1146;
        return 0x333;
    }

    private int GetStandEmote(int id)
    {
        var weapon = items.GetItemName(id);
        if (id == 4151) return 10080;
        if (weapon.EndsWith("2h sword", StringComparison.Ordinal) || weapon.EndsWith("godsword", StringComparison.Ordinal) || weapon.Equals("Saradomin sword", StringComparison.Ordinal)) return 7047;
        if (id == 4718) return 2065;
        if (id == 12842) return 8961;
        if (id == 11259) return 0x811;
        if (id == 4755) return 2061;
        if (id == 1337) return 2065;
        if (id == 4734) return 2074;
        if (id == 6528 || id == 1319) return 0x811;
        if (weapon is "Saradomin staff" or "Guthix staff" or "Zamorak staff") return 0x328;
        if (id == 4726 || weapon.EndsWith("spear", StringComparison.Ordinal) || weapon.EndsWith("halberd", StringComparison.Ordinal) || weapon.Contains("Staff", StringComparison.Ordinal) || weapon.Contains("staff", StringComparison.Ordinal) || id == 1305 || weapon.Equals("Staff of Light", StringComparison.Ordinal)) return 809;
        if (weapon.Equals("Abyssal whip", StringComparison.Ordinal)) return 1832;
        if (id == 4153) return 1662;
        return 0x328;
    }

    private int GetAttackEmote(int id)
    {
        var weapon = items.GetItemName(id);
        if (weapon.EndsWith("2h sword", StringComparison.Ordinal) || weapon.EndsWith("godsword", StringComparison.Ordinal) || weapon.StartsWith("Anger", StringComparison.Ordinal) || weapon.Equals("Saradomin sword", StringComparison.Ordinal)) return 7041;
        if (weapon.Equals("Abyssal whip", StringComparison.Ordinal)) return 1658;
        if (id == 4153) return 1665;
        if (id == 1231) return 2068;
        if (id == 4710 || weapon.Contains("staff", StringComparison.Ordinal) || weapon.Contains("Staff", StringComparison.Ordinal)) return 1665;
        if (id == 11235) return 426;
        if (id == 4718) return 2067;
        if (id == 4726) return 2082;
        if (id == 4734) return 2075;
        if (id == 3101) return 2068;
        if (id == 4747) return 2068;
        if (id == 4755) return 2062;
        if (id == 1337) return 2067;
        if (weapon.Contains("longsword", StringComparison.OrdinalIgnoreCase) || weapon.EndsWith("scimitar", StringComparison.Ordinal) || weapon.EndsWith("battleaxe", StringComparison.Ordinal)) return 451;
        if (weapon.EndsWith("shortbow", StringComparison.Ordinal) || weapon.EndsWith("bow full", StringComparison.Ordinal)) return 426;
        return 422;
    }

    private int GetAttackDelay(int id)
    {
        var weapon = items.GetItemName(id);
        if (weapon.EndsWith("2h sword", StringComparison.Ordinal) || weapon.EndsWith("godsword", StringComparison.Ordinal) || weapon.Equals("Saradomin sword", StringComparison.Ordinal) || weapon.Equals("Staff of Light", StringComparison.Ordinal)) return 5;
        if (id == 1203 || id == 3101) return 4;
        if (id == 1337) return 5;
        if (weapon.EndsWith("battleaxe", StringComparison.Ordinal)) return 4;
        if (weapon.EndsWith("longsword", StringComparison.Ordinal)) return 4;
        if (weapon.Equals("Abyssal whip", StringComparison.Ordinal) || weapon.EndsWith("scimitar", StringComparison.Ordinal) || weapon.EndsWith("dagger", StringComparison.Ordinal) || weapon.StartsWith("Anger", StringComparison.Ordinal)) return 4;
        return 5;
    }

    private static bool ContainsAny(string value, IEnumerable<string> needles) => needles.Any(value.Contains);

    private static bool StartsOrEndsWithAny(string value, IEnumerable<string> needles) =>
        needles.Any(needle => value.StartsWith(needle, StringComparison.Ordinal) || value.EndsWith(needle, StringComparison.Ordinal));
}
