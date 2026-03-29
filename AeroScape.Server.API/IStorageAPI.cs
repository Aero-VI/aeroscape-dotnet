namespace AeroScape.Server.API;

/// <summary>
/// Plugin-specific storage API.
/// </summary>
public interface IStorageAPI
{
    /// <summary>
    /// Store a value with a key.
    /// </summary>
    Task SetAsync<T>(string key, T value);
    
    /// <summary>
    /// Retrieve a value by key.
    /// </summary>
    Task<T?> GetAsync<T>(string key);
    
    /// <summary>
    /// Check if a key exists.
    /// </summary>
    Task<bool> ExistsAsync(string key);
    
    /// <summary>
    /// Delete a value by key.
    /// </summary>
    Task<bool> DeleteAsync(string key);
    
    /// <summary>
    /// Get all keys with a prefix.
    /// </summary>
    Task<IReadOnlyList<string>> GetKeysAsync(string prefix = "");
    
    /// <summary>
    /// Clear all storage for this plugin.
    /// </summary>
    Task ClearAsync();
}