namespace AeroScape.Server.Network.Protocol;

/// <summary>
/// Describes a single incoming packet: its opcode, fixed size (or -1 for variable-length),
/// and a human-readable name for logging/debugging.
/// </summary>
public sealed record PacketDefinition(int Opcode, int Size, string Name)
{
    /// <summary>
    /// Size sentinel: the real size is sent as a single byte prefix from the client.
    /// </summary>
    public const int VariableSize = -1;

    /// <summary>
    /// Size sentinel: packet is undocumented / unused — skip all available bytes.
    /// </summary>
    public const int UnknownSize = -3;

    public bool IsVariable => Size == VariableSize;
    public bool IsUnknown  => Size == UnknownSize;
}
