using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Dispatching;
using MineSweeper.Models;

namespace MineSweeper.ViewModels;

/// <summary>
/// Interface for the game view model, providing access to game state, operations, and UI-related functionality
/// </summary>
public interface IGameViewModel
{
    // Properties
    ObservableCollection<SweeperItem>? Items { get; }
    int Rows { get; }
    int Columns { get; }
    int Mines { get; }
    int RemainingMines { get; }
    int GameTime { get; }
    GameEnums.GameStatus GameStatus { get; set; }
    GameEnums.GameDifficulty GameDifficulty { get; set; }
    
    // Commands
    ICommand NewGameCommand { get; }
    ICommand PlayCommand { get; }
    ICommand FlagCommand { get; }
    
    // For testing
    IGameModel Model { get; }
    IDispatcherTimer? Timer { get; }
    
    // Testing helper methods
    #if DEBUG
    void SetGameStatus(GameEnums.GameStatus status);
    void InvokeCheckGameStatus();
    #endif
}
