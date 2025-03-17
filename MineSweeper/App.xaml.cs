﻿﻿﻿﻿namespace MineSweeper;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // TEMPORARY: Directly create and use MainPageDebug without dependency injection
        // This bypasses the AppShell and any navigation issues
        try
        {
            System.Diagnostics.Debug.WriteLine("App: Creating MainPageDebug directly");
            var mainPageDebug = new MainPageDebug();
            return new Window(mainPageDebug);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"App: Error creating MainPageDebug: {ex}");
            return new Window(new AppShell());
        }
    }
}
