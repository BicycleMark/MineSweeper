﻿using Microsoft.Extensions.Logging;

namespace MineSweeper;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        // Register FileSystem Dependency   
        // builder.Services.AddSingleton(_ => File);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}