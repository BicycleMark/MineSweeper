using MineSweeper.Models;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class MainPage : ContentPage
{
    

    public MainPage()
    {
        InitializeComponent();
        
    }


    private void OnEasyClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NewGame_Clicked(object? sender, EventArgs e)
    {
        var gg = new GameGrid();
        gg.ItemSource = new List<SweeperItem>
        {
            new SweeperItem {IsMine = true},
            new SweeperItem {IsMine = false},
            new SweeperItem {IsMine = false},
            new SweeperItem {IsMine = false}
        };
        gg.ItemTemplate = new DataTemplate(() =>
        {
            var label = new Label();
            label.SetBinding(Label.TextProperty, nameof(SweeperItem.IsMine));
            return label;
        });
    }
}