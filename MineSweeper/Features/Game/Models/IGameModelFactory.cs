namespace MineSweeper.Features.Game.Models;

/// <summary>
/// Factory interface for creating game model instances
/// </summary>
public interface IGameModelFactory : IModelFactory<IGameModel>
{
    /// <summary>
    /// Creates a new game model with the specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty level for the game</param>
    /// <returns>A new game model instance</returns>
    IGameModel CreateModel(GameEnums.GameDifficulty difficulty);
}
