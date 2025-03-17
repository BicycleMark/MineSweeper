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
    
    private void OnPageLoaded(object sender, EventArgs e)
    {
        // Start a new game with Easy difficulty
        _viewModel.NewGameCommand.Execute(GameEnums.GameDifficulty.Easy);
    }
}
