using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;
using MineSweeper.Features.Game.Models;

namespace MineSweeper.Tests.Models;

/// <summary>
/// A mock implementation of IGameModel for testing purposes.
/// This class allows creating a game model with a known state.
/// </summary>
public class MockGameModel : IGameModel, IDisposable
{
    private readonly Dictionary<(int, int), bool> _flaggedCells = new();
    private readonly Dictionary<(int, int), bool> _revealedCells = new();
    private readonly Dictionary<(int, int), bool> _mineCells = new();
    private readonly Dictionary<(int, int), int> _mineCounts = new();
    private int _remainingMines;
    private ObservableCollection<SweeperItem> _items;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of MockGameModel with default values
    /// </summary>
    public MockGameModel()
    {
        Rows = 10;
        Columns = 10;
        Mines = 10;
        _remainingMines = Mines - FlaggedItems;
        GameStatus = GameEnums.GameStatus.NotStarted;
        GameDifficulty = GameEnums.GameDifficulty.Easy;
        
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
    
    /// <summary>
    /// Creates a new instance of MockGameModel with the specified parameters
    /// </summary>
    public MockGameModel(int rows, int columns, int mines, GameEnums.GameStatus status = GameEnums.GameStatus.NotStarted)
    {
        Rows = rows;
        Columns = columns;
        Mines = mines;
        _remainingMines = mines;
        GameStatus = status;
        GameDifficulty = GameEnums.GameDifficulty.Easy; // Use Easy as default for custom sizes
        
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

    /// <summary>
    /// Gets the collection of sweeper items (cells) in the game grid
    /// </summary>
    public ObservableCollection<SweeperItem>? Items => _items;

    /// <summary>
    /// Gets the number of rows in the game grid
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// Gets the number of columns in the game grid
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the total number of mines in the game
    /// </summary>
    public int Mines { get; }

    /// <summary>
    /// Gets the number of cells that have been flagged by the player
    /// </summary>
    public int FlaggedItems => _flaggedCells.Count(kv => kv.Value);

    /// <summary>
    /// Gets the number of mines remaining to be flagged
    /// </summary>
    public int RemainingMines => Mines - FlaggedItems;

    /// <summary>
    /// Gets or sets the current game status
    /// </summary>
    public GameEnums.GameStatus GameStatus { get; set; }
    
    /// <summary>
    /// Gets the current game difficulty
    /// </summary>
    public GameEnums.GameDifficulty GameDifficulty { get; }

    /// <summary>
    /// Command to play (reveal) a cell at the specified position
    /// </summary>
    public ICommand PlayCommand { get; }

    /// <summary>
    /// Command to flag a cell at the specified position
    /// </summary>
    public ICommand FlagCommand { get; }

    /// <summary>
    /// Gets the sweeper item at the specified row and column
    /// </summary>
    public SweeperItem this[int row, int column]
    {
        get
        {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException();
                
            return new SweeperItem
            {
                Point = new Point(row, column),
                IsRevealed = IsRevealed(row, column),
                IsFlagged = IsFlagged(row, column),
                IsMine = IsMine(row, column),
                MineCount = GetMineCount(row, column)
            };
        }
    }
    
    /// <summary>
    /// Checks if a cell is flagged
    /// </summary>
    public bool IsFlagged(int row, int column) => 
        _flaggedCells.ContainsKey((row, column)) && _flaggedCells[(row, column)];
    
    /// <summary>
    /// Checks if a cell is revealed
    /// </summary>
    public bool IsRevealed(int row, int column) => 
        _revealedCells.ContainsKey((row, column)) && _revealedCells[(row, column)];
    
    /// <summary>
    /// Checks if a cell contains a mine
    /// </summary>
    public bool IsMine(int row, int column) => 
        _mineCells.ContainsKey((row, column)) && _mineCells[(row, column)];
    
    /// <summary>
    /// Gets the mine count for a cell
    /// </summary>
    public int GetMineCount(int row, int column) => 
        _mineCounts.ContainsKey((row, column)) ? _mineCounts[(row, column)] : 0;
    
    /// <summary>
    /// Sets a cell as a mine
    /// </summary>
    public void SetMine(int row, int column, bool isMine = true)
    {
        _mineCells[(row, column)] = isMine;
        
        // Update the item in the collection
        int index = row * Columns + column;
        if (index >= 0 && index < _items.Count)
        {
            var item = _items[index];
            item.IsMine = isMine;
            _items[index] = item;
        }
    }
    
    /// <summary>
    /// Sets the mine count for a cell
    /// </summary>
    public void SetMineCount(int row, int column, int count)
    {
        _mineCounts[(row, column)] = count;
        
        // Update the item in the collection
        int index = row * Columns + column;
        if (index >= 0 && index < _items.Count)
        {
            var item = _items[index];
            item.MineCount = count;
            _items[index] = item;
        }
    }
    
    /// <summary>
    /// Sets a cell as revealed
    /// </summary>
    public void SetRevealed(int row, int column, bool isRevealed = true)
    {
        _revealedCells[(row, column)] = isRevealed;
        
        // Update the item in the collection
        int index = row * Columns + column;
        if (index >= 0 && index < _items.Count)
        {
            var item = _items[index];
            item.IsRevealed = isRevealed;
            _items[index] = item;
        }
    }
    
    /// <summary>
    /// Sets a cell as flagged
    /// </summary>
    public void SetFlagged(int row, int column, bool isFlagged = true)
    {
        _flaggedCells[(row, column)] = isFlagged;
        
        // Update remaining mines
        if (isFlagged)
            _remainingMines--;
        else
            _remainingMines++;
            
        // Update the item in the collection
        int index = row * Columns + column;
        if (index >= 0 && index < _items.Count)
        {
            var item = _items[index];
            item.IsFlagged = isFlagged;
            _items[index] = item;
        }
    }

    /// <summary>
    /// Flags a cell at the specified position
    /// </summary>
    public void Flag(int row, int column)
    {
        if (row < 0 || row >= Rows || column < 0 || column >= Columns)
            return;
            
        if (GameStatus != GameEnums.GameStatus.InProgress)
            return;
            
        if (IsRevealed(row, column))
            return;
            
        bool isFlagged = IsFlagged(row, column);
        SetFlagged(row, column, !isFlagged);
    }

    /// <summary>
    /// Plays (reveals) a cell at the specified position
    /// </summary>
    public void Play(int row, int column)
    {
        if (row < 0 || row >= Rows || column < 0 || column >= Columns)
            return;
            
        if (GameStatus == GameEnums.GameStatus.Won || GameStatus == GameEnums.GameStatus.Lost)
            return;
            
        if (IsFlagged(row, column))
            return;
            
        if (IsRevealed(row, column))
            return;
            
        // First move
        if (GameStatus == GameEnums.GameStatus.NotStarted)
            GameStatus = GameEnums.GameStatus.InProgress;
            
        SetRevealed(row, column, true);
        
        // Check if mine
        if (IsMine(row, column))
        {
            GameStatus = GameEnums.GameStatus.Lost;
            return;
        }
        
        // Auto-reveal neighbors if mine count is 0
        if (GetMineCount(row, column) == 0)
        {
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = column - 1; j <= column + 1; j++)
                {
                    if (i >= 0 && i < Rows && j >= 0 && j < Columns && !IsRevealed(i, j))
                        Play(i, j);
                }
            }
        }
        
        // Check if game is won
        CheckIfWon();
    }
    
    /// <summary>
    /// Checks if the game is won
    /// </summary>
    private void CheckIfWon()
    {
        // Game is won if all non-mine cells are revealed
        int nonMineCells = Rows * Columns - Mines;
        int revealedCells = _revealedCells.Count(kv => kv.Value);
        
        if (revealedCells == nonMineCells)
            GameStatus = GameEnums.GameStatus.Won;
    }

    /// <summary>
    /// Reveals all mines on the board when the game is lost
    /// </summary>
    public void RevealAllMines()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (IsMine(i, j))
                    SetRevealed(i, j, true);
            }
        }
    }
    
    /// <summary>
    /// Disposes resources used by the model
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;
            
        _items.Clear();
        _disposed = true;
    }
}
