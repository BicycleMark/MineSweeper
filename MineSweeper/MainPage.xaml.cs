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
        
        var lst = GetAllEmbeddedImages();
        var cat = GetCategorizedImages();
        
        // Set the help button image programmatically
       // SetHelpButtonImage();
        
        // Start a new game when the page is loaded
        Loaded += OnPageLoaded;
    }
    
    private void SetHelpButtonImage()
    {
        try
        {
            // Set the image source to dotnet_bot.png
            var imageSource = ImageSource.FromFile("dotnet_bot.png");
          
            
            System.Diagnostics.Debug.WriteLine("Help button image set to dotnet_bot.png");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting help button image: {ex}");
            
            // If image loading fails, fall back to text
            
            
            System.Diagnostics.Debug.WriteLine("Falling back to text '?' for help button");
        }
    }
    
    
    private List<string> GetAllEmbeddedImages()
    {
        var assembly = GetType().Assembly;
        var resourceNames = assembly.GetManifestResourceNames();
    
        // Filter for common image extensions
        var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp" };
        var imageResources = resourceNames
           // .Where(name => imageExtensions.Any(ext => name.ToLowerInvariant().EndsWith(ext)))
            .ToList();
    
        return imageResources;
    }
    
    private Dictionary<string, List<string>> GetCategorizedImages()
    {
        var assembly = GetType().Assembly;
        var resourceNames = assembly.GetManifestResourceNames();
        var imageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp" };
    
        var result = new Dictionary<string, List<string>>
        {
            ["All"] = resourceNames.Where(name => 
                imageExtensions.Any(ext => name.ToLowerInvariant().EndsWith(ext))).ToList(),
            ["AppIcons"] = resourceNames.Where(name => 
                name.Contains("AppIcon") || name.Contains("Icon")).ToList(),
            ["MauiAssets"] = resourceNames.Where(name => 
                name.Contains(".Resources.") && 
                imageExtensions.Any(ext => name.ToLowerInvariant().EndsWith(ext))).ToList(),
            ["Raw"] = resourceNames.Where(name => 
                (name.Contains(".Raw.") || name.Contains(".rawassets.")) && 
                imageExtensions.Any(ext => name.ToLowerInvariant().EndsWith(ext))).ToList()
        };
    
        return result;
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
            
            // The grid size is now set via the GridSize property in XAML
            // No need to call CreateGrid here as it's handled by the property change
            
            // Set up the GetCellImage event handler
            GameGrid.GetCellImage += (s, args) =>
            {
                try
                {
                    // Get the corresponding SweeperItem from the view model
                    var row = args.Row;
                    var col = args.Column;
                    
                    if (row >= 0 && row < _viewModel.Rows && col >= 0 && col < _viewModel.Columns)
                    {
                        // Create an image based on the cell position
                        var image = new Image
                        {
                            Source = "unplayed.png",
                            Aspect = Aspect.AspectFill
                        };
                        
                        args.Image = image;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in GetCellImage: {ex}");
                }
            };
            
            // Add tap gesture recognizer to handle cell taps
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnGridCellTapped;
            GameGrid.GestureRecognizers.Add(tapGestureRecognizer);
            
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"MainPage: Game creation completed in {stopwatch.ElapsedMilliseconds}ms");
            
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in OnPageLoaded: {ex}");
        }
    }
    
    /// <summary>
    /// Handles the tap event on a grid cell
    /// </summary>
    private void OnGridCellTapped(object? sender, TappedEventArgs e)
    {
        // Handle cell taps here
        System.Diagnostics.Debug.WriteLine("Grid cell tapped");
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
    
    /// <summary>
    /// Handles the click event for the Square Image Grid Example button
    /// </summary>
    private async void OnSquareImageGridExampleClicked(object sender, EventArgs e)
    {
        try
        {
            // Navigate to the SquareImageGridExample page
            await Shell.Current.GoToAsync("SquareImageGridExample");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error navigating to SquareImageGridExample: {ex}");
        }
    }
    
    /// <summary>
    /// Handles the click event for the LED Control Example button
    /// </summary>
    private async void OnLedControlExampleClicked(object sender, EventArgs e)
    {
        try
        {
            // Navigate to the LedControlExample page
            await Shell.Current.GoToAsync("LedControlExample");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error navigating to LedControlExample: {ex}");
        }
    }
    
    /// <summary>
    /// Handles the click event for the Help button
    /// </summary>
    private async void OnHelpButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Display a help message
            await DisplayAlert("Help", "Welcome to MineSweeper!\n\n" +
                "- Tap to reveal a cell\n" +
                "- Double-tap to flag a mine\n" +
                "- Clear all non-mine cells to win!", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error displaying help: {ex}");
        }
    }
}
