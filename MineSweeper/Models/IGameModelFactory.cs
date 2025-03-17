namespace MineSweeper.Models;

/// <summary>
/// Factory interface for creating game models
/// </summary>
public interface IGameModelFactory
{
    /// <summary>
    /// Creates a new game model with the specified difficulty
    /// </summary>
    /// <param name="difficulty">The difficulty level</param>
    /// <returns>A new game model</returns>
    IGameModel CreateModel(GameEnums.GameDifficulty difficulty);
}
