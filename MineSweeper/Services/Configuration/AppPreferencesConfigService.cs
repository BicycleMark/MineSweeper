using MineSweeper.Services.Logging;

namespace MineSweeper.Services.Configuration;

/// <summary>
/// Implementation of IConfigurationService that uses application preferences
/// for configuration storage
/// </summary>
public class AppPreferencesConfigService : IConfigurationService
{
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of AppPreferencesConfigService
    /// </summary>
    /// <param name="logger">Logger for diagnostic information</param>
    public AppPreferencesConfigService(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <inheritdoc />
    public T GetValue<T>(string key, T? defaultValue = default)
    {
        try
        {
            if (!Preferences.ContainsKey(key))
            {
                _logger.Log($"Configuration key not found: {key}, using default value");
                return defaultValue!;
            }
            
            // Handle different types appropriately
            if (typeof(T) == typeof(string))
            {
                var defaultStr = defaultValue != null ? (string)(object)defaultValue : null;
                return (T)(object)Preferences.Get(key, defaultStr)!;
            }
            else if (typeof(T) == typeof(int))
            {
                var defaultInt = defaultValue != null ? (int)(object)defaultValue : 0;
                return (T)(object)Preferences.Get(key, defaultInt);
            }
            else if (typeof(T) == typeof(bool))
            {
                var defaultBool = defaultValue != null ? (bool)(object)defaultValue : false;
                return (T)(object)Preferences.Get(key, defaultBool);
            }
            else if (typeof(T) == typeof(double))
            {
                var defaultDouble = defaultValue != null ? (double)(object)defaultValue : 0.0;
                return (T)(object)Preferences.Get(key, defaultDouble);
            }
            else if (typeof(T) == typeof(float))
            {
                var defaultFloat = defaultValue != null ? (float)(object)defaultValue : 0.0f;
                return (T)(object)Preferences.Get(key, defaultFloat);
            }
            else if (typeof(T) == typeof(long))
            {
                var defaultLong = defaultValue != null ? (long)(object)defaultValue : 0L;
                return (T)(object)Preferences.Get(key, defaultLong);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                var ticks = Preferences.Get(key, 0L);
                if (ticks == 0)
                {
                    return defaultValue!;
                }
                return (T)(object)new DateTime(ticks);
            }
            
            // For more complex types, use serialization in a real implementation
            _logger.LogWarning($"Unsupported type for configuration: {typeof(T).Name}");
            return defaultValue!;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving configuration value for key {key}: {ex.Message}");
            return defaultValue!;
        }
    }
    
    /// <inheritdoc />
    public void SetValue<T>(string key, T? value)
    {
        try
        {
            // If value is null, remove the key instead of storing null
            if (value == null)
            {
                if (Preferences.ContainsKey(key))
                {
                    Preferences.Remove(key);
                }
                return;
            }
            
            // Handle different types appropriately
            if (typeof(T) == typeof(string))
            {
                var stringValue = (string)(object)value;
                Preferences.Set(key, stringValue);
            }
            else if (typeof(T) == typeof(int))
            {
                var intValue = (int)(object)value;
                Preferences.Set(key, intValue);
            }
            else if (typeof(T) == typeof(bool))
            {
                var boolValue = (bool)(object)value;
                Preferences.Set(key, boolValue);
            }
            else if (typeof(T) == typeof(double))
            {
                var doubleValue = (double)(object)value;
                Preferences.Set(key, doubleValue);
            }
            else if (typeof(T) == typeof(float))
            {
                var floatValue = (float)(object)value;
                Preferences.Set(key, floatValue);
            }
            else if (typeof(T) == typeof(long))
            {
                var longValue = (long)(object)value;
                Preferences.Set(key, longValue);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                var dateTime = (DateTime)(object)value;
                Preferences.Set(key, dateTime.Ticks);
            }
            else
            {
                // For more complex types, use serialization in a real implementation
                _logger.LogWarning($"Unsupported type for configuration: {typeof(T).Name}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting configuration value for key {key}: {ex.Message}");
        }
    }
    
    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
        return Preferences.ContainsKey(key);
    }
    
    /// <inheritdoc />
    public bool RemoveKey(string key)
    {
        if (!Preferences.ContainsKey(key))
        {
            return false;
        }
        
        Preferences.Remove(key);
        return true;
    }
}
