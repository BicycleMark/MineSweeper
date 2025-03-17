namespace MineSweeper.Models;

/// <summary>
/// Default implementation of IGameModelFactory
/// </summary>
public class GameModelFactory : IGameModelFactory
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor with dependency injection for logger
    /// </summary>
    /// <param name="logger">Logger for debugging</param>
    public GameModelFactory(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new game model with the specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty level</param>
    /// <returns>A new game model</returns>
    public IGameModel CreateModel(GameEnums.GameDifficulty difficulty)
    {
        return new GameModel(difficulty, _logger);
    }
}
