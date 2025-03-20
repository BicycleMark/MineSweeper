using Foundation;
using UIKit;

namespace MineSweeper;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    
    public UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
    {
        // Lock to Portrait orientation
        return UIInterfaceOrientationMask.Portrait;
    }
}