using Microsoft.Maui.Graphics;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Services.Logging;

namespace MineSweeper.Tests.Integration.ServiceIntegrationTests;

/// <summary>
/// Integration tests for the Logger service
/// </summary>
public class LoggerIntegrationTests
{
    [Fact]
    public void Logger_CapturesViewModelEvents()
    {
        // Arrange
        var testLogger = new TestLogger();
        var dispatcher = new TestDispatcher();
        var modelFactory = new GameModelFactory(testLogger);
        
        // Act
        var viewModel = new GameViewModel(dispatcher, testLogger, modelFactory);
        
        // Assert
        Assert.Contains(testLogger.LogMessages, m => m.Contains("GameViewModel initialized"));
    }
    
    [Fact]
    public async Task Logger_CapturesGameLifecycleEvents()
    {
        // Arrange
        var testLogger = new TestLogger();
        var dispatcher = new TestDispatcher();
        var modelFactory = new GameModelFactory(testLogger);
        var viewModel = new GameViewModel(dispatcher, testLogger, modelFactory);
        
        // Clear any initialization messages
        testLogger.LogMessages.Clear();
        
        // Act
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Assert
        Assert.Contains(testLogger.LogMessages, m => m.Contains("Starting new game"));
        Assert.Contains(testLogger.LogMessages, m => m.Contains("Creating game model"));
    }
    
    [Fact]
    public async Task Logger_CapturesGameplayEvents()
    {
        // Arrange
        var testLogger = new TestLogger();
        var dispatcher = new TestDispatcher();
        var modelFactory = new GameModelFactory(testLogger);
        var viewModel = new GameViewModel(dispatcher, testLogger, modelFactory);
        
        // Initialize game
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Clear messages from initialization
        testLogger.LogMessages.Clear();
        
        // Act - Make first move
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Assert
        Assert.Contains(testLogger.LogMessages, m => m.Contains("Play called"));
        Assert.Contains(testLogger.LogMessages, m => m.Contains("First move detected"));
    }
    
    [Fact]
    public async Task Logger_CapturesErrorEvents()
    {
        // Arrange
        var testLogger = new TestLogger();
        var dispatcher = new TestDispatcher();
        var modelFactory = new GameModelFactory(testLogger);
        var viewModel = new GameViewModel(dispatcher, testLogger, modelFactory);
        
        // Initialize game
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Dispose the viewModel to cause errors on subsequent operations
        viewModel.Dispose();
        
        // Clear messages
        testLogger.ErrorMessages.Clear();
        
        // Act - Try to play after disposal
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Assert
        Assert.Contains(testLogger.WarningMessages, m => m.Contains("after disposal"));
    }
    
    [Fact]
    public void Logger_IsSharedBetweenComponents()
    {
        // Arrange
        var testLogger = new TestLogger();
        var dispatcher = new TestDispatcher();
        
        // Act - Create multiple components that share the same logger
        var modelFactory = new GameModelFactory(testLogger);
        
        // Clear any existing messages
        testLogger.LogMessages.Clear();
        
        // Create a model - this should log a message
        var model = modelFactory.CreateModel(GameEnums.GameDifficulty.Easy);
        
        // Verify the model factory logged a message
        Assert.Contains(testLogger.LogMessages, m => m.Contains("Creating") && m.Contains("model"));
        
        // Clear messages again
        testLogger.LogMessages.Clear();
        
        // Create a view model - this should log a different message
        var viewModel = new GameViewModel(dispatcher, testLogger, modelFactory);
        
        // Assert - Verify the logger captured messages from the view model
        Assert.Contains(testLogger.LogMessages, m => m.Contains("GameViewModel initialized"));
    }
}
