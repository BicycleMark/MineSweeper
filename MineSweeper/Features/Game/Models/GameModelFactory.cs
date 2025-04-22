using MineSweeper.Services.Logging;

namespace MineSweeper.Features.Game.Models;

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
    /// Creates a new model instance with variable parameters
    /// </summary>
    /// <param name="args">Optional arguments for model creation</param>
    /// <returns>A new game model instance</returns>
    public IGameModel Create(params object[] args)
    {
        // Extract difficulty from args if possible
        GameEnums.GameDifficulty difficulty = GameEnums.GameDifficulty.Easy;
        
        if (args.Length > 0 && args[0] is GameEnums.GameDifficulty difficultyArg)
        {
            difficulty = difficultyArg;
        }
        
        return CreateModel(difficulty);
    }

    /// <summary>
    /// Creates a new game model with the specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty level</param>
    /// <returns>A new game model</returns>
    public IGameModel CreateModel(GameEnums.GameDifficulty difficulty)
    {
        _logger.Log($"Creating new game model with difficulty: {difficulty}");
        return new GameModel(difficulty, _logger);
    }
}
