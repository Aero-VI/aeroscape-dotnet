using System.Text.Json;
using System.Text.Json.Serialization;

namespace AeroScape.Server.Network.Protocol;

/// <summary>
/// Loads the 256-entry packet-size table from a JSON protocol dictionary file.
/// Replaces the legacy hardcoded approach from <c>Packets.setPacketSizes()</c>.
/// </summary>
public static class ProtocolDictionary
{
    /// <summary>
    /// Indexed by opcode (0-255). Each entry is a <see cref="PacketDefinition"/>.
    /// </summary>
    public static readonly PacketDefinition[] Incoming = LoadFromJson();

    private static PacketDefinition[] LoadFromJson()
    {
        // Look for the JSON file relative to the assembly location first, then fall back to
        // the well-known path inside the Network project.
        var assemblyDir = Path.GetDirectoryName(typeof(ProtocolDictionary).Assembly.Location) ?? ".";
        var candidates = new[]
        {
            Path.Combine(assemblyDir, "Protocol_508.json"),
            Path.Combine(assemblyDir, "Protocol", "Protocol_508.json"),
            Path.Combine(AppContext.BaseDirectory, "Protocol_508.json"),
            Path.Combine(AppContext.BaseDirectory, "Protocol", "Protocol_508.json"),
        };

        string? jsonPath = candidates.FirstOrDefault(File.Exists);

        if (jsonPath is null)
        {
            throw new FileNotFoundException(
                "Protocol_508.json not found. Ensure it is copied to the output directory. " +
                $"Searched: {string.Join(", ", candidates)}");
        }

        var json = File.ReadAllText(jsonPath);
        var proto = JsonSerializer.Deserialize<ProtocolFile>(json)
                    ?? throw new InvalidOperationException("Failed to deserialise Protocol_508.json");

        var table = new PacketDefinition[256];

        // Default everything to unknown
        for (int i = 0; i < 256; i++)
            table[i] = new PacketDefinition(i, PacketDefinition.UnknownSize, $"Unknown{i}");

        foreach (var entry in proto.Packets)
        {
            if (entry.Opcode is < 0 or > 255) continue;
            table[entry.Opcode] = new PacketDefinition(entry.Opcode, entry.Size, entry.Name ?? $"Unknown{entry.Opcode}");
        }

        return table;
    }

    // ── JSON model ──────────────────────────────────────────────────────────

    private sealed class ProtocolFile
    {
        [JsonPropertyName("revision")]
        public int Revision { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("packets")]
        public List<PacketEntry> Packets { get; set; } = new();
    }

    private sealed class PacketEntry
    {
        [JsonPropertyName("opcode")]
        public int Opcode { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
