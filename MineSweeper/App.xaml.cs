﻿﻿﻿﻿﻿﻿namespace MineSweeper;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        // Add debug logging
        System.Diagnostics.Debug.WriteLine("App: Constructor called");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        try
        {
            // Use AppShell to enable navigation between pages
            System.Diagnostics.Debug.WriteLine("App: Creating AppShell for navigation");
            var appShell = new AppShell();
            System.Diagnostics.Debug.WriteLine("App: AppShell created successfully");
            
            // Create and return a window with AppShell
            var window = new Window(appShell);
            System.Diagnostics.Debug.WriteLine("App: Window created with AppShell");
            return window;
        }
        catch (Exception ex)
        {
            // Log any exceptions
            System.Diagnostics.Debug.WriteLine($"App: Error creating AppShell: {ex}");
            
            // Fall back to MainPageDebug directly if there's an error
            System.Diagnostics.Debug.WriteLine("App: Falling back to MainPageDebug directly");
            return new Window(new MainPageDebug());
        }
    }
}
