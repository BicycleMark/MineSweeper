using Microsoft.Extensions.DependencyInjection;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Services.Configuration;
using MineSweeper.Services.Logging;
using MineSweeper.Services.Platform;

namespace MineSweeper.Tests.Integration.DIContainerTests;

/// <summary>
/// Integration tests for the Dependency Injection container
/// </summary>
public class DIContainerIntegrationTests
{
    [Fact]
    public void Container_ResolvesAllDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Register services as in MauiProgram.cs, but with test implementations where needed
        services.AddSingleton<ILogger, CustomDebugLogger>();
        services.AddSingleton<IConfigurationService, AppPreferencesConfigService>();
        services.AddSingleton<IPlatformService, DefaultPlatformService>();
        services.AddSingleton<IGameModelFactory, GameModelFactory>();
        services.AddSingleton<IDispatcher, TestDispatcher>(); // Add test dispatcher
        services.AddSingleton<GameViewModel>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act & Assert
        var viewModel = serviceProvider.GetService<GameViewModel>();
        Assert.NotNull(viewModel);
        
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);
        Assert.IsType<CustomDebugLogger>(logger);
        
        var modelFactory = serviceProvider.GetService<IGameModelFactory>();
        Assert.NotNull(modelFactory);
        Assert.IsType<GameModelFactory>(modelFactory);
        
        var configService = serviceProvider.GetService<IConfigurationService>();
        Assert.NotNull(configService);
        Assert.IsType<AppPreferencesConfigService>(configService);
        
        var platformService = serviceProvider.GetService<IPlatformService>();
        Assert.NotNull(platformService);
        Assert.IsType<DefaultPlatformService>(platformService);
    }
    
    [Fact(Skip = "This test requires a MAUI application context which is not available in the test environment")]
    public void MauiProgram_RegistersAllServices()
    {
        // This test is skipped because it requires a MAUI application context
        // which is not available in the test environment
    }
    
    [Fact]
    public void Container_ServiceLifetimes_AreCorrect()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Register services as in MauiProgram.cs, but with test implementations where needed
        services.AddSingleton<ILogger, CustomDebugLogger>();
        services.AddSingleton<IConfigurationService, AppPreferencesConfigService>();
        services.AddSingleton<IPlatformService, DefaultPlatformService>();
        services.AddSingleton<IGameModelFactory, GameModelFactory>();
        services.AddSingleton<IDispatcher, TestDispatcher>(); // Add test dispatcher
        services.AddSingleton<GameViewModel>();
        
        // Add a test transient service to verify transient lifetime
        services.AddTransient<TestTransientService>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act - Resolve the same services twice
        var logger1 = serviceProvider.GetService<ILogger>();
        var logger2 = serviceProvider.GetService<ILogger>();
        
        var viewModel1 = serviceProvider.GetService<GameViewModel>();
        var viewModel2 = serviceProvider.GetService<GameViewModel>();
        
        var transient1 = serviceProvider.GetService<TestTransientService>();
        var transient2 = serviceProvider.GetService<TestTransientService>();
        
        // Assert
        // Singletons should return the same instance
        Assert.Same(logger1, logger2);
        Assert.Same(viewModel1, viewModel2);
        
        // Transients should return different instances
        Assert.NotSame(transient1, transient2);
    }
    
    // Simple class to test transient lifetime
    private class TestTransientService
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
    
    [Fact]
    public void Container_ResolvesViewModelWithCorrectDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Register services with test implementations
        services.AddSingleton<ILogger, TestLogger>();
        services.AddSingleton<IDispatcher, TestDispatcher>();
        services.AddSingleton<IGameModelFactory, GameModelFactory>();
        
        // Register the ViewModel with a factory method to ensure proper initialization
        services.AddSingleton<GameViewModel>(serviceProvider => 
        {
            var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
            var logger = serviceProvider.GetRequiredService<ILogger>();
            var modelFactory = serviceProvider.GetRequiredService<IGameModelFactory>();
            
            return new GameViewModel(dispatcher, logger, modelFactory);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        var viewModel = serviceProvider.GetService<GameViewModel>();
        
        // Assert
        Assert.NotNull(viewModel);
        
        // Verify the logger captured the initialization message
        var logger = serviceProvider.GetService<ILogger>() as TestLogger;
        Assert.NotNull(logger);
        Assert.Contains(logger!.LogMessages, m => m.Contains("GameViewModel initialized"));
    }
}
