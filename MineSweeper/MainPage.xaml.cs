using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using MineSweeper.Views.Controls;
using MineSweeper.Views.ImageLoaders;

namespace MineSweeper;

public partial class MainPage : ContentPage
{
    private readonly GameViewModel _viewModel;
    private readonly ILogger _logger;
    private readonly SvgLoader _svgLoader;
    private GridAnimationManager _animationManager;
    
    // Dictionary to track which cells have been tapped for double-tap detection
    private readonly Dictionary<int, bool> _tappedCells = new();
    private DateTime _lastTapTime = DateTime.MinValue;
    private const int DoubleTapThresholdMs = 300; // Double tap threshold in milliseconds
    
    public MainPage(GameViewModel viewModel, SvgLoader svgLoader)
    {
        InitializeComponent();
        _logger = new Models.CustomDebugLogger();
        _viewModel = viewModel;
        _svgLoader = svgLoader;
        BindingContext = _viewModel;
        
        // Initialize animation manager
        _animationManager = new GridAnimationManager(GameGrid);
        
        // Set up the chiseled border for the top panel
        SetupTopPanelBorder();
        
        // Start a new game when the page is loaded
        Loaded += OnPageLoaded;
    }
    
    /// <summary>
    /// Sets up the chiseled border for the top panel.
    /// </summary>
    private void SetupTopPanelBorder()
    {
        try
        {
            // Get the current app theme
            var isDarkTheme = Application.Current?.RequestedTheme == AppTheme.Dark;
            
            // Create a new ChiseledBorderDrawable for the top panel
            var borderDrawable = new ChiseledBorderDrawable
            {
                BorderThickness = 6,
                // Match the colors used in the game grid's ChiseledBorder
                ShadowColor = isDarkTheme ? Colors.Black : Colors.DimGray,
                HighlightColor = isDarkTheme ? Color.FromArgb("#444444") : Colors.LightGray,
                IsRecessed = true
            };
            
            // Set the drawable for the top panel border
            TopPanelBorder.Drawable = borderDrawable;
            
            // Force a redraw
            TopPanelBorder.Invalidate();
            
            System.Diagnostics.Debug.WriteLine("Top panel border set up successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting up top panel border: {ex}");
        }
    }
    
    
    /// <summary>
    /// Selects a random animation style for the game grid.
    /// </summary>
    public void SelectRandomGameAnimationStyle()
    {
        _animationManager.SelectRandomAnimationStyle();
    }
    
    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        try
        {
            // Delay the game initialization to improve navigation performance
            await Task.Delay(200);
            
            // Select a random animation style for this game
            _animationManager.SelectRandomAnimationStyle();
            
            // Start a new game with Easy difficulty
            await _viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);

            // Set up animations
            _animationManager.SetupAnimations();
            
            // Create the grid
            GameGrid.CreateGrid(_viewModel.Rows, _viewModel.Columns);
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
            
            // Clean up animation manager
            _animationManager.Cleanup();
            
            base.OnDisappearing();
            
            System.Diagnostics.Debug.WriteLine("MainPage: OnDisappearing completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPage: Exception in OnDisappearing: {ex}");
        }
    }
    
}
