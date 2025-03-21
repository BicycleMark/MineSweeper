using MineSweeper.Models;
using MineSweeper.ViewModels;

namespace MineSweeper;

public partial class MainPage : ContentPage
{
    private readonly GameViewModel _viewModel;
    
    public MainPage(GameViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // Start a new game when the page is loaded
        Loaded += OnPageLoaded;
    }
    
    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        // Delay the game initialization to improve navigation performance
        await Task.Delay(100);
        
        // Start a new game with Easy difficulty
        _viewModel.NewGameCommand.Execute(GameEnums.GameDifficulty.Easy);
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
}
