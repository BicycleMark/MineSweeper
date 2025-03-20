using MineSweeper.Models;
using MineSweeper.ViewModels;

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
    
    // Navigation to Debug page is now handled through the flyout menu
    private void OnCellTapped(object sender, EventArgs e)
{
    // Find the tapped cell
    if (sender is Element element && element.BindingContext is SweeperItem item)
    {
        System.Diagnostics.Debug.WriteLine($"Cell tapped at: {item.Point}");
        // Call your view model's Play method directly
        if (BindingContext is GameViewModel vm)
        {
            vm.PlayCommand.Execute(item.Point);
        }
    }
}
}
