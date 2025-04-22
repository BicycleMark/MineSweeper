namespace MineSweeper.Features.Game.Models;

/// <summary>
/// Generic factory interface for creating model instances
/// </summary>
/// <typeparam name="T">The type of model to create</typeparam>
public interface IModelFactory<out T> where T : class
{
    /// <summary>
    /// Creates a new model instance
    /// </summary>
    /// <param name="args">Optional arguments for model creation</param>
    /// <returns>A new model instance</returns>
    T Create(params object[] args);
}
