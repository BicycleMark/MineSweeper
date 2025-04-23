namespace MineSweeper.Extensions;

/// <summary>
/// Contains the LikeAWallThatFallsIntoPlaceFromAbove animation method implementation.
/// </summary>
public static class LikeAWallThatFallsIntoPlaceFromAboveAnimationExtension
{
    /// <summary>
    /// Performs an animation where cells fall into place from above like a wall being constructed.
    /// </summary>
    /// <param name="image">The image to animate.</param>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="totalRows">The total number of rows in the grid.</param>
    /// <param name="totalColumns">The total number of columns in the grid.</param>
    /// <returns>A task representing the animation operation.</returns>
    public static async Task LikeAWallThatFallsIntoPlaceFromAboveAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        // Initial state: invisible and positioned above its final position
        image.Opacity = 0;
        
        // Set initial position above the grid
        var initialY = -100 - (row * 20); // Higher rows start higher up
        image.TranslationY = initialY;
        
        // Add a slight rotation for a more dynamic effect
        image.Rotation = _random.Next(-10, 11);
        
        // Make the image visible
        image.Opacity = 1;
        
        // Calculate delay based on row (top rows fall first)
        var delay = row * 50; // 50ms between each row
        await Task.Delay(delay);
        
        // Animate the tile falling into place
        // Use a bounce effect to simulate hitting the ground
        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.BounceOut),
            image.RotateTo(0, 250, Easing.CubicOut)
        );
    }
    
    // Random number generator for animation effects
    private static readonly Random _random = new();
}
