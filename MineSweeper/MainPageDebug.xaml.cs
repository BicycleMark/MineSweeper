using MineSweeper.ViewModels;

namespace MineSweeper;

public partial class MainPageDebug : ContentPage
{
    private readonly GameViewModel? _viewModel;
    
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
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Phase 3 initialization - Simplified for debugging");
        
        // We've removed the UniformGrid for now to simplify debugging
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Using simplified layout without UniformGrid");
        
        // Log that we're using the regular grid instead
        System.Diagnostics.Debug.WriteLine($"MainPageDebug: Using regularGrid with 3x3 colored boxes");
    }
    
    private async void OnGoToMainPageClicked(object sender, EventArgs e)
    {
        // Navigate back to the main page
        await Shell.Current.GoToAsync("///MainPage");
    }
}
