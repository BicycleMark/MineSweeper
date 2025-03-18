using System.Text.Json;
using MineSweeper.Models;

namespace MineSweeper.Tests.Models;

public class GameModelTests
{
    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy)]
    [InlineData(GameEnums.GameDifficulty.Medium)]
    [InlineData(GameEnums.GameDifficulty.Hard)]
    public void GameDificultyConstructorTest(GameEnums.GameDifficulty gd)
    {
        var game = new GameModel(gd);
        // TODO: Determine if we should have a GameDifficulty
        // field added to the GameModel class
        // Assert.Equal(gd, game.Difficulty);
        // Assert.Equal(GameEnums.GameStatus.NotStarted, game.Status);
        Assert.Equal(GameConstants.GameLevels[gd].rows, game.Rows);
        Assert.Equal(GameConstants.GameLevels[gd].columns, game.Columns);
        Assert.Equal(GameConstants.GameLevels[gd].mines, game.Mines);
        Assert.NotNull(game.Items);
        if (game.Items != null)
            Assert.Equal(game.Items.Count, game.Rows * game.Columns);
        // Ensure there are no mines yet in any SweeperItems
        if (game.Items != null)
            Assert.DoesNotContain(game.Items, i => i.IsMine);
    }

    // Test the Custom Game Constructor with several different values
    [Theory]
    [InlineData(10, 10, 10)] // Easy
    [InlineData(15, 15, 40)] // Medium
    [InlineData(20, 20, 80)] // Hard
    [InlineData(5, 5, 5)] // Custom
    public void CustomGameConstructorTest(int rows, int columns, int mines)
    {
        var game = new GameModel(rows, columns, mines);
        // Assert.Equal(GameEnums.GameDifficulty.Custom, game.Difficulty);
        // Assert.Equal(GameEnums.GameStatus.NotStarted, game.Status);
        Assert.Equal(rows, game.Rows);
        Assert.Equal(columns, game.Columns);
        Assert.Equal(mines, game.Mines);
        Assert.NotNull(game.Items);
        if (game.Items != null)
            Assert.Equal(game.Items.Count, game.Rows * game.Columns);
        // Ensure there are no mines yet in any SweeperItems
        if (game.Items != null)
            Assert.DoesNotContain(game.Items, i => i.IsMine);
    }

    [Theory]
    [InlineData(10, 10, 10, "SavedGame.json")]
    [InlineData(15, 15, 40, "SavedGame.json")]
    [InlineData(20, 20, 80, "SavedGame.json")]
    public void SaveGameCustomConstructorTest(int rows, int columns, int mines, string fileName)
    {
        var game = new GameModel(rows, columns, mines);
        game.SaveGameCommand.Execute(fileName);
        var jsonFile = File.ReadAllText(fileName);
        
        // Use our custom constructor instead of JsonSerializer.Deserialize
        var constructedSavedGame = new GameModel(jsonFile);
        Assert.NotNull(constructedSavedGame);
        Assert.Equal(rows, constructedSavedGame.Rows);
        Assert.Equal(columns, constructedSavedGame.Columns);
        Assert.Equal(mines, constructedSavedGame.Mines);
        Assert.NotNull(constructedSavedGame.Items);
        if (constructedSavedGame.Items != null)
            Assert.Equal(constructedSavedGame.Items.Count, rows * columns);
    }

    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy, "SavedGame.json")]
    [InlineData(GameEnums.GameDifficulty.Medium, "SavedGame.json")]
    [InlineData(GameEnums.GameDifficulty.Hard, "SavedGame.json")]
    public void SaveGameDifficultyLevelConstructorTest
        (GameEnums.GameDifficulty gd, string fileName)
    {
        var game = new GameModel(gd);
        game.SaveGameCommand.Execute(fileName);
        var jsonFile = File.ReadAllText(fileName);
        
        // Use our custom constructor instead of JsonSerializer.Deserialize
        var constructedSavedGame = new GameModel(jsonFile);
        Assert.NotNull(constructedSavedGame);
        Assert.Equal(GameConstants.GameLevels[gd].rows, constructedSavedGame.Rows);
        Assert.Equal(GameConstants.GameLevels[gd].columns, constructedSavedGame.Columns);
        Assert.Equal(GameConstants.GameLevels[gd].mines, constructedSavedGame.Mines);
        Assert.NotNull(constructedSavedGame.Items);
        if (constructedSavedGame.Items != null)
            Assert.Equal(constructedSavedGame.Items.Count, constructedSavedGame.Rows * constructedSavedGame.Columns);
    }

    [Theory]
    [InlineData(20, 20, 0, GameEnums.GameStatus.InProgress)]
    [InlineData(20, 20, 1, GameEnums.GameStatus.Lost)]
    [InlineData(20, 20, 5, GameEnums.GameStatus.Lost)]
    public void PlayAllPiecesGameCustomConstructorTest(int rows, int columns, int mines,
        GameEnums.GameStatus expectedStatus)
    {
        var game = new GameModel(rows, columns, mines);
        // Play all pieces
        for (var i = 0; i < game.Rows; i++)
        for (var j = 0; j < game.Columns; j++)
        {
            game.PlayCommand.Execute(new Point(i, j));
            if (game.GameStatus != GameEnums.GameStatus.InProgress)
                goto TEST_GAME_STATUS;
        }

        TEST_GAME_STATUS:
        Assert.Equal(expectedStatus, game.GameStatus);
    }

    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy)]
    [InlineData(GameEnums.GameDifficulty.Medium)]
    [InlineData(GameEnums.GameDifficulty.Hard)]
    public void CreateGameAndPlayOneItemTest(GameEnums.GameDifficulty gd)
    {
        var game = new GameModel(gd);
        // get Random index point and call Play(randomI, randomJ)
        var random = new Random();
        var observableCollection = game!.Items;
        if (observableCollection != null)
        {
            Assert.Equal(observableCollection.Count, game.Rows * game.Columns);
            var r = random.Next(0, game.Rows);
            var c = random.Next(0, game.Columns);
            Assert.Equal(GameEnums.GameStatus.NotStarted, game.GameStatus);
            Assert.Equal(0, game.Items.Count(i => i.IsMine));
            game.PlayCommand.Execute(new Point(r, c));
            Assert.Equal(GameConstants.GameLevels[gd].mines, game.Items.Count(i => i.IsMine));
        }

        Assert.Equal(GameEnums.GameStatus.InProgress, game.GameStatus);
    }

    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy)]
    [InlineData(GameEnums.GameDifficulty.Medium)]
    [InlineData(GameEnums.GameDifficulty.Hard)]
    public void Flag_FirstMove_NotAllowed(GameEnums.GameDifficulty gd)
    {
        // Arrange
        var game = new GameModel(gd);
        Assert.Equal(GameEnums.GameStatus.NotStarted, game.GameStatus);
        Assert.Equal(0, game.Items.Count(i => i.IsMine));
        
        // Act - Flag a cell as the first move
        var point = new Point(0, 0);
        game.FlagCommand.Execute(point);
        
        // Assert
        // Game status should remain NotStarted
        Assert.Equal(GameEnums.GameStatus.NotStarted, game.GameStatus);
        
        // Cell should not be flagged
        Assert.False(game[0, 0].IsFlagged);
        
        // Verify no mines are placed yet
        Assert.Equal(0, game.Items.Count(i => i.IsMine));
        
        // Verify FlaggedItems count is not updated
        Assert.Equal(0, game.FlaggedItems);
        
        // Verify RemainingMines matches the expected mines for the difficulty
        var expectedMines = GameConstants.GameLevels[gd].mines;
        Assert.Equal(expectedMines, game.RemainingMines);
    }

    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy)]
    [InlineData(GameEnums.GameDifficulty.Medium)]
    [InlineData(GameEnums.GameDifficulty.Hard)]
    public void FlagAllMinesAndEnsureGameWasWonTest(GameEnums.GameDifficulty gd)
    {
        var game = new GameModel(gd);
        // get Random index point and call Play(randomI, randomJ)
        var random = new Random();
        var r = random.Next(0, game.Rows);
        var c = random.Next(0, game.Columns);
        Assert.Equal(GameEnums.GameStatus.NotStarted, game.GameStatus);
        Assert.Equal(0, game.Items.Count(i => i.IsMine));
        game.PlayCommand.Execute(new Point(r, c));
        Assert.Equal(GameConstants.GameLevels[gd].mines, game.Items.Count(i => i.IsMine));
        Assert.Equal(GameEnums.GameStatus.InProgress, game.GameStatus);

        for (var i = 0; i < game.Rows; i++)
        for (var j = 0; j < game.Columns; j++)
            if (game[i, j].IsMine && !game[i, j].IsRevealed)
            {
                {
                    game.FlagCommand.Execute(new Point(i, j));
                    if (game.GameStatus == GameEnums.GameStatus.Won)
                        goto ASSERT_WON;
                }
                Assert.Equal(GameEnums.GameStatus.InProgress, game.GameStatus);
            }

        ASSERT_WON:
        Assert.Equal(GameEnums.GameStatus.Won, game.GameStatus);
        // Assert Count of flagged items = game.Mins
        Assert.Equal(game.Mines, game.Items.Count(i => i.IsFlagged));
    }
}
