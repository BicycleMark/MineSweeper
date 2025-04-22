using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using MineSweeper.Features.Game.Models;
using MineSweeper.Services.Logging;
using ILogger = MineSweeper.Services.Logging.ILogger;

namespace MineSweeper.Features.Game.ViewModels;

/// <summary>
///     ViewModel for the Minesweeper game, implementing the MVVM pattern.
///     This implementation uses a pull-based approach for most property updates,
///     with event-driven updates for critical properties like GameStatus.
///     Properties are updated from the model using the UpdatePropertiesFromModel method,
///     which should be called after any operation that changes the model state.
/// </summary>
public partial class GameViewModel : ObservableObject, IGameViewModel, IDisposable
{
    private readonly ILogger? _logger;
    private readonly IDispatcher _dispatcher;
    private readonly IGameModelFactory _modelFactory;
    private bool _disposed;
    private IGameModel _gameModel;
    private IDispatcherTimer? _timer;

    /// <summary>
    ///     Initializes a new instance of the GameViewModel class
    /// </summary>
    /// <param name="dispatcher">The dispatcher for UI thread operations</param>
    /// <param name="logger">Logger for debugging and error reporting</param>
    /// <param name="modelFactory">The factory for creating game models</param>
    public GameViewModel(
        IDispatcher dispatcher,
        ILogger logger,
        IGameModelFactory modelFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
        _gameModel = _modelFactory.CreateModel(GameEnums.GameDifficulty.Easy);

        // Initialize timer
        InitializeTimer();

        _logger.Log("GameViewModel initialized");
    }

    // ICommand properties for IGameViewModel interface

    IAsyncRelayCommand IGameViewModel.NewGameAsyncCommand => NewGameCommand;
    ICommand IGameViewModel.PlayCommand => PlayCommand;
    ICommand IGameViewModel.FlagCommand => FlagCommand;

    #region Properties

    /// <summary>
    ///     Gets or sets the collection of sweeper items (cells) in the game grid
    /// </summary>
    [ObservableProperty] private ObservableCollection<SweeperItem>? _items;

    /// <summary>
    ///     Gets or sets the number of rows in the game grid
    /// </summary>
    [ObservableProperty] private int _rows;

    /// <summary>
    ///     Gets or sets the number of columns in the game grid
    /// </summary>
    [ObservableProperty] private int _columns;

    /// <summary>
    ///     Gets or sets the total number of mines in the game
    /// </summary>
    [ObservableProperty] private int _mines;

    /// <summary>
    ///     Gets or sets the number of mines remaining to be flagged
    /// </summary>
    [ObservableProperty] private int _remainingMines;

    /// <summary>
    ///     Gets or sets the elapsed game time in seconds
    /// </summary>
    [ObservableProperty] private int _gameTime;

    /// <summary>
    ///     Gets or sets the current game status
    /// </summary>
    [ObservableProperty] private GameEnums.GameStatus _gameStatus;

    /// <summary>
    ///     Gets or sets the current game difficulty level
    /// </summary>
    [ObservableProperty] private GameEnums.GameDifficulty _gameDifficulty = GameEnums.GameDifficulty.Easy;

    /// <summary>
    ///     Gets or sets whether the game is currently loading
    /// </summary>
    [ObservableProperty] private bool _isLoading;

    /// <summary>
    ///     Gets the underlying game model (for testing purposes).
    ///     Returns null if the ViewModel has been disposed.
    /// </summary>
    public IGameModel? Model => _disposed ? null : _gameModel;

    /// <summary>
    ///     Gets the game timer (for testing purposes).
    ///     Returns null if the ViewModel has been disposed.
    /// </summary>
    public IDispatcherTimer? Timer => _disposed ? null : _timer;

#if DEBUG
    /// <summary>
    ///     Sets the game status directly (for testing purposes)
    /// </summary>
    /// <param name="status">The game status to set</param>
    public void SetGameStatus(GameEnums.GameStatus status)
    {
        if (_disposed)
        {
            _logger?.LogWarning("Cannot set game status: model is null or disposed");
            return;
        }

        if (_gameModel != null)
        {
            _gameModel.GameStatus = status;
        }
        GameStatus = status;
    }

