using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AeroScape.Server.API;

namespace AeroScape.Server.PluginHost;

/// <summary>
/// Manages the lifecycle of all plugins.
/// </summary>
public class PluginManager
{
    private readonly ILogger<PluginManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly PluginLoader _pluginLoader;
    private readonly EventManager _globalEventManager;
    private readonly ConcurrentDictionary<string, LoadedPlugin> _plugins = new();
    private readonly string _pluginsDirectory;

    public PluginManager(
        ILogger<PluginManager> logger,
        IServiceProvider serviceProvider,
        PluginLoader pluginLoader,
        string pluginsDirectory)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _pluginLoader = pluginLoader;
        _pluginsDirectory = pluginsDirectory;
        
        // Create a global event manager for cross-plugin events
        var eventLogger = _serviceProvider.GetRequiredService<ILogger<EventManager>>();
        _globalEventManager = new EventManager(eventLogger, "CORE");
    }

    /// <summary>
    /// Get the global event manager for firing events.
    /// </summary>
    public EventManager GlobalEventManager => _globalEventManager;

    /// <summary>
    /// Initialize and load all plugins from the plugins directory.
    /// </summary>
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing plugin system from {Directory}", _pluginsDirectory);

        // Scan for plugins
        var pluginInfos = await _pluginLoader.ScanForPluginsAsync(_pluginsDirectory);
        
        // Load each plugin
        foreach (var (directory, metadata) in pluginInfos)
        {
            var loaded = _pluginLoader.LoadPlugin(directory, metadata);
            if (loaded != null)
            {
                _plugins[metadata.Name] = loaded;
            }
        }

        // Resolve dependencies and determine load order
        var loadOrder = ResolveDependencies();
        
        // Call OnLoad for each plugin in dependency order
        foreach (var pluginName in loadOrder)
        {
            if (_plugins.TryGetValue(pluginName, out var plugin))
            {
                try
                {
                    var api = CreatePluginAPI(plugin);
                    plugin.Instance.OnLoad(api);
                    _logger.LogInformation("Loaded plugin {Name}", pluginName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load plugin {Name}", pluginName);
                    plugin.State = PluginState.Failed;
                }
            }
        }

        // Call OnEnable for successfully loaded plugins
        foreach (var pluginName in loadOrder)
        {
            if (_plugins.TryGetValue(pluginName, out var plugin) && plugin.State == PluginState.Loaded)
            {
                try
                {
                    plugin.Instance.OnEnable();
                    plugin.State = PluginState.Enabled;
                    _logger.LogInformation("Enabled plugin {Name}", pluginName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to enable plugin {Name}", pluginName);
                    plugin.State = PluginState.Failed;
                }
            }
        }
    }

    /// <summary>
    /// Disable and unload all plugins.
    /// </summary>
    public async Task ShutdownAsync()
    {
        _logger.LogInformation("Shutting down plugin system");

        // Get plugins in reverse dependency order
        var unloadOrder = ResolveDependencies();
        unloadOrder.Reverse();

        // Disable all plugins
        foreach (var pluginName in unloadOrder)
        {
            if (_plugins.TryGetValue(pluginName, out var plugin) && plugin.State == PluginState.Enabled)
            {
                try
                {
                    plugin.Instance.OnDisable();
                    plugin.State = PluginState.Disabled;
                    _logger.LogInformation("Disabled plugin {Name}", pluginName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disabling plugin {Name}", pluginName);
                }
            }
        }

        // Unload all plugins
        foreach (var pluginName in unloadOrder)
        {
            if (_plugins.TryRemove(pluginName, out var plugin))
            {
                try
                {
                    plugin.Instance.OnUnload();
                    _globalEventManager.UnregisterAllForPlugin(pluginName);
                    
                    // Unload the assembly context
                    plugin.LoadContext.Unload();
                    
                    _logger.LogInformation("Unloaded plugin {Name}", pluginName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error unloading plugin {Name}", pluginName);
                }
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Create a plugin-specific API instance.
    /// </summary>
    private IPluginAPI CreatePluginAPI(LoadedPlugin plugin)
    {
        return new PluginAPI(
            _serviceProvider,
            _globalEventManager,
            plugin.Name,
            plugin.Directory);
    }

    /// <summary>
    /// Resolve plugin dependencies and return load order.
    /// </summary>
    private List<string> ResolveDependencies()
    {
        var resolved = new List<string>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();

        foreach (var plugin in _plugins.Values)
        {
            Visit(plugin.Name, plugin, resolved, visited, visiting);
        }

        return resolved;
    }

    private void Visit(
        string name,
        LoadedPlugin plugin,
        List<string> resolved,
        HashSet<string> visited,
        HashSet<string> visiting)
    {
        if (visited.Contains(name))
            return;

        if (visiting.Contains(name))
        {
            _logger.LogError("Circular dependency detected involving plugin {Name}", name);
            return;
        }

        visiting.Add(name);

        foreach (var dep in plugin.DependsOn)
        {
            if (_plugins.TryGetValue(dep, out var depPlugin))
            {
                Visit(dep, depPlugin, resolved, visited, visiting);
                depPlugin.DependedBy.Add(name);
            }
            else
            {
                _logger.LogWarning("Plugin {Name} depends on missing plugin {Dependency}", name, dep);
            }
        }

        visiting.Remove(name);
        visited.Add(name);
        resolved.Add(name);
    }

    /// <summary>
    /// Get a loaded plugin by name.
    /// </summary>
    public IPlugin? GetPlugin(string name)
    {
        return _plugins.TryGetValue(name, out var plugin) ? plugin.Instance : null;
    }

    /// <summary>
    /// Get all loaded plugins.
    /// </summary>
    public IReadOnlyDictionary<string, IPlugin> GetAllPlugins()
    {
        return _plugins.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Instance);
    }
}