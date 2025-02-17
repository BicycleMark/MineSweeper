using MineSweeper.Models;
using MineSweeper.PageModels;

namespace MineSweeper.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}