using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using MineSweeper.Views.Controls;
using MineSweeper.Views.ImageLoaders;

namespace MineSweeper;

public partial class MainPage : ContentPage
{
    private readonly GameViewModel _viewModel;

    private readonly ILogger _logger;
    
    public MainPage(GameViewModel viewModel, SvgLoader svgLoader)
    {
        InitializeComponent();
        _logger = new CustomDebugLogger();
        _viewModel = viewModel;
        _svgLoader = svgLoader;
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
            SetGridSize(_viewModel.Rows, _viewModel.Columns);
            
            
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
    private readonly SvgLoader _svgLoader;
    private const int DoubleTapThresholdMs = 300; // Double tap threshold in milliseconds
    
    // This method is kept for reference but is no longer used since we're using SquareGameGrid
  
}
