using MineSweeper.Models;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Error during InitializeComponent: {ex.Message}");
        }
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
            new() {IsMine = true},
            new() {IsMine = false},
            new() {IsMine = false},
            new() {IsMine = false}
        };
        gg.ItemTemplate = new DataTemplate(() =>
        {
            var label = new Label();
            label.SetBinding(Label.TextProperty, nameof(SweeperItem.IsMine));
            return label;
        });
    }
}