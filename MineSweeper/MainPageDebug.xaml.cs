using MineSweeper.ViewModels;

namespace MineSweeper;

public partial class MainPageDebug : ContentPage
{
    private readonly GameViewModel? _viewModel;
    
    // Constructor without ViewModel for initial testing
    public MainPageDebug()
    {
        InitializeComponent();
        
        // Log that we're in the basic constructor
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Basic constructor called");
    }
    
    // Constructor with ViewModel for later phases
    public MainPageDebug(GameViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        
        // We'll uncomment this in later phases
        // BindingContext = _viewModel;
        
        System.Diagnostics.Debug.WriteLine("MainPageDebug: ViewModel constructor called");
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("MainPageDebug: OnAppearing called");
        
        // We'll add phase-specific initialization here
        InitializePhase1();
    }
    
    private void InitializePhase1()
    {
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Phase 1 initialization");
        // Basic initialization for Phase 1
        // Just verify the page loads correctly
    }
    
    // We'll add these methods in later phases
    /*
    private void InitializePhase2()
    {
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Phase 2 initialization");
        
        // Hide the regular grid and show the UniformGrid
        regularGrid.IsVisible = false;
        uniformGrid.IsVisible = true;
        
        // Add colored boxes to the UniformGrid programmatically
        for (int i = 0; i < 25; i++)
        {
            var color = i % 5 switch
            {
                0 => Colors.Red,
                1 => Colors.Blue,
                2 => Colors.Green,
                3 => Colors.Yellow,
                _ => Colors.Purple
            };
            
            var box = new BoxView { Color = color };
            uniformGrid.Children.Add(box);
        }
    }
    */
    
    private async void OnGoToMainPageClicked(object sender, EventArgs e)
    {
        // Navigate back to the main page
        await Shell.Current.GoToAsync("///MainPage");
    }
}
