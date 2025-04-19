using Microsoft.Maui.Graphics;

namespace MineSweeper.Views.Controls;

/// <summary>
/// Record that contains two points representing a move from one position to another.
/// Used for games like Chess or Checkers where pieces move from one position to another.
/// </summary>
public record PlayFromToRecord(Point FromPoint, Point ToPoint);
