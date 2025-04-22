namespace MineSweeper.Views.Controls;

/// <summary>
///     Interface for grid controls that can be animated.
///     Implementing this interface allows a grid to work with the GridAnimationExtensions.
/// </summary>
public interface IAnimatableGrid
{
    /// <summary>
    ///     Gets the number of rows in the grid.
    /// </summary>
    int Rows { get; }

    /// <summary>
    ///     Gets the number of columns in the grid.
    /// </summary>
    int Columns { get; }

    /// <summary>
    ///     Event triggered to get the image for a specific cell at [row, column].
    /// </summary>
    event EventHandler<GetCellImageEventArgs>? GetCellImage;

    /// <summary>
    ///     Creates the grid with the specified number of rows and columns.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    void CreateGrid(int rows, int columns);
}