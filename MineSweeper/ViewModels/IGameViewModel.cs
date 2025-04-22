using System.Windows.Input;
using MineSweeper.Models;

namespace MineSweeper.ViewModels;

/// <summary>
///     Interface for the game view model, providing access to game state, operations, and UI-related functionality.
///     Implements IDisposable to ensure proper cleanup of resources.
/// </summary>
public interface IGameViewModel : IDisposable
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
    ///     Gets the number of mines remaining to be flagged
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
    ///     Gets or sets the current game difficulty level
    /// </summary>
    GameEnums.GameDifficulty GameDifficulty { get; set; }

    /// <summary>
    ///     Command to start a new game with the specified difficulty
    /// </summary>
    IAsyncRelayCommand NewGameAsyncCommand { get; }

    /// <summary>
    ///     Command to play (reveal) a cell at the specified position
    /// </summary>
    ICommand PlayCommand { get; }

    /// <summary>
    ///     Command to flag a cell at the specified position
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