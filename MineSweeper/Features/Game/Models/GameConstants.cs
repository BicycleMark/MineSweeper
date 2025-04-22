namespace MineSweeper.Features.Game.Models;

/// <summary>
/// Contains game-related constants 
/// </summary>
public static class GameConstants
{
    /// <summary>
    /// Defines the game grid dimensions (rows, columns, mines) for each difficulty level
    /// </summary>
    public static readonly Dictionary<GameEnums.GameDifficulty, (int rows, int columns, int mines)> GameLevels = new()
    {
        { GameEnums.GameDifficulty.Easy, (10, 10, 10) },
        { GameEnums.GameDifficulty.Medium, (15, 15, 40) },
        { GameEnums.GameDifficulty.Hard, (20, 20, 80) }
    };
}
