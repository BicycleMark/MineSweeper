using System.Collections.ObjectModel;
using System.Windows.Input;
using MineSweeper.Features.Game.Models;

namespace MineSweeper.Features.Game.ViewModels;

/// <summary>
///     Interface for the game view model, providing access to game state, operations, and UI-related functionality.
///     Implements IDisposable to ensure proper cleanup of resources like the game timer.
/// </summary>
public interface IGameViewModel : IDisposable
{
    /// <summary>
    ///     Gets the collection of sweeper items (cells) in the game grid.
    ///     May be null during initialization or after disposal.
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
    ///     Gets the number of mines remaining to be flagged.
    ///     This is calculated as total mines minus the number of flagged cells.
    /// </summary>
    int RemainingMines { get; }

    /// <summary>
    ///     Gets the elapsed game time in seconds
    /// </summary>
    int GameTime { get; }

    /// <summary>
    ///     Gets or sets the current game status
    /// </summary>
    GameEnums.GameStatus GameStatus { get; set; }

    /// <summary>
    ///     Gets or sets the current game difficulty level.
    ///     Changing this property does not affect the current game until NewGameAsyncCommand is executed.
    /// </summary>
    GameEnums.GameDifficulty GameDifficulty { get; set; }

    /// <summary>
    ///     Command to start a new game with the specified difficulty.
    ///     Parameter can be a GameDifficulty enum value, a string representation of a difficulty, or null for default.
    /// </summary>
    IAsyncRelayCommand NewGameAsyncCommand { get; }

    /// <summary>
    ///     Command to play (reveal) a cell at the specified position.
    ///     Parameter should be a Point with Row and Column coordinates.
    /// </summary>
    ICommand PlayCommand { get; }

    /// <summary>
    ///     Command to flag a cell at the specified position.
    ///     Parameter should be a Point with Row and Column coordinates.
    /// </summary>
    ICommand FlagCommand { get; }

    /// <summary>
    ///     Gets the underlying game model (for testing purposes).
    ///     Returns null if the ViewModel has been disposed.
    /// </summary>
    IGameModel? Model { get; }

    /// <summary>
    ///     Gets the game timer (for testing purposes).
    ///     Returns null if the ViewModel has been disposed.
    /// </summary>
    IDispatcherTimer? Timer { get; }

    // Testing helper methods
#if DEBUG
    /// <summary>
    ///     Sets the game status directly (for testing purposes).
    ///     Does nothing if the ViewModel has been disposed.
    /// </summary>
    /// <param name="status">The game status to set</param>
    void SetGameStatus(GameEnums.GameStatus status);

    /// <summary>
    ///     Invokes the CheckGameStatus method directly (for testing purposes).
    ///     Does nothing if the ViewModel has been disposed.
    /// </summary>
    void InvokeCheckGameStatus();
#endif
}
