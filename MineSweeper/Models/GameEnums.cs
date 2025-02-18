namespace MineSweeper.Models;

public class GameEnums
{
    public enum GameStatus
    {
        NotStarted,
        InProgress,
        InPress,
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
    
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard
    }
    
}