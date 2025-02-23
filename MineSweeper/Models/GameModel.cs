
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Graphics;

namespace MineSweeper.Models;
public partial class GameModel : ObservableObject
{
    public static class GameConstants
    {
        public static readonly Dictionary
            <GameEnums.GameDifficulty, (int rows, int columns, int mines)> GameLevels = new()
            {
                {GameEnums.GameDifficulty.Easy, (10, 10, 10)},
                {GameEnums.GameDifficulty.Medium, (15, 15, 40)},
                {GameEnums.GameDifficulty.Hard, (20, 20, 80)}
            };
    }
    /// <summary>
    /// The Number of Rows in the Game
    /// </summary>
    [ObservableProperty] private int _rows = 10;
    
    /// <summary>
    /// The Number of Columns in the Game
    /// </summary>
    [ObservableProperty] private int _columns = 10;

    /// <summary>
    /// The Number of Mines in the Game
    /// </summary>
    [ObservableProperty] private int _mines = 10;

    /// <summary>
    /// The Number of Flagged Mines Items in the Game
    /// </summary>
    [ObservableProperty] private int _flaggedItems = 0;

    /// <summary>
    /// The Remaining Mines left to be flagged correctly
    /// </summary>
    [ObservableProperty] private int _remainingMines = 10;

    /// <summary>
    /// The Time since first piece was played
    /// </summary>
    [ObservableProperty] private int _gameTime;

    /// <summary>
    /// The Game Status as defined by <sew cref="GameEnums.GameStatus"/>
    /// </summary>
    [ObservableProperty] private GameEnums.GameStatus _gameStatus;

    /// <summary>
    /// The Collection of Sweeper Items - Game Pieces
    /// </summary>
    [ObservableProperty] private ObservableCollection<SweeperItem>? _items;

    /// <summary>
    /// The Indexer allows for accessing the SweeperItem at a specific row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    public SweeperItem this[int row, int column]
    {
        get => Items[row * Columns + column];
        private set => Items[row * Columns + column] = value;
    }

    /// <summary>
    /// Default Constructor - this was added to allow for serialization
    /// </summary>
    public GameModel()
    {
    }

