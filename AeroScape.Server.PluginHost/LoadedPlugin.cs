using System.Runtime.Loader;
using AeroScape.Server.API;

namespace AeroScape.Server.PluginHost;

/// <summary>
/// Represents a loaded plugin instance.
/// </summary>
public class LoadedPlugin
{
    public required string Name { get; init; }
    public required string Directory { get; init; }
    public required PluginMetadata Metadata { get; init; }
    public required IPlugin Instance { get; init; }
    public required PluginLoadContext LoadContext { get; init; }
    public required PluginState State { get; set; }
    public List<string> DependsOn { get; init; } = new();
    public List<string> DependedBy { get; set; } = new();
}

/// <summary>
/// Plugin lifecycle state.
/// </summary>
public enum PluginState
{
    Loaded,
    Enabled,
    Disabled,
    Failed
}