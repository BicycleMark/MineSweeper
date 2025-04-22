namespace MineSweeper.Views.Controls;

/// <summary>
///     A drawable that renders a 3D chiseled border effect.
/// </summary>
public class ChiseledBorderDrawable : IDrawable
{
    /// <summary>
    ///     Gets or sets the shadow color for the 3D effect (top and left edges for recessed look).
    /// </summary>
    public Color ShadowColor { get; set; } = Colors.DimGray;

    /// <summary>
    ///     Gets or sets the highlight color for the 3D effect (bottom and right edges for recessed look).
    /// </summary>
    public Color HighlightColor { get; set; } = Colors.LightGray;

    /// <summary>
    ///     Gets or sets the thickness of the border in pixels.
    /// </summary>
    public int BorderThickness { get; set; } = 6;

    /// <summary>
    ///     Gets or sets whether the border appears recessed (true) or raised (false).
    /// </summary>
    public bool IsRecessed { get; set; } = true;

    /// <summary>
    ///     Draws the chiseled border effect.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="dirtyRect">The rectangle that needs to be redrawn.</param>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // Determine which colors to use for which edges based on whether it's recessed or raised
        var topLeftColor = IsRecessed ? ShadowColor : HighlightColor;
        var bottomRightColor = IsRecessed ? HighlightColor : ShadowColor;

        // Clear the canvas with a transparent color to ensure we're drawing on a clean surface
        canvas.FillColor = Colors.Transparent;
        canvas.FillRectangle(0, 0, width, height);

        // Draw the beveled edge (3D chiseled effect)
        for (var i = 0; i < BorderThickness; i++)
        {
            // Use a thicker stroke for better visibility
            canvas.StrokeSize = 2;

            // Top shadow/highlight line
            canvas.StrokeColor = topLeftColor;
            canvas.DrawLine(i, i, width - i - 1, i);

            // Left shadow/highlight line
            canvas.StrokeColor = topLeftColor;
            canvas.DrawLine(i, i, i, height - i - 1);

            // Bottom highlight/shadow line
            canvas.StrokeColor = bottomRightColor;
            canvas.DrawLine(i, height - i - 1, width - i, height - i - 1);

            // Right highlight/shadow line
            canvas.StrokeColor = bottomRightColor;
            canvas.DrawLine(width - i - 1, i, width - i - 1, height - i);
        }

        // Draw corner pixels to ensure clean corners
        for (var i = 0; i < BorderThickness; i++)
        {
            // Top-left corner
            canvas.StrokeColor = topLeftColor;
            canvas.DrawLine(i, i, i, i);

            // Top-right corner
            canvas.StrokeColor = IsRecessed ? topLeftColor : bottomRightColor;
            canvas.DrawLine(width - i - 1, i, width - i - 1, i);

            // Bottom-left corner
            canvas.StrokeColor = IsRecessed ? bottomRightColor : topLeftColor;
            canvas.DrawLine(i, height - i - 1, i, height - i - 1);

            // Bottom-right corner
            canvas.StrokeColor = bottomRightColor;
            canvas.DrawLine(width - i - 1, height - i - 1, width - i - 1, height - i - 1);
        }
    }
}