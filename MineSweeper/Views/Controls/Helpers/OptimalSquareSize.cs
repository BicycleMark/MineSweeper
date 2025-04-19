namespace MineSweeper.Views.Controls.Helpers;

public class OptimalSquareSize
{
    public static (double MaxSquareSize, int Rows, int Columns) Optimize(
    double width,
    double height,
    int maxNumberOfSquares,
    double spaceBetween = 0)
{
    if (maxNumberOfSquares <= 0)
    {
        throw new ArgumentException("Number of squares must be positive", nameof(maxNumberOfSquares));
    }
   
    if (width <= 0 || height <= 0)
    {
        throw new ArgumentException("Width and height must be positive");
    }
   
    if (spaceBetween < 0)
    {
        throw new ArgumentException("Space between squares cannot be negative", nameof(spaceBetween));
    }
   
    double maxSquareSize = 0;
    int optimalRows = 0;
    int optimalColumns = 0;
   
    // Try all possible row and column combinations that could fit within maxNumberOfSquares
    for (int rows = 1; rows <= maxNumberOfSquares; rows++)
    {
        for (int columns = 1; columns <= maxNumberOfSquares; columns++)
        {
            // Skip if this arrangement would need more squares than we have
            if (rows * columns > maxNumberOfSquares)
            {
                continue;
            }
           
            // Calculate available space considering the spacing between squares
            double availableWidth = width - spaceBetween * (columns - 1);
            double availableHeight = height - spaceBetween * (rows - 1);
           
            // Skip if the arrangement doesn't leave enough space for squares
            if (availableWidth <= 0 || availableHeight <= 0)
            {
                continue;
            }
           
            // Calculate maximum possible square size for this arrangement
            double maxWidthPerSquare = availableWidth / columns;
            double maxHeightPerSquare = availableHeight / rows;
           
            // Square size is limited by the smaller dimension
            double squareSize = Math.Min(maxWidthPerSquare, maxHeightPerSquare);
           
            // Update if this arrangement gives a larger square size
            if (squareSize > maxSquareSize)
            {
                maxSquareSize = squareSize;
                optimalRows = rows;
                optimalColumns = columns;
            }
        }
    }
   
    return (maxSquareSize, optimalRows, optimalColumns);
}
}