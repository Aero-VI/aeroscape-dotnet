using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AeroScape.Server.API;
using AeroScape.Server.API.Events;

namespace AeroScape.Server.PluginHost;

/// <summary>
/// Implementation of IPluginAPI that provides access to game services.
/// </summary>
internal class PluginAPI : IPluginAPI
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EventManager _globalEventManager;
    private readonly string _pluginName;
    private readonly string _pluginDirectory;
    
    // Lazy-loaded services
    private IEventAPI? _events;
    private IPlayerAPI? _players;
    private IWorldAPI? _world;
    private IItemAPI? _items;
    private ISkillAPI? _skills;
    private IInventoryAPI? _inventory;
    private IPacketAPI? _packets;
    private IConfigAPI? _config;
    private ISchedulerAPI? _scheduler;
    private IStorageAPI? _storage;
    private ILogger? _logger;

    public PluginAPI(
        IServiceProvider serviceProvider,
        EventManager globalEventManager,
        string pluginName,
        string pluginDirectory)
    {
        _serviceProvider = serviceProvider;
        _globalEventManager = globalEventManager;
        _pluginName = pluginName;
        _pluginDirectory = pluginDirectory;
    }

    public IEventAPI Events => _events ??= CreatePluginEventAPI();
    
    public IPlayerAPI Players => _players ??= _serviceProvider.GetRequiredService<IPlayerAPI>();
    
    public IWorldAPI World => _world ??= _serviceProvider.GetRequiredService<IWorldAPI>();
    
    public IItemAPI Items => _items ??= _serviceProvider.GetRequiredService<IItemAPI>();
    
    public ISkillAPI Skills => _skills ??= _serviceProvider.GetRequiredService<ISkillAPI>();
    
    public IInventoryAPI Inventory => _inventory ??= _serviceProvider.GetRequiredService<IInventoryAPI>();
    
    public IPacketAPI Packets => _packets ??= _serviceProvider.GetRequiredService<IPacketAPI>();
    
    public IConfigAPI Config => _config ??= CreatePluginConfigAPI();
    
    public ISchedulerAPI Scheduler => _scheduler ??= CreatePluginSchedulerAPI();
    
    public IStorageAPI Storage => _storage ??= CreatePluginStorageAPI();
    
    public ILogger Logger => _logger ??= CreatePluginLogger();

    private IEventAPI CreatePluginEventAPI()
    {
        // Create a plugin-specific event API that tracks this plugin's handlers
        var logger = _serviceProvider.GetRequiredService<ILogger<EventManager>>();
        return new EventManager(logger, _pluginName);
    }

    private IConfigAPI CreatePluginConfigAPI()
    {
        // TODO: Implement plugin-specific config loading from _pluginDirectory
        throw new NotImplementedException("Config API not yet implemented");
    }

    private ISchedulerAPI CreatePluginSchedulerAPI()
    {
        // TODO: Implement scheduler that tracks tasks per plugin
        throw new NotImplementedException("Scheduler API not yet implemented");
    }

    private IStorageAPI CreatePluginStorageAPI()
    {
        // TODO: Implement plugin-specific storage (SQLite or key-value store)
        throw new NotImplementedException("Storage API not yet implemented");
    }

    private ILogger CreatePluginLogger()
    {
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        return loggerFactory.CreateLogger($"Plugin.{_pluginName}");
    }
}