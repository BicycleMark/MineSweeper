namespace MineSweeper.Services.Platform;

/// <summary>
/// Defines screen orientation options for the application
/// </summary>
public enum ScreenOrientation
{
    /// <summary>
    /// Portrait orientation (vertical)
    /// </summary>
    Portrait,
    
    /// <summary>
    /// Landscape orientation (horizontal)
    /// </summary>
    Landscape,
    
    /// <summary>
    /// Portrait orientation, upside down
    /// </summary>
    PortraitUpsideDown,
    
    /// <summary>
    /// Landscape orientation, left side up
    /// </summary>
    LandscapeLeft,
    
    /// <summary>
    /// Landscape orientation, right side up
    /// </summary>
    LandscapeRight,
    
    /// <summary>
    /// Allow the system to determine the orientation based on sensors
    /// </summary>
    Auto
}

/// <summary>
/// Interface for platform-specific functionality
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Gets the current operating system name
    /// </summary>
    string OperatingSystem { get; }
    
    /// <summary>
    /// Gets the device model name
    /// </summary>
    string DeviceModel { get; }
    
    /// <summary>
    /// Gets a unique identifier for the device
    /// </summary>
    string DeviceId { get; }
    
    /// <summary>
    /// Sets the screen orientation for the application
    /// </summary>
    /// <param name="orientation">The desired orientation</param>
    void SetOrientation(ScreenOrientation orientation);
    
    /// <summary>
    /// Vibrates the device with the specified duration
    /// </summary>
    /// <param name="durationMs">Duration in milliseconds</param>
    /// <returns>True if vibration is supported and succeeded, false otherwise</returns>
    bool Vibrate(int durationMs);
    
    /// <summary>
    /// Opens the system settings for the application
    /// </summary>
    /// <returns>True if successfully opened settings, false otherwise</returns>
    bool OpenAppSettings();
}
