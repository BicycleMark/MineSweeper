namespace MineSweeper.Extensions;

/// <summary>
///     Extension methods for animating grid cells in IAnimatableGrid implementations.
/// </summary>
public static class GridAnimationExtensions
{
    /// <summary>
    ///     Defines patterns for how cells are animated in sequence.
    /// </summary>
    public enum AnimationPattern
    {
        /// <summary>Cell animations follow a sequential order based on row and column indices.</summary>
        Sequential,
        
        /// <summary>Animation starts from top-left corner and spreads like a wave.</summary>
        WaveFromTopLeft,
        
        /// <summary>Animation spreads outward from the center.</summary>
        WaveFromCenter,
        
        /// <summary>Animation follows a spiral pattern.</summary>
        Spiral,
        
        /// <summary>Cells animate in random order.</summary>
        Random,
        
        /// <summary>Cells animate in a checkerboard pattern.</summary>
        Checkerboard
    }

    /// <summary>
    ///     Defines visual animation types that can be applied to grid cells.
    /// </summary>
    public enum AnimationType
    {
        /// <summary>Cells flip into view along the Y-axis.</summary>
        FlipIn,
        
        /// <summary>Cells fade into view.</summary>
        FadeIn,
        
        /// <summary>Cells scale from small to full size.</summary>
        ScaleIn,
        
        /// <summary>Cells bounce into view with an elastic effect.</summary>
        BounceIn,
        
        /// <summary>Cells spin into view.</summary>
        SpinIn,
        
        /// <summary>Cells slide into view.</summary>
        SlideIn,
        
        /// <summary>Each cell uses a randomly selected animation.</summary>
        RandomPerCell,
        
        /// <summary>Animation sweeps diagonally from top-left to bottom-right.</summary>
        SwipeDiagonalTopLeftBottomRight,
        
        /// <summary>Animation sweeps diagonally from top-right to bottom-left.</summary>
        SwipeDiagonalTopRightBottomLeft,
        
        /// <summary>Animation sweeps diagonally from bottom-left to top-right.</summary>
        SwipeDiagonalBottomLeftTopRight,
        
        /// <summary>Animation sweeps diagonally from bottom-right to top-left.</summary>
        SwipeDiagonalBottomRightTopLeft,
        
        /// <summary>Animation spirals from the center outward.</summary>
        SwirlInnerToOuter,
        
        /// <summary>Animation spirals from outside toward the center.</summary>
        SwirlOuterToInner,
        
        /// <summary>Animation sweeps from left to right.</summary>
        LeftToRight,
        
        /// <summary>Animation sweeps from right to left.</summary>
        RightToLeft,
        
        /// <summary>Animation sweeps from top to bottom.</summary>
        TopToBottom,
        
    /// <summary>Animation sweeps from bottom to top.</summary>
    BottomToTop,
    
    /// <summary>Cells appear pixelated and gradually sharpen into view.</summary>
    Pixelated,
    
    /// <summary>Cells scale in and out like breathing.</summary>
    BreathInBreathOut,
    
    /// <summary>Cells vibrate with decreasing intensity.</summary>
    AttenuatedVibration,
    
    /// <summary>Cells swirl into place like water going down a drain.</summary>
    SwirlLikeADrainIntoPlace,
    
    /// <summary>Cells appear with a radar-like sweeping pattern with varying opacity.</summary>
    RadarPaternWitRadarLikeOpacity
    }

    /// <summary>
    ///     Random number generator for animation effects.
    /// </summary>
    private static readonly Random _random = new();

