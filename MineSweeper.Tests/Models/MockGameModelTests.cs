using Microsoft.Maui.Graphics;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Services.Logging;
using MineSweeper.Tests.Integration;

namespace MineSweeper.Tests.Models;

/// <summary>
/// Tests for the MockGameModel class
/// </summary>
public class MockGameModelTests
{
    [Fact]
    public void CreateMockGameWithKnownState_Test()
    {
        // Create a mock game model with a known state
        var mockModel = new MockGameModel(5, 5, 3);
        
        // Note: In a real game, mines are not placed until the first move
        // to prevent the user from hitting a mine on the first move.
        // For testing purposes, we'll set the game status to NotStarted initially.
        mockModel.GameStatus = GameEnums.GameStatus.NotStarted;
        
        // Set mine counts for cells around mines
        mockModel.SetMineCount(0, 1, 1);
        mockModel.SetMineCount(1, 0, 1);
        mockModel.SetMineCount(1, 1, 1);
        
        mockModel.SetMineCount(1, 2, 1);
        mockModel.SetMineCount(2, 1, 1);
        mockModel.SetMineCount(2, 3, 1);
        mockModel.SetMineCount(3, 2, 1);
        mockModel.SetMineCount(3, 3, 1);
        
        mockModel.SetMineCount(3, 4, 1);
        mockModel.SetMineCount(4, 3, 1);
        
        // Reveal some cells
        mockModel.SetRevealed(1, 1, true);
        mockModel.SetRevealed(2, 3, true);
        mockModel.SetRevealed(3, 3, true);
        
        // Flag some cells
        mockModel.SetFlagged(0, 0, true);
        mockModel.SetFlagged(4, 4, true);
        
        // Set the game status to in progress for testing
        mockModel.GameStatus = GameEnums.GameStatus.InProgress;
        
        // Set mines at specific positions (in a real game, this would happen after the first move)
        mockModel.SetMine(0, 0, true);
        mockModel.SetMine(2, 2, true);
        mockModel.SetMine(4, 4, true);
        
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
        
        // Verify mine counts
        Assert.Equal(1, mockModel.GetMineCount(0, 1));
        Assert.Equal(1, mockModel.GetMineCount(1, 0));
        Assert.Equal(1, mockModel.GetMineCount(1, 1));
        
        // Verify revealed cells
        Assert.True(mockModel.IsRevealed(1, 1));
        Assert.True(mockModel.IsRevealed(2, 3));
        Assert.True(mockModel.IsRevealed(3, 3));
        Assert.False(mockModel.IsRevealed(0, 0));
        
        // Verify flagged cells
        Assert.True(mockModel.IsFlagged(0, 0));
        Assert.True(mockModel.IsFlagged(4, 4));
        Assert.False(mockModel.IsFlagged(2, 2));
        
        // Verify remaining mines
        Assert.Equal(1, mockModel.RemainingMines);
    }
    
