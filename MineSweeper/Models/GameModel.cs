
namespace MineSweeper.Models;
public partial class GameModel : ObservableObject
{
    [ObservableProperty] private int _rows = 10;

    [ObservableProperty] private int _columns = 10;

    [ObservableProperty] private int _mines = 10;

    [ObservableProperty] private int _flaggedItems = 0;

    [ObservableProperty] private int _remainingMines = 10;

    [ObservableProperty] private int _gameTime;

    [ObservableProperty] private GameEnums.GameStatus _gameStatus;

    [ObservableProperty] private ObservableCollection<SweeperItem>? _items;

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

        EvaluateIfWon();
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
    
    [RelayCommand]
    private void Play(Point pt)
    {
        void InitializeGame(int rows, int columns, int mines)
        {
            var random = new Random();
            var minesPlaced = 0;
            while (minesPlaced < mines)
            {
                var r = random.Next(0, rows);
                var c = random.Next(0, columns);
                var it = this[r, c];
                if (!it.IsMine)
                {
                    it.IsMine = true;
                    minesPlaced++;
                }
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
                    }
                }
            }
        }

        var (row, column) = ExtractRowColTuple(pt);

        if (!InBounds(row, column))
        {
            return;
        }

        var item = this[row, column];

        if (item.IsFlagged)
            return;

        if (item.IsRevealed)
            return;

        if (GameStatus != GameEnums.GameStatus.InProgress)
        {
            InitializeGame(Rows, Columns, Mines);
        }

        if (item.IsMine)
        {
            GameStatus = GameEnums.GameStatus.Lost;
            item.IsRevealed = true;
            return;
        }

        item.IsRevealed = true;
        if (this.GameStatus == GameEnums.GameStatus.NotStarted)
        {
            GameStatus = GameEnums.GameStatus.InProgress;
        }

        if (GameStatus == GameEnums.GameStatus.InProgress && item.MineCount == 0)
        {
            Play(new Point(row - 1, column - 1));
            Play(new Point(row - 1, column));
            Play(new Point(row - 1, column + 1));
            Play(new Point(row, column - 1));
            Play(new Point(row, column + 1));
            Play(new Point(row + 1, column - 1));
            Play(new Point(row + 1, column));
            Play(new Point(row + 1, column + 1));
        }
        return;
    }

    private bool InBounds(int row, int column)
    {
        return !(row < 0 || row >= Rows || column < 0 || column >= Columns);
    }
}