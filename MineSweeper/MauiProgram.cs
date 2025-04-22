using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using MineSweeper.Views.ImageLoaders;
using ILogger = MineSweeper.Models.ILogger;

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
        builder.Services.AddSingleton<ILogger, CustomDebugLogger>();
        builder.Services.AddSingleton<IGameModelFactory, GameModelFactory>();

        // Register Image Loaders
        builder.Services.AddSingleton<IImageLoader>(provider => new SvgLoader());
        builder.Services.AddSingleton<SvgLoader>();

        // Register ViewModels
        builder.Services.AddSingleton<GameViewModel>();

        // Register Pages as transient to allow multiple instances
        builder.Services.AddTransient<MainPage>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}