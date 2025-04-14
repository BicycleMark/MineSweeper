using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MineSweeper.Views.Controls;

/// <summary>
/// A drawable that renders a 3D chiseled border effect.
/// </summary>
public class ChiseledBorderDrawable : IDrawable
{
    /// <summary>
    /// Gets or sets the shadow color for the 3D effect (top and left edges for recessed look).
    /// </summary>
    public Color ShadowColor { get; set; } = Colors.DimGray;
    
    /// <summary>
    /// Gets or sets the highlight color for the 3D effect (bottom and right edges for recessed look).
    /// </summary>
    public Color HighlightColor { get; set; } = Colors.LightGray;
    
    /// <summary>
    /// Gets or sets the thickness of the border in pixels.
    /// </summary>
    public int BorderThickness { get; set; } = 6;
    
    /// <summary>
    /// Gets or sets whether the border appears recessed (true) or raised (false).
    /// </summary>
    public bool IsRecessed { get; set; } = true;
    
    /// <summary>
    /// Draws the chiseled border effect.
    /// </summary>
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float width = dirtyRect.Width;
        float height = dirtyRect.Height;
        
        // Determine which colors to use for which edges based on whether it's recessed or raised
        Color topLeftColor = IsRecessed ? ShadowColor : HighlightColor;
        Color bottomRightColor = IsRecessed ? HighlightColor : ShadowColor;
        
        // Draw the beveled edge (3D chiseled effect)
        for (int i = 0; i < BorderThickness; i++)
        {
            canvas.StrokeSize = 1;
            
            // Top shadow/highlight line
            canvas.StrokeColor = topLeftColor;
            canvas.DrawLine(i, i, width - i - 1, i);
            
            // Left shadow/highlight line
            canvas.StrokeColor = topLeftColor;
            canvas.DrawLine(i, i, i, height - i - 1);
            
            // Bottom highlight/shadow line
            canvas.StrokeColor = bottomRightColor;
            canvas.DrawLine(i, height - i - 1, width - i - 1, height - i - 1);
            
            // Right highlight/shadow line
            canvas.StrokeColor = bottomRightColor;
            canvas.DrawLine(width - i - 1, i, width - i - 1, height - i - 1);
        }
    }
}
