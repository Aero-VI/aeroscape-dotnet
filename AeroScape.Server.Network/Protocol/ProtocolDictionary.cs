namespace AeroScape.Server.Network.Protocol;

/// <summary>
/// Holds the 256-entry packet-size table for the 508 revision protocol.
/// Ported directly from the legacy Java <c>Packets.setPacketSizes()</c>.
/// </summary>
public static class ProtocolDictionary
{
    /// <summary>
    /// Indexed by opcode (0-255). Each entry is a <see cref="PacketDefinition"/>.
    /// </summary>
    public static readonly PacketDefinition[] Incoming = BuildTable();

    private static PacketDefinition[] BuildTable()
    {
        // Raw sizes copied 1-to-1 from the Java source.
        // -3 = undocumented/unused, -1 = variable (size byte follows opcode).
        int[] sizes =
        {
            -3, -3,  8,  8, -3, -3, -3,  2, -3, -3, -3, -3, -3, -3, -3, -3, // 0-15
            -3, -3, -3, -3, -3,  6,  4, -3, -3, -3, -3, -3, -3, -3,  8, -3, // 16-31
            -3, -3, -3, -3, -3,  2,  2, -3, -3, -3, -3, -3, -3, -3, -3,  0, // 32-47
            -3, -1, -3, -3,  2, -3, -3, -3, -3, -3, -3,  6,  0,  8, -3, -3, // 48-63
            -3, -3, -3, -3, -3, -3, -3, -3, -3, -3,  8, -3, -3, -3, -3, -3, // 64-79
            -3, -3, -3, -3,  2, -3, -3, -3,  2, -3, -3, -3, -3, -3, -3, -3, // 80-95
            -3, -3, -3,  4, -3, -3, -3, -3, -3, -3, -3, -1,  0, -3, -3, -3, // 96-111
            -3,  4, -3,  0, -3, -1, -3, -1, -3, -3, -3, -3, -3,  2, -3, -3, // 112-127
            -3, -3, -3,  7,  8, -3, -3, -3, -3, -3, -1, -3, -3, -3, -3, -3, // 128-143
            -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3, -3,  6, -3, // 144-159
             2, -3, -3, -3, -3,  4, -3,  9, -3,  6, -3, -3, -3,  6, -3, -3, // 160-175
            -3, -3, -1, 12, -3, -3, -3, -3, -3, -3,  8, -3, -3, -3, -3, -3, // 176-191
            -3, -3, -3, -3, -3, -3, -3, -3, -3,  6, -3,  8, -3,  8, -3, -3, // 192-207
            -3, -3, -3,  8, -3, -3, -3, -3, -3, -3, -3, -3,  8, -3, -1, -3, // 208-223
            -3, -3, -3,  2,  6, -3, -3, -3,  6,  6, -3, -3, -3, -3, -3, -3, // 224-239
            -3, -3, -3, -3, -3, -3, -3,  4,  1, -3, -3, -3, -3, -3, -3, -3, // 240-255
        };

        // Human-readable names for the known opcodes.
        var names = new Dictionary<int, string>
        {
            [2]   = "RemoveIgnore",
            [3]   = "EquipItem",
            [7]   = "NpcOption1",
            [21]  = "ActionButton",
            [22]  = "UpdateAck",
            [24]  = "MagicOnNpc",
            [30]  = "AddFriend",
            [37]  = "PlayerOption2",
            [38]  = "ItemExamine",
            [40]  = "ItemOnItem",
            [42]  = "ClanJoin",
            [43]  = "UserInput",
            [47]  = "Idle",
            [49]  = "WalkMain",
            [52]  = "NpcOption2",
            [59]  = "MouseClick",
            [60]  = "MapRegionLoaded",
            [61]  = "AddIgnore",
            [62]  = "SpawnObjects",
            [63]  = "DialogueContinue",
            [70]  = "MagicOnPlayer",
            [84]  = "ObjectExamine",
            [88]  = "NpcExamine",
            [99]  = "Unknown99",
            [107] = "Command",
            [108] = "CloseInterface",
            [113] = "ActionButton2",
            [115] = "Ping",
            [117] = "Unknown117",
            [119] = "WalkMinimap",
            [123] = "NpcAttack",
            [127] = "StringInput",
            [131] = "ItemGive",
            [132] = "RemoveFriend",
            [134] = "ItemSelect2",
            [138] = "WalkOther",
            [152] = "ItemOption1b",
            [154] = "MagicOnItem",
            [158] = "ObjectOption1",
            [160] = "PlayerOption1",
            [165] = "SettingsButton",
            [167] = "SwitchItems",
            [169] = "ActionButton3",
            [173] = "ActionButton4",
            [178] = "PrivateMessage",
            [179] = "SwitchItems2",
            [186] = "ItemOperate",
            [189] = "LongInput",
            [190] = "ObjectBuild",
            [199] = "NpcOption3",
            [200] = "ClanKick",
            [201] = "PickupItem",
            [203] = "ItemOption1",
            [211] = "DropItem",
            [220] = "ItemSelect",
            [222] = "PublicChat",
            [224] = "ItemOnObject",
            [227] = "PlayerOption3",
            [228] = "ObjectOption2",
            [232] = "ActionButton5",
            [233] = "ActionButton6",
            [247] = "Unknown247",
            [248] = "Unknown248",
            [253] = "TradeAccept",
        };

        var table = new PacketDefinition[256];
        for (int i = 0; i < 256; i++)
        {
            names.TryGetValue(i, out var name);
            table[i] = new PacketDefinition(i, sizes[i], name ?? $"Unknown{i}");
        }
        return table;
    }
}
