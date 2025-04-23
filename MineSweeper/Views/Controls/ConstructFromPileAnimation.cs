namespace MineSweeper.Extensions;

/// <summary>
/// Contains the ConstructFromPileAnimation method implementation.
/// </summary>
public static class ConstructFromPileAnimationExtension
{
    /// <summary>
    /// Performs an animation where cells are pulled one by one from a pile and placed into position.
    /// </summary>
    /// <param name="image">The image to animate.</param>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="totalRows">The total number of rows in the grid.</param>
    /// <param name="totalColumns">The total number of columns in the grid.</param>
    /// <returns>A task representing the animation operation.</returns>
    public static async Task ConstructFromPileAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        // Initial state: invisible
        image.Opacity = 0;
        
        // Define the pile location (bottom of the screen, centered horizontally)
        var pileX = totalColumns * 20; // Center of the grid horizontally
        var pileY = totalRows * 40;    // Below the grid
        
        // Set initial position at the pile
        image.TranslationX = pileX;
        image.TranslationY = pileY;
        image.Scale = 0.8;
        image.Rotation = _random.Next(-30, 31); // Random initial rotation
        
        // Calculate a random order for placing tiles
        var cellIndex = row * totalColumns + col;
        var totalCells = totalRows * totalColumns;
        
        // Create a random placement order (0 to totalCells-1)
        var placementOrder = new List<int>();
        for (int i = 0; i < totalCells; i++)
            placementOrder.Add(i);
        
        // Shuffle the placement order
        for (int i = 0; i < placementOrder.Count; i++)
        {
            int j = _random.Next(i, placementOrder.Count);
            int temp = placementOrder[i];
            placementOrder[i] = placementOrder[j];
            placementOrder[j] = temp;
        }
        
        // Find the position of this cell in the random order
        var placementIndex = placementOrder.IndexOf(cellIndex);
        
        // Calculate delay based on placement order
        var delay = placementIndex * 8; // 8ms between each tile placement (2.5x faster than before)
        await Task.Delay(delay);
        
        // Make the image visible
        image.Opacity = 1;
        
        // Animate the tile from the pile to its final position
        // Use a cubic easing for faster animation with shorter duration
        await Task.WhenAll(
            image.TranslateTo(0, 0, 80, Easing.CubicOut),
            image.RotateTo(0, 60, Easing.CubicOut),
            image.ScaleTo(1.0, 60, Easing.CubicOut)
        );
    }
    
    // Random number generator for animation effects
    private static readonly Random _random = new();
}
