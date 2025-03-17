﻿﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MineSweeper.Models;
using MineSweeper.ViewModels;

namespace MineSweeper;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        // Register Services
        builder.Services.AddSingleton<MineSweeper.Models.ILogger, MineSweeper.Models.CustomDebugLogger>();
        builder.Services.AddSingleton<IGameModelFactory, GameModelFactory>();
        
        // Register ViewModels
        builder.Services.AddSingleton<GameViewModel>();
        
        // Register Pages
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageDebug>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
