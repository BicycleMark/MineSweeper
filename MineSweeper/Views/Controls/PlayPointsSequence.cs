using Microsoft.Maui.Graphics;

namespace MineSweeper.Views.Controls;

/// <summary>
/// Class that contains a sequence of points representing a move.
/// Used for games like Chess or Checkers where pieces move from one position to another,
/// potentially with intermediate steps.
/// </summary>
public class PlayPointsSequence
{
    /// <summary>
    /// Gets the sequence of points in the move.
    /// </summary>
    public Point[] Points { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayPointsSequence"/> class.
    /// </summary>
    /// <param name="points">The sequence of points in the move.</param>
    public PlayPointsSequence(params Point[] points)
    {
        Points = points;
    }
    
    /// <summary>
    /// Gets the first point in the sequence (the "from" position).
    /// </summary>
    public Point FromPoint => Points.Length > 0 ? Points[0] : default;
    
    /// <summary>
    /// Gets the last point in the sequence (the "to" position).
    /// </summary>
    public Point ToPoint => Points.Length > 0 ? Points[^1] : default;
}
