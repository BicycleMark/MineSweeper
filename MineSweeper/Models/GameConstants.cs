namespace MineSweeper.Models;

// Test Commit
public static class GameConstants
{
    public static readonly Dictionary
        <GameEnums.GameDifficulty, (int rows, int columns, int mines)>
        GameLevels = new()
        {
            {GameEnums.GameDifficulty.Easy, (10, 10, 10)},
            {GameEnums.GameDifficulty.Medium, (15, 15, 40)},
            {GameEnums.GameDifficulty.Hard, (20, 20, 80)}
        };
}