    /// <summary>
    /// Custom Game Constructor
    /// </summary>
    /// <param name="rows">Number of rows in the game</param>
    /// <param name="columns">Number of columns in the game</param>
    /// <param name="mines">Number of Mines to be generated in random spots</param>
    public GameModel(int rows = 10, int columns = 10, int mines = 10)
    {
        _rows = rows;
        _columns = columns;
        _gameTime = 0;
        _gameStatus = GameEnums.GameStatus.NotStarted;
        _items = new ObservableCollection<SweeperItem>();
        _mines = mines;

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                _items.Add(new SweeperItem());
            }
        }
    }

    
    /// <summary>
    /// Creates a sweeper game from a pre-defined Game Difficulty
    /// The Rows, Columns and Mines are defined in the GameConstants
    /// <see cref="GameConstants.GameLevels"/>
    /// </summary>
    /// <param name="gameDifficulty"></param>
    public GameModel(GameEnums.GameDifficulty gameDifficulty)
    {
        var (rows, columns, mines) = GameConstants.GameLevels[gameDifficulty];
        _rows = rows;
        _columns = columns;
        _mines = mines;
        _gameTime = 0;
        _gameStatus = GameEnums.GameStatus.NotStarted;
        _items = new ObservableCollection<SweeperItem>();

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                _items.Add(new SweeperItem());
            }
        }
    }

    /// <summary>
    /// Constructor that takes a JSON file and deserializes it into a GameModel
    /// </summary>
    /// <param name="jsonFile"></param>
    public GameModel(string jsonFile)
    {
        
        var gameModel = JsonSerializer.Deserialize<GameModel>(jsonFile);
        if (gameModel != null)
        {
            _rows = gameModel.Rows;
            _columns = gameModel.Columns;
            _mines = gameModel.Mines;
            _gameTime = gameModel.GameTime;
            _gameStatus = gameModel.GameStatus;
            _items = gameModel.Items;
        }
    }

    /// <summary>
    /// Saving the Game to a JSON file
    /// </summary>
    /// <param name="fileName">The file does not need to exist</param>
    [RelayCommand]
    private void SaveGame(string fileName)
    {
        var jsonFile = JsonSerializer.Serialize(this);
        File.WriteAllText(fileName, jsonFile);
    }


    /// <summary>
    /// Evaluates if the Game is Won
    /// </summary>
    /// <returns>GameStatus</returns>
    private GameEnums.GameStatus EvaluateIfWon()
    {
        if (GameStatus == GameEnums.GameStatus.InProgress)
        {
            // Linq that returns Count of all mines on the board that are not Flagged
            var minesNotFlagged = Items!.Count(i => i.IsMine && !i.IsFlagged);
            if (minesNotFlagged == 0)
            {
                GameStatus = GameEnums.GameStatus.Won;
            }
            else
            {
                // GameStatus = GameEnums.GameStatus.InProgress;
            }
        }

        return GameStatus;
    }

    /// <summary>
    /// Flags a game Piece at a specific row and column
    /// Flagging indicates the player believes the piece is a mine
    /// </summary>
    /// <param name="pt"></param>
    [RelayCommand]
    private void Flag(Point pt)
    {
        var (row, column) = ExtractRowColTuple(pt);
        if (GameStatus == GameEnums.GameStatus.NotStarted)
        {
            GameStatus = GameEnums.GameStatus.InProgress;
        }

        if (!InBounds(row, column))
        {
            return;
        }

        var item = this[row, column];

        if (item.IsRevealed)
            return;

        item.IsFlagged = !item.IsFlagged;

        FlaggedItems = CountFlaggedItems();
        RemainingMines = Mines - FlaggedItems;

        GameStatus = EvaluateIfWon();
    }

    /// <summary>
    /// Plays a game piece at a specific row and column
    /// </summary>
    /// <param name="pt">The Point to play</param>
    
    // What is wrong with this code?
    
    [RelayCommand]
    private void Play(Point pt)
    {
        void InitializeGame(int rows, int columns, int mines, int firstRow, int firstColumn)
        {
            var random = new Random();
            var minesPlaced = 0;
            while (minesPlaced < mines)
            {
                var r = random.Next(0, rows);
                var c = random.Next(0, columns);
                if ((r == firstRow && c == firstColumn) || this[r, c].IsMine)
                {
                    continue;
                }
                this[r, c].IsMine = true;
                minesPlaced++;
            }

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    var it = this[i, j];
                    if (!it.IsMine)
                    {
                        var neighbors = GetNeighbors(i, j);
                        it.MineCount = neighbors.Count(n => n.IsMine);
                        it.Point = new Point(i, j);
                    }
                }
            }
        }

        var (row, column) = ExtractRowColTuple(pt);

        if (!InBounds(row, column) || this[row, column].IsFlagged || this[row, column].IsRevealed)
        {
            return;
        }

        if (GameStatus != GameEnums.GameStatus.InProgress)
        {
            InitializeGame(Rows, Columns, Mines, row, column);
            GameStatus = GameEnums.GameStatus.InProgress;
        }

        var item = this[row, column];

        if (item.IsMine && !item.IsFlagged && !item.IsRevealed)
        {
            GameStatus = GameEnums.GameStatus.Lost;
            item.IsRevealed = true;
            return;
        }


        if (item.IsRevealed)
            return;
        
        item.IsRevealed = true;

        if (item.MineCount == 0)
        {
            var neighbors = GetNeighbors(row, column);
            foreach (var neighbor in neighbors)
            {
                if (!neighbor.IsRevealed && !neighbor.IsFlagged && GameStatus == GameEnums.GameStatus.InProgress)
                {
                    Play(neighbor.Point);
                }
            }
        }
    }
    
    private int CountFlaggedItems()
    {
        return Items != null ? Items.Count(i => i.IsFlagged) : 0;
    }

    private static (int r, int c) ExtractRowColTuple(Point pt)
    {
        return ((int r, int c)) (pt.X, pt.Y);
    }

    private List<SweeperItem> GetNeighbors(int row, int column)
    {
        var neighbors = new List<SweeperItem>();
        for (var i = row - 1; i <= row + 1; i++)
        {
            for (var j = column - 1; j <= column + 1; j++)
            {
                if (InBounds(i,j))
                {
                    neighbors.Add(this[i, j]);
                }
            }
        }

        return neighbors;
    }
    private bool InBounds(int row, int column)
    {
        return !(row < 0 || row >= Rows || column < 0 || column >= Columns);
    }
}