    /// <summary>
    ///     Invokes the CheckGameStatus method directly (for testing purposes)
    /// </summary>
    public void InvokeCheckGameStatus()
    {
        if (_disposed)
        {
            _logger?.LogWarning("Cannot invoke check game status: disposed");
            return;
        }

        OnGameStatusChanged(GameStatus);
    }
#endif

    #endregion

    #region Commands

    /// <summary>
    ///     Gets or sets whether the grid is fully loaded
    /// </summary>
    [ObservableProperty] private bool _isGridFullyLoaded;

    /// <summary>
    ///     Gets or sets the loading progress (0-100)
    /// </summary>
    [ObservableProperty] private int _loadingProgress;

    /// <summary>
    ///     Creates a new game with the specified difficulty
    /// </summary>
    /// <param name="difficultyParam">Difficulty parameter (can be GameDifficulty enum or string)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    [RelayCommand]
    private async Task NewGame(object? difficultyParam)
    {
        if (_disposed)
        {
            _logger?.LogWarning("Attempted to create new game after disposal");
            return;
        }

        // Parse the difficulty parameter
        var difficulty = ParseDifficultyParameter(difficultyParam);

        _logger?.Log($"Starting new game with difficulty: {difficulty}");

        // Set loading state
        IsLoading = true;
        IsGridFullyLoaded = false;
        LoadingProgress = 0;
        var totalStopwatch = Stopwatch.StartNew();
        var phaseStopwatch = new Stopwatch();

        try
        {
            // Stop timer if running
            _timer?.Stop();

            // Create model on a background thread
            IGameModel? newModel = null;

            _logger?.Log("Creating game model on background thread");

            // Phase 1: Model Creation
            phaseStopwatch.Restart();
            // Use Task.Run to offload the model creation to a background thread
            await Task.Run(() =>
            {
                // Create the model
                newModel = _modelFactory.CreateModel(difficulty);
            });
            phaseStopwatch.Stop();
            _logger?.Log($"Phase 1 - Model Creation: {phaseStopwatch.ElapsedMilliseconds}ms");

            _logger?.Log("Game model created, updating UI properties");

            // Phase 2: Store model and prepare for UI update
            phaseStopwatch.Restart();
            // Store the new model
            if (newModel == null)
            {
                _logger?.LogError("Failed to create game model");
                return;
            }

            _gameModel = newModel;
            phaseStopwatch.Stop();
            _logger?.Log($"Phase 2 - Store Model: {phaseStopwatch.ElapsedMilliseconds}ms");

            // Phase 3: Update basic UI properties
            phaseStopwatch.Restart();
            // Update basic properties first
            GameDifficulty = difficulty;
            
            // Update properties from the model (with null check)
            if (_gameModel != null)
            {
                Rows = _gameModel.Rows;
                Columns = _gameModel.Columns;
                Mines = _gameModel.Mines;
                RemainingMines = _gameModel.Mines - _gameModel.FlaggedItems;
                GameStatus = _gameModel.GameStatus;
                GameTime = 0;

                // Set the Items directly from the model
                Items = _gameModel.Items;
            }

            phaseStopwatch.Stop();
            _logger?.Log($"Phase 3 - Update Basic UI Properties: {phaseStopwatch.ElapsedMilliseconds}ms");

            // Phase 4: Simulate progressive loading for UI feedback
            phaseStopwatch.Restart();

            // Keep loading indicator visible during progressive loading
            // but make it semi-transparent to allow interaction

            // Simulate loading progress
            await SimulateProgressiveLoading(Items?.Count ?? 0);

            phaseStopwatch.Stop();
            _logger?.Log($"Phase 4 - Progressive Item Loading: {phaseStopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error creating new game: {ex}");
        }
        finally
        {
            // Clear loading state
            IsLoading = false;
            IsGridFullyLoaded = true;
            LoadingProgress = 100;
            totalStopwatch.Stop();
            var elapsedMs = totalStopwatch.ElapsedMilliseconds;
            _logger?.Log($"Game Creation Time was {elapsedMs}ms ({elapsedMs / 1000.0:F2} seconds)");
        }
    }

