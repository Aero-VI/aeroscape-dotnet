using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Session;
using AeroScape.Server.Core.Util;

namespace AeroScape.Server.Network.Login;

/// <summary>
/// Handles the RS 508 login handshake directly on the raw NetworkStream,
/// BEFORE the connection is handed off to the pipe-based game packet router.
///
/// Port of DavidScape/io/Login.java — the three-stage handshake:
///   Stage 0: Read connection type (14=login, 15=update) → send server session key
///   Stage 1: Read login type byte (16 or 18)
///   Stage 2: Read the full login block (version, username, password) → send response
/// </summary>
public sealed class LoginHandler
{
    private readonly ILogger _logger;

    public LoginHandler(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Perform the full login handshake on the given session.
    /// Returns a <see cref="LoginResult"/> on success, or null if the handshake fails.
    /// </summary>
    public async Task<LoginResult?> HandleLoginAsync(PlayerSession session, CancellationToken ct)
    {
        var stream = session.GetStream();
        stream.ReadTimeout = 15_000;  // 15s total login timeout
        stream.WriteTimeout = 5_000;

        try
        {
            // ═══════════════════════════════════════════════════════════════
            // Stage 0: Connection type
            // ═══════════════════════════════════════════════════════════════
            var header = new byte[2];
            await ReadExactAsync(stream, header, 0, 2, ct);

            int connectionType = header[0] & 0xFF;
            int _nameHash = header[1] & 0xFF; // unused but consumed

            if (connectionType == 15)
            {
                // Update server request — not supported, disconnect
                _logger.LogDebug("Session {Id}: update server request, disconnecting", session.SessionId);
                return null;
            }

            if (connectionType != 14)
            {
                _logger.LogDebug("Session {Id}: unexpected connection type {Type}", session.SessionId, connectionType);
                return null;
            }

            // Generate server session key
            var rng = new Random();
            long serverSessionKey = ((long)(rng.NextDouble() * 99999999D) << 32)
                                  + (long)(rng.NextDouble() * 99999999D);

            // Send response: 0 (success byte) + 8 bytes (server session key)
            var stage0Response = new byte[9];
            stage0Response[0] = 0; // status
            WriteLong(stage0Response, 1, serverSessionKey);
            await stream.WriteAsync(stage0Response, ct);
            await stream.FlushAsync(ct);

            // ═══════════════════════════════════════════════════════════════
            // Stage 1: Login type byte + login packet size
            // ═══════════════════════════════════════════════════════════════
            var stage1Header = new byte[3];
            await ReadExactAsync(stream, stage1Header, 0, 3, ct);

            int loginType = stage1Header[0] & 0xFF;
            if (loginType != 16 && loginType != 18 && loginType != 14)
            {
                _logger.LogDebug("Session {Id}: unexpected login type {Type}", session.SessionId, loginType);
                return null;
            }

            int loginPacketSize = ((stage1Header[1] & 0xFF) << 8) | (stage1Header[2] & 0xFF);

            // ═══════════════════════════════════════════════════════════════
            // Stage 2: Read the full login block
            // ═══════════════════════════════════════════════════════════════
            if (loginPacketSize <= 0 || loginPacketSize > 500)
            {
                _logger.LogDebug("Session {Id}: invalid login packet size {Size}", session.SessionId, loginPacketSize);
                return null;
            }

            var loginBlock = new byte[loginPacketSize];
            await ReadExactAsync(stream, loginBlock, 0, loginPacketSize, ct);

            int offset = 0;

            // Client version (4 bytes)
            int clientVersion = ReadInt(loginBlock, ref offset);
            if (clientVersion != 508 && clientVersion != 800 && clientVersion != 900)
            {
                _logger.LogDebug("Session {Id}: unsupported client version {Version}", session.SessionId, clientVersion);
                return null;
            }
            session.Revision = clientVersion;

            bool usingHD = false;

            // Skip: 1 byte (unknown) + 3 words (6 bytes) + 24 bytes (cache IDX) = 31 bytes
            offset += 1; // unknown byte
            offset += 2; // word
            offset += 2; // word
            offset += 2; // word
            offset += 24; // 24 cache idx bytes

            // Read junk string (null/newline terminated)
            while (offset < loginBlock.Length && loginBlock[offset] != 10 && loginBlock[offset] != 0)
                offset++;
            if (offset < loginBlock.Length) offset++; // skip terminator

            // 29 DWords (116 bytes)
            offset += 29 * 4;

            // HD/LD detection byte
            if (offset >= loginBlock.Length)
            {
                _logger.LogDebug("Session {Id}: login block too short at HD byte", session.SessionId);
                return null;
            }

            int hdByte = loginBlock[offset++] & 0xFF;
            usingHD = (hdByte == 10);

            int encryption = hdByte;
            if (encryption != 10 && encryption != 64)
            {
                if (offset < loginBlock.Length)
                    encryption = loginBlock[offset++] & 0xFF;
            }

            if (encryption != 10 && encryption != 64)
            {
                _logger.LogDebug("Session {Id}: invalid encryption marker {E}", session.SessionId, encryption);
                return null;
            }

            // Client session key (8 bytes)
            if (offset + 8 > loginBlock.Length) return null;
            long clientSessionKey = ReadLong(loginBlock, ref offset);

            // Server session key echo (8 bytes)
            if (offset + 8 > loginBlock.Length) return null;
            long serverKeyEcho = ReadLong(loginBlock, ref offset);

            // Username as encoded long (8 bytes)
            if (offset + 8 > loginBlock.Length) return null;
            long usernameLong = ReadLong(loginBlock, ref offset);
            string username = NameUtil.LongToString(usernameLong);
            username = NameUtil.Normalise(username);

            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogDebug("Session {Id}: empty username", session.SessionId);
                return null;
            }

            // Validate username characters
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != ' ')
                {
                    _logger.LogDebug("Session {Id}: invalid character in username", session.SessionId);
                    return null;
                }
            }

