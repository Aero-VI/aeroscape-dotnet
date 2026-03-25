using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AeroScape.Server.Core.Session;
using AeroScape.Server.Data;
using AeroScape.Server.Network.Listeners;
using AeroScape.Server.Network.Protocol;

var builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ── Data / EF Core ──────────────────────────────────────────────────────────
// Switch between SQL Server and SQLite via appsettings.json "DatabaseProvider".
// Default on Windows: SQL Server (DefaultConnection).
// Default on Linux/dev: SQLite (SqliteConnection).
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";

builder.Services.AddDbContext<AeroScapeDbContext>(options =>
{
    if (dbProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        var sqliteConn = builder.Configuration["ConnectionStrings:SqliteConnection"]
                         ?? "Data Source=AeroScape.db";
        options.UseSqlite(sqliteConn);
    }
    else
    {
        var sqlServerConn = builder.Configuration["ConnectionStrings:DefaultConnection"]
                            ?? throw new InvalidOperationException(
                                "Missing ConnectionStrings:DefaultConnection in appsettings.json");
        options.UseSqlServer(sqlServerConn);
    }
});

// Core services
builder.Services.AddSingleton<IPlayerSessionManager, PlayerSessionManager>();

// Network / protocol
builder.Services.AddSingleton<PacketRouter>();
builder.Services.AddHostedService<TcpBackgroundService>();

var host = builder.Build();

// Ensure database is created (dev convenience — use migrations in production)
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AeroScapeDbContext>();
    await db.Database.EnsureCreatedAsync();
}

host.Run();
