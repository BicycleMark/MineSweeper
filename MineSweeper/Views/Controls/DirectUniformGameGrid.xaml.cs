using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;
using MineSweeper.Models;
using MineSweeper.Views.Controls;

namespace MineSweeper.Views.Controls;

public partial class DirectUniformGameGrid : ContentView
{
    private readonly ILogger _logger;
    #region Bindable Properties

    public static readonly BindableProperty PlayCommandProperty = BindableProperty.Create(
        nameof(PlayCommand),
        typeof(ICommand),
        typeof(DirectUniformGameGrid),
        null);

    public static readonly BindableProperty FlagCommandProperty = BindableProperty.Create(
        nameof(FlagCommand),
        typeof(ICommand),
        typeof(DirectUniformGameGrid),
        null);

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<SweeperItem>),
        typeof(DirectUniformGameGrid),
        null,
        propertyChanged: OnItemsSourceChanged);

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(DirectUniformGameGrid),
        null,
        propertyChanged: OnItemTemplateChanged);

    public static readonly BindableProperty RowsProperty = BindableProperty.Create(
        nameof(Rows),
        typeof(int),
        typeof(DirectUniformGameGrid),
        10,
        propertyChanged: OnGridSizeChanged);

    public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(
        nameof(Columns),
        typeof(int),
        typeof(DirectUniformGameGrid),
        10,
        propertyChanged: OnGridSizeChanged);

    #endregion

    #region Properties

    public ICommand PlayCommand
    {
        get => (ICommand)GetValue(PlayCommandProperty);
        set => SetValue(PlayCommandProperty, value);
    }

    public ICommand FlagCommand
    {
        get => (ICommand)GetValue(FlagCommandProperty);
        set => SetValue(FlagCommandProperty, value);
    }

    public IEnumerable<SweeperItem> ItemsSource
    {
        get => (IEnumerable<SweeperItem>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    #endregion

    // Dictionary to track which cells have been tapped
    private readonly Dictionary<int, bool> _tappedCells = new();
    private DateTime _lastTapTime = DateTime.MinValue;
    private const int DoubleTapThresholdMs = 300; // Double tap threshold in milliseconds

public DirectUniformGameGrid()
{
    InitializeComponent();
    
    // Add tap gesture recognizer for normal play and double-tap for flagging
    this.Loaded += OnLoaded;
    
    // Add handler for SizeChanged event
    this.SizeChanged += OnSizeChanged;
}

public DirectUniformGameGrid(ILogger logger)
{
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    InitializeComponent();
    
    _logger?.Log("DirectUniformGameGrid: Constructor called");
    
    // Add tap gesture recognizer for normal play and double-tap for flagging
    this.Loaded += OnLoaded;
    
    // Add handler for SizeChanged event
    this.SizeChanged += OnSizeChanged;
}
    
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        _logger?.Log($"DirectUniformGameGrid: SizeChanged - Width={Width}, Height={Height}");
        _logger?.Log($"DirectUniformGameGrid: board size - Width={board?.Width ?? 0}, Height={board?.Height ?? 0}");
    }
    
    #region Property Change Handlers
    
    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DirectUniformGameGrid grid)
        {
            grid.UpdateGrid();
        }
    }
    
    private static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DirectUniformGameGrid grid)
        {
            grid.UpdateGrid();
        }
    }
    
    private static void OnGridSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is DirectUniformGameGrid grid)
        {
            grid.UpdateGridSize();
            grid.UpdateGrid();
        }
    }
    
    #endregion
    
    #region Grid Update Methods
    
    private void UpdateGridSize()
    {
        try
        {
            // Update the UniformGrid rows and columns
            if (board != null)
            {
                board.Rows = Rows;
                board.Columns = Columns;
                
                _logger?.Log($"DirectUniformGameGrid: Updated grid size - Rows={Rows}, Columns={Columns}");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError($"DirectUniformGameGrid: Exception in UpdateGridSize: {ex}");
        }
    }
    
    private void UpdateGrid()
    {
        try
        {
            // Update the UniformGrid properties
            if (board != null)
            {
                _logger?.Log($"DirectUniformGameGrid: UpdateGrid called with ItemsSource count = {ItemsSource?.Count() ?? 0}, Rows={Rows}, Columns={Columns}");
                
                // Make the board transparent to see the cells
                board.BackgroundColor = Colors.Transparent;
                
                // Clear existing children to ensure we don't have duplicates
                board.Children.Clear();
                
                // Create cells manually instead of using ItemsSource binding
                if (ItemsSource != null && Rows > 0 && Columns > 0)
                {
                    var items = ItemsSource.ToList();
                    int totalItems = items.Count;
                    
                    _logger?.Log($"DirectUniformGameGrid: Creating {totalItems} cells manually");
                    
                    for (int i = 0; i < totalItems; i++)
                    {
                        var item = items[i];
                        
                        // Create a cell for this item
                        var cell = CreateCellForItem(item);
                        
                        // Calculate row and column
                        int row = i / Columns;
                        int column = i % Columns;
                        
                        // Set the cell's position in the grid
                        Grid.SetRow(cell, row);
                        Grid.SetColumn(cell, column);
                        
                        // Add the cell to the board
                        board.Children.Add(cell);
                        
                        if (i < 5) // Log only the first few items to avoid flooding the log
                        {
                            _logger?.Log($"DirectUniformGameGrid: Created cell for item {i} at row={row}, column={column}");
                        }
                    }
                    
                    _logger?.Log($"DirectUniformGameGrid: Added {board.Children.Count} cells to the board");
                }
                
                // Set the rows and columns
                board.Rows = Rows;
                board.Columns = Columns;
                
                _logger?.Log($"DirectUniformGameGrid: Updated grid with {board.Children.Count} cells");
                _logger?.Log($"DirectUniformGameGrid: board.Rows = {board.Rows}, board.Columns = {board.Columns}");
                _logger?.Log($"DirectUniformGameGrid: board.Width = {board.Width}, board.Height = {board.Height}");
                
                // Force a layout update
                board.InvalidateMeasure();
                
                // Set a minimum size for the board if it's not being sized properly
                if (board.Width <= 0 || board.Height <= 0)
                {
                    _logger?.Log($"DirectUniformGameGrid: Setting minimum size for board");
                    board.MinimumWidthRequest = 400;
                    board.MinimumHeightRequest = 400;
                }
                
                // Force the parent Border to be visible too
                mainBorder.BackgroundColor = Colors.Transparent;
            }
            else
            {
                _logger?.LogWarning("DirectUniformGameGrid: board is null in UpdateGrid");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError($"DirectUniformGameGrid: Exception in UpdateGrid: {ex}");
        }
    }
    
    /// <summary>
    /// Creates a cell view for a SweeperItem
    /// </summary>
    private View CreateCellForItem(SweeperItem item)
    {
        // Create a border for the cell
        var border = new Border
        {
            BackgroundColor = item.IsRevealed ? Colors.LightGray : Colors.DarkGray,
            Stroke = Colors.Gray,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 2 },
            Padding = 2,
            Margin = 1,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        
        // Create a grid to hold the content
        var grid = new Grid();
        
        // Flag emoji (when flagged and not revealed)
        if (item.IsFlagged && !item.IsRevealed)
        {
            var flagLabel = new Label
            {
                Text = "🚩",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 16,
                TextColor = Colors.Red
            };
            grid.Add(flagLabel);
        }
        
        // Mine emoji (when revealed and is mine)
        if (item.IsRevealed && item.IsMine)
        {
            var mineLabel = new Label
            {
                Text = "💣",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 16,
                TextColor = Colors.Black
            };
            grid.Add(mineLabel);
        }
        
        // Mine count (when revealed, not a mine, and count > 0)
        if (item.IsRevealed && !item.IsMine && item.MineCount > 0)
        {
            var countLabel = new Label
            {
                Text = item.MineCount.ToString(),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = GetColorForMineCount(item.MineCount)
            };
            grid.Add(countLabel);
        }
        
        // Set the grid as the border's content
        border.Content = grid;
        
        // Set up a tap gesture recognizer for the cell
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (sender, e) => OnCellTapped(sender, e, item.Point);
        border.GestureRecognizers.Add(tapGesture);
        
        return border;
    }
    
    /// <summary>
    /// Gets the color for a mine count
    /// </summary>
    private Color GetColorForMineCount(int count)
    {
        return count switch
        {
            1 => Colors.Blue,
            2 => Colors.Green,
            3 => Colors.Red,
            4 => Colors.DarkBlue,
            5 => Colors.DarkRed,
            6 => Colors.Teal,
            7 => Colors.Black,
            8 => Colors.Gray,
            _ => Colors.Transparent
        };
    }
    
    /// <summary>
    /// Handles tapping on a cell
    /// </summary>
    private void OnCellTapped(object? sender, TappedEventArgs e, Point point)
    {
        try
        {
            // Get the current time to detect double taps
            var now = DateTime.Now;
            
            // Calculate a unique cell ID
            var cellId = (int)(point.X * Columns + point.Y);
            
            // Check if this is a double tap (for flagging)
            var isDoubleTap = (now - _lastTapTime).TotalMilliseconds < DoubleTapThresholdMs && 
                             _tappedCells.TryGetValue(cellId, out var wasTapped) && wasTapped;
            
            _lastTapTime = now;
            _tappedCells[cellId] = true;
            
            // Execute the appropriate command
            if (isDoubleTap)
            {
                _logger?.Log($"DirectUniformGameGrid: Double tap detected at point={point}");
                if (FlagCommand?.CanExecute(null) == true)
                    FlagCommand.Execute(point);
            }
            else
            {
                _logger?.Log($"DirectUniformGameGrid: Single tap detected at point={point}");
                if (PlayCommand?.CanExecute(null) == true)
                    PlayCommand.Execute(point);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError($"DirectUniformGameGrid: Exception in OnCellTapped: {ex}");
        }
    }
    
    #endregion
    
    #region Event Handlers
    
    private void OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            // Remove the event handler to avoid multiple registrations
            this.Loaded -= OnLoaded;
            
            // Initialize the grid
            UpdateGridSize();
            
            // Now that the control is loaded, add the tap gesture recognizer
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnCellTapped;
            
            // Check if board is not null before adding the gesture recognizer
            if (board != null)
            {
                board.GestureRecognizers.Add(tapGesture);
                _logger?.Log("DirectUniformGameGrid: Added tap gesture recognizer to board");
            }
            else
            {
                _logger?.LogWarning("DirectUniformGameGrid: board is null in OnLoaded");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"DirectUniformGameGrid: Exception in OnLoaded: {ex}");
        }
    }

    private void OnCellTapped(object? sender, TappedEventArgs e)
    {
        // Check if board is null
        if (board == null)
        {
            _logger.LogWarning("DirectUniformGameGrid: board is null in OnCellTapped");
            return;
        }
        
        if (board.Width <= 0 || board.Height <= 0 || Rows <= 0 || Columns <= 0)
        {
            _logger.LogWarning($"DirectUniformGameGrid: Invalid dimensions - Width={board.Width}, Height={board.Height}, Rows={Rows}, Columns={Columns}");
            return;
        }

        // Get the tap location
        if (e is not TappedEventArgs tappedEventArgs)
        {
            _logger.LogWarning("DirectUniformGameGrid: Event is not TappedEventArgs");
            return;
        }

        // Calculate the cell position
        var location = tappedEventArgs.GetPosition(board);
        if (location == null)
        {
            _logger?.LogWarning("DirectUniformGameGrid: Tap location is null");
            return;
        }

        // Calculate cell size
        var cellWidth = board.Width / Columns;
        var cellHeight = board.Height / Rows;
        
        var column = (int)(location.Value.X / cellWidth);
        var row = (int)(location.Value.Y / cellHeight);
        
        // Ensure we're within bounds
        if (row < 0 || row >= Rows || column < 0 || column >= Columns)
        {
            _logger?.LogWarning("DirectUniformGameGrid: Tap outside grid bounds");
            return;
        }
        
        // Calculate a unique cell ID
        var cellId = row * Columns + column;
        
        // Check if this is a double tap (for flagging)
        var now = DateTime.Now;
        var isDoubleTap = (now - _lastTapTime).TotalMilliseconds < DoubleTapThresholdMs && 
                          _tappedCells.TryGetValue(cellId, out var wasTapped) && wasTapped;
        
        _lastTapTime = now;
        _tappedCells[cellId] = true;
        
        // Execute the appropriate command
        if (isDoubleTap)
        {
            _logger?.Log($"DirectUniformGameGrid: Double tap detected at row={row}, column={column}");
            if (FlagCommand?.CanExecute(null) == true)
                FlagCommand.Execute(new Point(row, column));
        }
        else
        {
            _logger?.Log($"DirectUniformGameGrid: Single tap detected at row={row}, column={column}");
            if (PlayCommand?.CanExecute(null) == true)
                PlayCommand.Execute(new Point(row, column));
        }
    }
    
    #endregion
}
