using System.Buffers;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Network.Protocol;

/// <summary>
/// Decodes raw packet bytes into a protocol-agnostic message record.
/// One decoder per opcode (or group of opcodes that map to the same message).
/// </summary>
public interface IPacketDecoder
{
    /// <summary>The message type this decoder produces (e.g. typeof(WalkMessage)).</summary>
    Type MessageType { get; }

    /// <summary>
    /// Decode the payload into a message record.
    /// Returns null if the packet should be silently dropped.
    /// </summary>
    object? Decode(PlayerSession session, int opcode, ReadOnlySequence<byte> payload);
}
