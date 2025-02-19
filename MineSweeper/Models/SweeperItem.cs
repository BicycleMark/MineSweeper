namespace MineSweeper.Models;

/// <summary>
/// SweeperItem has all the properties of a single cell in the game board.
/// </summary>
public partial class SweeperItem : ObservableObject
{
    [ObservableProperty]
    
    private bool _isRevealed = false;
    [ObservableProperty]
    private bool _isMine = false;
    [ObservableProperty]
    private bool _isFlagged = false;
    [ObservableProperty] 
    private int _mineCount = 0;
    [ObservableProperty]
    private int _neighborMineCount = 0;
}