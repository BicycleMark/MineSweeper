using MineSweeper.Services.Logging;

namespace MineSweeper.Services.Platform;

/// <summary>
/// Default implementation of the IPlatformService interface
/// </summary>
public class DefaultPlatformService : IPlatformService
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of DefaultPlatformService
    /// </summary>
    /// <param name="logger">Logger for diagnostic information</param>
    public DefaultPlatformService(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public string OperatingSystem => DeviceInfo.Current.Platform.ToString();

    /// <inheritdoc />
    public string DeviceModel => DeviceInfo.Current.Model;

    /// <inheritdoc />
    public string DeviceId => DeviceInfo.Current.Idiom.ToString();

    /// <inheritdoc />
    public void SetOrientation(ScreenOrientation orientation)
    {
        try
        {
            // Convert to platform-specific orientation
            switch (orientation)
            {
                case ScreenOrientation.Portrait:
                    SetPortraitOrientation();
                    break;
                case ScreenOrientation.Landscape:
                    SetLandscapeOrientation();
                    break;
                case ScreenOrientation.Auto:
                    SetAutoOrientation();
                    break;
                default:
                    _logger.LogWarning($"Unsupported orientation: {orientation}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting orientation: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public bool Vibrate(int durationMs)
    {
        try
        {
            // Ensure duration is reasonable
            durationMs = Math.Clamp(durationMs, 1, 1000);
            
            // Use Maui's vibration service
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(durationMs));
            
            return true;
        }
        catch (FeatureNotSupportedException)
        {
            _logger.LogWarning("Vibration not supported on this device");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error vibrating device: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc />
    public bool OpenAppSettings()
    {
        try
        {
            AppInfo.Current.ShowSettingsUI();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error opening app settings: {ex.Message}");
            return false;
        }
    }

    private void SetPortraitOrientation()
    {
#if ANDROID
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity != null)
        {
            activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            _logger.Log("Set Android orientation to Portrait");
        }
#elif IOS || MACCATALYST
        // iOS-specific orientation code
        UIKit.UIDevice.CurrentDevice.SetValueForKey(
            Foundation.NSNumber.FromNInt((int)UIKit.UIInterfaceOrientation.Portrait),
            new Foundation.NSString("orientation"));
        _logger.Log("Set iOS orientation to Portrait");
#else
        _logger.LogWarning("Portrait orientation not implemented for this platform");
#endif
    }

    private void SetLandscapeOrientation()
    {
#if ANDROID
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity != null)
        {
            activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            _logger.Log("Set Android orientation to Landscape");
        }
#elif IOS || MACCATALYST
        // iOS-specific orientation code
        UIKit.UIDevice.CurrentDevice.SetValueForKey(
            Foundation.NSNumber.FromNInt((int)UIKit.UIInterfaceOrientation.LandscapeRight),
            new Foundation.NSString("orientation"));
        _logger.Log("Set iOS orientation to Landscape");
#else
        _logger.LogWarning("Landscape orientation not implemented for this platform");
#endif
    }

    private void SetAutoOrientation()
    {
#if ANDROID
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity != null)
        {
            activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Unspecified;
            _logger.Log("Set Android orientation to Auto");
        }
#elif IOS || MACCATALYST
        // For iOS, setting auto orientation requires changes to Info.plist
        // and should be managed by the app's configuration
        _logger.Log("Auto orientation for iOS requires configuration in Info.plist");
#else
        _logger.LogWarning("Auto orientation not implemented for this platform");
#endif
    }
}
