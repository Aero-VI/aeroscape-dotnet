using System;
using System.Net.Sockets;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using AeroScape.Server.Core.Crypto;
using AeroScape.Server.Core.Entities;

namespace AeroScape.Server.Core.Session;

public class PlayerSession : IAsyncDisposable
{
    public Guid SessionId { get; } = Guid.NewGuid();

    /// <summary>Client revision (e.g. 508). Set during login handshake.</summary>
    public int Revision { get; set; }

    /// <summary>ISAAC cipher for decoding incoming packet opcodes.</summary>
    public IsaacCipher? InCipher { get; set; }

    /// <summary>ISAAC cipher for encoding outgoing packet opcodes.</summary>
    public IsaacCipher? OutCipher { get; set; }

    public Player? Entity { get; set; }
    public string IpAddress { get; }

    private readonly TcpClient _tcpClient;
    private readonly PipeWriter _writer;
    private readonly CancellationTokenSource _cts = new();

    public PlayerSession(TcpClient tcpClient, PipeWriter writer)
    {
        _tcpClient = tcpClient;
        _writer = writer;
        IpAddress = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";
    }

    /// <summary>
    /// Initialise ISAAC ciphers from the login handshake seed keys.
    /// The inbound cipher uses the raw seed; the outbound cipher uses seed + 50.
    /// </summary>
    public void InitIsaac(int[] isaacSeed)
    {
        InCipher = new IsaacCipher(isaacSeed);

        var outSeed = new int[isaacSeed.Length];
        for (int i = 0; i < isaacSeed.Length; i++)
            outSeed[i] = isaacSeed[i] + 50;
        OutCipher = new IsaacCipher(outSeed);
    }

    public CancellationToken CancellationToken => _cts.Token;

    /// <summary>Access the underlying pipe writer for sending data to the client.</summary>
    public PipeWriter Writer => _writer;

    /// <summary>Access the underlying network stream.</summary>
    public NetworkStream GetStream() => _tcpClient.GetStream();

    public void Disconnect()
    {
        _cts.Cancel();
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _tcpClient.Dispose();
        await _writer.CompleteAsync();
    }
}
