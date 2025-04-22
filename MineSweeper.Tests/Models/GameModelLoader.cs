using System.Text.Json;
using MineSweeper.Features.Game.Models;

namespace MineSweeper.Tests.Models;

/// <summary>
/// Helper class for loading game models from JSON
/// </summary>
public static class GameModelLoader
{
    /// <summary>
    /// Creates a mock game model from JSON data
    /// </summary>
    /// <param name="jsonData">The JSON data to load from</param>
    /// <returns>A mock game model with the state defined in the JSON</returns>
    public static MockGameModel LoadFromJson(string jsonData)
    {
        // Parse the JSON data
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var gameData = JsonSerializer.Deserialize<GameModelData>(jsonData, options);
        
        if (gameData == null)
            throw new ArgumentException("Invalid JSON data", nameof(jsonData));
            
        // Create a mock game model with the parsed data
        var mockModel = new MockGameModel(
            gameData.Rows, 
            gameData.Columns, 
            gameData.Mines);
            
        // Count flagged cells to calculate remaining mines
        int flaggedCount = 0;
        if (gameData.FlaggedPositions != null)
        {
            flaggedCount = gameData.FlaggedPositions.Length;
        }
            
        // Set the game status
        if (Enum.TryParse<GameEnums.GameStatus>(gameData.Status, true, out var status))
            mockModel.GameStatus = status;
            
        // Set mines
        if (gameData.MinePositions != null)
        {
            foreach (var position in gameData.MinePositions)
            {
                if (position.Length == 2)
                    mockModel.SetMine(position[0], position[1], true);
            }
        }
        
        // Temporarily set the remaining mines to a higher value
        // This is needed because SetFlagged decrements the remaining mines
        var field = typeof(MockGameModel).GetField("_remainingMines", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            // Set to mines + flaggedCount so that after SetFlagged decrements it, it will be correct
            field.SetValue(mockModel, gameData.Mines + flaggedCount);
        }
        
        // Set flagged cells
        if (gameData.FlaggedPositions != null)
        {
            foreach (var position in gameData.FlaggedPositions)
            {
                if (position.Length == 2)
                {
                    mockModel.SetFlagged(position[0], position[1], true);
                }
            }
        }
        
        // Set revealed cells
        if (gameData.RevealedPositions != null)
        {
            foreach (var position in gameData.RevealedPositions)
            {
                if (position.Length == 2)
                    mockModel.SetRevealed(position[0], position[1], true);
            }
        }
        
        // Calculate mine counts
        CalculateMineCountsForModel(mockModel);
        
        return mockModel;
    }
    
    /// <summary>
    /// Helper method to calculate mine counts for cells in a mock model
    /// </summary>
    private static void CalculateMineCountsForModel(MockGameModel model)
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
    private static int CountAdjacentMines(MockGameModel model, int row, int col)
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
    
    /// <summary>
    /// Data class for deserializing game model JSON
    /// </summary>
    private class GameModelData
    {
        public int Rows { get; set; } = 10;
        public int Columns { get; set; } = 10;
        public int Mines { get; set; } = 10;
        public string Status { get; set; } = "NotStarted";
        public int[][] MinePositions { get; set; } = Array.Empty<int[]>();
        public int[][] FlaggedPositions { get; set; } = Array.Empty<int[]>();
        public int[][] RevealedPositions { get; set; } = Array.Empty<int[]>();
    }
}
