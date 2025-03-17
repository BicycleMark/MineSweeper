﻿﻿﻿﻿namespace MineSweeper;

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
            // Create MainPageDebug directly without using AppShell
            System.Diagnostics.Debug.WriteLine("App: Creating MainPageDebug directly");
            var mainPageDebug = new MainPageDebug();
            System.Diagnostics.Debug.WriteLine("App: MainPageDebug created successfully");
            
            // Create and return a window with MainPageDebug
            var window = new Window(mainPageDebug);
            System.Diagnostics.Debug.WriteLine("App: Window created with MainPageDebug");
            return window;
        }
        catch (Exception ex)
        {
            // Log any exceptions
            System.Diagnostics.Debug.WriteLine($"App: Error creating MainPageDebug: {ex}");
            
            // Fall back to AppShell if there's an error
            return new Window(new AppShell());
        }
    }
}
