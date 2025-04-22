namespace MineSweeper.Views.Controls;

/// <summary>
/// Event arguments for the GetCellImage event.
/// </summary>
public class GetCellImageEventArgs : EventArgs
{
    /// <summary>
    /// Gets the row index of the cell.
    /// </summary>
    public int Row { get; }
    
    /// <summary>
    /// Gets the column index of the cell.
    /// </summary>
    public int Column { get; }
    
    /// <summary>
    /// Gets or sets the image view for the cell.
    /// </summary>
    public View? Image { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="GetCellImageEventArgs"/> class.
    /// </summary>
    public GetCellImageEventArgs(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
