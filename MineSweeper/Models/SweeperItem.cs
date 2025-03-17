using System.Text.Json.Serialization;

namespace MineSweeper.Models;

/// <summary>
/// SweeperItem has all the properties of a single cell in the game board.
/// </summary>
[JsonConverter(typeof(SweeperItemJsonConverter))]
public partial class SweeperItem : ObservableObject
{
    /// <summary>
    /// Whether the cell has been revealed by the player
    /// </summary>
    [ObservableProperty] private bool _isRevealed = false;
    
    /// <summary>
    /// Whether the cell contains a mine
    /// </summary>
    [ObservableProperty] private bool _isMine = false;
    
    /// <summary>
    /// Whether the cell has been flagged by the player
    /// </summary>
    [ObservableProperty] private bool _isFlagged = false;
    
    /// <summary>
    /// The number of mines in adjacent cells
    /// </summary>
    [ObservableProperty] private int _mineCount = 0;
    
    /// <summary>
    /// The position of this cell in the grid
    /// </summary>
    [ObservableProperty] private Point _point;
}
