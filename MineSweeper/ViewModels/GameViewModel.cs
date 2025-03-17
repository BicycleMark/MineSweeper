using System.Collections.ObjectModel;
using System.Windows.Input;
using MineSweeper.Models;
using Microsoft.Maui.Dispatching;
using System.Diagnostics;

namespace MineSweeper.ViewModels;

public partial class GameViewModel : ObservableObject, IGameViewModel
{
    private readonly IDispatcher _dispatcher;
    private IDispatcherTimer? _timer;
    private readonly IGameModelFactory _modelFactory;
    private GameModel _gameModel;
    private readonly ILogger _logger;

    public GameViewModel(
        IDispatcher dispatcher, 
        ILogger? logger = null,
        IGameModelFactory? modelFactory = null)
    {
        _dispatcher = dispatcher;
        _logger = logger ?? new ConsoleLogger();
        _modelFactory = modelFactory ?? new GameModelFactory();
        _gameModel = (GameModel)_modelFactory.CreateModel(GameEnums.GameDifficulty.Easy, _logger);
        
        // Initialize commands
        NewGameCommand = new RelayCommand<object>(NewGame);
        PlayCommand = new RelayCommand<Point>(Play);
        FlagCommand = new RelayCommand<Point>(Flag);
        
        // Initialize timer
        InitializeTimer();
        
        _logger.Log("GameViewModel initialized");
    }

    #region Properties

    [ObservableProperty]
    private ObservableCollection<SweeperItem>? _items;

    [ObservableProperty]
    private int _rows;

    [ObservableProperty]
    private int _columns;

    [ObservableProperty]
    private int _mines;

    [ObservableProperty]
    private int _remainingMines;

    [ObservableProperty]
    private int _gameTime;

    [ObservableProperty]
    private GameEnums.GameStatus _gameStatus;

    [ObservableProperty]
    private GameEnums.GameDifficulty _gameDifficulty = GameEnums.GameDifficulty.Easy;
    
    // IGameViewModel interface implementation
    public IGameModel Model => _gameModel;
    public IDispatcherTimer? Timer => _timer;
    
    #if DEBUG
    public void SetGameStatus(GameEnums.GameStatus status)
    {
        _gameModel.GameStatus = status;
        GameStatus = status;
    }
    
    public void InvokeCheckGameStatus()
    {
        CheckGameStatus();
    }
    #endif

    #endregion

    #region Commands

    public ICommand NewGameCommand { get; }
    public ICommand PlayCommand { get; }
    public ICommand FlagCommand { get; }

    #endregion

    #region Methods

    private void NewGame(object difficultyParam)
    {
        GameEnums.GameDifficulty difficulty;
        
        // Parse the difficulty parameter
        if (difficultyParam is GameEnums.GameDifficulty enumValue)
        {
            difficulty = enumValue;
        }
        else if (difficultyParam is string stringValue && int.TryParse(stringValue, out var intValue))
        {
            // Convert integer to enum
            difficulty = (GameEnums.GameDifficulty)intValue;
        }
        else
        {
            // Default to Easy if parameter is invalid
            difficulty = GameEnums.GameDifficulty.Easy;
        }
        
        // Stop timer if running
        if (_timer != null)
        {
            _timer.Stop();
        }
        
        // Create new game model with selected difficulty using the factory
        _gameModel = (GameModel)_modelFactory.CreateModel(difficulty, _logger);
        GameDifficulty = difficulty;
        
        // Update properties from model
        UpdatePropertiesFromModel();
        
        // Reset timer
        GameTime = 0;
    }

    private void Play(Point point)
    {
        if (_gameModel.GameStatus == GameEnums.GameStatus.NotStarted)
        {
            // Start timer on first move
            if (_timer != null)
            {
                _timer.Start();
            }
            _gameModel.GameStatus = GameEnums.GameStatus.InProgress;
            GameStatus = GameEnums.GameStatus.InProgress;
        }
        
        // Execute play command on model
        _gameModel.PlayCommand.Execute(point);
        
        // Update properties from model
        UpdatePropertiesFromModel();
        
        // Check game status
        CheckGameStatus();
    }

