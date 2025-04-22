namespace MineSweeper.Extensions;

/// <summary>
///     Extension methods for animating grid cells in IAnimatableGrid implementations.
/// </summary>
public static class GridAnimationExtensions
{
    // Animation pattern enum
    public enum AnimationPattern
    {
        Sequential,
        WaveFromTopLeft,
        WaveFromCenter,
        Spiral,
        Random,
        Checkerboard
    }

    // Animation type enum
    public enum AnimationType
    {
        FlipIn,
        FadeIn,
        ScaleIn,
        BounceIn,
        SpinIn,
        SlideIn,
        RandomPerCell,
        SwipeDiagonalTopLeftBottomRight,
        SwipeDiagonalTopRightBottomLeft,
        SwipeDiagonalBottomLeftTopRight,
        SwipeDiagonalBottomRightTopLeft,
        SwirlInnerToOuter,
        SwirlOuterToInner,
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }

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

    private static async Task FadeInAnimation(Image image, int row, int col)
    {
        image.Opacity = 0;

        var delay = row * 8 + col * 8;
        await Task.Delay(delay);

        await image.FadeTo(1, 300, Easing.CubicOut);
    }

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
}