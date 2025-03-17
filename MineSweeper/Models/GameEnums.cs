namespace MineSweeper.Models;

/// <summary>
/// Contains all enumerations used in the game
/// </summary>
public class GameEnums
{
    /// <summary>
    /// Represents the current status of the game
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// Game has not started yet, no cells have been revealed
        /// </summary>
        NotStarted,
        
        /// <summary>
        /// Game is currently being played
        /// </summary>
        InProgress,
        
        /// <summary>
        /// Cell is currently being pressed but not yet released
        /// </summary>
        InPress,
        
        /// <summary>
        /// Game has been won (all mines flagged correctly)
        /// </summary>
        Won,
        
        /// <summary>
        /// Game has been lost (a mine was revealed)
        /// </summary>
        Lost
    }

    /// <summary>
    /// Represents the current status of a sweeper item (cell)
    /// </summary>
    public enum SweeperItemStatus
    {
        /// <summary>
        /// Cell is hidden (not revealed)
        /// </summary>
        Hidden,
        
        /// <summary>
        /// Cell has been revealed by the player
        /// </summary>
        Revealed,
        
        /// <summary>
        /// Cell has been flagged by the player
        /// </summary>
        Flagged
    }

    /// <summary>
    /// Return values for the Play method
    /// </summary>
    public enum PlayReturnValues
    {
        /// <summary>
        /// The item has already been played (revealed)
        /// </summary>
        ItemAlreadyPlayed,
        
        /// <summary>
        /// Cannot play a flagged item
        /// </summary>
        CantPlayFlaggedItem,
        
        /// <summary>
        /// A mine was played (game over)
        /// </summary>
        MinePlayed,
        
        /// <summary>
        /// The item played is invalid (out of bounds)
        /// </summary>
        InvalidItemPlayed,
        
        /// <summary>
        /// The item was successfully played
        /// </summary>
        ValidItemPlayed
    }

    /// <summary>
    /// Game difficulty levels
    /// </summary>
    public enum GameDifficulty
    {
        /// <summary>
        /// Easy difficulty (10x10 grid with 10 mines)
        /// </summary>
        Easy,
        
        /// <summary>
        /// Medium difficulty (15x15 grid with 40 mines)
        /// </summary>
        Medium,
        
        /// <summary>
        /// Hard difficulty (20x20 grid with 80 mines)
        /// </summary>
        Hard
    }
}
