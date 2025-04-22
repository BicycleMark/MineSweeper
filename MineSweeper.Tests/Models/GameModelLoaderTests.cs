using Microsoft.Maui.Graphics;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Services.Logging;
using MineSweeper.Tests.Integration;
using System.Text.Json;

namespace MineSweeper.Tests.Models;

/// <summary>
/// Tests for the GameModelLoader class
/// </summary>
public class GameModelLoaderTests
{
    [Fact]
    public void LoadFromJson_CreatesCorrectModel_Test()
    {
        // Sample JSON representation
        string jsonData = @"{
            ""rows"": 5,
            ""columns"": 5,
            ""mines"": 3,
            ""status"": ""InProgress"",
            ""minePositions"": [[0,0], [2,2], [4,4]],
            ""flaggedPositions"": [[0,0], [4,4]],
            ""revealedPositions"": [[1,1], [2,3], [3,3]]
        }";
        
        // Load the model from JSON
        var mockModel = GameModelLoader.LoadFromJson(jsonData);
        
        // Verify the state
        Assert.Equal(5, mockModel.Rows);
        Assert.Equal(5, mockModel.Columns);
        Assert.Equal(3, mockModel.Mines);
        Assert.Equal(GameEnums.GameStatus.InProgress, mockModel.GameStatus);
        
        // Verify mines
        Assert.True(mockModel.IsMine(0, 0));
        Assert.True(mockModel.IsMine(2, 2));
        Assert.True(mockModel.IsMine(4, 4));
        Assert.False(mockModel.IsMine(1, 1));
        
        // Verify flagged cells
        Assert.True(mockModel.IsFlagged(0, 0));
        Assert.True(mockModel.IsFlagged(4, 4));
        Assert.False(mockModel.IsFlagged(2, 2));
        
        // Verify revealed cells
        Assert.True(mockModel.IsRevealed(1, 1));
        Assert.True(mockModel.IsRevealed(2, 3));
        Assert.True(mockModel.IsRevealed(3, 3));
        Assert.False(mockModel.IsRevealed(0, 0));
        
        // Verify mine counts
        Assert.Equal(1, mockModel.GetMineCount(0, 1));
        Assert.Equal(1, mockModel.GetMineCount(1, 0));
        Assert.Equal(2, mockModel.GetMineCount(1, 1));
        
        // Verify remaining mines
        Assert.Equal(1, mockModel.RemainingMines);
    }
    
    [Fact]
    public void LoadFromJson_WithViewModel_Test()
    {
        // Sample JSON representation
        string jsonData = @"{
            ""rows"": 5,
            ""columns"": 5,
            ""mines"": 3,
            ""status"": ""InProgress"",
            ""minePositions"": [[0,0], [2,2], [4,4]],
            ""flaggedPositions"": [[0,0]],
            ""revealedPositions"": [[1,1], [2,3], [3,3]]
        }";
        
        // Load the model from JSON
        var mockModel = GameModelLoader.LoadFromJson(jsonData);
        
        // Create a view model with dependencies
        var dispatcher = new TestDispatcher();
        var logger = new TestLogger();
        var modelFactory = new GameModelFactory(logger);
        var viewModel = new GameViewModel(dispatcher, logger, modelFactory);
        
        // Use reflection to set the mock model in the view model
        var modelField = typeof(GameViewModel).GetField("_gameModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        modelField?.SetValue(viewModel, mockModel);
        
        // Update properties from model
        typeof(GameViewModel)
            .GetMethod("UpdatePropertiesFromModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(viewModel, Array.Empty<object>());
        
        // Verify the view model has the correct properties
        Assert.Equal(5, viewModel.Rows);
        Assert.Equal(5, viewModel.Columns);
        Assert.Equal(3, viewModel.Mines);
        Assert.Equal(GameEnums.GameStatus.InProgress, viewModel.GameStatus);
        Assert.Equal(2, viewModel.RemainingMines);
        
        // Play a move
        viewModel.PlayCommand.Execute(new Point(1, 1));
        
        // Verify the cell is revealed
        Assert.True(mockModel.IsRevealed(1, 1));
        
        // Flag a cell
        viewModel.FlagCommand.Execute(new Point(2, 2));
        
        // Verify the cell is flagged
        Assert.True(mockModel.IsFlagged(2, 2));
        
        // Verify remaining mines is updated
        Assert.Equal(1, viewModel.RemainingMines);
    }
    
    [Fact]
    public void LoadFromJson_InvalidJson_ThrowsException_Test()
    {
        // Invalid JSON
        string invalidJson = @"{
            ""rows"": 5,
            ""columns"": 5,
            ""mines"": 3,
            ""status"": ""InProgress"",
            ""minePositions"": [[0,0], [2,2], [4,4]],
            ""flaggedPositions"": [[0,0], [4,4]],
            ""revealedPositions"": [[1,1], [2,3], [3,3]]
            invalid
        }";
        
        // Verify that an exception is thrown
        Assert.Throws<JsonException>(() => GameModelLoader.LoadFromJson(invalidJson));
    }
    
    [Fact]
    public void LoadFromJson_EmptyJson_ThrowsException_Test()
    {
        // Empty JSON
        string emptyJson = "";
        
        // Verify that an exception is thrown
        Assert.Throws<JsonException>(() => GameModelLoader.LoadFromJson(emptyJson));
    }
    
    [Fact]
    public void LoadFromJson_NullJson_ThrowsException_Test()
    {
        // Verify that an exception is thrown
        Assert.Throws<ArgumentNullException>(() => GameModelLoader.LoadFromJson(null));
    }
}
