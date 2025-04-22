namespace MineSweeper.Features.Game.Models;

/// <summary>
/// Contains enumeration types related to the game
/// </summary>
public static class GameEnums
{
    /// <summary>
    /// Indicates the current status of the game
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// The game hasn't started yet (first move not made)
        /// </summary>
        NotStarted = 0,
        
        /// <summary>
        /// The game is currently in progress
        /// </summary>
        InProgress = 1,
        
        /// <summary>
        /// The game has been won
        /// </summary>
        Won = 2,
        
        /// <summary>
        /// The game has been lost (hit a mine)
        /// </summary>
        Lost = 3,
        
        /// <summary>
        /// The user is pressing down on the game state control
        /// </summary>
        InPress = 4
    }

    /// <summary>
    /// Specifies the difficulty level of the game
    /// </summary>
    public enum GameDifficulty
    {
        /// <summary>
        /// Easy difficulty (small grid with few mines)
        /// </summary>
        Easy = 0,
        
        /// <summary>
        /// Medium difficulty (medium grid with moderate mines)
        /// </summary>
        Medium = 1,
        
        /// <summary>
        /// Hard difficulty (large grid with many mines)
        /// </summary>
        Hard = 2
    }
}
