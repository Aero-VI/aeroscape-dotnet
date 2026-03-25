using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Session;
using AeroScape.Server.Network.Protocol;

namespace AeroScape.Server.Network.Listeners;

/// <summary>
/// Long-running hosted service that listens for incoming TCP connections on the
/// game port (default 43594) and spawns a read loop per client using
/// <see cref="System.IO.Pipelines"/>.
/// </summary>
public sealed class TcpBackgroundService : BackgroundService
{
    private readonly ILogger<TcpBackgroundService> _logger;
    private readonly IPlayerSessionManager _sessions;
    private readonly PacketRouter _router;

    /// <summary>Game port — classic RS 508 default.</summary>
    private const int DefaultPort = 43594;

    public TcpBackgroundService(
        ILogger<TcpBackgroundService> logger,
        IPlayerSessionManager sessions,
        PacketRouter router)
    {
        _logger   = logger;
        _sessions = sessions;
        _router   = router;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new TcpListener(IPAddress.Any, DefaultPort);
        listener.Start();
        _logger.LogInformation("AeroScape TCP listener started on port {Port}", DefaultPort);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var tcp = await listener.AcceptTcpClientAsync(stoppingToken);
                tcp.NoDelay = true;
                _ = HandleClientAsync(tcp, stoppingToken);
            }
        }
        finally
        {
            listener.Stop();
            _logger.LogInformation("AeroScape TCP listener stopped");
        }
    }

    private async Task HandleClientAsync(TcpClient tcp, CancellationToken stoppingToken)
    {
        var pipe = new Pipe();
        var session = new PlayerSession(tcp, pipe.Writer);
        _sessions.AddSession(session);
        _logger.LogInformation("Client connected: {Ip} (session {Id})", session.IpAddress, session.SessionId);

        try
        {
            var stream = tcp.GetStream();
            var fillTask  = FillPipeAsync(stream, pipe.Writer, session.CancellationToken, stoppingToken);
            var readTask  = ReadPipeAsync(pipe.Reader, session, stoppingToken);
            await Task.WhenAll(fillTask, readTask);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Session {Id} error", session.SessionId);
        }
        finally
        {
            _sessions.RemoveSession(session.SessionId);
            await session.DisposeAsync();
            _logger.LogInformation("Session {Id} disconnected", session.SessionId);
        }
    }

    /// <summary>Reads raw bytes from the network stream into the pipe.</summary>
    private static async Task FillPipeAsync(
        NetworkStream stream, PipeWriter writer,
        CancellationToken sessionToken, CancellationToken appToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(sessionToken, appToken);

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var memory = writer.GetMemory(512);
                int bytesRead = await stream.ReadAsync(memory, cts.Token);
                if (bytesRead == 0) break; // client disconnected

                writer.Advance(bytesRead);
                var result = await writer.FlushAsync(cts.Token);
                if (result.IsCompleted) break;
            }
        }
        finally
        {
            await writer.CompleteAsync();
        }
    }

    /// <summary>Reads framed packets from the pipe and routes them.</summary>
    private async Task ReadPipeAsync(
        PipeReader reader, PlayerSession session, CancellationToken stoppingToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(session.CancellationToken, stoppingToken);

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var result = await reader.ReadAsync(cts.Token);
                var buffer = result.Buffer;

                long consumed = _router.ProcessBuffer(session, buffer);
                reader.AdvanceTo(buffer.GetPosition(consumed), buffer.End);

                if (result.IsCompleted) break;
            }
        }
        finally
        {
            await reader.CompleteAsync();
        }
    }
}
