namespace MineSweeper.Extensions;

/// <summary>
/// Contains the ABrickLayerfromAbove animation method implementation.
/// </summary>
public static class ABrickLayerfromAboveAnimationExtension
{
    /// <summary>
    /// Performs an animation where cells fall from the absolute top, building from the bottom up.
    /// </summary>
    /// <param name="image">The image to animate.</param>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="totalRows">The total number of rows in the grid.</param>
    /// <param name="totalColumns">The total number of columns in the grid.</param>
    /// <returns>A task representing the animation operation.</returns>
    public static async Task ABrickLayerfromAboveAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        try
        {
            // Initial state: invisible and positioned at the absolute top
            image.Opacity = 0;
            
        // Set initial position at the very top of the grid based on grid size
        var initialY = -(totalRows * 40); // Calculate based on grid size
        image.TranslationY = initialY;
            
            // Add a slight rotation for a more dynamic effect
            image.Rotation = _random.Next(-5, 6);
            
            // Calculate a unique index for each cell
            // We want to build from the bottom up, so reverse the row index
            int reversedRow = totalRows - 1 - row;
            
            // Determine if this row goes left-to-right or right-to-left
            bool leftToRight = (reversedRow % 2 == 0);
            
            // Calculate cell index (bottom rows first, then moving up)
            int cellIndex;
            if (leftToRight)
            {
                // Left to right rows
                cellIndex = (reversedRow * totalColumns) + col;
            }
            else
            {
                // Right to left rows
                cellIndex = (reversedRow * totalColumns) + (totalColumns - 1 - col);
            }
            
            // Calculate delay based on the cell index
            // Each cell waits for its turn, but with a much shorter delay
            var delay = cellIndex * 30; // 30ms between each cell (was 80ms)
            await Task.Delay(delay);
            
            // Make the image visible immediately before animation
            image.Opacity = 1;
            
            // Animate the brick falling into place
            // Fast drop from the top
            await image.TranslateTo(0, 5, 100, Easing.CubicIn);
            
            // Then settle into final position with a small bounce
            await Task.WhenAll(
                image.TranslateTo(0, 0, 80, Easing.BounceOut),
                image.RotateTo(0, 80, Easing.CubicOut)
            );
            
            // Add a small "settling" effect (faster)
            await image.ScaleTo(1.05, 30, Easing.CubicOut);
            await image.ScaleTo(1.0, 30, Easing.CubicIn);
        }
        catch (Exception ex)
        {
            // Fallback in case of any errors - just make the cell visible
            image.Opacity = 1;
            image.TranslationY = 0;
            image.Rotation = 0;
            image.Scale = 1.0;
            
            // Log the error (if logging is available)
            System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }
    
    // Random number generator for animation effects
    private static readonly Random _random = new();
}
