namespace MineSweeper.Extensions;

/// <summary>
/// Contains the SineWaveBuilder animation method implementation.
/// </summary>
public static class SineWaveBuilderAnimationExtension
{
    /// <summary>
    /// Performs an animation where cells move in a sine wave pattern, one row at a time,
    /// alternating between left-to-right and right-to-left.
    /// </summary>
    /// <param name="image">The image to animate.</param>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="totalRows">The total number of rows in the grid.</param>
    /// <param name="totalColumns">The total number of columns in the grid.</param>
    /// <returns>A task representing the animation operation.</returns>
    public static async Task SineWaveBuilderAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        try
        {
            // Initial state: invisible
            image.Opacity = 0;
            
            // Determine if this row goes left-to-right or right-to-left
            bool leftToRight = (row % 2 == 0);
            
            // Calculate the actual column position based on direction
            int actualCol = leftToRight ? col : (totalColumns - 1 - col);
            
            // Calculate a unique index for each cell
            // We want to build from the bottom up, so reverse the row index
            int reversedRow = totalRows - 1 - row;
            
            // Determine if this row goes left-to-right or right-to-left
            bool reversedLeftToRight = (reversedRow % 2 == 0);
            
            // Calculate cell index (bottom rows first, then moving up)
            int cellIndex;
            if (reversedLeftToRight)
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
            var delay = cellIndex * 50; // 50ms between each cell
            
            // Calculate sine wave parameters
            double amplitude = 100; // Height of the sine wave (increased from 50 to 100)
            double frequency = 0.3; // Frequency of the sine wave
            
            // Set initial position based on sine wave
            // The sine wave is applied horizontally (X-axis)
            double initialX = amplitude * Math.Sin(frequency * actualCol);
            double initialY = -100; // Start above the grid
            
            image.TranslationX = initialX;
            image.TranslationY = initialY;
            
            // Wait for the calculated delay
            await Task.Delay(delay);
            
            // Make the image visible
            image.Opacity = 1;
            
            // Animate the cell moving down in a sine wave pattern
            uint duration = 500; // Animation duration in milliseconds
            uint steps = 20; // Number of animation steps
            uint stepDuration = duration / steps;
            
            for (int i = 0; i < steps; i++)
            {
                // Calculate progress (0 to 1)
                double progress = (double)i / steps;
                
                // Calculate current Y position (linear from top to bottom)
                double currentY = initialY + (progress * (Math.Abs(initialY)));
                
                // Calculate current X position (sine wave that diminishes as it approaches the bottom)
                double waveAmplitude = amplitude * (1 - progress); // Diminishing amplitude
                double currentX = waveAmplitude * Math.Sin(frequency * actualCol + (progress * Math.PI * 2));
                
                // Update position
                await image.TranslateTo(currentX, currentY, stepDuration);
            }
            
            // Final animation to settle into place
            await Task.WhenAll(
                image.TranslateTo(0, 0, 100, Easing.BounceOut),
                image.RotateTo(0, 100)
            );
        }
        catch (Exception ex)
        {
            // Fallback in case of any errors - just make the cell visible
            image.Opacity = 1;
            image.TranslationX = 0;
            image.TranslationY = 0;
            
            // Log the error (if logging is available)
            System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }
}