            // Password (newline/null terminated string)
            string password = ReadString(loginBlock, ref offset);
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogDebug("Session {Id}: empty password", session.SessionId);
                return null;
            }

            // Build ISAAC seeds from client+server session keys
            int[] isaacSeed = new int[4];
            isaacSeed[0] = (int)(clientSessionKey >> 32);
            isaacSeed[1] = (int)clientSessionKey;
            isaacSeed[2] = (int)(serverSessionKey >> 32);
            isaacSeed[3] = (int)serverSessionKey;

            _logger.LogInformation("Session {Id}: login request from '{Username}' (rev {Rev}, HD={HD})",
                session.SessionId, username, clientVersion, usingHD);

            return new LoginResult(username, password, isaacSeed, usingHD, clientVersion);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Session {Id}: login handshake error", session.SessionId);
            return null;
        }
    }

    /// <summary>
    /// Send the login response to the client.
    /// </summary>
    public async Task SendLoginResponseAsync(PlayerSession session, int returnCode, int rights, int playerId, CancellationToken ct)
    {
        var stream = session.GetStream();

        // The Java server sends: returnCode, rights, 0, 0, 0, 1, 0, playerId, 0
        var response = new byte[9];
        response[0] = (byte)returnCode;
        response[1] = (byte)rights;
        response[2] = 0;
        response[3] = 0;
        response[4] = 0;
        response[5] = 1;
        response[6] = 0;
        response[7] = (byte)playerId;
        response[8] = 0;

        await stream.WriteAsync(response, ct);
        await stream.FlushAsync(ct);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static async Task ReadExactAsync(Stream stream, byte[] buffer, int offset, int count, CancellationToken ct)
    {
        int totalRead = 0;
        while (totalRead < count)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(offset + totalRead, count - totalRead), ct);
            if (read == 0)
                throw new EndOfStreamException("Client disconnected during login handshake");
            totalRead += read;
        }
    }

    private static int ReadInt(byte[] buf, ref int offset)
    {
        int val = (buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3];
        offset += 4;
        return val;
    }

    private static long ReadLong(byte[] buf, ref int offset)
    {
        long hi = (uint)ReadInt(buf, ref offset);
        long lo = (uint)ReadInt(buf, ref offset);
        return (hi << 32) | lo;
    }

    private static void WriteLong(byte[] buf, int offset, long val)
    {
        buf[offset]     = (byte)(val >> 56);
        buf[offset + 1] = (byte)(val >> 48);
        buf[offset + 2] = (byte)(val >> 40);
        buf[offset + 3] = (byte)(val >> 32);
        buf[offset + 4] = (byte)(val >> 24);
        buf[offset + 5] = (byte)(val >> 16);
        buf[offset + 6] = (byte)(val >> 8);
        buf[offset + 7] = (byte)val;
    }

    private static string ReadString(byte[] buf, ref int offset)
    {
        var sb = new StringBuilder();
        while (offset < buf.Length)
        {
            byte b = buf[offset++];
            if (b == 10 || b == 0) break;
            sb.Append((char)b);
        }
        return sb.ToString();
    }
}

/// <summary>
/// Result of a successful login handshake parse.
/// </summary>
public sealed record LoginResult(
    string Username,
    string Password,
    int[] IsaacSeed,
    bool UsingHD,
    int ClientVersion
);
