using System.Windows.Input;
using MineSweeper.Models;

namespace MineSweeper.Views.Controls;

public partial class GameGrid : ContentView
{
    public static readonly BindableProperty PlayCommandProperty = BindableProperty.Create(
        nameof(PlayCommand),
        typeof(ICommand),
        typeof(GameGrid),
        null);

    public static readonly BindableProperty FlagCommandProperty = BindableProperty.Create(
        nameof(FlagCommand),
        typeof(ICommand),
        typeof(GameGrid),
        null);

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

    // Dictionary to track which cells have been tapped
    private readonly Dictionary<int, bool> _tappedCells = new();
    private DateTime _lastTapTime = DateTime.MinValue;
    private const int DoubleTapThresholdMs = 300; // Double tap threshold in milliseconds

    public GameGrid()
    {
        InitializeComponent();
        
        // Add tap gesture recognizer for normal play and double-tap for flagging
        this.Loaded += OnLoaded;
    }
    
    private void OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            // Remove the event handler to avoid multiple registrations
            this.Loaded -= OnLoaded;
            
            // Now that the control is loaded, add the tap gesture recognizer
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += OnCellTapped;
            
            // Check if board is not null before adding the gesture recognizer
            if (board != null)
            {
                board.GestureRecognizers.Add(tapGesture);
                System.Diagnostics.Debug.WriteLine("GameGrid: Added tap gesture recognizer to board");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GameGrid: board is null in OnLoaded");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GameGrid: Exception in OnLoaded: {ex}");
        }
    }

    private void OnCellTapped(object? sender, TappedEventArgs e)
    {
        // Check if board is null
        if (board == null)
        {
            System.Diagnostics.Debug.WriteLine("GameGrid: board is null in OnCellTapped");
            return;
        }
        
        if (board.Width <= 0 || board.Height <= 0 || board.Rows <= 0 || board.Columns <= 0)
        {
            System.Diagnostics.Debug.WriteLine($"GameGrid: Invalid dimensions - Width={board.Width}, Height={board.Height}, Rows={board.Rows}, Columns={board.Columns}");
            return;
        }

        // Get the tap location
        if (e is not TappedEventArgs tappedEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("GameGrid: Event is not TappedEventArgs");
            return;
        }

        // Calculate the cell position
        var location = tappedEventArgs.GetPosition(board);
        if (location == null)
        {
            System.Diagnostics.Debug.WriteLine("GameGrid: Tap location is null");
            return;
        }

        // Use the UniformGrid's item size for more accurate calculations
        var cellWidth = board.Width / board.Columns;
        var cellHeight = board.Height / board.Rows;
        
        var column = (int)(location.Value.X / cellWidth);
        var row = (int)(location.Value.Y / cellHeight);
        
        // Log the tap location for debugging
        System.Diagnostics.Debug.WriteLine($"Tap at ({location.Value.X}, {location.Value.Y}), Cell: row={row}, column={column}");
        
        // Ensure we're within bounds
        if (row < 0 || row >= board.Rows || column < 0 || column >= board.Columns)
        {
            System.Diagnostics.Debug.WriteLine("Tap outside grid bounds");
            return;
        }
        
        // Calculate a unique cell ID
        var cellId = row * board.Columns + column;
        
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
