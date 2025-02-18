using System.Text.Json;
using MineSweeper.Models;

namespace MineSweeper.Tests.Models;

public class GameModelTests
{
    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy)]
    [InlineData(GameEnums.GameDifficulty.Medium)]
    [InlineData(GameEnums.GameDifficulty.Hard)]
    public void TestGameDificultyConstructor(GameEnums.GameDifficulty gd)
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
        
    }
    
    // Test the Custom Game Constructor with several different values
    [Theory]
    [InlineData(10, 10, 10)] // Easy
    [InlineData(15, 15, 40)] // Medium
    [InlineData(20, 20, 80)] // Hard
    [InlineData(5, 5, 5)] // Custom
    public void TestCustomGameConstructor(int rows, int columns, int mines)
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
    }
    
    [Theory]
    [InlineData(10, 10, 10, "SavedGame.json")]
    [InlineData(15, 15, 40, "SavedGame.json")]
    [InlineData(20, 20, 80, "SavedGame.json")]
    public void TestSaveGameCustomConstructor(int rows, int columns, int mines, string fileName)
    {
        var game = new GameModel(rows, columns, mines);
        game.SaveGameCommand.Execute(fileName);
        var jsonFile = File.ReadAllText(fileName);
        var savedGame = JsonSerializer.Deserialize<GameModel>(jsonFile);
        Assert.NotNull(savedGame);
        Assert.Equal(rows, savedGame.Rows);
        Assert.Equal(columns, savedGame.Columns);
        Assert.Equal(mines, savedGame.Mines);
        Assert.NotNull(savedGame.Items);
        if (savedGame.Items != null) 
            Assert.Equal(savedGame.Items.Count, savedGame.Rows * savedGame.Columns);
    }
    
    [Theory]
    [InlineData(GameEnums.GameDifficulty.Easy, "SavedGame.json")]
    [InlineData(GameEnums.GameDifficulty.Medium, "SavedGame.json")]
    [InlineData(GameEnums.GameDifficulty.Hard, "SavedGame.json")]
    public void TestSaveGameDifficultyLevelConstructor
        (GameEnums.GameDifficulty gd, string fileName)
    {
        var game = new GameModel(gd);
        game.SaveGameCommand.Execute(fileName);
        var jsonFile = File.ReadAllText(fileName);
        var savedGame = JsonSerializer.Deserialize<GameModel>(jsonFile);
        Assert.NotNull(savedGame);
        Assert.Equal(GameConstants.GameLevels[gd].rows, savedGame.Rows);
        Assert.Equal(GameConstants.GameLevels[gd].columns, savedGame.Columns);
        Assert.Equal(GameConstants.GameLevels[gd].mines, savedGame.Mines);
        Assert.NotNull(savedGame.Items);
        if (savedGame.Items != null) 
            Assert.Equal(savedGame.Items.Count, savedGame.Rows * savedGame.Columns);
    }
}