using Microsoft.Maui.Dispatching;
using MineSweeper.ViewModels;
using MineSweeper.Views.Controls;
using MineSweeper.Models;

namespace MineSweeper;

public partial class MainPageDebug : ContentPage
{
    private readonly GameViewModel? _viewModel;
    private GameGrid? _gameGrid;
    private readonly ILogger _logger = new DebugLogger();
    
    /// <summary>
    /// Simple debug logger implementation for testing
    /// </summary>
    private class DebugLogger : ILogger
    {
        public void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] {message}");
        }

        public void LogError(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] {message}");
        }

        public void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[WARNING] {message}");
        }
    }
    
    // Constructor without ViewModel for initial testing
    public MainPageDebug()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MainPageDebug: Basic constructor starting");
            
            // Initialize the XAML components
            InitializeComponent();
            
            // Set a simple background color to verify the page is loading
            BackgroundColor = Colors.LightBlue;
            
            System.Diagnostics.Debug.WriteLine("MainPageDebug: Basic constructor completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in basic constructor: {ex}");
            throw; // Rethrow to see the error
        }
    }
    
    // Constructor with ViewModel for later phases
    public MainPageDebug(GameViewModel viewModel)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MainPageDebug: ViewModel constructor starting");
            
            InitializeComponent();
            _viewModel = viewModel;
            
            // We'll uncomment this in later phases
            // BindingContext = _viewModel;
            
            System.Diagnostics.Debug.WriteLine("MainPageDebug: ViewModel constructor completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in ViewModel constructor: {ex}");
            throw; // Rethrow to see the error
        }
    }
    
    protected override void OnAppearing()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MainPageDebug: OnAppearing starting");
            
            base.OnAppearing();
            
            // Initialize all phases
            InitializePhase1();
            InitializePhase3();
            
            System.Diagnostics.Debug.WriteLine("MainPageDebug: OnAppearing completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in OnAppearing: {ex}");
        }
    }
    
    private void InitializePhase1()
    {
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Phase 1 initialization");
        // Basic initialization for Phase 1
        // Just verify the page loads correctly
        
        // Add more detailed logging
        System.Diagnostics.Debug.WriteLine($"MainPageDebug: regularGrid is null? {regularGrid == null}");
        if (regularGrid != null)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: regularGrid Width={regularGrid.Width}, Height={regularGrid.Height}");
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: regularGrid Children count={regularGrid.Children.Count}");
        }
    }
    
    private void InitializePhase3()
    {
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Phase 3 initialization");
        
        // Just log information about the uniformGrid (now a regular Grid)
        System.Diagnostics.Debug.WriteLine($"MainPageDebug: uniformGrid is null? {uniformGrid == null}");
        if (uniformGrid != null)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: uniformGrid Width={uniformGrid.Width}, Height={uniformGrid.Height}");
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: uniformGrid Children count={uniformGrid.Children.Count}");
            
            // Log each child's position
            for (int i = 0; i < uniformGrid.Children.Count; i++)
            {
                var child = uniformGrid.Children[i];
                var row = Microsoft.Maui.Controls.Grid.GetRow((View)child);
                var column = Microsoft.Maui.Controls.Grid.GetColumn((View)child);
                System.Diagnostics.Debug.WriteLine($"MainPageDebug: Child {i} at row={row}, column={column}");
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("MainPageDebug: uniformGrid is null");
        }
    }
    
    private void OnLaunchMainGridClicked(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MainPageDebug: OnLaunchMainGridClicked starting");
            
            // Toggle visibility of the game grid container
            gameGridContainer.IsVisible = !gameGridContainer.IsVisible;
            
            if (gameGridContainer.IsVisible)
            {
                // Create the game grid if it doesn't exist
                if (_gameGrid == null)
                {
                    System.Diagnostics.Debug.WriteLine("MainPageDebug: Creating new GameGrid");
                    
                    // Create a simple GameViewModel if we don't have one
                    var viewModel = _viewModel ?? new GameViewModel(
                        Dispatcher,
                        _logger,
                        new GameModelFactory(_logger));
                    
                    // Create the GameGrid
                    _gameGrid = new GameGrid
                    {
                        BindingContext = viewModel,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    };
                    
                    // Add the GameGrid to the container
                    gameGridContainer.Content = _gameGrid;
                    
                    // Start a new game with Easy difficulty
                    viewModel.NewGameCommand.Execute(GameEnums.GameDifficulty.Easy);
                    
                    System.Diagnostics.Debug.WriteLine("MainPageDebug: GameGrid created and added to container");
                }
                
                // Update button text
                ((Button)sender).Text = "Hide Main Grid";
            }
            else
            {
                // Update button text
                ((Button)sender).Text = "Launch Main Grid";
            }
            
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: GameGrid container visibility: {gameGridContainer.IsVisible}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in OnLaunchMainGridClicked: {ex}");
        }
    }
    
    private async void OnGoToMainPageClicked(object sender, EventArgs e)
    {
        // Navigate back to the main page
        await Shell.Current.GoToAsync("///MainPage");
    }
}
