namespace MineSweeper.Models;

public partial class GameModel : ObservableObject
{
    private readonly ILogger? _logger;
    
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
    /// <param name="logger">Optional logger for debugging</param>
    public GameModel(int rows = 10, int columns = 10, int mines = 10, ILogger? logger = null)
    {
        _logger = logger;
        _rows = rows;
        _columns = columns;
        _gameTime = 0;
        _gameStatus = GameEnums.GameStatus.NotStarted;
        _items = new ObservableCollection<SweeperItem>();
        _mines = mines;

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < columns; j++)
            _items.Add(new SweeperItem());
        
        _logger?.Log($"GameModel created with {rows}x{columns} grid and {mines} mines");
    }


    /// <summary>
    /// Creates a sweeper game from a pre-defined Game Difficulty
    /// The Rows, Columns and Mines are defined in the GameConstants
    /// <see cref="GameConstants.GameLevels"/>
    /// </summary>
    /// <param name="gameDifficulty">The difficulty level</param>
    /// <param name="logger">Optional logger for debugging</param>
    public GameModel(GameEnums.GameDifficulty gameDifficulty, ILogger? logger = null)
    {
        _logger = logger;
        var (rows, columns, mines) = GameConstants.GameLevels[gameDifficulty];
        _rows = rows;
        _columns = columns;
        _mines = mines;
        _gameTime = 0;
        _gameStatus = GameEnums.GameStatus.NotStarted;
        _items = new ObservableCollection<SweeperItem>();

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < columns; j++)
            _items.Add(new SweeperItem());
        
        _logger?.Log($"GameModel created with difficulty {gameDifficulty}: {rows}x{columns} grid and {mines} mines");
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
        _logger?.Log($"EvaluateIfWon called. Current GameStatus: {GameStatus}");
        
        if (GameStatus == GameEnums.GameStatus.InProgress)
        {
            // Only evaluate win condition if there are mines on the board
            var minesOnBoard = Items!.Count(i => i.IsMine);
            _logger?.Log($"Mines on board: {minesOnBoard}");
            
            if (minesOnBoard > 0)
            {
                // Linq that returns Count of all mines on the board that are not Flagged
                var minesNotFlagged = Items!.Count(i => i.IsMine && !i.IsFlagged);
                var minesFlagged = Items!.Count(i => i.IsMine && i.IsFlagged);
                _logger?.Log($"Mines not flagged: {minesNotFlagged}, Mines flagged: {minesFlagged}");
                
                if (minesNotFlagged == 0)
                {
                    GameStatus = GameEnums.GameStatus.Won;
                    _logger?.Log("All mines flagged, game won!");
                }
                else
                {
                    _logger?.Log("Not all mines flagged, game continues");
                    // GameStatus = GameEnums.GameStatus.InProgress;
                }
            }
            else
            {
                _logger?.Log("No mines on board yet, cannot evaluate win condition");
            }
        }
        else
        {
            _logger?.Log($"Game not in progress, status remains: {GameStatus}");
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
        _logger?.Log($"Flag called at ({row},{column}). Current GameStatus: {GameStatus}, FlaggedItems: {FlaggedItems}");
        
        if (GameStatus == GameEnums.GameStatus.NotStarted)
        {
            GameStatus = GameEnums.GameStatus.InProgress;
            _logger?.Log("Game status changed from NotStarted to InProgress");
        }

        if (!InBounds(row, column))
        {
            _logger?.LogWarning($"Point ({row},{column}) is out of bounds");
            return;
        }

        var item = this[row, column];

        if (item.IsRevealed)
        {
            _logger?.Log($"Item at ({row},{column}) is already revealed, cannot flag");
            return;
        }

        bool wasFlagged = item.IsFlagged;
        item.IsFlagged = !wasFlagged;
        _logger?.Log($"Item at ({row},{column}) flag toggled from {wasFlagged} to {item.IsFlagged}");

        int oldFlaggedItems = FlaggedItems;
        FlaggedItems = CountFlaggedItems();
        _logger?.Log($"FlaggedItems updated from {oldFlaggedItems} to {FlaggedItems}");
        
        int oldRemainingMines = RemainingMines;
        RemainingMines = Mines - FlaggedItems;
        _logger?.Log($"RemainingMines updated from {oldRemainingMines} to {RemainingMines}");

        GameEnums.GameStatus oldStatus = GameStatus;
        GameStatus = EvaluateIfWon();
        if (oldStatus != GameStatus)
        {
            _logger?.Log($"Game status changed from {oldStatus} to {GameStatus}");
        }
        
        _logger?.Log($"Flag operation completed. FlaggedItems: {FlaggedItems}, RemainingMines: {RemainingMines}");
    }

    /// <summary>
    /// Plays a game piece at a specific row and column
    /// </summary>
    /// <param name="pt">The Point to play</param>
    [RelayCommand]
    private void Play(Point pt)
    {
        void InitializeGame(int rows, int columns, int mines, int firstRow, int firstColumn)
        {
            _logger?.Log($"Initializing game with {rows}x{columns} grid and {mines} mines. First move at ({firstRow},{firstColumn})");
            var random = new Random();
            var minesPlaced = 0;
            while (minesPlaced < mines)
            {
                var r = random.Next(0, rows);
                var c = random.Next(0, columns);
                if ((r == firstRow && c == firstColumn) || this[r, c].IsMine) continue;
                this[r, c].IsMine = true;
                minesPlaced++;
            }
            _logger?.Log($"Placed {minesPlaced} mines on the board");

            for (var i = 0; i < rows; i++)
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
            _logger?.Log("Calculated mine counts for all cells");
        }

        var (row, column) = ExtractRowColTuple(pt);
        _logger?.Log($"Play called at ({row},{column}). Current GameStatus: {GameStatus}");

        if (!InBounds(row, column))
        {
            _logger?.LogWarning($"Point ({row},{column}) is out of bounds");
            return;
        }
        
        if (this[row, column].IsFlagged)
        {
            _logger?.Log($"Cell at ({row},{column}) is flagged, cannot play");
            return;
        }
        
        if (this[row, column].IsRevealed)
        {
            _logger?.Log($"Cell at ({row},{column}) is already revealed");
            return;
        }

        if (GameStatus != GameEnums.GameStatus.InProgress)
        {
            _logger?.Log("First play, initializing game board with mines");
            InitializeGame(Rows, Columns, Mines, row, column);
            GameStatus = GameEnums.GameStatus.InProgress;
            _logger?.Log("Game status set to InProgress");
        }

        var item = this[row, column];

        if (item.IsMine && !item.IsFlagged && !item.IsRevealed)
        {
            GameStatus = GameEnums.GameStatus.Lost;
            item.IsRevealed = true;
            _logger?.Log($"Mine hit at ({row},{column}), game lost!");
            return;
        }

        if (item.IsRevealed)
        {
            _logger?.Log($"Cell at ({row},{column}) is already revealed");
            return;
        }

        item.IsRevealed = true;
        _logger?.Log($"Revealed cell at ({row},{column}), mine count: {item.MineCount}");

        if (item.MineCount == 0)
        {
            _logger?.Log($"Cell at ({row},{column}) has no adjacent mines, revealing neighbors");
            var neighbors = GetNeighbors(row, column);
            foreach (var neighbor in neighbors)
                if (!neighbor.IsRevealed && !neighbor.IsFlagged && GameStatus == GameEnums.GameStatus.InProgress)
                    Play(neighbor.Point);
        }
        
        _logger?.Log($"Play operation completed. GameStatus: {GameStatus}");
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
        for (var j = column - 1; j <= column + 1; j++)
            if (InBounds(i, j))
                neighbors.Add(this[i, j]);

        return neighbors;
    }

    private bool InBounds(int row, int column)
    {
        return !(row < 0 || row >= Rows || column < 0 || column >= Columns);
    }
}
