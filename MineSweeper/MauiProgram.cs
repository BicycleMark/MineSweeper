﻿﻿﻿﻿﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Services.Configuration;
using MineSweeper.Services.Logging;
using MineSweeper.Services.Platform;
using ILogger = MineSweeper.Services.Logging.ILogger;

namespace MineSweeper;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // Configure app with performance optimizations
        ConfigureApp(builder);
        
        // Register dependencies with clear organization and performance considerations
        RegisterCore(builder.Services);
        RegisterFeatures(builder.Services);
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
    
    private static void RegisterCore(IServiceCollection services)
    {
        // Core Services
        services.AddSingleton<ILogger, CustomDebugLogger>();
        services.AddSingleton<IConfigurationService, AppPreferencesConfigService>();
        services.AddSingleton<IPlatformService, DefaultPlatformService>();
    }
    
    private static void RegisterFeatures(IServiceCollection services)
    {
        // Game feature components
        RegisterGameFeature(services);
        
        // Future features can be added here as separate methods
        // RegisterSettingsFeature(services);
        // RegisterHighScoreFeature(services);
    }
    
    private static void RegisterGameFeature(IServiceCollection services)
    {
        // Register game models
        services.AddSingleton<IGameModelFactory, GameModelFactory>();
        
        // Register game view models with lazy initialization for performance
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
        
        // Register feature-specific pages
        services.AddTransient<Features.Game.Pages.GamePage>();
    }
}
