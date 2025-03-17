using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MineSweeper.Models;

/// <summary>
/// Interface for the game model, providing access to game state and operations
/// </summary>
public interface IGameModel
{
    // Properties
    ObservableCollection<SweeperItem>? Items { get; }
    int Rows { get; }
    int Columns { get; }
    int Mines { get; }
    int FlaggedItems { get; }
    int RemainingMines { get; }
    GameEnums.GameStatus GameStatus { get; set; }
    
    // Commands
    ICommand PlayCommand { get; }
    ICommand FlagCommand { get; }
    
    // Indexer
    SweeperItem this[int row, int column] { get; }
}
