using Microsoft.Maui.Dispatching;
using MineSweeper.ViewModels;
using MineSweeper.Views.Controls;
using MineSweeper.Models;

namespace MineSweeper;

public partial class MainPageDebug : ContentPage
{
    private readonly GameViewModel? _viewModel;
    private ContentView? _gameGrid;
    private bool _useStandardGrid = false;
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
    
    protected override void OnDisappearing()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("MainPageDebug: OnDisappearing starting");
            
            base.OnDisappearing();
            
            System.Diagnostics.Debug.WriteLine("MainPageDebug: OnDisappearing completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in OnDisappearing: {ex}");
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
    
    private void OnGridTypeChanged(object sender, EventArgs e)
    {
        try
        {
            // Get the selected grid type
            var selectedIndex = gridTypePicker.SelectedIndex;
            _useStandardGrid = selectedIndex == 1; // 1 = Standard Grid, 0 = UniformGrid
            
            // Log the selection
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Grid type changed to {(selectedIndex == 1 ? "Standard Grid" : "UniformGrid")}");
            
            // If the grid is already visible, recreate it with the new type
            if (gameGridContainer.IsVisible && _gameGrid != null)
            {
                // Store the current ViewModel
                var viewModel = _gameGrid.BindingContext;
                
                // Clear the existing grid
                gameGridContainer.Content = null;
                _gameGrid = null;
                
                // Create a new grid of the selected type
                CreateGameGrid(viewModel as GameViewModel);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in OnGridTypeChanged: {ex}");
        }
    }
    
    private void CreateGameGrid(GameViewModel? viewModel)
    {
        try
        {
            // Create a ViewModel if one wasn't provided
            viewModel ??= new GameViewModel(
                Dispatcher,
                _logger,
                new GameModelFactory(_logger));
            
            // Create DirectUniformGameGrid (optimized grid)
            System.Diagnostics.Debug.WriteLine("MainPageDebug: Creating new DirectUniformGameGrid");
            _gameGrid = new DirectUniformGameGrid(_logger)
            {
                BindingContext = viewModel,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            
            // Add the grid to the container
            gameGridContainer.Content = _gameGrid;
            
            // Start a new game with Easy difficulty
            viewModel.NewGameCommand.Execute(GameEnums.GameDifficulty.Easy);
            
            System.Diagnostics.Debug.WriteLine("MainPageDebug: DirectUniformGameGrid created and added to container");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in CreateGameGrid: {ex}");
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
                    // Create the appropriate grid type
                    CreateGameGrid(_viewModel);
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
    
    // Navigation to Main Page is now handled through the flyout menu
}
