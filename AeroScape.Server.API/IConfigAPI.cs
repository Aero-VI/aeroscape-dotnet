namespace AeroScape.Server.API;

/// <summary>
/// Plugin configuration management API.
/// </summary>
public interface IConfigAPI
{
    /// <summary>
    /// Load the plugin's configuration file.
    /// Creates default config if it doesn't exist.
    /// </summary>
    IPluginConfig Load();
    
    /// <summary>
    /// Save the plugin's configuration file.
    /// </summary>
    void Save(IPluginConfig config);
    
    /// <summary>
    /// Reload configuration from disk.
    /// </summary>
    IPluginConfig Reload();
}

/// <summary>
/// Plugin configuration interface.
/// </summary>
public interface IPluginConfig
{
    /// <summary>Get string value</summary>
    string GetString(string key, string defaultValue = "");
    
    /// <summary>Get integer value</summary>
    int GetInt(string key, int defaultValue = 0);
    
    /// <summary>Get double value</summary>
    double GetDouble(string key, double defaultValue = 0.0);
    
    /// <summary>Get boolean value</summary>
    bool GetBool(string key, bool defaultValue = false);
    
    /// <summary>Get list of strings</summary>
    List<string> GetStringList(string key);
    
    /// <summary>Set a value</summary>
    void Set(string key, object value);
    
    /// <summary>Check if key exists</summary>
    bool Contains(string key);
    
    /// <summary>Remove a key</summary>
    void Remove(string key);
}