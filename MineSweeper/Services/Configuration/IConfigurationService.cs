namespace MineSweeper.Services.Configuration;

/// <summary>
/// Interface for configuration and settings management
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Gets a configuration value by key
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve</typeparam>
    /// <param name="key">The configuration key</param>
    /// <param name="defaultValue">Default value if the key doesn't exist</param>
    /// <returns>The configuration value or the default value if not found</returns>
    T GetValue<T>(string key, T? defaultValue = default);
    
    /// <summary>
    /// Sets a configuration value
    /// </summary>
    /// <typeparam name="T">The type of the value to store</typeparam>
    /// <param name="key">The configuration key</param>
    /// <param name="value">The value to store</param>
    void SetValue<T>(string key, T? value);
    
    /// <summary>
    /// Determines if a configuration key exists
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if the key exists, false otherwise</returns>
    bool ContainsKey(string key);
    
    /// <summary>
    /// Removes a configuration key and its value
    /// </summary>
    /// <param name="key">The key to remove</param>
    /// <returns>True if the key was removed, false if it didn't exist</returns>
    bool RemoveKey(string key);
}
