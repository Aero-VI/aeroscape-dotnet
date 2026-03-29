namespace AeroScape.Server.API;

/// <summary>
/// Base interface for all AeroScape plugins.
/// </summary>
public interface IPlugin
{
    /// <summary>Plugin metadata</summary>
    PluginInfo Info { get; }
    
    /// <summary>Called when plugin is loaded into memory</summary>
    void OnLoad(IPluginAPI api);
    
    /// <summary>Called when plugin is enabled (after all plugins loaded)</summary>
    void OnEnable();
    
    /// <summary>Called when plugin is being disabled</summary>
    void OnDisable();
    
    /// <summary>Called when plugin is being unloaded from memory</summary>
    void OnUnload();
}

/// <summary>
/// Plugin metadata information.
/// </summary>
public class PluginInfo
{
    public required string Name { get; init; }
    public required string Version { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
    public string[] Dependencies { get; init; } = Array.Empty<string>();
}