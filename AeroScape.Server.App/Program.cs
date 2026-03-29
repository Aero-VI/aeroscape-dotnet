using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using AeroScape.Server.App.Services;
using AeroScape.Server.Core.Engine;
using AeroScape.Server.Core.Handlers;
using AeroScape.Server.Core.Items;
using AeroScape.Server.Core.Messages;
using AeroScape.Server.Core.Movement;
using AeroScape.Server.Core.Session;
using AeroScape.Server.Core.Services;
using AeroScape.Server.Core.Api;
using AeroScape.Server.Data;
using AeroScape.Server.Core.World;
using AeroScape.Server.Network.Listeners;
using AeroScape.Server.Core.Frames;
using AeroScape.Server.Network.Login;
using AeroScape.Server.Network.Protocol;
using AeroScape.Server.Network.Update;
using AeroScape.Server.API;
using AeroScape.Server.PluginHost;

var builder = Host.CreateApplicationBuilder(args);

// Logging — Serilog replaces the default logger; ILogger<T> injection continues to work.
builder.Logging.ClearProviders();
builder.Services.AddSerilog(config => config
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Error)
    .WriteTo.Console());

// ── Data / EF Core ──────────────────────────────────────────────────────────
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

// ── Core services (MINIMAL - LOGIN/WALKING/WOODCUTTING ONLY) ───────────────
builder.Services.AddSingleton<IPlayerSessionManager, PlayerSessionManager>();
builder.Services.AddSingleton<ItemDefinitionLoader>();
builder.Services.AddSingleton<InventoryService>();
builder.Services.AddSingleton<PlayerItemsService>();
builder.Services.AddSingleton<PlayerEquipmentService>();
builder.Services.AddSingleton<GroundItemManager>();
builder.Services.AddSingleton<WalkQueue>();
builder.Services.AddSingleton<LegacyFileManager>();
builder.Services.AddSingleton<IClientUiService, ClientUiService>();
builder.Services.AddSingleton<ObjectInteractionService>();
builder.Services.AddSingleton<ObjectLoaderService>();
builder.Services.AddSingleton<NpcSpawnLoader>();

// All stub services removed - GameEngine dependencies fixed
builder.Services.AddSingleton<GameEngine>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<GameEngine>());

// ── Plugin system ───────────────────────────────────────────────────────────
builder.Services.AddSingleton<PluginLoader>();
builder.Services.AddSingleton<PluginManager>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<PluginManager>>();
    var pluginLoader = sp.GetRequiredService<PluginLoader>();
    var pluginsDir = Path.Combine(AppContext.BaseDirectory, "plugins");
    return new PluginManager(logger, sp, pluginLoader, pluginsDir);
});

// Plugin API implementations
builder.Services.AddSingleton<IPlayerAPI, PlayerAPIImpl>();
builder.Services.AddSingleton<ISkillAPI, SkillAPIImpl>();
builder.Services.AddSingleton<IInventoryAPI, InventoryAPIImpl>();
builder.Services.AddSingleton<IItemAPI, ItemAPIImpl>();
builder.Services.AddSingleton<IWorldAPI, WorldAPIImpl>();
builder.Services.AddSingleton<IPacketAPI, PacketAPIImpl>();

// ── Map data service ─────────────────────────────────────────────────────────
builder.Services.AddSingleton<MapDataService>();
builder.Services.AddSingleton<GameFrames>();
builder.Services.AddSingleton<PlayerUpdateWriter>();
builder.Services.AddSingleton<NpcUpdateWriter>();
builder.Services.AddSingleton<IGameUpdateService, GameUpdateService>();

// ── Login service ────────────────────────────────────────────────────────────
builder.Services.AddSingleton<IPlayerLoginService, PlayerLoginService>();
builder.Services.AddSingleton<IPlayerPersistenceService, PlayerPersistenceService>();
builder.Services.AddHostedService(sp => (PlayerPersistenceService)sp.GetRequiredService<IPlayerPersistenceService>());

