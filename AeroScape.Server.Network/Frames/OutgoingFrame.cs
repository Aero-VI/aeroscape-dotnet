using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace AeroScape.Server.Network.Frames;

/// <summary>
/// Low-level outgoing packet builder for RS 508 server→client frames.
/// Mirrors the Java Stream class's createFrame/writeByte/writeWord/etc methods.
/// Writes to an expandable buffer, then flushes to a NetworkStream.
/// </summary>
public sealed class FrameWriter : IDisposable
{
    private byte[] _buf;
    private int _offset;
    private readonly int[] _frameStack = new int[10];
    private int _frameStackPtr = -1;

    public FrameWriter(int initialCapacity = 4096)
    {
        _buf = new byte[initialCapacity];
    }

    public int Length => _offset;

    public ReadOnlySpan<byte> WrittenSpan => _buf.AsSpan(0, _offset);

    public void Reset() { _offset = 0; _frameStackPtr = -1; }

    // ── Frame creation (mirrors Java Stream) ────────────────────────────────

    public void CreateFrame(int opcode)
    {
        WriteByte(opcode);
    }

    public void CreateFrameVarSize(int opcode)
    {
        WriteByte(opcode);
        WriteByte(0); // placeholder for size
        _frameStack[++_frameStackPtr] = _offset;
    }

    public void CreateFrameVarSizeWord(int opcode)
    {
        WriteByte(opcode);
        WriteWord(0); // placeholder for size (2 bytes)
        _frameStack[++_frameStackPtr] = _offset;
    }

    public void EndFrameVarSize()
    {
        int start = _frameStack[_frameStackPtr--];
        int size = _offset - start;
        _buf[start - 1] = (byte)size;
    }

    public void EndFrameVarSizeWord()
    {
        int start = _frameStack[_frameStackPtr--];
        int size = _offset - start;
        _buf[start - 2] = (byte)(size >> 8);
        _buf[start - 1] = (byte)size;
    }

    // ── Primitive writes ────────────────────────────────────────────────────

    public void WriteByte(int val)
    {
        EnsureCapacity(1);
        _buf[_offset++] = (byte)val;
    }

    public void WriteByteA(int val)
    {
        WriteByte(val + 128);
    }

    public void WriteByteC(int val)
    {
        WriteByte(-val);
    }

    public void WriteByteS(int val)
    {
        WriteByte(128 - val);
    }

    public void WriteWord(int val)
    {
        EnsureCapacity(2);
        _buf[_offset++] = (byte)(val >> 8);
        _buf[_offset++] = (byte)val;
    }

    public void WriteWordBigEndian(int val)
    {
        // Same as WriteWord in RS terminology (big-endian = MSB first)
        WriteWord(val);
    }

    public void WriteWordA(int val)
    {
        EnsureCapacity(2);
        _buf[_offset++] = (byte)(val >> 8);
        _buf[_offset++] = (byte)(val + 128);
    }

    public void WriteWordBigEndianA(int val)
    {
        WriteWordA(val);
    }

    public void WriteRShort(int val)
    {
        // Reverse short (little-endian)
        EnsureCapacity(2);
        _buf[_offset++] = (byte)val;
        _buf[_offset++] = (byte)(val >> 8);
    }

    public void WriteDWord(int val)
    {
        EnsureCapacity(4);
        _buf[_offset++] = (byte)(val >> 24);
        _buf[_offset++] = (byte)(val >> 16);
        _buf[_offset++] = (byte)(val >> 8);
        _buf[_offset++] = (byte)val;
    }

    public void WriteDWordBigEndian(int val)
    {
        WriteDWord(val);
    }

    /// <summary>writeDWord_v1 — middle-endian variant 1 (bytes: 2,3,0,1)</summary>
    public void WriteDWordV1(int val)
    {
        EnsureCapacity(4);
        _buf[_offset++] = (byte)(val >> 8);
        _buf[_offset++] = (byte)val;
        _buf[_offset++] = (byte)(val >> 24);
        _buf[_offset++] = (byte)(val >> 16);
    }

    /// <summary>writeDWord_v2 — middle-endian variant 2 (bytes: 1,0,3,2)</summary>
    public void WriteDWordV2(int val)
    {
        EnsureCapacity(4);
        _buf[_offset++] = (byte)(val >> 16);
        _buf[_offset++] = (byte)(val >> 24);
        _buf[_offset++] = (byte)val;
        _buf[_offset++] = (byte)(val >> 8);
    }

    public void WriteQWord(long val)
    {
        WriteDWord((int)(val >> 32));
        WriteDWord((int)val);
    }

    public void WriteString(string s)
    {
        foreach (char c in s)
            WriteByte(c);
        WriteByte(10); // newline terminator
    }

    public void WriteBytes(byte[] data, int length, int startOffset)
    {
        EnsureCapacity(length);
        Buffer.BlockCopy(data, startOffset, _buf, _offset, length);
        _offset += length;
    }

    // ── Flush to stream ─────────────────────────────────────────────────────

    public async Task FlushToAsync(Stream stream, CancellationToken ct = default)
    {
        if (_offset > 0)
        {
            await stream.WriteAsync(_buf.AsMemory(0, _offset), ct);
            await stream.FlushAsync(ct);
            _offset = 0;
        }
    }

    // ── Capacity ────────────────────────────────────────────────────────────

    private void EnsureCapacity(int additional)
    {
        if (_offset + additional > _buf.Length)
        {
            int newSize = Math.Max(_buf.Length * 2, _offset + additional + 256);
            Array.Resize(ref _buf, newSize);
        }
    }

    public void Dispose() { }
}
