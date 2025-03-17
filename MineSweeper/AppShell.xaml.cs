﻿﻿﻿﻿﻿namespace MineSweeper;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("MainPageDebug", typeof(MainPageDebug));
        
        // Add debug logging
        System.Diagnostics.Debug.WriteLine("AppShell: Constructor called");
        System.Diagnostics.Debug.WriteLine("AppShell: Routes registered for navigation");
    }
}
