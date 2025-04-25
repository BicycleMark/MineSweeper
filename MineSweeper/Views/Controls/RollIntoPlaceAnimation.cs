namespace MineSweeper.Extensions;

/// <summary>
/// Contains the RollIntoPlace animation method implementation.
/// </summary>
public static class RollIntoPlaceAnimationExtension
{
    private static readonly Random _random = new Random();
    
    /// <summary>
    /// Performs an animation where tiles roll into place from alternating sides.
    /// Tiles start at the bottom left, roll across to the right, then on the next row
    /// start from the right and roll to the left, alternating for each row.
    /// </summary>
    /// <param name="image">The image to animate.</param>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="totalRows">The total number of rows in the grid.</param>
    /// <param name="totalColumns">The total number of columns in the grid.</param>
    /// <returns>A task representing the animation operation.</returns>
    public static async Task RollIntoPlaceAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        try
        {
            // Initial state: invisible
            image.Opacity = 0;
            
            // Determine if this row goes left-to-right or right-to-left
            // Even rows (0, 2, 4...) go left-to-right, odd rows go right-to-left
            bool leftToRight = (row % 2 == 0);
            
            // Calculate the actual column position based on direction
            int actualCol = leftToRight ? col : (totalColumns - 1 - col);
            
            // Calculate a unique index for each cell based on row and position in row
            int cellIndex = (row * totalColumns) + actualCol;
            
            // Calculate delay based on the cell index (70% faster)
            var delay = cellIndex * 15; // Reduced from 50ms to 15ms between each cell
            
            // Set initial position based on direction
            double initialX, initialY;
            
            if (leftToRight)
            {
                // For left-to-right rows, start from left bottom
                initialX = -50 - (actualCol * 10); // Further left for cells that are more to the right
                initialY = 20; // Slightly below final position
            }
            else
            {
                // For right-to-left rows, start from right bottom
                initialX = 50 + (actualCol * 10); // Further right for cells that are more to the left
                initialY = 20; // Slightly below final position
            }
            
            // Set initial rotation based on direction
            double initialRotation = leftToRight ? -90 : 90; // Rotated 90 degrees in the direction of travel
            
            // Set initial position and rotation
            image.TranslationX = initialX;
            image.TranslationY = initialY;
            image.Rotation = initialRotation;
            
            // Wait for the calculated delay
            await Task.Delay(delay);
            
            // Make the image visible
            image.Opacity = 1;
            
            // Calculate the number of rotations based on distance
            int rotations = leftToRight ? 1 : -1; // One full rotation in the direction of travel
            
            // Animate the tile rolling into place (70% faster)
            uint duration = 150; // Reduced from 500ms to 150ms (30% of original)
            
            // Perform the animation
            await Task.WhenAll(
                // Move to final position
                image.TranslateTo(0, 0, duration, Easing.CubicOut),
                
                // Rotate as it moves (360 degrees in the direction of travel)
                image.RotateTo(initialRotation + (rotations * 360), duration, Easing.CubicOut)
            );
            
            // Ensure final rotation is 0 (square orientation) - 70% faster
            await image.RotateTo(0, 30, Easing.CubicOut); // Reduced from 100ms to 30ms (30% of original)
        }
        catch (Exception ex)
        {
            // Fallback in case of any errors - just make the cell visible
            image.Opacity = 1;
            image.TranslationX = 0;
            image.TranslationY = 0;
            image.Rotation = 0;
            
            // Log the error (if logging is available)
            System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }
}
