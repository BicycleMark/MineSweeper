using System.Windows.Input;
using Microsoft.Maui.Dispatching;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using System.Diagnostics;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Moq;

namespace MineSweeper.Tests.ViewModels;

public class GameViewModelTests
{
    // Mock classes for testing
    private class MockDispatcherTimer : IDispatcherTimer
    {
        public TimeSpan Interval { get; set; }
        public bool IsRunning { get; private set; }
        public bool IsRepeating { get; set; } = true;
        public event EventHandler? Tick;

        public void Start() 
        {
            IsRunning = true;
        }

        public void Stop() 
        {
            IsRunning = false;
        }

        // Method to simulate timer ticks for testing
        public void SimulateTick()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }
    }

    private class MockLogger : ILogger
    {
        public List<string> LogMessages { get; } = new List<string>();
        public List<string> ErrorMessages { get; } = new List<string>();
        public List<string> WarningMessages { get; } = new List<string>();

        public void Log(string message)
        {
            LogMessages.Add(message);
            Debug.WriteLine($"[INFO] {message}");
        }

        public void LogError(string message)
        {
            ErrorMessages.Add(message);
            Debug.WriteLine($"[ERROR] {message}");
        }

        public void LogWarning(string message)
        {
            WarningMessages.Add(message);
            Debug.WriteLine($"[WARNING] {message}");
        }
    }

    private class MockDispatcher : IDispatcher
    {
        private readonly MockDispatcherTimer _timer = new();
        public int CreateTimerCallCount { get; private set; }
        public bool IsDispatchRequired => false;

        public IDispatcherTimer CreateTimer() 
        {
            CreateTimerCallCount++;
            return _timer;
        }

        public bool Dispatch(Action action)
        {
            action();
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action) => true;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_InitializesPropertiesAndCommands()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        // Act
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Assert
        Assert.NotNull(viewModel.NewGameCommand);
        Assert.NotNull(viewModel.PlayCommand);
        Assert.NotNull(viewModel.FlagCommand);
        Assert.Equal(GameEnums.GameDifficulty.Easy, viewModel.GameDifficulty);
        Assert.Equal(GameEnums.GameStatus.NotStarted, viewModel.GameStatus);
        
        // Verify timer was initialized
        Assert.Equal(1, mockDispatcher.CreateTimerCallCount);
    }

    #endregion

    #region NewGame Command Tests

    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy, 10, 10, 10)]
    [InlineData(GameEnums.GameDifficulty.Medium, 15, 15, 40)]
    [InlineData(GameEnums.GameDifficulty.Hard, 20, 20, 80)]
    public void NewGame_WithDifficulty_SetsCorrectProperties(
        GameEnums.GameDifficulty difficulty, 
        int expectedRows, 
        int expectedColumns, 
        int expectedMines)
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(expectedRows);
        mockModel.Setup(m => m.Columns).Returns(expectedColumns);
        mockModel.Setup(m => m.Mines).Returns(expectedMines);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(expectedMines);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(difficulty))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Act
        viewModel.NewGameCommand.Execute(difficulty);
        
        // Assert
        Assert.Equal(difficulty, viewModel.GameDifficulty);
        Assert.Equal(expectedRows, viewModel.Rows);
        Assert.Equal(expectedColumns, viewModel.Columns);
        Assert.Equal(expectedMines, viewModel.Mines);
        Assert.Equal(expectedMines, viewModel.RemainingMines);
        Assert.Equal(GameEnums.GameStatus.NotStarted, viewModel.GameStatus);
    }

    [Theory]
    [InlineData("0", GameEnums.GameDifficulty.Easy)]
    [InlineData("1", GameEnums.GameDifficulty.Medium)]
    [InlineData("2", GameEnums.GameDifficulty.Hard)]
    public void NewGame_WithStringParameter_ParsesCorrectly(
        string difficultyString, 
        GameEnums.GameDifficulty expectedDifficulty)
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Act
        viewModel.NewGameCommand.Execute(difficultyString);
        
        // Assert
        Assert.Equal(expectedDifficulty, viewModel.GameDifficulty);
    }

    [Fact]
    public void NewGame_WithInvalidParameter_DefaultsToEasy()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(GameEnums.GameDifficulty.Easy))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Act
        viewModel.NewGameCommand.Execute("invalid");
        
        // Assert
        Assert.Equal(GameEnums.GameDifficulty.Easy, viewModel.GameDifficulty);
        Assert.Equal(10, viewModel.Rows);
        Assert.Equal(10, viewModel.Columns);
        Assert.Equal(10, viewModel.Mines);
    }

    [Fact]
    public void NewGame_ResetsGameTime()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Set initial game time
        var gameTimeProperty = typeof(GameViewModel).GetProperty("GameTime");
        gameTimeProperty?.SetValue(viewModel, 100);
        
        // Act
        viewModel.NewGameCommand.Execute(GameEnums.GameDifficulty.Easy);
        
        // Assert
        Assert.Equal(0, viewModel.GameTime);
    }

    #endregion

    #region Play Command Tests

    [Fact]
    public void Play_FirstMove_StartsTimer()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Get the timer
        var timer = viewModel.Timer as MockDispatcherTimer;
        
        // Verify timer is not running before the play operation
        Assert.NotNull(timer);
        Assert.False(timer!.IsRunning, "Timer should not be running before first move");
        
        // Act
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Assert
        Assert.NotNull(timer);
        Assert.True(timer!.IsRunning, "Timer should be running after first play move");
    }

    [Fact]
    public void Play_UpdatesPropertiesFromModel()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Create a collection of 100 SweeperItems (10x10 grid)
        var items = new ObservableCollection<SweeperItem>();
        for (int i = 0; i < 100; i++)
        {
            items.Add(new SweeperItem());
        }
        
        // Setup mock model with a game status that can be changed
        var gameStatus = GameEnums.GameStatus.NotStarted;
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(() => gameStatus);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { 
            // Update game status to InProgress when Play is called
            gameStatus = GameEnums.GameStatus.InProgress;
        }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(items);
        
        // Allow setting the GameStatus property
        mockModel.SetupSet(m => m.GameStatus = It.IsAny<GameEnums.GameStatus>())
            .Callback<GameEnums.GameStatus>(status => gameStatus = status);
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Act
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Assert
        Assert.Equal(GameEnums.GameStatus.InProgress, viewModel.GameStatus);
        Assert.NotNull(viewModel.Items);
        Assert.Equal(100, viewModel.Items!.Count); // 10x10 grid for Easy difficulty
    }

    #endregion

    #region Flag Command Tests

    [Fact]
    public void Flag_FirstMove_StartsTimer()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Get the timer
        var timer = viewModel.Timer as MockDispatcherTimer;
        
        // Verify timer is not running before the flag operation
        Assert.NotNull(timer);
        Assert.False(timer!.IsRunning, "Timer should not be running before first move");
        
        // Act
        viewModel.FlagCommand.Execute(new Point(0, 0));
        
        // Debug output
        Console.WriteLine("--- Log Messages ---");
        foreach (var message in mockLogger.LogMessages)
        {
            Console.WriteLine($"LOG: {message}");
        }
        Console.WriteLine("--- Warning Messages ---");
        foreach (var message in mockLogger.WarningMessages)
        {
            Console.WriteLine($"WARNING: {message}");
        }
        Console.WriteLine("--- Error Messages ---");
        foreach (var message in mockLogger.ErrorMessages)
        {
            Console.WriteLine($"ERROR: {message}");
        }
        
        // Assert
        Assert.NotNull(timer);
        Assert.True(timer!.IsRunning, "Timer should be running after first flag move");
    }

    [Fact]
    public void Flag_UpdatesRemainingMines()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        
        // Setup flagged items to change when flag command is executed
        var flaggedItems = 0;
        mockModel.Setup(m => m.FlaggedItems).Returns(() => flaggedItems);
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => flaggedItems++));
        
        mockModel.Setup(m => m.RemainingMines).Returns(() => 10 - flaggedItems);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.InProgress);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Initialize game with a play move to set up the board
        viewModel.PlayCommand.Execute(new Point(0, 0));
        var initialRemainingMines = viewModel.RemainingMines;
        
        Console.WriteLine($"Initial remaining mines: {initialRemainingMines}");
        
        // Act - flag a specific cell that's not at (0,0) since that's where we played
        var flagPoint = new Point(1, 1);
        
        // Log the state before flagging
        Console.WriteLine("--- State before flagging ---");
        Console.WriteLine($"RemainingMines: {viewModel.RemainingMines}");
        Console.WriteLine($"FlaggedItems in model: {GetModelFlaggedItems(viewModel)}");
        
        // Execute the flag command
        viewModel.FlagCommand.Execute(flagPoint);
        
        // Log the state after flagging
        Console.WriteLine("--- State after flagging ---");
        Console.WriteLine($"RemainingMines: {viewModel.RemainingMines}");
        Console.WriteLine($"FlaggedItems in model: {GetModelFlaggedItems(viewModel)}");
        
        // Debug output
        Console.WriteLine("--- Log Messages ---");
        foreach (var message in mockLogger.LogMessages)
        {
            Console.WriteLine($"LOG: {message}");
        }
        Console.WriteLine("--- Warning Messages ---");
        foreach (var message in mockLogger.WarningMessages)
        {
            Console.WriteLine($"WARNING: {message}");
        }
        Console.WriteLine("--- Error Messages ---");
        foreach (var message in mockLogger.ErrorMessages)
        {
            Console.WriteLine($"ERROR: {message}");
        }
        
        // Assert
        Assert.Equal(initialRemainingMines - 1, viewModel.RemainingMines);
    }
    
    // Helper method to get the FlaggedItems count from the model
    private int GetModelFlaggedItems(GameViewModel viewModel)
    {
        return viewModel.Model?.FlaggedItems ?? -1;
    }

    #endregion

    #region Timer Tests

    [Fact]
    public void Timer_Tick_IncrementsGameTime()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock factory with mock model
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(GameEnums.GameStatus.NotStarted);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Get the timer
        var timer = viewModel.Timer as MockDispatcherTimer;
        
        // Act - simulate timer tick
        timer?.SimulateTick();
        
        // Assert
        Assert.Equal(1, viewModel.GameTime);
    }

    [Fact]
    public void GameOver_StopsTimer()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Setup mock model with a game status that can be changed
        var gameStatus = GameEnums.GameStatus.NotStarted;
        var mockModel = new Mock<IGameModel>();
        
        // Setup mock model properties
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(() => gameStatus);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { 
            // Update game status to InProgress when Play is called
            gameStatus = GameEnums.GameStatus.InProgress;
        }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(new ObservableCollection<SweeperItem>());
        
        // Allow setting the GameStatus property
        mockModel.SetupSet(m => m.GameStatus = It.IsAny<GameEnums.GameStatus>())
            .Callback<GameEnums.GameStatus>(status => gameStatus = status);
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Initialize game
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        // Get the timer
        var timer = viewModel.Timer as MockDispatcherTimer;
        
        // Verify timer is running
        Assert.NotNull(timer);
        Assert.True(timer!.IsRunning, "Timer should be running after first move");
        
        // Act - set game status to lost
        viewModel.SetGameStatus(GameEnums.GameStatus.Lost);
        
        // Call CheckGameStatus method
        viewModel.InvokeCheckGameStatus();
        
        // Debug output
        Console.WriteLine("--- Log Messages ---");
        foreach (var message in mockLogger.LogMessages)
        {
            Console.WriteLine($"LOG: {message}");
        }
        
        // Assert
        Assert.NotNull(timer);
        Assert.False(timer!.IsRunning, "Timer should be stopped when game is lost");
    }

    #endregion

    #region Game Status Tests

    [Fact]
    public void GameLost_RevealsAllMines()
    {
        // Arrange
        var mockDispatcher = new MockDispatcher();
        var mockLogger = new MockLogger();
        var mockFactory = new Mock<IGameModelFactory>();
        
        // Create a collection of SweeperItems with one mine
        var items = new ObservableCollection<SweeperItem>();
        for (int i = 0; i < 100; i++)
        {
            items.Add(new SweeperItem());
        }
        // Set the first item as a mine
        items[0].IsMine = true;
        
        // Setup mock model with a game status that can be changed
        var gameStatus = GameEnums.GameStatus.InProgress;
        var mockModel = new Mock<IGameModel>();
        mockModel.Setup(m => m.Rows).Returns(10);
        mockModel.Setup(m => m.Columns).Returns(10);
        mockModel.Setup(m => m.Mines).Returns(10);
        mockModel.Setup(m => m.FlaggedItems).Returns(0);
        mockModel.Setup(m => m.RemainingMines).Returns(10);
        mockModel.Setup(m => m.GameStatus).Returns(() => gameStatus);
        mockModel.Setup(m => m.PlayCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.FlagCommand).Returns(new RelayCommand<Point>(p => { }));
        mockModel.Setup(m => m.Items).Returns(items);
        
        // Setup the RevealAllMines method to reveal all mines
        mockModel.Setup(m => m.RevealAllMines())
            .Callback(() => {
                foreach (var item in items.Where(i => i.IsMine))
                {
                    item.IsRevealed = true;
                    item.IsFlagged = false;
                }
            });
        
        // Allow setting the GameStatus property
        mockModel.SetupSet(m => m.GameStatus = It.IsAny<GameEnums.GameStatus>())
            .Callback<GameEnums.GameStatus>(status => gameStatus = status);
        
        mockFactory.Setup(f => f.CreateModel(It.IsAny<GameEnums.GameDifficulty>()))
            .Returns(mockModel.Object);
        
        var viewModel = new GameViewModel(mockDispatcher, mockLogger, mockFactory.Object);
        
        // Initialize game
        viewModel.PlayCommand.Execute(new Point(0, 0));
        
        Assert.NotNull(viewModel.Items);
        
        // Act - set game status to lost
        viewModel.SetGameStatus(GameEnums.GameStatus.Lost);
        
        // Call CheckGameStatus method
        viewModel.InvokeCheckGameStatus();
        
        // Assert
        Assert.True(items[0].IsRevealed, "Mine should be revealed when game is lost");
    }

    #endregion
}
