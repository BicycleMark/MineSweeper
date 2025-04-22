using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;

namespace MineSweeper.Features.Game.Models;

/// <summary>
/// Represents a single cell in the minesweeper grid
/// </summary>
public partial class SweeperItem : ObservableObject
{
    /// <summary>
    /// Gets or sets a value indicating whether this item is flagged by the player
    /// </summary>
    [ObservableProperty] private bool _isFlagged;

    /// <summary>
    /// Gets or sets a value indicating whether this item contains a mine
    /// </summary>
    [ObservableProperty] private bool _isMine;

    /// <summary>
    /// Gets or sets a value indicating whether this item has been revealed by the player
    /// </summary>
    [ObservableProperty] private bool _isRevealed;

    /// <summary>
    /// Gets or sets the number of mines adjacent to this item
    /// </summary>
    [ObservableProperty] private int _mineCount;

    /// <summary>
    /// Gets or sets the position of this item in the grid
    /// </summary>
    [ObservableProperty] private Point _point;
}
