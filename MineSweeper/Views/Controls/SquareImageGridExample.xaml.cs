using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;

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
        
        // Initialize the border controls
        borderThicknessSlider.Value = imageGrid.BorderThickness;
        borderThicknessLabel.Text = imageGrid.BorderThickness.ToString();
        showBorderSwitch.IsToggled = imageGrid.ShowBorder;
        isRecessedSwitch.IsToggled = imageGrid.IsRecessed;
        shadowColorButton.BackgroundColor = imageGrid.ShadowColor;
        highlightColorButton.BackgroundColor = imageGrid.HighlightColor;
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
    
    /// <summary>
    /// Handles the border thickness slider value changed event.
    /// </summary>
    private void OnBorderThicknessChanged(object sender, ValueChangedEventArgs e)
    {
        // Get the new border thickness
        int borderThickness = (int)Math.Round(e.NewValue);
        
        // Update the grid
        imageGrid.BorderThickness = borderThickness;
        
        // Update the label
        borderThicknessLabel.Text = borderThickness.ToString();
        
        // Update the status label
        statusLabel.Text = $"Border thickness changed to {borderThickness}";
    }
    
    /// <summary>
    /// Handles the show border switch toggled event.
    /// </summary>
    private void OnShowBorderToggled(object sender, ToggledEventArgs e)
    {
        // Update the grid
        imageGrid.ShowBorder = e.Value;
        
        // Update the status label
        statusLabel.Text = $"Show border set to {e.Value}";
    }
    
    /// <summary>
    /// Handles the is recessed switch toggled event.
    /// </summary>
    private void OnIsRecessedToggled(object sender, ToggledEventArgs e)
    {
        // Update the grid
        imageGrid.IsRecessed = e.Value;
        
        // Update the status label
        statusLabel.Text = $"Is recessed set to {e.Value}";
    }
    
    /// <summary>
    /// Handles the shadow color button click event.
    /// </summary>
    private void OnShadowColorClicked(object sender, EventArgs e)
    {
        // Rotate through some common shadow colors
        Color[] colors = new Color[]
        {
            Colors.DimGray, // Default
            Colors.Black,
            Colors.DarkGray,
            Colors.DarkBlue,
            Colors.DarkGreen,
            Colors.DarkRed
        };
        
        // Find the current color in the array
        int currentIndex = -1;
        for (int i = 0; i < colors.Length; i++)
        {
            if (imageGrid.ShadowColor.ToHex() == colors[i].ToHex())
            {
                currentIndex = i;
                break;
            }
        }
        
        // Move to the next color
        int nextIndex = (currentIndex + 1) % colors.Length;
        Color newColor = colors[nextIndex];
        
        // Update the grid
        imageGrid.ShadowColor = newColor;
        
        // Update the button
        shadowColorButton.BackgroundColor = newColor;
        
        // Update the status label
        statusLabel.Text = $"Shadow color changed to {newColor.ToHex()}";
    }
    
    /// <summary>
    /// Handles the highlight color button click event.
    /// </summary>
    private void OnHighlightColorClicked(object sender, EventArgs e)
    {
        // Rotate through some common highlight colors
        Color[] colors = new Color[]
        {
            Colors.LightGray, // Default
            Colors.White,
            Colors.Silver,
            Colors.LightBlue,
            Colors.LightGreen,
            Colors.LightYellow
        };
        
        // Find the current color in the array
        int currentIndex = -1;
        for (int i = 0; i < colors.Length; i++)
        {
            if (imageGrid.HighlightColor.ToHex() == colors[i].ToHex())
            {
                currentIndex = i;
                break;
            }
        }
        
        // Move to the next color
        int nextIndex = (currentIndex + 1) % colors.Length;
        Color newColor = colors[nextIndex];
        
        // Update the grid
        imageGrid.HighlightColor = newColor;
        
        // Update the button
        highlightColorButton.BackgroundColor = newColor;
        
        // Update the status label
        statusLabel.Text = $"Highlight color changed to {newColor.ToHex()}";
    }
}
