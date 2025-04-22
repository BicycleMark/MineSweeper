﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using ILogger = MineSweeper.Models.ILogger;

namespace MineSweeper;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Configure app with performance optimizations
        ConfigureApp(builder);
        
        // Register dependencies with performance considerations
        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterPages(builder.Services);
        
        #if DEBUG
        // Only add debug logging in debug builds
        builder.Logging.AddDebug();
        #endif

        return builder.Build();
    }
    
    private static void ConfigureApp(MauiAppBuilder builder)
    {
        builder
            .UseMauiApp<App>()
            // Performance: Configure handlers for optimized rendering
            .ConfigureMauiHandlers(handlers => 
            {
                // Add performance-optimized handlers here when needed
            })
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
    }
    
    private static void RegisterServices(IServiceCollection services)
    {
        // Core Services - Use appropriate lifetimes for performance
        services.AddSingleton<ILogger, CustomDebugLogger>(); // Singleton for shared logging
        
        // Performance: Factory pattern for more control over instance creation and lifetime
        services.AddSingleton<IGameModelFactory>(serviceProvider => 
        {
            var logger = serviceProvider.GetRequiredService<ILogger>();
            return new GameModelFactory(logger);
        });
    }
    
    private static void RegisterViewModels(IServiceCollection services)
    {
        // Performance: Lazy initialization through factory
        services.AddSingleton<GameViewModel>(serviceProvider => 
        {
            var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var modelFactory = serviceProvider.GetRequiredService<IGameModelFactory>();
            
            return new GameViewModel(dispatcher, logger, modelFactory);
        });
    }
    
    private static void RegisterPages(IServiceCollection services)
    {
        // Register Pages as transient for lower memory usage
        // Each instance is created when requested and disposed when finished
        services.AddTransient<MainPage>();
    }
}
