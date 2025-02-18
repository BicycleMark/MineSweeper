namespace MineSweeper.Models;

public class GameEnums
{
    public enum GameStatus
    {
        NotStarted,
        InProgress,
        Won,
        Lost
    }
    
    public enum SweeperItemStatus
    {
        Hidden,
        Revealed,
        Flagged
    }

    public enum PlayReturnValues
    {
        ItemAlreadyPlayed,
        CantPlayFlaggedItem,
        MinePlayed,
        InvalidItemPlayed,
        ValidItemPlayed
        
    }
}