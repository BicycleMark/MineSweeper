﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using CommunityToolkit.Maui;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using MineSweeper.Views.ImageLoaders;

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
        
        // Register Image Loaders
        builder.Services.AddSingleton<IImageLoader>(provider => new SvgLoader());
        builder.Services.AddSingleton<SvgLoader>();
        
        // Register ViewModels
        builder.Services.AddSingleton<GameViewModel>();
        
        // Register Pages as transient to allow multiple instances
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainPageDebug>();
        

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
