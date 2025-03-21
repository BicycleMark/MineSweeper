using MineSweeper.Models;
using MineSweeper.ViewModels;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class MainPage : ContentPage
{
    private readonly GameViewModel _viewModel;

    private readonly ILogger _logger;
    
    public MainPage(GameViewModel viewModel)
    {
        InitializeComponent();
        _logger = new CustomDebugLogger();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // Start a new game when the page is loaded
        Loaded += OnPageLoaded;
    }
    
    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        try
        {
            // Delay the game initialization to improve navigation performance
            // This allows the UI to render before starting the potentially heavy game creation
            await Task.Delay(200);
            
            // Start a new game with Easy difficulty
            // The NewGameCommand will handle running on a background thread
            System.Diagnostics.Debug.WriteLine("MainPage: Starting game creation");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            await _viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
            
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"MainPage: Game creation completed in {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnPageLoaded: {ex}");
        }
    }
    
    protected override void OnAppearing()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MainPage: OnAppearing starting");
            
            base.OnAppearing();
            
            System.Diagnostics.Debug.WriteLine("MainPage: OnAppearing completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPage: Exception in OnAppearing: {ex}");
        }
    }
    
    protected override void OnDisappearing()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MainPage: OnDisappearing starting");
            
            base.OnDisappearing();
            
            System.Diagnostics.Debug.WriteLine("MainPage: OnDisappearing completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPage: Exception in OnDisappearing: {ex}");
        }
    }
    
    // Dictionary to track which cells have been tapped for double-tap detection
    private readonly Dictionary<int, bool> _tappedCells = new();
    private DateTime _lastTapTime = DateTime.MinValue;
    private const int DoubleTapThresholdMs = 300; // Double tap threshold in milliseconds
    
    // This method is kept for reference but is no longer used since we're using SquareGameGrid
    private void OnCellTapped(object sender, EventArgs e)
    {
        // Find the tapped cell
        if (sender is Element element && element.BindingContext is SweeperItem item)
        {
            _logger?.Log($"Cell tapped at: {item.Point}");
            // Call your view model's Play method directly
            if (BindingContext is GameViewModel vm)
            {
                vm.PlayCommand.Execute(item.Point);
            }
        }
    }
    
    // New method to handle taps on the SquareGameGrid
    private void OnGridTapped(object sender, TappedEventArgs e)
    {
        try
        {
            // Check if gameGrid is null
            if (gameGrid == null)
            {
                _logger?.LogWarning("MainPage: gameGrid is null in OnGridTapped");
                return;
            }
            
            if (gameGrid.Width <= 0 || gameGrid.Height <= 0 || gameGrid.Rows <= 0 || gameGrid.Columns <= 0)
            {
                _logger?.LogWarning($"MainPage: Invalid dimensions - Width={gameGrid.Width}, Height={gameGrid.Height}, Rows={gameGrid.Rows}, Columns={gameGrid.Columns}");
                return;
            }

            // Get the tap location
            var location = e.GetPosition(gameGrid);
            if (location == null)
            {
                _logger?.LogWarning("MainPage: Tap location is null");
                return;
            }

            // Calculate cell size
            var cellWidth = gameGrid.Width / gameGrid.Columns;
            var cellHeight = gameGrid.Height / gameGrid.Rows;
            
            var column = (int)(location.Value.X / cellWidth);
            var row = (int)(location.Value.Y / cellHeight);
            
            // Ensure we're within bounds
            if (row < 0 || row >= gameGrid.Rows || column < 0 || column >= gameGrid.Columns)
            {
                _logger?.LogWarning("MainPage: Tap outside grid bounds");
                return;
            }
            
            // Calculate a unique cell ID
            var cellId = row * gameGrid.Columns + column;
            
            // Check if this is a double tap (for flagging)
            var now = DateTime.Now;
            var isDoubleTap = (now - _lastTapTime).TotalMilliseconds < DoubleTapThresholdMs && 
                             _tappedCells.TryGetValue(cellId, out var wasTapped) && wasTapped;
            
            _lastTapTime = now;
            _tappedCells[cellId] = true;
            
            // Create a Point to represent the cell position
            var point = new Point(row, column);
            
            // Execute the appropriate command
            if (BindingContext is GameViewModel vm)
            {
                if (isDoubleTap)
                {
                    _logger?.Log($"MainPage: Double tap detected at row={row}, column={column}");
                    vm.FlagCommand?.Execute(point);
                }
                else
                {
                    _logger?.Log($"MainPage: Single tap detected at row={row}, column={column}");
                    vm.PlayCommand?.Execute(point);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError($"MainPage: Exception in OnGridTapped: {ex}");
        }
    }
}
