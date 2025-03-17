using System.Windows.Input;
using MineSweeper.Models;

namespace MineSweeper.Views.Controls;

public partial class StandardGameGrid : ContentView
{
    public static readonly BindableProperty PlayCommandProperty = BindableProperty.Create(
        nameof(PlayCommand),
        typeof(ICommand),
        typeof(StandardGameGrid),
        null);

    public static readonly BindableProperty FlagCommandProperty = BindableProperty.Create(
        nameof(FlagCommand),
        typeof(ICommand),
        typeof(StandardGameGrid),
        null);

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<SweeperItem>),
        typeof(StandardGameGrid),
        null,
        propertyChanged: OnItemsSourceChanged);

    public static readonly BindableProperty RowsProperty = BindableProperty.Create(
        nameof(Rows),
        typeof(int),
        typeof(StandardGameGrid),
        10,
        propertyChanged: OnGridSizeChanged);

    public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(
        nameof(Columns),
        typeof(int),
        typeof(StandardGameGrid),
        10,
        propertyChanged: OnGridSizeChanged);

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

    // Dictionary to track which cells have been tapped
    private readonly Dictionary<int, bool> _tappedCells = new();
    private DateTime _lastTapTime = DateTime.MinValue;
    private const int DoubleTapThresholdMs = 300; // Double tap threshold in milliseconds

    public StandardGameGrid()
    {
        InitializeComponent();
        
        // Add tap gesture recognizer for normal play and double-tap for flagging
        this.Loaded += OnLoaded;
    }
    
    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StandardGameGrid grid)
        {
            grid.UpdateGrid();
        }
    }
    
    private static void OnGridSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is StandardGameGrid grid)
        {
            grid.UpdateGridDefinitions();
            grid.UpdateGrid();
        }
    }
    
    private void UpdateGridDefinitions()
    {
        try
        {
            // Clear existing row and column definitions
            board.RowDefinitions.Clear();
            board.ColumnDefinitions.Clear();
            
            // Add row definitions
            for (int i = 0; i < Rows; i++)
            {
                board.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            
            // Add column definitions
            for (int i = 0; i < Columns; i++)
            {
                board.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            
            System.Diagnostics.Debug.WriteLine($"StandardGameGrid: Updated grid definitions - Rows={Rows}, Columns={Columns}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"StandardGameGrid: Exception in UpdateGridDefinitions: {ex}");
        }
    }
    
    private void UpdateGrid()
    {
        try
        {
            // Clear existing children
            board.Children.Clear();
            
            // If items source is null, return
            if (ItemsSource == null)
            {
                System.Diagnostics.Debug.WriteLine("StandardGameGrid: ItemsSource is null");
                return;
            }
            
            // Get the item template
            var itemTemplate = Resources["SweeperItemTemplate"] as DataTemplate;
            if (itemTemplate == null)
            {
                System.Diagnostics.Debug.WriteLine("StandardGameGrid: SweeperItemTemplate is null");
                return;
            }
            
            // Check if Columns is zero to prevent divide by zero error
            if (Columns <= 0)
            {
                System.Diagnostics.Debug.WriteLine("StandardGameGrid: Columns must be greater than zero");
                return;
            }
            
            // Add items to the grid
            int index = 0;
            foreach (var item in ItemsSource)
            {
                // Create the view from the template
                var view = itemTemplate.CreateContent() as View;
                if (view == null)
                {
                    System.Diagnostics.Debug.WriteLine("StandardGameGrid: Failed to create view from template");
                    continue;
                }
                
                // Set the binding context
                view.BindingContext = item;
                
                // Calculate row and column
                int row = index / Columns;
                int column = index % Columns;
                
                // Set grid position
                Grid.SetRow(view, row);
                Grid.SetColumn(view, column);
                
                // Add to the grid
                board.Children.Add(view);
                
                index++;
            }
            
            System.Diagnostics.Debug.WriteLine($"StandardGameGrid: Updated grid with {index} items");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"StandardGameGrid: Exception in UpdateGrid: {ex}");
        }
    }
    
    private void OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            // Remove the event handler to avoid multiple registrations
            this.Loaded -= OnLoaded;
            
            // Initialize the grid
            UpdateGridDefinitions();
            
            // Now that the control is loaded, add the tap gesture recognizer
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnCellTapped;
            
            // Check if board is not null before adding the gesture recognizer
            if (board != null)
            {
                board.GestureRecognizers.Add(tapGesture);
                System.Diagnostics.Debug.WriteLine("StandardGameGrid: Added tap gesture recognizer to board");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("StandardGameGrid: board is null in OnLoaded");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"StandardGameGrid: Exception in OnLoaded: {ex}");
        }
    }

    private void OnCellTapped(object? sender, TappedEventArgs e)
    {
        // Check if board is null
        if (board == null)
        {
            System.Diagnostics.Debug.WriteLine("StandardGameGrid: board is null in OnCellTapped");
            return;
        }
        
        if (board.Width <= 0 || board.Height <= 0 || Rows <= 0 || Columns <= 0)
        {
            System.Diagnostics.Debug.WriteLine($"StandardGameGrid: Invalid dimensions - Width={board.Width}, Height={board.Height}, Rows={Rows}, Columns={Columns}");
            return;
        }

        // Get the tap location
        if (e is not TappedEventArgs tappedEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("StandardGameGrid: Event is not TappedEventArgs");
            return;
        }

        // Calculate the cell position
        var location = tappedEventArgs.GetPosition(board);
        if (location == null)
        {
            System.Diagnostics.Debug.WriteLine("StandardGameGrid: Tap location is null");
            return;
        }

        // Calculate cell size
        var cellWidth = board.Width / Columns;
        var cellHeight = board.Height / Rows;
        
        var column = (int)(location.Value.X / cellWidth);
        var row = (int)(location.Value.Y / cellHeight);
        
        // Log the tap location for debugging
        System.Diagnostics.Debug.WriteLine($"Tap at ({location.Value.X}, {location.Value.Y}), Cell: row={row}, column={column}");
        
        // Ensure we're within bounds
        if (row < 0 || row >= Rows || column < 0 || column >= Columns)
        {
            System.Diagnostics.Debug.WriteLine("Tap outside grid bounds");
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
            System.Diagnostics.Debug.WriteLine($"Double tap detected at row={row}, column={column}");
            if (FlagCommand?.CanExecute(null) == true)
                FlagCommand.Execute(new Point(row, column));
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Single tap detected at row={row}, column={column}");
            if (PlayCommand?.CanExecute(null) == true)
                PlayCommand.Execute(new Point(row, column));
        }
    }
}
