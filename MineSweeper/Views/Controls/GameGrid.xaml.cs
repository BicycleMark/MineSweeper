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
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnCellTapped;
        board.GestureRecognizers.Add(tapGesture);
    }

    private void OnCellTapped(object sender, EventArgs e)
    {
        if (board.Width <= 0 || board.Height <= 0 || board.Rows <= 0 || board.Columns <= 0)
            return;

        // Get the tap location
        if (e is not TappedEventArgs tappedEventArgs)
            return;

        // Calculate the cell position
        var location = tappedEventArgs.GetPosition(board);
        if (location == null)
            return;

        var cellWidth = board.Width / board.Columns;
        var cellHeight = board.Height / board.Rows;
        
        var column = (int)(location.Value.X / cellWidth);
        var row = (int)(location.Value.Y / cellHeight);
        
        // Ensure we're within bounds
        if (row < 0 || row >= board.Rows || column < 0 || column >= board.Columns)
            return;
        
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
            if (FlagCommand?.CanExecute(null) == true)
                FlagCommand.Execute(new Point(row, column));
        }
        else
        {
            if (PlayCommand?.CanExecute(null) == true)
                PlayCommand.Execute(new Point(row, column));
        }
    }
}
