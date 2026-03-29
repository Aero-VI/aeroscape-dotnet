using System.Reflection;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AeroScape.Server.API;

namespace AeroScape.Server.PluginHost;

/// <summary>
/// Handles loading plugins from disk.
/// </summary>
public class PluginLoader
{
    private readonly ILogger<PluginLoader> _logger;
    private readonly IDeserializer _yamlDeserializer;

    public PluginLoader(ILogger<PluginLoader> logger)
    {
        _logger = logger;
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    /// <summary>
    /// Scan a directory for plugins and load their metadata.
    /// </summary>
    public async Task<List<(string directory, PluginMetadata metadata)>> ScanForPluginsAsync(string pluginsDirectory)
    {
        var plugins = new List<(string, PluginMetadata)>();

        if (!Directory.Exists(pluginsDirectory))
        {
            _logger.LogWarning("Plugins directory does not exist: {Directory}", pluginsDirectory);
            return plugins;
        }

        foreach (var pluginDir in Directory.GetDirectories(pluginsDirectory))
        {
            var pluginYmlPath = Path.Combine(pluginDir, "plugin.yml");
            if (!File.Exists(pluginYmlPath))
            {
                _logger.LogWarning("Plugin directory {Directory} missing plugin.yml", pluginDir);
                continue;
            }

            try
            {
                var yamlContent = await File.ReadAllTextAsync(pluginYmlPath);
                var metadata = _yamlDeserializer.Deserialize<PluginMetadata>(yamlContent);
                
                if (string.IsNullOrEmpty(metadata?.Name) || string.IsNullOrEmpty(metadata.Main))
                {
                    _logger.LogError("Invalid plugin.yml in {Directory} - missing name or main", pluginDir);
                    continue;
                }

                plugins.Add((pluginDir, metadata));
                _logger.LogInformation("Found plugin: {Name} v{Version} in {Directory}", 
                    metadata.Name, metadata.Version, pluginDir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load plugin.yml from {Directory}", pluginDir);
            }
        }

        return plugins;
    }

    /// <summary>
    /// Load a plugin from its directory.
    /// </summary>
    public LoadedPlugin? LoadPlugin(string pluginDirectory, PluginMetadata metadata)
    {
        try
        {
            // Find the main DLL (should match directory name or be specified)
            var dllName = $"{metadata.Name}.dll";
            var dllPath = Path.Combine(pluginDirectory, dllName);
            
            if (!File.Exists(dllPath))
            {
                // Try alternative naming
                dllPath = Path.Combine(pluginDirectory, $"AeroScape.Plugin.{metadata.Name}.dll");
                if (!File.Exists(dllPath))
                {
                    // Look for any DLL
                    var dlls = Directory.GetFiles(pluginDirectory, "*.dll");
                    if (dlls.Length == 0)
                    {
                        _logger.LogError("No DLL found in plugin directory {Directory}", pluginDirectory);
                        return null;
                    }
                    dllPath = dlls[0];
                }
            }

            _logger.LogDebug("Loading plugin DLL from {Path}", dllPath);

            // Create isolated load context
            var loadContext = new PluginLoadContext(dllPath);
            
            // Load the assembly
            var assembly = loadContext.LoadFromAssemblyPath(dllPath);
            
            // Find the main plugin class
            var pluginType = assembly.GetType(metadata.Main);
            if (pluginType == null)
            {
                _logger.LogError("Main class {Main} not found in assembly {Assembly}", 
                    metadata.Main, assembly.FullName);
                return null;
            }

            // Check if it implements IPlugin
            if (!typeof(IPlugin).IsAssignableFrom(pluginType))
            {
                _logger.LogError("Main class {Main} does not implement IPlugin", metadata.Main);
                return null;
            }

            // Create instance
            var instance = Activator.CreateInstance(pluginType) as IPlugin;
            if (instance == null)
            {
                _logger.LogError("Failed to create instance of {Main}", metadata.Main);
                return null;
            }

            return new LoadedPlugin
            {
                Name = metadata.Name,
                Directory = pluginDirectory,
                Metadata = metadata,
                Instance = instance,
                LoadContext = loadContext,
                State = PluginState.Loaded,
                DependsOn = metadata.Dependencies ?? new()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load plugin {Name} from {Directory}", 
                metadata.Name, pluginDirectory);
            return null;
        }
    }
}