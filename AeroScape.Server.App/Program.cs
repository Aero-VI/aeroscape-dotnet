using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Session;
using AeroScape.Server.Network.Listeners;
using AeroScape.Server.Network.Protocol;

var builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Core services
builder.Services.AddSingleton<IPlayerSessionManager, PlayerSessionManager>();

// Network / protocol
builder.Services.AddSingleton<PacketRouter>();
builder.Services.AddHostedService<TcpBackgroundService>();

var host = builder.Build();
host.Run();
