namespace MineSweeper.Models;

/// <summary>
///     Contains constant values used throughout the game
/// </summary>
public static class GameConstants
{
    /// <summary>
    ///     Dictionary mapping game difficulty levels to their corresponding grid dimensions and mine counts
    /// </summary>
    /// <remarks>
    ///     Each entry contains a tuple with:
    ///     - rows: The number of rows in the grid
    ///     - columns: The number of columns in the grid
    ///     - mines: The number of mines to place in the grid
    /// </remarks>
    public static readonly Dictionary
        <GameEnums.GameDifficulty, (int rows, int columns, int mines)>
        GameLevels = new()
        {
            {GameEnums.GameDifficulty.Easy, (10, 10, 10)},
            {GameEnums.GameDifficulty.Medium, (15, 15, 40)},
            {GameEnums.GameDifficulty.Hard, (20, 20, 80)}
        };
}