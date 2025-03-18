using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace MineSweeper.Models;

/// <summary>
/// Model class for the Minesweeper game, containing all game logic and state
/// </summary>
public partial class GameModel : ObservableObject, IGameModel
{
    /// <summary>
    /// Logger for debugging purposes
    /// </summary>
    private readonly ILogger? _logger;

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
    [ObservableProperty] private int _remainingMines;

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
    [ObservableProperty] private ObservableCollection<SweeperItem> _items = new();

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
        _mines = mines;
        _remainingMines = mines; // Initialize RemainingMines to match Mines

        _logger?.Log($"Starting creation of {rows}x{columns} grid with {mines} mines");
        
        // Pre-allocate capacity to avoid resizing
        var capacity = rows * columns;
        var itemsList = new List<SweeperItem>(capacity);
        
        // Create all items with their positions in one batch
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var item = new SweeperItem
                {
                    Point = new Point(i, j)
                };
                itemsList.Add(item);
            }
        }
        
        // Create ObservableCollection from the list in one operation
        _items = new ObservableCollection<SweeperItem>(itemsList);
        
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
        _remainingMines = mines; // Initialize RemainingMines to match Mines
        _gameTime = 0;
        _gameStatus = GameEnums.GameStatus.NotStarted;
        
        _logger?.Log($"Starting creation of {gameDifficulty} grid: {rows}x{columns} with {mines} mines");
        
        // Pre-allocate capacity to avoid resizing
        var capacity = rows * columns;
        var itemsList = new List<SweeperItem>(capacity);
        
        // Create all items with their positions in one batch
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var item = new SweeperItem
                {
                    Point = new Point(i, j)
                };
                itemsList.Add(item);
            }
        }
        
        // Create ObservableCollection from the list in one operation
        _items = new ObservableCollection<SweeperItem>(itemsList);
        
        _logger?.Log($"GameModel created with difficulty {gameDifficulty}: {rows}x{columns} grid and {mines} mines");
    }

    /// <summary>
    /// Constructor that takes a JSON file and deserializes it into a GameModel
    /// </summary>
    /// <param name="jsonFile"></param>
    public GameModel(string jsonFile)
    {
        try
        {
            // Parse the JSON manually
            var jsonDocument = JsonDocument.Parse(jsonFile);
            var root = jsonDocument.RootElement;
            
            // Extract basic properties
            if (root.TryGetProperty("Rows", out var rowsElement))
                _rows = rowsElement.GetInt32();
            
            if (root.TryGetProperty("Columns", out var columnsElement))
                _columns = columnsElement.GetInt32();
            
            if (root.TryGetProperty("Mines", out var minesElement))
                _mines = minesElement.GetInt32();
            
            if (root.TryGetProperty("GameTime", out var gameTimeElement))
                _gameTime = gameTimeElement.GetInt32();
            
            if (root.TryGetProperty("GameStatus", out var gameStatusElement))
            {
                if (gameStatusElement.ValueKind == JsonValueKind.String)
                {
                    var statusString = gameStatusElement.GetString();
                    if (Enum.TryParse<GameEnums.GameStatus>(statusString, out var status))
                        _gameStatus = status;
                }
                else if (gameStatusElement.ValueKind == JsonValueKind.Number)
                {
                    var statusInt = gameStatusElement.GetInt32();
                    if (Enum.IsDefined(typeof(GameEnums.GameStatus), statusInt))
                        _gameStatus = (GameEnums.GameStatus)statusInt;
                }
            }
            
            // Initialize items collection
            _items = new ObservableCollection<SweeperItem>();
            
            // Parse items if present
            if (root.TryGetProperty("Items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var itemElement in itemsElement.EnumerateArray())
                {
                    var item = new SweeperItem();
                    
                    if (itemElement.TryGetProperty("IsRevealed", out var isRevealedElement))
                        item.IsRevealed = isRevealedElement.GetBoolean();
                    
                    if (itemElement.TryGetProperty("IsMine", out var isMineElement))
                        item.IsMine = isMineElement.GetBoolean();
                    
                    if (itemElement.TryGetProperty("IsFlagged", out var isFlaggedElement))
                        item.IsFlagged = isFlaggedElement.GetBoolean();
                    
                    if (itemElement.TryGetProperty("MineCount", out var mineCountElement))
                        item.MineCount = mineCountElement.GetInt32();
                    
                    if (itemElement.TryGetProperty("Point", out var pointElement))
                    {
                        double x = 0, y = 0;
                        
                        if (pointElement.TryGetProperty("X", out var xElement))
                            x = xElement.GetDouble();
                        
                        if (pointElement.TryGetProperty("Y", out var yElement))
                            y = yElement.GetDouble();
                        
                        item.Point = new Point(x, y);
                    }
                    
                    _items.Add(item);
                }
            }
            else
            {
                // If no items in JSON, create default grid
                for (var i = 0; i < _rows; i++)
                for (var j = 0; j < _columns; j++)
                    _items.Add(new SweeperItem());
            }
            
            _logger?.Log($"Game loaded from JSON with {Rows}x{Columns} grid and {Mines} mines");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error loading game from JSON: {ex.Message}");
            // Initialize with default values
            _rows = 10;
            _columns = 10;
            _mines = 10;
            _gameTime = 0;
            _gameStatus = GameEnums.GameStatus.NotStarted;
            _items = new ObservableCollection<SweeperItem>();
            
            for (var i = 0; i < _rows; i++)
            for (var j = 0; j < _columns; j++)
                _items.Add(new SweeperItem());
        }
    }

    /// <summary>
    /// Saving the Game to a JSON file
    /// </summary>
    /// <param name="fileName">The file does not need to exist</param>
    [RelayCommand]
    private void SaveGame(string fileName)
    {
        try
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
            
            writer.WriteStartObject();
            
            // Write basic properties
            writer.WriteNumber("Rows", Rows);
            writer.WriteNumber("Columns", Columns);
            writer.WriteNumber("Mines", Mines);
            writer.WriteNumber("FlaggedItems", FlaggedItems);
            writer.WriteNumber("RemainingMines", RemainingMines);
            writer.WriteNumber("GameTime", GameTime);
            writer.WriteString("GameStatus", GameStatus.ToString());
            
            // Write items array
            writer.WriteStartArray("Items");
            
            foreach (var item in Items)
            {
                writer.WriteStartObject();
                writer.WriteBoolean("IsRevealed", item.IsRevealed);
                writer.WriteBoolean("IsMine", item.IsMine);
                writer.WriteBoolean("IsFlagged", item.IsFlagged);
                writer.WriteNumber("MineCount", item.MineCount);
                
                writer.WriteStartObject("Point");
                writer.WriteNumber("X", item.Point.X);
                writer.WriteNumber("Y", item.Point.Y);
                writer.WriteEndObject();
                
                writer.WriteEndObject();
            }
            
            writer.WriteEndArray();
            writer.WriteEndObject();
            
            writer.Flush();
            
            File.WriteAllBytes(fileName, stream.ToArray());
            _logger?.Log($"Game saved to {fileName}");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error saving game to {fileName}: {ex.Message}");
        }
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

    // ICommand properties for IGameModel interface
    ICommand IGameModel.PlayCommand => PlayCommand;
    ICommand IGameModel.FlagCommand => FlagCommand;

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
        
        // Prevent flagging before first click (when game is not started)
        if (GameStatus == GameEnums.GameStatus.NotStarted)
        {
            _logger?.Log("Cannot flag before first move");
            return;
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
    /// Initializes the game board with mines, avoiding the first clicked position
    /// </summary>
    /// <param name="rows">Number of rows in the game</param>
    /// <param name="columns">Number of columns in the game</param>
    /// <param name="mines">Number of mines to place</param>
    /// <param name="firstRow">Row of first click (to avoid placing mine here)</param>
    /// <param name="firstColumn">Column of first click (to avoid placing mine here)</param>
    private void InitializeGame(int rows, int columns, int mines, int firstRow, int firstColumn)
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

    /// <summary>
    /// Plays a game piece at a specific row and column
    /// </summary>
    /// <param name="pt">The Point to play</param>
    [RelayCommand]
    private void Play(Point pt)
    {

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

    /// <summary>
    /// Counts the number of flagged items in the game
    /// </summary>
    /// <returns>The number of flagged items</returns>
    private int CountFlaggedItems()
    {
        return Items != null ? Items.Count(i => i.IsFlagged) : 0;
    }

    /// <summary>
    /// Extracts row and column values from a Point
    /// </summary>
    /// <param name="pt">The point to extract from</param>
    /// <returns>A tuple containing row and column values</returns>
    private static (int r, int c) ExtractRowColTuple(Point pt)
    {
        return ((int r, int c)) (pt.X, pt.Y);
    }

    /// <summary>
    /// Gets all neighboring cells for a given position
    /// </summary>
    /// <param name="row">The row index</param>
    /// <param name="column">The column index</param>
    /// <returns>A list of neighboring cells</returns>
    private List<SweeperItem> GetNeighbors(int row, int column)
    {
        var neighbors = new List<SweeperItem>();
        for (var i = row - 1; i <= row + 1; i++)
        for (var j = column - 1; j <= column + 1; j++)
            if (InBounds(i, j))
                neighbors.Add(this[i, j]);

        return neighbors;
    }

    /// <summary>
    /// Checks if a position is within the bounds of the game grid
    /// </summary>
    /// <param name="row">The row index to check</param>
    /// <param name="column">The column index to check</param>
    /// <returns>True if the position is within bounds, false otherwise</returns>
    private bool InBounds(int row, int column)
    {
        return !(row < 0 || row >= Rows || column < 0 || column >= Columns);
    }
    
    /// <summary>
    /// Reveals all mines on the board when the game is lost
    /// </summary>
    public void RevealAllMines()
    {
        _logger?.Log("Revealing all mines in the model");
        
        // Find all mine items and reveal them
        foreach (var item in Items.Where(i => i.IsMine))
        {
            item.IsRevealed = true;
            item.IsFlagged = false; // Ensure mines are not flagged
        }
    }
}