    /// <summary>
    ///     Extension method to animate a cell.
    /// </summary>
    /// <param name="image">The image to animate.</param>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="animationType">The type of animation to apply.</param>
    /// <param name="totalRows">The total number of rows in the grid.</param>
    /// <param name="totalColumns">The total number of columns in the grid.</param>
    /// <returns>A task representing the animation operation.</returns>
    public static Task AnimateCellAsync(this Image image, int row, int col,
        AnimationType animationType, int totalRows, int totalColumns)
    {
        return animationType switch
        {
            AnimationType.FlipIn => FlipInAnimation(image, row, col),
            AnimationType.FadeIn => FadeInAnimation(image, row, col),
            AnimationType.ScaleIn => ScaleInAnimation(image, row, col),
            AnimationType.BounceIn => BounceInAnimation(image, row, col),
            AnimationType.SpinIn => SpinInAnimation(image, row, col),
            AnimationType.SlideIn => SlideInAnimation(image, row, col),
            AnimationType.RandomPerCell => RandomPerCellAnimation(image, row, col, totalRows, totalColumns),
            AnimationType.SwipeDiagonalTopLeftBottomRight => SwipeDiagonalTopLeftBottomRightAnimation(image, row, col),
            AnimationType.SwipeDiagonalTopRightBottomLeft => SwipeDiagonalTopRightBottomLeftAnimation(image, row, col,
                totalColumns),
            AnimationType.SwipeDiagonalBottomLeftTopRight => SwipeDiagonalBottomLeftTopRightAnimation(image, row, col,
                totalRows),
            AnimationType.SwipeDiagonalBottomRightTopLeft => SwipeDiagonalBottomRightTopLeftAnimation(image, row, col,
                totalRows, totalColumns),
            AnimationType.SwirlInnerToOuter => SwirlInnerToOuterAnimation(image, row, col, totalRows, totalColumns),
            AnimationType.SwirlOuterToInner => SwirlOuterToInnerAnimation(image, row, col, totalRows, totalColumns),
            AnimationType.LeftToRight => LeftToRightAnimation(image, row, col, totalColumns),
            AnimationType.RightToLeft => RightToLeftAnimation(image, row, col, totalColumns),
            AnimationType.TopToBottom => TopToBottomAnimation(image, row, col, totalRows),
            AnimationType.BottomToTop => BottomToTopAnimation(image, row, col, totalRows),
            AnimationType.Pixelated => PixelatedAnimation(image, row, col),
            AnimationType.BreathInBreathOut => BreathInBreathOutAnimation(image, row, col),
            AnimationType.AttenuatedVibration => AttenuatedVibrationAnimation(image, row, col),
            AnimationType.SwirlLikeADrainIntoPlace => SwirlLikeADrainIntoPlaceAnimation(image, row, col, totalRows, totalColumns),
            AnimationType.RadarPaternWitRadarLikeOpacity => RadarPaternWitRadarLikeOpacityAnimation(image, row, col, totalRows, totalColumns),
            _ => FadeInAnimation(image, row, col)
        };
    }

    /// <summary>
    ///     Calculates the animation delay for a cell based on its position and the animation pattern.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="pattern">The animation pattern to use.</param>
    /// <param name="totalRows">The total number of rows in the grid.</param>
    /// <param name="totalCols">The total number of columns in the grid.</param>
    /// <returns>The delay in milliseconds.</returns>
    public static int CalculateAnimationDelay(int row, int col,
        AnimationPattern pattern, int totalRows, int totalCols)
    {
        const int baseDelay = 10;

        switch (pattern)
        {
            case AnimationPattern.WaveFromTopLeft:
                return (row + col) * baseDelay;

            case AnimationPattern.WaveFromCenter:
            {
                var centerRow = totalRows / 2;
                var centerCol = totalCols / 2;
                var distanceFromCenter = Math.Abs(row - centerRow) + Math.Abs(col - centerCol);
                return distanceFromCenter * baseDelay;
            }

            case AnimationPattern.Spiral:
            {
                var maxDist = Math.Max(Math.Max(row, totalRows - 1 - row),
                    Math.Max(col, totalCols - 1 - col));
                return maxDist * baseDelay * 2;
            }

            case AnimationPattern.Random:
                return _random.Next(0, baseDelay * 10);

            case AnimationPattern.Checkerboard:
                return (row + col) % 2 == 0 ? 0 : baseDelay * 5;

            case AnimationPattern.Sequential:
                return (row * totalCols + col) * baseDelay / 2;

            default:
                return (row * totalCols + col) * baseDelay / 2;
        }
    }

    // Animation implementations
    