    private void Flag(Point point)
    {
        bool isFirstMove = _gameModel.GameStatus == GameEnums.GameStatus.NotStarted;
        _logger.Log($"Flag method called. IsFirstMove: {isFirstMove}, Timer: {_timer != null}, Point: {point}");
        
        if (isFirstMove)
        {
            _logger.Log("First move detected, starting timer");
            // Start timer on first move
            if (_timer != null)
            {
                _timer.Start();
                _logger.Log($"Timer started. IsRunning: {_timer.IsRunning}");
            }
            else
            {
                _logger.LogError("Timer is null!");
            }
            
            // Set game status to in progress
            _gameModel.GameStatus = GameEnums.GameStatus.InProgress;
            GameStatus = GameEnums.GameStatus.InProgress;
            _logger.Log($"Game status set to: {GameStatus}");
        }
        
        // Execute flag command on model
        _logger.Log("Executing flag command on model");
        _gameModel.FlagCommand.Execute(point);
        
        // Update properties from model
        _logger.Log("Updating properties from model");
        UpdatePropertiesFromModel();
        _logger.Log($"After update, GameStatus: {GameStatus}");
        
        // For the first move, we need to ensure the game status stays in progress
        // This is because the model might set it to Won since no mines are placed yet
        if (isFirstMove)
        {
            _logger.Log("First move: Ensuring game status stays in progress");
            if (GameStatus != GameEnums.GameStatus.InProgress)
            {
                _logger.LogWarning($"Game status changed to {GameStatus} after first flag. Resetting to InProgress.");
                _gameModel.GameStatus = GameEnums.GameStatus.InProgress;
                GameStatus = GameEnums.GameStatus.InProgress;
            }
            
            // Ensure timer is still running
            if (_timer != null && !_timer.IsRunning)
            {
                _logger.LogWarning("Timer stopped after model update! Restarting timer.");
                _timer.Start();
            }
        }
        
        // Only check game status if it's not the first move
        if (!isFirstMove)
        {
            _logger.Log("Not first move, checking game status");
            CheckGameStatus();
        }
        
        _logger.Log($"Flag method completed. Timer running: {_timer?.IsRunning}, GameStatus: {GameStatus}");
    }

    private void UpdatePropertiesFromModel()
    {
        _logger.Log("UpdatePropertiesFromModel called");
        
        Items = _gameModel.Items;
        Rows = _gameModel.Rows;
        Columns = _gameModel.Columns;
        
        int oldMines = Mines;
        Mines = _gameModel.Mines;
        if (oldMines != Mines)
        {
            _logger.Log($"Mines updated from {oldMines} to {Mines}");
        }
        
        int oldRemainingMines = RemainingMines;
        RemainingMines = _gameModel.Mines - _gameModel.FlaggedItems;
        _logger.Log($"RemainingMines calculation: {_gameModel.Mines} - {_gameModel.FlaggedItems} = {RemainingMines}");
        if (oldRemainingMines != RemainingMines)
        {
            _logger.Log($"RemainingMines updated from {oldRemainingMines} to {RemainingMines}");
        }
        
        GameEnums.GameStatus oldStatus = GameStatus;
        GameStatus = _gameModel.GameStatus;
        if (oldStatus != GameStatus)
        {
            _logger.Log($"GameStatus updated from {oldStatus} to {GameStatus}");
        }
        
        _logger.Log("UpdatePropertiesFromModel completed");
    }

    private void CheckGameStatus()
    {
        _logger.Log($"CheckGameStatus called. GameStatus: {_gameModel.GameStatus}, Timer running: {_timer?.IsRunning}");
        
        if (_gameModel.GameStatus == GameEnums.GameStatus.Won || 
            _gameModel.GameStatus == GameEnums.GameStatus.Lost)
        {
            _logger.Log("Game is over, stopping timer");
            // Stop timer when game is over
            if (_timer != null)
            {
                _timer.Stop();
                _logger.Log($"Timer stopped. IsRunning: {_timer.IsRunning}");
            }
            
            // Reveal all mines if game is lost
            if (_gameModel.GameStatus == GameEnums.GameStatus.Lost)
            {
                RevealAllMines();
            }
        }
    }

    private void RevealAllMines()
    {
        if (Items == null) return;
        
        // Force update of all mine items to be revealed
        foreach (var item in Items)
        {
            if (item.IsMine)
            {
                item.IsRevealed = true;
                item.IsFlagged = false; // Ensure mines are not flagged
            }
        }
    }

    private void InitializeTimer()
    {
        _timer = _dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => GameTime++;
    }

    #endregion
}