    [Fact]
    public void UseWithViewModel_Test()
    {
        // Create a mock game model with a known state
        var mockModel = new MockGameModel(5, 5, 3);
        
        // Note: In a real game, mines are not placed until the first move
        // to prevent the user from hitting a mine on the first move.
        // For testing purposes, we'll set the game status to NotStarted initially.
        mockModel.GameStatus = GameEnums.GameStatus.NotStarted;
        
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
        Assert.Equal(GameEnums.GameStatus.NotStarted, viewModel.GameStatus);
        Assert.Equal(3, viewModel.RemainingMines);
        
        // Play a move to ensure the game is in progress
        viewModel.PlayCommand.Execute(new Point(1, 1));
        
        // Verify the cell is revealed
        Assert.True(mockModel.IsRevealed(1, 1));
        
        // Verify the game status is InProgress
        Assert.Equal(GameEnums.GameStatus.InProgress, mockModel.GameStatus);
        
        // After the first move, place mines (in a real game, this would happen automatically)
        mockModel.SetMine(0, 0, true);
        mockModel.SetMine(2, 2, true);
        mockModel.SetMine(4, 4, true);
        
        // Set the game status to InProgress to allow flagging
        mockModel.GameStatus = GameEnums.GameStatus.InProgress;
        
        // Flag a cell directly using the model
        mockModel.SetFlagged(2, 2, true);
        
        // Verify the cell is flagged
        Assert.True(mockModel.IsFlagged(2, 2));
        
        // Update properties from model to ensure the view model has the latest values
        typeof(GameViewModel)
            .GetMethod("UpdatePropertiesFromModel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(viewModel, Array.Empty<object>());
            
        // Verify remaining mines is updated
        Assert.Equal(2, mockModel.RemainingMines);
        Assert.Equal(2, viewModel.RemainingMines);
    }
    
    [Fact]
    public void CreateMockGameFromJson_Test()
    {
        // This test demonstrates how you could create a mock game from JSON data
        // In a real implementation, you would deserialize the JSON into a model
        
        // Sample JSON representation (as a string for demonstration)
        string jsonData = @"{
            ""rows"": 5,
            ""columns"": 5,
            ""mines"": 3,
            ""status"": ""InProgress"",
            ""minePositions"": [[0,0], [2,2], [4,4]],
            ""flaggedPositions"": [[0,0], [4,4]],
            ""revealedPositions"": [[1,1], [2,3], [3,3]]
        }";
        
        // In a real implementation, you would deserialize the JSON
        // For this test, we'll manually create the model based on the JSON data
        
        // Create a mock game model
        var mockModel = new MockGameModel(5, 5, 3);
        
        // Note: In a real game, mines are not placed until the first move
        // to prevent the user from hitting a mine on the first move.
        // For this test, we're simulating a game that's already in progress.
        mockModel.GameStatus = GameEnums.GameStatus.InProgress;
        
        // Set mines (in a real game, this would happen after the first move)
        mockModel.SetMine(0, 0, true);
        mockModel.SetMine(2, 2, true);
        mockModel.SetMine(4, 4, true);
        
        // Set flagged cells
        mockModel.SetFlagged(0, 0, true);
        mockModel.SetFlagged(4, 4, true);
        
        // Set revealed cells
        mockModel.SetRevealed(1, 1, true);
        mockModel.SetRevealed(2, 3, true);
        mockModel.SetRevealed(3, 3, true);
        
        // Calculate mine counts
        CalculateMineCountsForMockModel(mockModel);
        
        // Verify the state
        Assert.Equal(5, mockModel.Rows);
        Assert.Equal(5, mockModel.Columns);
        Assert.Equal(3, mockModel.Mines);
        Assert.Equal(GameEnums.GameStatus.InProgress, mockModel.GameStatus);
        
        // Verify mines
        Assert.True(mockModel.IsMine(0, 0));
        Assert.True(mockModel.IsMine(2, 2));
        Assert.True(mockModel.IsMine(4, 4));
        
        // Verify flagged cells
        Assert.True(mockModel.IsFlagged(0, 0));
        Assert.True(mockModel.IsFlagged(4, 4));
        
        // Verify revealed cells
        Assert.True(mockModel.IsRevealed(1, 1));
        Assert.True(mockModel.IsRevealed(2, 3));
        Assert.True(mockModel.IsRevealed(3, 3));
        
        // Verify remaining mines
        Assert.Equal(1, mockModel.RemainingMines);
    }
    
    /// <summary>
    /// Helper method to calculate mine counts for cells in a mock model
    /// </summary>
    private void CalculateMineCountsForMockModel(MockGameModel model)
    {
        for (int row = 0; row < model.Rows; row++)
        {
            for (int col = 0; col < model.Columns; col++)
            {
                if (!model.IsMine(row, col))
                {
                    int count = CountAdjacentMines(model, row, col);
                    if (count > 0)
                    {
                        model.SetMineCount(row, col, count);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Helper method to count adjacent mines for a cell
    /// </summary>
    private int CountAdjacentMines(MockGameModel model, int row, int col)
    {
        int count = 0;
        
        for (int i = Math.Max(0, row - 1); i <= Math.Min(model.Rows - 1, row + 1); i++)
        {
            for (int j = Math.Max(0, col - 1); j <= Math.Min(model.Columns - 1, col + 1); j++)
            {
                if (i != row || j != col)
                {
                    if (model.IsMine(i, j))
                    {
                        count++;
                    }
                }
            }
        }
        
        return count;
    }
}
