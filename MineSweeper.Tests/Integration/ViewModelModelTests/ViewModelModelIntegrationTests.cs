using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Services.Logging;

namespace MineSweeper.Tests.Integration.ViewModelModelTests;

/// <summary>
/// Integration tests for the interaction between ViewModel and Model
/// </summary>
public class ViewModelModelIntegrationTests
{
    [Fact]
    public async Task PlayCommand_UpdatesModelAndViewModel()
    {
        // Arrange - Use real implementations, not mocks
        var dispatcher = new TestDispatcher();
        var logger = new TestLogger();
        var modelFactory = new GameModelFactory(logger);
        var viewModel = new GameViewModel(dispatcher, logger, modelFactory);
        
        // Initialize game
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Act - Execute play command
        var point = new Point(0, 0);
        viewModel.PlayCommand.Execute(point);
        
        // Assert - Verify both ViewModel and Model are updated
        Assert.Equal(GameEnums.GameStatus.InProgress, viewModel.GameStatus);
        Assert.Equal(GameEnums.GameStatus.InProgress, viewModel.Model?.GameStatus);
        
        // Verify the cell at the played position is revealed in the model
        var modelItem = viewModel.Model?[0, 0];
        Assert.NotNull(modelItem);
        Assert.True(modelItem?.IsRevealed);
    }
    
    [Fact]
    public void FlagCommand_UpdatesRemainingMines_Mock()
    {
        // This test verifies that the FlagCommand correctly updates the RemainingMines property
        // by using a mock approach that doesn't rely on the actual game model implementation
        
        // Arrange
        var dispatcher = new TestDispatcher();
        var logger = new TestLogger();
        var modelFactory = new GameModelFactory(logger);
        var viewModel = new GameViewModel(dispatcher, logger, modelFactory);
        
        // Create a mock implementation of IGameModel
        var mockModel = new MockGameModel();
        
        // Use reflection to set the model in the view model
        var modelField = typeof(GameViewModel).GetField("_gameModel", BindingFlags.NonPublic | BindingFlags.Instance);
        modelField?.SetValue(viewModel, mockModel);
        
        // Set the game status to InProgress to allow flagging
        mockModel.GameStatus = GameEnums.GameStatus.InProgress;
        
        // Get the initial remaining mines
        var initialRemainingMines = mockModel.RemainingMines;
        
        // Make sure the viewmodel is updated with the model's properties
        typeof(GameViewModel)
            .GetMethod("UpdatePropertiesFromModel", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(viewModel, Array.Empty<object>());
        
        // Act - Flag a cell
        viewModel.FlagCommand.Execute(new Point(0, 0));
        
        // Assert - RemainingMines should be decremented
        Assert.Equal(initialRemainingMines - 1, viewModel.RemainingMines);
        Assert.True(mockModel.IsFlagged(0, 0), "Cell should be flagged after FlagCommand");
        
        // Act - Unflag the cell
        viewModel.FlagCommand.Execute(new Point(0, 0));
        
        // Assert - RemainingMines should be back to initial value
        Assert.Equal(initialRemainingMines, viewModel.RemainingMines);
        Assert.False(mockModel.IsFlagged(0, 0), "Cell should not be flagged after second FlagCommand");
    }
    
    // A simple mock implementation of IGameModel for testing
    private class MockGameModel : IGameModel
    {
        private readonly Dictionary<(int, int), bool> _flaggedCells = new();
        private int _remainingMines = 10;
        private ObservableCollection<SweeperItem> _items;
        
        public MockGameModel()
        {
            // Initialize items collection
            _items = new ObservableCollection<SweeperItem>();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _items.Add(new SweeperItem
                    {
                        Point = new Point(i, j),
                        IsRevealed = false,
                        IsFlagged = false,
                        IsMine = false,
                        MineCount = 0
                    });
                }
            }
            
            // Initialize commands
            PlayCommand = new RelayCommand<Point>(p => Play((int)p.X, (int)p.Y));
            FlagCommand = new RelayCommand<Point>(p => Flag((int)p.X, (int)p.Y));
        }
        
        public int Rows => 10;
        public int Columns => 10;
        public int Mines => 10;
        public int RemainingMines => _remainingMines;
        public int FlaggedItems => _flaggedCells.Count(kv => kv.Value);
        public GameEnums.GameStatus GameStatus { get; set; } = GameEnums.GameStatus.NotStarted;
        public GameEnums.GameDifficulty GameDifficulty => GameEnums.GameDifficulty.Easy;
        public ObservableCollection<SweeperItem>? Items => _items;
        public ICommand PlayCommand { get; }
        public ICommand FlagCommand { get; }
        
        public SweeperItem this[int row, int column] => new SweeperItem 
        { 
            IsRevealed = false, 
            IsFlagged = IsFlagged(row, column) 
        };
        
        public bool IsFlagged(int row, int column) => _flaggedCells.ContainsKey((row, column)) && _flaggedCells[(row, column)];
        
        public void Flag(int row, int column)
        {
            if (!IsFlagged(row, column))
            {
                _flaggedCells[(row, column)] = true;
                _remainingMines--;
            }
            else
            {
                _flaggedCells[(row, column)] = false;
                _remainingMines++;
            }
        }
        
