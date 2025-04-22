using System.Windows.Input;

namespace MineSweeper.Models;

/// <summary>
///     Interface for the game model, providing access to game state and operations
/// </summary>
public interface IGameModel
{
    /// <summary>
    ///     Gets the collection of sweeper items (cells) in the game grid
    /// </summary>
    ObservableCollection<SweeperItem>? Items { get; }

    /// <summary>
    ///     Gets the number of rows in the game grid
    /// </summary>
    int Rows { get; }

    /// <summary>
    ///     Gets the number of columns in the game grid
    /// </summary>
    int Columns { get; }

    /// <summary>
    ///     Gets the total number of mines in the game
    /// </summary>
    int Mines { get; }

    /// <summary>
    ///     Gets the number of cells that have been flagged by the player
    /// </summary>
    int FlaggedItems { get; }

    /// <summary>
    ///     Gets the number of mines remaining to be flagged
    /// </summary>
    int RemainingMines { get; }

    /// <summary>
    ///     Gets or sets the current game status
    /// </summary>
    GameEnums.GameStatus GameStatus { get; set; }

    /// <summary>
    ///     Command to play (reveal) a cell at the specified position
    /// </summary>
    ICommand PlayCommand { get; }

    /// <summary>
    ///     Command to flag a cell at the specified position
    /// </summary>
    ICommand FlagCommand { get; }

    /// <summary>
    ///     Gets the sweeper item at the specified row and column
    /// </summary>
    /// <param name="row">The row index</param>
    /// <param name="column">The column index</param>
    /// <returns>The sweeper item at the specified position</returns>
    SweeperItem this[int row, int column] { get; }

    /// <summary>
    ///     Reveals all mines on the board when the game is lost
    /// </summary>
    void RevealAllMines();
}