    /// <summary>
    ///     Performs a flip-in animation on the specified cell.
    /// </summary>
    private static async Task FlipInAnimation(Image image, int row, int col)
    {
        image.Opacity = 0.3;
        image.RotationY = 90;
        image.AnchorX = 0.5;

        var delay = row * 5 + col * 5;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.RotateYTo(0, 250, Easing.BounceOut),
            image.FadeTo(1, 200)
        );
    }

    /// <summary>
    ///     Performs a fade-in animation on the specified cell.
    /// </summary>
    private static async Task FadeInAnimation(Image image, int row, int col)
    {
        image.Opacity = 0;

        var delay = row * 8 + col * 8;
        await Task.Delay(delay);

        await image.FadeTo(1, 300, Easing.CubicOut);
    }

    /// <summary>
    ///     Performs a scale-in animation on the specified cell.
    /// </summary>
    private static async Task ScaleInAnimation(Image image, int row, int col)
    {
        image.Opacity = 0;
        image.Scale = 0.5;

        var delay = row * 7 + col * 7;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.ScaleTo(1.0, 300, Easing.SpringOut),
            image.FadeTo(1, 200)
        );
    }

    /// <summary>
    ///     Performs a bounce-in animation on the specified cell.
    /// </summary>
    private static async Task BounceInAnimation(Image image, int row, int col)
    {
        image.Opacity = 0.5;
        image.Scale = 1.5;

        var delay = row * 6 + col * 6;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.ScaleTo(1.0, 400, Easing.BounceOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a spin-in animation on the specified cell.
    /// </summary>
    private static async Task SpinInAnimation(Image image, int row, int col)
    {
        image.Opacity = 0;
        image.Rotation = 180;

        var delay = (row + col) * 4;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.RotateTo(0, 350, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a slide-in animation on the specified cell.
    /// </summary>
    private static async Task SlideInAnimation(Image image, int row, int col)
    {
        image.Opacity = 0.3;
        image.TranslationX = col % 2 == 0 ? -40 : 40;

        var delay = row * 10;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a random animation for each cell.
    /// </summary>
    private static async Task RandomPerCellAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        // Select a random animation type (excluding RandomPerCell)
        var animationTypes = Enum.GetValues<AnimationType>()
            .Where(t => t != AnimationType.RandomPerCell)
            .ToArray();

        var randomType = animationTypes[_random.Next(animationTypes.Length)];

        // Use the extension method recursively
        await image.AnimateCellAsync(row, col, randomType, totalRows, totalColumns);
    }

    /// <summary>
    ///     Performs a diagonal swipe animation from top-left to bottom-right.
    /// </summary>
    private static async Task SwipeDiagonalTopLeftBottomRightAnimation(Image image, int row, int col)
    {
        image.Opacity = 0;
        image.TranslationX = -30;
        image.TranslationY = -30;

        // Delay based on diagonal distance from top-left
        var delay = (row + col) * 15;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a diagonal swipe animation from top-right to bottom-left.
    /// </summary>
    private static async Task SwipeDiagonalTopRightBottomLeftAnimation(Image image, int row, int col, int totalCols)
    {
        image.Opacity = 0;
        image.TranslationX = 30;
        image.TranslationY = -30;

        // Delay based on diagonal distance from top-right
        var delay = (row + (totalCols - 1 - col)) * 15;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a diagonal swipe animation from bottom-left to top-right.
    /// </summary>
    private static async Task SwipeDiagonalBottomLeftTopRightAnimation(Image image, int row, int col, int totalRows)
    {
        image.Opacity = 0;
        image.TranslationX = -30;
        image.TranslationY = 30;

        // Delay based on diagonal distance from bottom-left
        var delay = (totalRows - 1 - row + col) * 15;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a diagonal swipe animation from bottom-right to top-left.
    /// </summary>
    private static async Task SwipeDiagonalBottomRightTopLeftAnimation(Image image, int row, int col, int totalRows,
        int totalCols)
    {
        image.Opacity = 0;
        image.TranslationX = 30;
        image.TranslationY = 30;

        // Delay based on diagonal distance from bottom-right
        var delay = (totalRows - 1 - row + (totalCols - 1 - col)) * 15;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a swirl animation from inner cells to outer cells.
    /// </summary>
    private static async Task SwirlInnerToOuterAnimation(Image image, int row, int col, int totalRows, int totalCols)
    {
        image.Opacity = 0;
        image.Scale = 0.7;

        // Calculate center coordinates
        var centerRow = totalRows / 2;
        var centerCol = totalCols / 2;

        // Calculate layer (distance from center in a spiral pattern)
        var rowDistance = Math.Abs(row - centerRow);
        var colDistance = Math.Abs(col - centerCol);
        var layer = Math.Max(rowDistance, colDistance);

        // Calculate position within the layer (for proper sequencing)
        var position = 0;

        if (row == centerRow - layer) // Top edge of the layer
            position = col - (centerCol - layer);
        else if (col == centerCol + layer) // Right edge
            position = 2 * layer + (row - (centerRow - layer));
        else if (row == centerRow + layer) // Bottom edge
            position = 4 * layer - (col - (centerCol - layer));
        else // Left edge
            position = 6 * layer - (row - (centerRow - layer));

        // Base delay per layer plus position in layer
        var delay = layer * 80 + position * 20;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.ScaleTo(1.0, 350, Easing.CubicOut),
            image.FadeTo(1, 300)
        );
    }

    /// <summary>
    ///     Performs a swirl animation from outer cells to inner cells.
    /// </summary>
    private static async Task SwirlOuterToInnerAnimation(Image image, int row, int col, int totalRows, int totalCols)
    {
        image.Opacity = 0;
        image.Scale = 0.7;

        // Calculate center coordinates
        var centerRow = totalRows / 2;
        var centerCol = totalCols / 2;

        // Calculate max possible layer
        var maxLayer = Math.Max(Math.Max(centerRow, totalRows - centerRow - 1),
            Math.Max(centerCol, totalCols - centerCol - 1));

        // Calculate current layer (distance from center in a spiral pattern)
        var rowDistance = Math.Abs(row - centerRow);
        var colDistance = Math.Abs(col - centerCol);
        var layer = Math.Max(rowDistance, colDistance);

        // Calculate position within the layer (for proper sequencing)
        var position = 0;

        if (row == centerRow - layer) // Top edge of the layer
            position = col - (centerCol - layer);
        else if (col == centerCol + layer) // Right edge
            position = 2 * layer + (row - (centerRow - layer));
        else if (row == centerRow + layer) // Bottom edge
            position = 4 * layer - (col - (centerCol - layer));
        else // Left edge
            position = 6 * layer - (row - (centerRow - layer));

        // Reverse the delay so outer layers animate first
        var delay = (maxLayer - layer) * 80 + position * 20;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.ScaleTo(1.0, 350, Easing.CubicOut),
            image.FadeTo(1, 300)
        );
    }

    /// <summary>
    ///     Performs a left-to-right sweep animation.
    /// </summary>
    private static async Task LeftToRightAnimation(Image image, int row, int col, int totalCols)
    {
        image.Opacity = 0;
        image.TranslationX = -40;

        // Calculate delay based on column position
        var delay = col * 20;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a right-to-left sweep animation.
    /// </summary>
    private static async Task RightToLeftAnimation(Image image, int row, int col, int totalCols)
    {
        image.Opacity = 0;
        image.TranslationX = 40;

        // Calculate delay based on reverse column position
        var delay = (totalCols - 1 - col) * 20;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a top-to-bottom sweep animation.
    /// </summary>
    private static async Task TopToBottomAnimation(Image image, int row, int col, int totalRows)
    {
        image.Opacity = 0;
        image.TranslationY = -40;

        // Calculate delay based on row position
        var delay = row * 20;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }

    /// <summary>
    ///     Performs a bottom-to-top sweep animation.
    /// </summary>
    private static async Task BottomToTopAnimation(Image image, int row, int col, int totalRows)
    {
        image.Opacity = 0;
        image.TranslationY = 40;

        // Calculate delay based on reverse row position
        var delay = (totalRows - 1 - row) * 20;
        await Task.Delay(delay);

        await Task.WhenAll(
            image.TranslateTo(0, 0, 300, Easing.CubicOut),
            image.FadeTo(1)
        );
    }
    
    /// <summary>
    ///     Performs a pixelation animation where the cell starts blurry and gradually sharpens.
    /// </summary>
    private static async Task PixelatedAnimation(Image image, int row, int col)
    {
        // Initial state: visible but blurry/pixelated
        image.Opacity = 1;
        image.Scale = 1.0;
        
        // Apply a blur effect or pixelation effect
        // We can simulate this with a combination of scaling and opacity
        image.ScaleX = 0.5;
        image.ScaleY = 0.5;
        image.Opacity = 0.7;
        
        // Calculate delay based on position
        var delay = row * 6 + col * 6;
        await Task.Delay(delay);
        
        // Animate from pixelated to clear in steps
        // First step - increase scale slightly and opacity
        await Task.WhenAll(
            image.ScaleXTo(0.7, 100),
            image.ScaleYTo(0.7, 100),
            image.FadeTo(0.8, 100)
        );
        
        // Second step - increase scale more and opacity
        await Task.WhenAll(
            image.ScaleXTo(0.9, 100),
            image.ScaleYTo(0.9, 100),
            image.FadeTo(0.9, 100)
        );
        
        // Final step - full scale and opacity
        await Task.WhenAll(
            image.ScaleXTo(1.0, 150, Easing.CubicOut),
            image.ScaleYTo(1.0, 150, Easing.CubicOut),
            image.FadeTo(1.0, 150)
        );
    }
    
    /// <summary>
    ///     Performs a breathing animation where the cell scales in and out like breathing.
    /// </summary>
    private static async Task BreathInBreathOutAnimation(Image image, int row, int col)
    {
        // Initial state: fully visible but slightly smaller
        image.Opacity = 1;
        image.Scale = 0.9;
        
        // Calculate delay based on position
        var delay = row * 5 + col * 5;
        await Task.Delay(delay);
        
        // First breath in - scale up
        await image.ScaleTo(1.05, 800, Easing.SinInOut);
        
        // First breath out - scale down
        await image.ScaleTo(0.95, 800, Easing.SinInOut);
        
        // Second breath in - scale up
        await image.ScaleTo(1.03, 700, Easing.SinInOut);
        
        // Second breath out - scale down
        await image.ScaleTo(0.97, 700, Easing.SinInOut);
        
        // Final breath in - settle to normal size
        await image.ScaleTo(1.0, 500, Easing.SinOut);
    }
    
    /// <summary>
    ///     Performs a vibration animation with decreasing intensity.
    /// </summary>
    private static async Task AttenuatedVibrationAnimation(Image image, int row, int col)
    {
        // Initial state: fully visible
        image.Opacity = 1;
        image.Scale = 1.0;
        
        // Calculate delay based on position
        var delay = row * 4 + col * 4;
        await Task.Delay(delay);
        
        // First vibration - high intensity
        await image.TranslateTo(-5, 0, 50, Easing.CubicOut);
        await image.TranslateTo(5, 0, 100, Easing.CubicInOut);
        await image.TranslateTo(-5, 0, 100, Easing.CubicInOut);
        await image.TranslateTo(5, 0, 100, Easing.CubicInOut);
        
        // Second vibration - medium intensity
        await image.TranslateTo(-3, 0, 80, Easing.CubicInOut);
        await image.TranslateTo(3, 0, 80, Easing.CubicInOut);
        await image.TranslateTo(-3, 0, 80, Easing.CubicInOut);
        
        // Third vibration - low intensity
        await image.TranslateTo(2, 0, 60, Easing.CubicInOut);
        await image.TranslateTo(-2, 0, 60, Easing.CubicInOut);
        
        // Return to original position
        await image.TranslateTo(0, 0, 50, Easing.CubicOut);
    }
    
    /// <summary>
    ///     Performs a swirling animation like water going down a drain.
    /// </summary>
    private static async Task SwirlLikeADrainIntoPlaceAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        // Initial state: invisible and positioned off-center
        image.Opacity = 0;
        image.Scale = 0.5;
        
        // Calculate center coordinates
        var centerRow = totalRows / 2.0;
        var centerCol = totalColumns / 2.0;
        
        // Calculate distance from center
        var rowDistance = row - centerRow;
        var colDistance = col - centerCol;
        
        // Calculate angle from center (in radians)
        var angle = Math.Atan2(rowDistance, colDistance);
        
        // Convert angle to degrees for rotation
        var angleDegrees = angle * (180 / Math.PI);
        
        // Calculate distance from center
        var distance = Math.Sqrt(rowDistance * rowDistance + colDistance * colDistance);
        
        // Calculate initial position (further out from final position)
        var initialDistance = distance * 2.5;
        var initialX = Math.Cos(angle) * initialDistance * 20; // Scale for visual effect
        var initialY = Math.Sin(angle) * initialDistance * 20;
        
        // Set initial position and rotation
        image.TranslationX = initialX;
        image.TranslationY = initialY;
        image.Rotation = angleDegrees;
        
        // Calculate delay based on distance from center
        var delay = (int)(distance * 15);
        await Task.Delay(delay);
        
        // Make visible
        await image.FadeTo(1, 50);
        
        // Number of rotations based on distance from center
        var rotations = 1 + distance;
        
        // Duration based on distance from center
        var duration = 700 + (int)(distance * 100);
        
        // First animate with rotation and translation
        await Task.WhenAll(
            image.RotateTo(360 * rotations, (uint)(duration * 0.8), Easing.CubicOut),
            image.TranslateTo(0, 0, (uint)duration, Easing.CubicOut),
            image.ScaleTo(1.0, (uint)duration, Easing.CubicOut)
        );
        
        // Then ensure final rotation is 0 (square orientation)
        await image.RotateTo(0, 200, Easing.CubicOut);
    }
    
    /// <summary>
    ///     Performs a radar-like sweeping animation with varying opacity.
    ///     Tiles become visible briefly during first two sweeps, then stay visible after third sweep.
    /// </summary>
    private static async Task RadarPaternWitRadarLikeOpacityAnimation(Image image, int row, int col, int totalRows, int totalColumns)
    {
        // Initial state: invisible
        image.Opacity = 0;
        image.Scale = 1.0;
        
        // Calculate center coordinates
        var centerRow = totalRows / 2.0;
        var centerCol = totalColumns / 2.0;
        
        // Calculate distance and angle from center
        var rowDistance = row - centerRow;
        var colDistance = col - centerCol;
        var angle = Math.Atan2(rowDistance, colDistance);
        var angleDegrees = angle * (180 / Math.PI);
        var distance = Math.Sqrt(rowDistance * rowDistance + colDistance * colDistance);
        
        // Normalize angle to 0-360 degrees
        var normalizedAngle = (angleDegrees + 360) % 360;
        
        // Define animation parameters
        const int sweeps = 3; // Exactly three radar sweeps
        const int totalDuration = 4500; // Total duration for all sweeps
        const int singleSweepDuration = totalDuration / sweeps;
        
        // Calculate the time at which this cell should be "hit" by the radar in each sweep
        // based on its angle from the center
        var sweepHitTime = normalizedAngle / 360.0;
        
        // For each of the three sweeps
        for (int sweep = 0; sweep < sweeps; sweep++)
        {
            // Calculate when this cell should be hit in this sweep
            var sweepStartTime = sweep * singleSweepDuration;
            var cellHitTime = sweepStartTime + (int)(sweepHitTime * singleSweepDuration);
            
            // Wait until it's time for this cell to be hit by the radar
            var waitTime = cellHitTime - (sweep > 0 ? 0 : 0); // No initial delay for subsequent sweeps
            if (waitTime > 0)
            {
                await Task.Delay(waitTime);
            }
            
            if (sweep < 2)
            {
                // For first two sweeps, briefly show the cell then hide it
                await image.FadeTo(0.8, 150, Easing.CubicOut);
                await image.FadeTo(0, 350, Easing.CubicIn);
            }
            else
            {
                // For the third sweep, make the cell fully visible and keep it that way
                await image.FadeTo(1.0, 200, Easing.CubicOut);
            }
        }
    }
}