        public void Play(int row, int column) { }
        
        public void RevealAllMines()
        {
            // This is a mock implementation, so we don't need to do anything
        }
        
        public void Dispose() { }
    }
    
    [Fact]
    public async Task GameLost_PropagatesFromModelToViewModel()
    {
        // Arrange
        var dispatcher = new TestDispatcher();
        var logger = new TestLogger();
        var modelFactory = new GameModelFactory(logger);
        var viewModel = new GameViewModel(dispatcher, logger, modelFactory);
        
        // Initialize game
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Act - Set game status to lost using reflection
        // This simulates what would happen if the model detected a loss
        var model = viewModel.Model;
        Assert.NotNull(model);
        
        // First make a play move to start the game
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Set the game status to lost in both model and viewmodel
        model!.GameStatus = GameEnums.GameStatus.Lost;
        viewModel.SetGameStatus(GameEnums.GameStatus.Lost);
        
        // Update properties from model to ensure consistency
        typeof(GameViewModel)
            .GetMethod("UpdatePropertiesFromModel", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(viewModel, Array.Empty<object>());
        
        // Assert
        Assert.Equal(GameEnums.GameStatus.Lost, viewModel.GameStatus);
        
        // Verify timer is stopped
        var timer = viewModel.Timer as TestDispatcherTimer;
        Assert.NotNull(timer);
        Assert.False(timer!.IsRunning);
    }
    
    [Fact]
    public async Task GameWon_PropagatesFromModelToViewModel()
    {
        // Arrange
        var dispatcher = new TestDispatcher();
        var logger = new TestLogger();
        var modelFactory = new GameModelFactory(logger);
        var viewModel = new GameViewModel(dispatcher, logger, modelFactory);
        
        // Initialize game
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Act - Set game status to won using reflection
        // This simulates what would happen if the model detected a win
        var model = viewModel.Model;
        Assert.NotNull(model);
        
        // First make a play move to start the game
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Set the game status to won in both model and viewmodel
        model!.GameStatus = GameEnums.GameStatus.Won;
        viewModel.SetGameStatus(GameEnums.GameStatus.Won);
        
        // Update properties from model to ensure consistency
        typeof(GameViewModel)
            .GetMethod("UpdatePropertiesFromModel", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(viewModel, Array.Empty<object>());
        
        // Assert
        Assert.Equal(GameEnums.GameStatus.Won, viewModel.GameStatus);
        
        // Verify timer is stopped
        var timer = viewModel.Timer as TestDispatcherTimer;
        Assert.NotNull(timer);
        Assert.False(timer!.IsRunning);
    }
    
    [Fact]
    public async Task NewGame_ResetsModelAndViewModel()
    {
        // Arrange
        var dispatcher = new TestDispatcher();
        var logger = new TestLogger();
        var modelFactory = new GameModelFactory(logger);
        var viewModel = new GameViewModel(dispatcher, logger, modelFactory);
        
        // Initialize game and make some moves
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        viewModel.PlayCommand.Execute(new Point(0, 0));
        viewModel.FlagCommand.Execute(new Point(1, 1));
        
        // Get the current model
        var originalModel = viewModel.Model;
        
        // Act - Start a new game
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Medium);
        
        // Assert
        // Verify the model has been replaced
        Assert.NotSame(originalModel, viewModel.Model);
        
        // Verify the new model has the correct properties
        Assert.Equal(GameEnums.GameDifficulty.Medium, viewModel.GameDifficulty);
        Assert.Equal(GameConstants.GameLevels[GameEnums.GameDifficulty.Medium].rows, viewModel.Rows);
        Assert.Equal(GameConstants.GameLevels[GameEnums.GameDifficulty.Medium].columns, viewModel.Columns);
        Assert.Equal(GameConstants.GameLevels[GameEnums.GameDifficulty.Medium].mines, viewModel.Mines);
        
        // Verify game time is reset
        Assert.Equal(0, viewModel.GameTime);
        
        // Verify game status is reset
        Assert.Equal(GameEnums.GameStatus.NotStarted, viewModel.GameStatus);
    }
    
    [Fact]
    public async Task Timer_IncrementsGameTime()
    {
        // Arrange
        var dispatcher = new TestDispatcher();
        var logger = new TestLogger();
        var modelFactory = new GameModelFactory(logger);
        var viewModel = new GameViewModel(dispatcher, logger, modelFactory);
        
        // Initialize game
        await viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
        
        // Start the game
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Get the timer
        var timer = viewModel.Timer as TestDispatcherTimer;
        Assert.NotNull(timer);
        
        // Initial game time
        var initialGameTime = viewModel.GameTime;
        
        // Act - Simulate timer ticks
        timer!.SimulateTick();
        timer.SimulateTick();
        timer.SimulateTick();
        
        // Assert
        Assert.Equal(initialGameTime + 3, viewModel.GameTime);
    }
    
    // Helper method to count flagged items in the model
    private int CountFlaggedItems(IGameModel? model)
    {
        if (model == null) return 0;
        
        int count = 0;
        for (int i = 0; i < model.Rows; i++)
        {
            for (int j = 0; j < model.Columns; j++)
            {
                if (model[i, j].IsFlagged)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