    /// <summary>
    ///     Simulates progressive loading for UI feedback
    /// </summary>
    /// <param name="totalItems">The total number of items</param>
    /// <param name="progressIncrement">The increment to use for progress updates (default: 10)</param>
    /// <param name="delayMs">Delay in milliseconds between progress updates (default: 1)</param>
    private async Task SimulateProgressiveLoading(int totalItems, int progressIncrement = 10, int delayMs = 1)
    {
        if (totalItems == 0)
        {
            _logger?.LogWarning("No items to simulate loading for");
            return;
        }

        // Validate parameters
        progressIncrement = Math.Clamp(progressIncrement, 1, 50); // Ensure reasonable range
        delayMs = Math.Clamp(delayMs, 1, 100); // Prevent excessive delays

        _logger?.Log($"Starting simulated progressive loading of {totalItems} items " +
                     $"with increment {progressIncrement} and delay {delayMs}ms");

        // Simulate loading progress in steps
        for (var progress = 0; progress <= 100; progress += progressIncrement)
        {
            LoadingProgress = progress;

            // Allow UI to update by yielding to the UI thread
            await Task.Delay(delayMs);
        }

        // Ensure we end at 100%
        if (LoadingProgress < 100)
        {
            LoadingProgress = 100;
        }

        _logger?.Log("Completed simulated progressive loading");
    }

    /// <summary>
    ///     Parses the difficulty parameter from various input types
    /// </summary>
    /// <param name="difficultyParam">The difficulty parameter to parse</param>
    /// <returns>The parsed game difficulty</returns>
    private GameEnums.GameDifficulty ParseDifficultyParameter(object? difficultyParam)
    {
        return difficultyParam switch
        {
            GameEnums.GameDifficulty enumValue => enumValue,
            string stringValue when int.TryParse(stringValue, out var intValue)
                                    && Enum.IsDefined(typeof(GameEnums.GameDifficulty), intValue)
                => (GameEnums.GameDifficulty) intValue,
            _ => GameEnums.GameDifficulty.Easy // default to Easy if parameter is invalid
        };
    }

    /// <summary>
    ///     Plays (reveals) a cell at the specified position
    /// </summary>
    /// <param name="point">The position to play</param>
    [RelayCommand]
    private void Play(Point point)
    {
        _logger?.Log($"Play called with point: {point}");
        if (_disposed)
        {
            _logger?.LogWarning("Attempted to play after disposal");
            return;
        }

        // Handle first move if needed
        HandleFirstMoveIfNeeded();

        // Execute play command on model
        if (_gameModel != null)
        {
            _gameModel.PlayCommand.Execute(point);
        }

        // Update properties from model
        UpdatePropertiesFromModel();

        // Game status changes will be handled by OnGameStatusChanged
    }

    /// <summary>
    ///     Flags a cell at the specified position
    /// </summary>
    /// <param name="point">The position to flag</param>
    [RelayCommand(CanExecute = nameof(CanFlag))]
    private void Flag(Point point)
    {
        if (_disposed)
        {
            _logger?.LogWarning("Attempted to flag after disposal");
            return;
        }

        _logger?.Log($"Flag method called at {point}");

        // Execute flag command on model
        if (_gameModel != null)
        {
            _gameModel.FlagCommand.Execute(point);
        }

        // Update properties from model
        UpdatePropertiesFromModel();
    }

    /// <summary>
    ///     Determines whether the Flag command can be executed
    /// </summary>
    /// <param name="point">The position to flag</param>
    /// <returns>True if the command can be executed, false otherwise</returns>
    private bool CanFlag(Point point)
    {
        if (_disposed || _gameModel == null)
            return false;

        // Only allow flagging when game is in progress (not before first move)
        return _gameModel.GameStatus != GameEnums.GameStatus.NotStarted;
    }

