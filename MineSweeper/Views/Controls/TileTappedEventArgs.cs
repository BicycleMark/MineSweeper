namespace MineSweeper.Views.Controls;

/// <summary>
///     Event arguments for when a tile in the grid is tapped.
/// </summary>
public class TileTappedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TileTappedEventArgs" /> class.
    /// </summary>
    public TileTappedEventArgs(int row, int column, View tileView, bool isLongHold = false)
    {
        Row = row;
        Column = column;
        TileView = tileView;
        IsLongHold = isLongHold;
    }

    /// <summary>
    ///     Gets the row index of the tapped tile.
    /// </summary>
    public int Row { get; }

    /// <summary>
    ///     Gets the column index of the tapped tile.
    /// </summary>
    public int Column { get; }

    /// <summary>
    ///     Gets the view representing the tapped tile.
    /// </summary>
    public View TileView { get; }

    /// <summary>
    ///     Gets a value indicating whether the tapped tile is a default/blank tile.
    /// </summary>
    public bool IsLongHold{ get; }

    /// <summary>
    ///     Gets the position of the tapped tile as a Point.
    /// </summary>
    public Point Position => new(Column, Row);
}