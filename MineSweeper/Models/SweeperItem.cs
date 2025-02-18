using CommunityToolkit.Mvvm.ComponentModel;

namespace MineSweeper.Models;

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