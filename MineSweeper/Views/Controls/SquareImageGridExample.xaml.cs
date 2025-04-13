using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace MineSweeper.Views.Controls;

public partial class SquareImageGridExample : ContentPage
{
    private Random _random = new Random();
    
    public SquareImageGridExample()
    {
        InitializeComponent();
      
        this.imageGrid.GetCellImage += (sender, e) =>
        {
            // Example: Set a default image for each cell
            e.Image = new Image
            {
                Source = "unplayed.png",
                Aspect = Aspect.AspectFill
            };
        };
        
        // Initialize the grid with the default size
        UpdateGrid(5);
        
        // Add tap gesture recognizer to the grid
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += OnGridTapped;
        imageGrid.GestureRecognizers.Add(tapGestureRecognizer);
    }
    
    /// <summary>
    /// Override the OnAppearing method to ensure the back button is visible
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Ensure the navigation bar is visible
        Shell.SetNavBarIsVisible(this, true);
        
        // Set up the back button behavior
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            Command = new Command(async () => await Shell.Current.GoToAsync("..")),
            IsVisible = true,
            IsEnabled = true
        });
    }
    
    /// <summary>
    /// Handles the tap event on the grid.
    /// </summary>
    private void OnGridTapped(object? sender, TappedEventArgs e)
    {
        // Get a random cell position
        int row = _random.Next(imageGrid.Rows);
        int col = _random.Next(imageGrid.Columns);
        
        // Update the cell image using the indexer
        UpdateRandomCell(row, col);
        
        // Update the status label
        statusLabel.Text = $"Updated cell at [{row}, {col}]";
    }
    
    /// <summary>
    /// Updates a random cell with a different image.
    /// </summary>
    private void UpdateRandomCell(int row, int col)
    {
        // Create a new image with a different source
        var newImage = new Image
        {
            Source = "flagged.png", // Use a different image
            Aspect = Aspect.AspectFill
        };
        
        // Use the indexer to update the cell
        imageGrid[row, col] = newImage;
    }

    /// <summary>
    /// Handles the grid size slider value changed event.
    /// </summary>
    private void OnGridSizeChanged(object sender, ValueChangedEventArgs e)
    {
        // Get the new grid size
        int gridSize = (int)Math.Round(e.NewValue);
        
        // Update the grid
        UpdateGrid(gridSize);
        
        // Update the label
        gridSizeLabel.Text = $"{gridSize}x{gridSize}";
        
        // Update the status label
        statusLabel.Text = $"Grid size changed to {gridSize}x{gridSize}";
    }
    
    /// <summary>
    /// Updates the grid with the specified size.
    /// </summary>
    private void UpdateGrid(int size)
    {
        // Create the grid with the specified size
        imageGrid.CreateGrid(size, size);
    }
    
    /// <summary>
    /// Event handler for the 3x3 button.
    /// </summary>
    private void OnSize3Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        gridSizeSlider.Value = 3;
    }
    
    /// <summary>
    /// Event handler for the 5x5 button.
    /// </summary>
    private void OnSize5Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        gridSizeSlider.Value = 5;
    }
    
    /// <summary>
    /// Event handler for the 8x8 button.
    /// </summary>
    private void OnSize8Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        gridSizeSlider.Value = 8;
    }
    
    /// <summary>
    /// Event handler for the 10x10 button.
    /// </summary>
    private void OnSize10Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        gridSizeSlider.Value = 10;
    }
    
    /// <summary>
    /// Event handler for the Update Random Cell button.
    /// </summary>
    private void OnUpdateRandomCellClicked(object sender, EventArgs e)
    {
        // Get a random cell position
        int row = _random.Next(imageGrid.Rows);
        int col = _random.Next(imageGrid.Columns);
        
        // Update the cell image using the indexer
        UpdateRandomCell(row, col);
        
        // Update the status label
        statusLabel.Text = $"Updated cell at [{row}, {col}] using indexer";
    }
}
