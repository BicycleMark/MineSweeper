namespace MineSweeper.Models;

/// <summary>
/// Default implementation of IGameModelFactory
/// </summary>
public class GameModelFactory : IGameModelFactory
{
    /// <summary>
    /// Creates a new game model with the specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty level</param>
    /// <param name="logger">Optional logger for debugging</param>
    /// <returns>A new game model</returns>
    public IGameModel CreateModel(GameEnums.GameDifficulty difficulty, ILogger? logger = null)
    {
        return new GameModel(difficulty, logger);
    }
}