// ── Network / protocol ──────────────────────────────────────────────────────
builder.Services.AddSingleton<PacketRouter>();
builder.Services.AddHostedService<TcpBackgroundService>();

// ── Scoped packet handlers (MINIMAL - LOGIN/WALKING/WOODCUTTING ONLY) ───────
builder.Services.AddScoped<IMessageHandler<WalkMessage>, WalkMessageHandler>();
builder.Services.AddScoped<IMessageHandler<PublicChatMessage>, PublicChatMessageHandler>();
// builder.Services.AddScoped<IMessageHandler<CommandMessage>, CommandMessageHandler>(); // Removed - not needed
// builder.Services.AddScoped<IMessageHandler<ActionButtonsMessage>, ActionButtonsMessageHandler>(); // Removed - handles UI for removed features
builder.Services.AddScoped<IMessageHandler<EquipItemMessage>, EquipItemMessageHandler>();
builder.Services.AddScoped<IMessageHandler<ItemOperateMessage>, ItemOperateMessageHandler>();
builder.Services.AddScoped<IMessageHandler<DropItemMessage>, DropItemMessageHandler>();
builder.Services.AddScoped<IMessageHandler<PickupItemMessage>, PickupItemMessageHandler>();
builder.Services.AddScoped<IMessageHandler<ObjectOption1Message>, ObjectOption1MessageHandler>();
builder.Services.AddScoped<IMessageHandler<ObjectOption2Message>, ObjectOption2MessageHandler>();
builder.Services.AddScoped<IMessageHandler<SwitchItemsMessage>, SwitchItemsMessageHandler>();
builder.Services.AddScoped<IMessageHandler<SwitchItems2Message>, SwitchItems2MessageHandler>();
builder.Services.AddScoped<IMessageHandler<ItemOnItemMessage>, ItemOnItemMessageHandler>();
builder.Services.AddScoped<IMessageHandler<ItemSelectMessage>, ItemSelectMessageHandler>();
builder.Services.AddScoped<IMessageHandler<ItemOption1Message>, ItemOption1MessageHandler>();
// builder.Services.AddScoped<IMessageHandler<ItemOnObjectMessage>, ItemOnObjectMessageHandler>(); // Removed - smithing
builder.Services.AddScoped<IMessageHandler<ItemOption2Message>, ItemOption2MessageHandler>();
builder.Services.AddScoped<IMessageHandler<StringInputMessage>, StringInputMessageHandler>();
builder.Services.AddScoped<IMessageHandler<LongInputMessage>, LongInputMessageHandler>();
builder.Services.AddScoped<IMessageHandler<IdleMessage>, IdleMessageHandler>();
// builder.Services.AddScoped<IMessageHandler<DialogueContinueMessage>, DialogueContinueMessageHandler>(); // Removed - no dialogues
builder.Services.AddScoped<IMessageHandler<CloseInterfaceMessage>, CloseInterfaceMessageHandler>();
builder.Services.AddScoped<IMessageHandler<ItemExamineMessage>, ItemExamineMessageHandler>();
builder.Services.AddScoped<IMessageHandler<ObjectExamineMessage>, ObjectExamineMessageHandler>();

var host = builder.Build();

host.Services.GetRequiredService<GameEngine>().GameUpdateService =
    host.Services.GetRequiredService<IGameUpdateService>();

// Ensure database is created (dev convenience — use migrations in production)
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AeroScapeDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// Load map region XTEA keys from data file
var mapDataService = host.Services.GetRequiredService<MapDataService>();
var mapDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "mapdata", "1.dat");
mapDataService.LoadRegions(mapDataPath);

// Initialize plugin system
var pluginManager = host.Services.GetRequiredService<PluginManager>();
await pluginManager.InitializeAsync();

// Register shutdown handler to clean up plugins
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(async () =>
{
    await pluginManager.ShutdownAsync();
});

host.Run();