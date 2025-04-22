namespace MineSweeper.Models;

/// <summary>
/// default implementation of IGameModelFactory
/// </summary>
public class GameModelFactory : IGameModelFactory
{
    private readonly ILogger _customDebugLogger;

    /// <summary>
    /// Constructor with dependency injection for customDebugLogger
    /// </summary>
    /// <param name="customDebugLogger">Logger for debugging</param>
    public GameModelFactory(ILogger customDebugLogger)
    {
        _customDebugLogger = customDebugLogger;
    }

    /// <summary>
    /// Creates a new game model with the specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty level</param>
    /// <returns>A new game model</returns>
    public IGameModel CreateModel(GameEnums.GameDifficulty difficulty)
    {
        return new GameModel(difficulty, _customDebugLogger);
    }
}
