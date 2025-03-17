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
        
        // Initialize all phases
        InitializePhase1();
        InitializePhase3();
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
        System.Diagnostics.Debug.WriteLine("MainPageDebug: Phase 3 initialization - Direct UniformGrid test");
        
        // Log UniformGrid state
        System.Diagnostics.Debug.WriteLine($"MainPageDebug: uniformGrid is null? {uniformGrid == null}");
        if (uniformGrid != null)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: uniformGrid Width={uniformGrid.Width}, Height={uniformGrid.Height}");
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: uniformGrid Children count={uniformGrid.Children.Count}");
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: uniformGrid Rows={uniformGrid.Rows}, Columns={uniformGrid.Columns}");
            
            // Add colored boxes to the UniformGrid programmatically
            try
            {
                uniformGrid.Children.Clear();
                
                for (int i = 0; i < 25; i++)
                {
                    var row = i / 5;
                    var col = i % 5;
                    
                    Color color;
                    switch (i % 5)
                    {
                        case 0:
                            color = Colors.Red;
                            break;
                        case 1:
                            color = Colors.Blue;
                            break;
                        case 2:
                            color = Colors.Green;
                            break;
                        case 3:
                            color = Colors.Yellow;
                            break;
                        default:
                            color = Colors.Purple;
                            break;
                    }
                    
                    var box = new BoxView { Color = color };
                    Microsoft.Maui.Controls.Grid.SetRow(box, row);
                    Microsoft.Maui.Controls.Grid.SetColumn(box, col);
                    
                    uniformGrid.Children.Add(box);
                    System.Diagnostics.Debug.WriteLine($"MainPageDebug: Added box at row={row}, col={col}, color={color}");
                }
                
                System.Diagnostics.Debug.WriteLine($"MainPageDebug: Added {uniformGrid.Children.Count} boxes to UniformGrid");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception adding boxes to UniformGrid: {ex}");
            }
        }
    }
    
    private async void OnGoToMainPageClicked(object sender, EventArgs e)
    {
        // Navigate back to the main page
        await Shell.Current.GoToAsync("///MainPage");
    }
}
