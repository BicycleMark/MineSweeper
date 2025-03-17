using System.Windows.Input;
using Microsoft.Maui.Dispatching;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using System.Diagnostics;
using Moq;

namespace MineSweeper.Tests.ViewModels;

public class LoggerInjectionTests
{
    // Mock classes for testing
    private class MockDispatcher : IDispatcher
    {
        private readonly MockDispatcherTimer _timer = new();
        public bool IsDispatchRequired => false;

        public IDispatcherTimer CreateTimer() 
        {
            return _timer;
        }

        public bool Dispatch(Action action)
        {
            action();
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action) => true;
    }

    private class MockDispatcherTimer : IDispatcherTimer
    {
        public TimeSpan Interval { get; set; }
        public bool IsRunning { get; private set; }
        public bool IsRepeating { get; set; } = true;
        
        #pragma warning disable CS0067 // The event is never used
        public event EventHandler? Tick;
        #pragma warning restore CS0067

        public void Start() 
        {
            IsRunning = true;
        }

        public void Stop() 
        {
            IsRunning = false;
        }
    }

    [Fact]
    public void Logger_IsInjectedCorrectly()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new Mock<ILogger>();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup the mock factory to return a mock model
        var mockModel = new Mock<IGameModel>();
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        // Act
        var viewModel = new GameViewModel(mockDispatcher, mockLogger.Object, mockFactory.Object);
        
        // Assert
        mockLogger.Verify(l => l.Log("GameViewModel initialized"), Times.Once);
    }

    [Fact]
    public void CustomLogger_FormatsMessagesCorrectly()
    {
        // Arrange
        var logger = new CustomDebugLogger();
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        // Act
        logger.Log("Test message");
        logger.LogWarning("Test warning");
        logger.LogError("Test error");
        
        // Assert
        var output = consoleOutput.ToString();
        Assert.Contains("[CUSTOM-DEBUG] Test message", output);
        Assert.Contains("[CUSTOM-WARNING] Test warning", output);
        Assert.Contains("[CUSTOM-ERROR] Test error", output);
    }
}
