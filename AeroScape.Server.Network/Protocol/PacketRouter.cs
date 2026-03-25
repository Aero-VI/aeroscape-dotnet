using System.Buffers;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Session;

namespace AeroScape.Server.Network.Protocol;

/// <summary>
/// Reads raw bytes from a client pipeline, frames them into packets using
/// <see cref="ProtocolDictionary"/>, and dispatches to registered handlers.
///
/// This is intentionally kept minimal for the initial scaffold — game-logic
/// handlers will be registered later via DI.
/// </summary>
public sealed class PacketRouter
{
    private readonly ILogger<PacketRouter> _logger;

    public PacketRouter(ILogger<PacketRouter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Attempt to consume as many complete packets as possible from <paramref name="buffer"/>.
    /// Returns the number of bytes consumed so the caller can advance the pipe reader.
    /// </summary>
    public long ProcessBuffer(PlayerSession session, ReadOnlySequence<byte> buffer)
    {
        var reader = new SequenceReader<byte>(buffer);
        int packetsRead = 0;

        while (packetsRead < 10) // cap per cycle, same as legacy Java
        {
            if (!reader.TryRead(out byte rawOpcode))
                break;

            int opcode = rawOpcode & 0xFF;
            var def = ProtocolDictionary.Incoming[opcode];
            int size = def.Size;

            // Variable-length: next byte is the real size.
            if (size == PacketDefinition.VariableSize)
            {
                if (!reader.TryRead(out byte sizeByte))
                {
                    // Not enough data yet — rewind the opcode byte.
                    reader.Rewind(1);
                    break;
                }
                size = sizeByte & 0xFF;
            }
            else if (size == PacketDefinition.UnknownSize)
            {
                // Undocumented packet — consume whatever remains in this read.
                size = (int)(reader.Remaining);
            }

            if (reader.Remaining < size)
            {
                // Rewind opcode (and maybe the size byte for variable packets).
                reader.Rewind(def.IsVariable ? 2 : 1);
                break;
            }

            // Read the payload.
            ReadOnlySequence<byte> payload = buffer.Slice(reader.Position, size);
            reader.Advance(size);

            // For now, just log. Actual handler dispatch will be wired later.
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(
                    "[{Session}] Packet {Name} (opcode={Opcode}, size={Size})",
                    session.SessionId, def.Name, opcode, size);
            }

            packetsRead++;
        }

        return reader.Consumed;
    }
}
