using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class MainPage
{
    
    private void GameBorder_OnSizeChanged(object sender, EventArgs e)
    {
        // Update the grid size when the game border size changes
        SetGridSize(_viewModel.Rows, _viewModel.Columns);
    }

    // https://shorturl.at/leJCN
    private void SetGridSize(int rows, int columns)
    {
        GameGrid.Children.Clear();
        var cellSize  = new Size( gameBorder.Width / columns, gameBorder.Height / rows);
       
        for (int i = 0; i < rows; i++)
        {
            HorizontalStackLayout hz = new HorizontalStackLayout()
            {
                Spacing = 0
            };
            for (int j = 0; j < columns; j++)
            {
                // Create a brush with an image
                
                // Create a new cell with the specified size
                var f = new Rectangle()
                {
                    WidthRequest = cellSize.Width,
                    HeightRequest = cellSize.Height,
                    Margin = 0,
                    Stroke = Colors.Black,
                    BackgroundColor = Colors.Transparent,
                    Fill = Colors.Transparent,
                    
                };
                hz.Children.Add(f);
            }
            GameGrid.Children.Add(hz);
           

        }


        //AddBordersToGrid();
    }


/*
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
                Source = "unplayed.png", // This will be used for reference, but we're using color for now
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
*/

}
