using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class MainPage
{

    private void SetGridSize(int rows, int columns)
    {
        GameGrid.Children.Clear();
        GameGrid.RowDefinitions.Clear();
        GameGrid.ColumnDefinitions.Clear();
        for (int i = 0; i < rows; i++)
        {
            RowDefinition rowDef = new RowDefinition {Height = GridLength.Star};
            GameGrid.RowDefinitions.Add(rowDef);
        }

        for (int j = 0; j < columns; j++)
        {
            ColumnDefinition colDef = new ColumnDefinition {Width = GridLength.Star};
            GameGrid.ColumnDefinitions.Add(colDef);
        }
        
        // Remove any spacing in the Game Grid itself
        GameGrid.RowSpacing = 0;
        GameGrid.ColumnSpacing = 0;

        AddBordersToGrid();
    }

    private void AddBordersToGrid()
    {
        for (int i = 0; i < GameGrid.RowDefinitions.Count; i++)
        {
            for (int j = 0; j < GameGrid.ColumnDefinitions.Count; j++)
            {
                
                // Use our new approach with a Rectangle filled with solid color 
                // (instead of image brush which isn't accessible)
                ImageBrushCell cell = new ImageBrushCell
                {
                    Source = "button.png", // This will be used for reference, but we're using color for now
                    Point = new Point(i, j),
                    // Ensure the cell fills the entire grid cell space
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };
                
                // Add gesture recognizer directly to the cell
                cell.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(OnCellTapped, (o) => true),
                    CommandParameter = cell
                });
                
                // Set grid cell position
                Grid.SetRow(cell, i);
                Grid.SetColumn(cell, j);
                
                // Add directly to the game grid
                GameGrid.Children.Add(cell);
            }
        }

    }

    private void OnCellTapped(object o)
    {
        if (o is ImageBrushCell imageBrushCell)
        {
            Console.WriteLine($"Clicked: {imageBrushCell.Point}");
        }
        else if (o is FullSizeImage fullSizeImage)
        {
            Console.WriteLine($"Clicked: {fullSizeImage.Point}");
        }
        else if (o is ItemImage itemImage)
        {
            Console.WriteLine($"Clicked: {itemImage.Point}");
        }
    }
    
}