    /// <summary>
    ///     Handles the first move logic if the game hasn't started yet
    /// </summary>
    private void HandleFirstMoveIfNeeded()
    {
        if (_disposed)
        {
            _logger?.LogWarning("Attempted to handle first move after disposal");
            return;
        }

        if (_gameModel == null || _gameModel.GameStatus != GameEnums.GameStatus.NotStarted)
            return;

        _logger?.Log("First move detected, starting timer");

        // Start timer
        _timer?.Start();

        // Set game status to in progress
        if (_gameModel != null)
        {
            _gameModel.GameStatus = GameEnums.GameStatus.InProgress;
            GameStatus = GameEnums.GameStatus.InProgress;
        }
    }

    /// <summary>
    ///     Ensures the game stays in progress state (used after first flag move)
    /// </summary>
    private void EnsureGameInProgress()
    {
        if (_disposed)
        {
            _logger?.LogWarning("Attempted to ensure game in progress after disposal");
            return;
        }

        if (GameStatus == GameEnums.GameStatus.InProgress && _timer?.IsRunning == true)
            return;

        _logger?.LogWarning("Ensuring game stays in progress state");

        // Reset game status if needed
        if (GameStatus != GameEnums.GameStatus.InProgress && _gameModel != null)
        {
            _gameModel.GameStatus = GameEnums.GameStatus.InProgress;
            GameStatus = GameEnums.GameStatus.InProgress;
        }

        // Ensure timer is running
        if (_timer?.IsRunning == false) _timer.Start();
    }

    /// <summary>
    ///     Updates the ViewModel properties from the Model.
    ///     This method should be called after any operation that changes the Model state.
    /// </summary>
    private void UpdatePropertiesFromModel()
    {
        if (_disposed)
        {
            _logger?.LogWarning("Attempted to update properties after disposal");
            return;
        }

        if (_gameModel == null)
        {
            _logger?.LogWarning("Cannot update properties: game model is null");
            return;
        }

        // Update basic properties
        Items = _gameModel.Items;
        Rows = _gameModel.Rows;
        Columns = _gameModel.Columns;
        Mines = _gameModel.Mines;

        // Calculate remaining mines
        RemainingMines = _gameModel.Mines - _gameModel.FlaggedItems;

        // Update game status
        GameStatus = _gameModel.GameStatus;

        _logger?.Log($"Properties updated: Status={GameStatus}, RemainingMines={RemainingMines}");
    }

    /// <summary>
    ///     Called when the GameStatus property changes.
    ///     This is an example of the event-driven part of our hybrid approach,
    ///     where critical state changes trigger automatic reactions.
    /// </summary>
    /// <param name="value">The new game status value</param>
    partial void OnGameStatusChanged(GameEnums.GameStatus value)
    {
        if (_disposed)
        {
            _logger?.LogWarning("GameStatus changed after disposal");
            return;
        }

        // Notify that CanFlag may have changed
        FlagCommand.NotifyCanExecuteChanged();

        // Only take action if game is over
        if (value != GameEnums.GameStatus.Won &&
            value != GameEnums.GameStatus.Lost)
            return;

        _logger?.Log($"Game over with status: {value}");

        // Stop timer
        _timer?.Stop();

        // Reveal all mines if game is lost
        if (value == GameEnums.GameStatus.Lost) RevealAllMines();
    }

    /// <summary>
    ///     Reveals all mines on the board when the game is lost
    /// </summary>
    private void RevealAllMines()
    {
        if (_disposed)
        {
            _logger?.LogWarning("Attempted to reveal mines after disposal");
            return;
        }

        _logger?.Log("Requesting model to reveal all mines");

        // Use the model's implementation to reveal all mines
        // This follows MVVM pattern by delegating model operations to the model
        if (_gameModel != null)
        {
            _gameModel.RevealAllMines();
        }
    }

    /// <summary>
    ///     Initializes the game timer
    /// </summary>
    private void InitializeTimer()
    {
        _timer = _dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) =>
        {
            if (!_disposed) GameTime++;
        };
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    ///     Disposes resources used by the ViewModel
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Disposes resources used by the ViewModel
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources
            _timer?.Stop();
            _timer = null;
        }

        _disposed = true;
    }

    /// <summary>
    ///     Finalizer
    /// </summary>
    ~GameViewModel()
    {
        Dispose(false);
    }

    #endregion
}
