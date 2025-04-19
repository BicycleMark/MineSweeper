using MineSweeper.Views.Controls.Helpers;
using Xunit;
namespace MineSweeper.Tests.Views.Controls.Helpers;

public class OptimalSquareSizeTests
{
    private const double Delta = 0.0001; // Delta for floating point comparison

    [Theory]
    [InlineData(10, 10, 4, 0, 5.0, 2, 2)]       // Basic square with 4 squares, no spacing
    [InlineData(16, 9, 12, 0, 3.0, 3, 4)]       // Rectangle with 12 squares, no spacing
    [InlineData(800, 600, 20, 0, 150.0, 4, 5)]   // Large rectangle with 20 squares, no spacing
    [InlineData(10, 10, 4, 0.5, 4.5, 2, 2)]     // Square with 4 squares, 0.5 spacing
    [InlineData(16, 9, 12, 1, 2.5, 3, 4)]       // Rectangle with 12 squares, 1.0 spacing
    [InlineData(100, 100, 25, 2, 3.84, 5, 5)]   // Square with 25 squares, 2.0 spacing
    public void OptimizeSquareSizeExact_ProducesCorrectResults(
        double width, double height, int squares, double spacing,
        double expectedSize, int expectedRows, int expectedCols)
    {
        // Act
        var result = OptimalSquareSize.Optimize(width, height, squares, spacing);
       
        // Assert
        Assert.Equal(expectedSize, result.MaxSquareSize, Delta);
        Assert.Equal(expectedRows, result.Rows);
        Assert.Equal(expectedCols, result.Columns);
    }

    [Theory]
    [InlineData(10, 10, 5, 0, 5.0, 2, 2)]       // At most 5 squares -> optimal is 4 squares
    [InlineData(16, 9, 13, 0, 3.0, 3, 4)]       // At most 13 squares -> optimal is 12 squares
    [InlineData(10, 10, 5, 0.5, 4.5, 2, 2)]     // At most 5 squares with spacing -> optimal is 4 squares
    [InlineData(100, 50, 100, 1, 9.8, 5, 10)]   // Rectangle with 100 squares max -> optimal is 50 squares
    [InlineData(15, 15, 9, 0, 5.0, 3, 3)]       // Square with 9 squares max -> uses all 9
    [InlineData(15, 15, 9, 1, 4.0, 3, 3)]       // Square with 9 squares max, 1.0 spacing
    [InlineData(24, 15, 16, 2, 4.0, 3, 5)]      // Rectangle with 16 squares max, 2.0 spacing -> uses 15 squares
    [InlineData(10, 10, 100, 0, 10.0, 1, 1)]    // Many squares available -> optimal is just 1 square
    public void OptimizeSquareSize_ProducesCorrectResults(
        double width, double height, int maxSquares, double spacing,
        double expectedSize, int expectedRows, int expectedCols)
    {
        // Act
        var result = OptimalSquareSize.Optimize(width, height, maxSquares, spacing);
       
        // Assert
        Assert.Equal(expectedSize, result.MaxSquareSize, Delta);
        Assert.Equal(expectedRows, result.Rows);
        Assert.Equal(expectedCols, result.Columns);
    }

    [Theory]
    [InlineData(0, 10, 5)]       // Zero width
    [InlineData(10, 0, 5)]       // Zero height
    [InlineData(10, 10, 0)]      // Zero squares
    [InlineData(10, 10, -1)]     // Negative squares
    [InlineData(-10, 10, 5)]     // Negative width
    [InlineData(10, -10, 5)]     // Negative height
    public void OptimizeSquareSizeExact_ThrowsOnInvalidInputs(double width, double height, int squares)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OptimalSquareSize.Optimize(width, height, squares));
    }

    [Theory]
    [InlineData(10, 10, 5, -1)]  // Negative spacing
    public void OptimizeSquareSizeExact_ThrowsOnNegativeSpacing(
        double width, double height, int squares, double spacing)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            OptimalSquareSize.Optimize(width, height, squares, spacing));
    }

    [Theory]
    [InlineData(100, 100, 11, 0)]  // Prime number with no spacing
    [InlineData(100, 100, 13, 2)]  // Prime number with spacing
    public void OptimizeSquareSizeExact_HandlesNonDivisibleCases(
        double width, double height, int squares, double spacing)
    {
        // For prime numbers, we expect either 1×N or N×1 arrangement
        var result = OptimalSquareSize.Optimize(width, height, squares, spacing);
       
        // Assert that rows or columns must be 1 for a prime number
        Assert.True(result.Rows == 1 || result.Columns == 1);
        Assert.Equal(squares, result.Rows * result.Columns);
    }

    [Theory]
    [InlineData(3, 3, 25, 0.5)]  // Too many squares with spacing
    public void OptimizeSquareSizeExact_HandlesImpossibleArrangements(
        double width, double height, int squares, double spacing)
    {
        // When spacing makes it impossible to fit all squares
        var result = OptimalSquareSize.Optimize(width, height, squares, spacing);
       
        // Assert that we still get a valid result (might be 0 size if truly impossible)
        // This test mainly ensures we don't throw exceptions
        Assert.True(result.MaxSquareSize >= 0);
    }

    [Theory]
    [InlineData(10, 10, 3, 0)]   // 3 squares - can only do 1×3 or 3×1
    [InlineData(16, 9, 7, 0)]    // 7 squares - can only do 1×7 or 7×1
    public void OptimizeSquareSizeExact_HandlesPrimeNumbers(
        double width, double height, int squares, double spacing)
    {
        var result = OptimalSquareSize.Optimize(width, height, squares, spacing);
       
        // For prime numbers, either rows=1 or columns=1
        bool validArrangement = (result.Rows == 1 || result.Columns == 1);
        Assert.True(validArrangement);
        Assert.Equal(squares, result.Rows * result.Columns);
    }

    [Theory]
    [InlineData(20, 10, 3, 0)]   // 3 squares in 2:1 rectangle
    public void OptimizeSquareSizeExact_ChoosingBetterOrientation(
        double width, double height, int squares, double spacing)
    {
        var result = OptimalSquareSize.Optimize(width, height, squares, spacing);
       
        // For a wide rectangle (20×10) with 3 squares, orientation should be 1×3 not 3×1
        Assert.Equal(1, result.Rows);
        Assert.Equal(3, result.Columns);
    }
    
}