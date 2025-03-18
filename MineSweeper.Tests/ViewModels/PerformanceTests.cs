using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Maui.Dispatching;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using Moq;
using Xunit;

namespace MineSweeper.Tests.ViewModels;

public class PerformanceTests
{
    private readonly Mock<IDispatcher> _mockDispatcher;
    private readonly Mock<IDispatcherTimer> _mockTimer;
    private readonly Mock<ILogger> _mockLogger;
    private readonly GameModelFactory _modelFactory;
    private readonly GameViewModel _viewModel;

    public PerformanceTests()
    {
        _mockDispatcher = new Mock<IDispatcher>();
        _mockTimer = new Mock<IDispatcherTimer>();
        _mockLogger = new Mock<ILogger>();
        _modelFactory = new GameModelFactory(_mockLogger.Object);

        _mockDispatcher.Setup(d => d.CreateTimer()).Returns(_mockTimer.Object);

        _viewModel = new GameViewModel(
            _mockDispatcher.Object,
            _mockLogger.Object,
            _modelFactory);
    }

    // Counter for property change notifications
    private int _propertyChangeCount;
    
    // Handler for property change events
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _propertyChangeCount++;
    }
    
    [Theory]
    [InlineData("Easy", 200)]    // Easy should be very fast
    [InlineData("Medium", 500)]  // Medium should be reasonably fast
    [InlineData("Hard", 1000)]   // Hard is the most demanding but still should be under 1 second
    public async Task NewGame_ShouldCreateGameWithinTimeLimit(string difficultyName, int maxAllowedTimeMs)
    {
        // Arrange
        var difficulty = Enum.Parse<GameEnums.GameDifficulty>(difficultyName);
        
        // Create a more complex test that measures actual model creation time
        var testStopwatch = new Stopwatch();
        var totalElapsedMs = 0L;
        var totalPropertyChanges = 0;
        
        // Run the test multiple times to get a more accurate measurement
        const int iterations = 5;
        for (int i = 0; i < iterations; i++)
        {
            // Force GC collection before each run to minimize interference
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            // Reset property change counter
            _propertyChangeCount = 0;
            
            // Subscribe to property change notifications
            _viewModel.PropertyChanged += OnPropertyChanged;
            
            // If Items collection exists, subscribe to its collection changed events
            if (_viewModel.Items != null)
            {
                _viewModel.Items.CollectionChanged += (s, e) => _propertyChangeCount++;
            }
            
            // Act
            testStopwatch.Restart();
            await _viewModel.NewGameCommand.ExecuteAsync(difficulty);
            testStopwatch.Stop();
            
            // Unsubscribe from property change notifications
            _viewModel.PropertyChanged -= OnPropertyChanged;
            
            // If Items collection exists, unsubscribe from its collection changed events
            if (_viewModel.Items != null)
            {
                _viewModel.Items.CollectionChanged -= (s, e) => _propertyChangeCount++;
            }
            
            totalElapsedMs += testStopwatch.ElapsedMilliseconds;
            totalPropertyChanges += _propertyChangeCount;
        }
        
        // Calculate average time and property changes
        var averageElapsedMs = totalElapsedMs / iterations;
        var averagePropertyChanges = totalPropertyChanges / iterations;
        
        // Output for debugging
        Console.WriteLine($"Creating {difficultyName} game took an average of {averageElapsedMs}ms over {iterations} iterations");
        Console.WriteLine($"Total time: {totalElapsedMs}ms");
        Console.WriteLine($"Average property change notifications: {averagePropertyChanges}");
        
        // Verify performance is within acceptable limits
        Assert.True(averageElapsedMs <= maxAllowedTimeMs, 
            $"{difficultyName} game creation took {averageElapsedMs}ms on average, which exceeds the limit of {maxAllowedTimeMs}ms");
        
        // Verify the game was created with the correct properties
        Assert.Equal(difficulty, _viewModel.GameDifficulty);
        Assert.NotNull(_viewModel.Items);
        
        // Get expected dimensions based on difficulty
        var (expectedRows, expectedColumns, expectedMines) = GameConstants.GameLevels[difficulty];
        Assert.Equal(expectedRows, _viewModel.Rows);
        Assert.Equal(expectedColumns, _viewModel.Columns);
        Assert.Equal(expectedMines, _viewModel.Mines);
    }
    
    [Fact]
    public async Task Flag_ShouldNotBeAllowed_BeforeFirstMove()
    {
        // Arrange
        await _viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        var point = new Point(0, 0);
        
        // Act & Assert
        // Check if CanExecute returns false for Flag command before first move
        var canFlag = _viewModel.FlagCommand.CanExecute(point);
        Assert.False(canFlag, "Flag command should not be executable before first move");
        
        // Verify game status is still NotStarted
        Assert.Equal(GameEnums.GameStatus.NotStarted, _viewModel.GameStatus);
    }
    
    [Fact]
    public async Task Flag_ShouldBeAllowed_AfterFirstMove()
    {
        // Arrange
        await _viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Act - Make first move to start the game
        _viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Assert
        // Verify game status changed to InProgress
        Assert.Equal(GameEnums.GameStatus.InProgress, _viewModel.GameStatus);
        
        // Check if CanExecute returns true for Flag command after first move
        var canFlag = _viewModel.FlagCommand.CanExecute(new Point(1, 1));
        Assert.True(canFlag, "Flag command should be executable after first move");
    }
}
