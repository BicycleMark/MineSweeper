using System.Diagnostics;
using MineSweeper.Models;
using MineSweeper.ViewModels;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("EasyGame", typeof(MainPage));
        Routing.RegisterRoute("MediumGame", typeof(MainPage));
        Routing.RegisterRoute("HardGame", typeof(MainPage));


        Routing.RegisterRoute("SquareImageGridExample", typeof(SquareImageGridExample));
        Routing.RegisterRoute("LedControlExample", typeof(LedControlExample));

        // Subscribe to route selection events
        Navigating += OnShellNavigating;

        // Add debug logging
        Debug.WriteLine("AppShell: Constructor called");
        Debug.WriteLine("AppShell: Routes registered for navigation");
    }

    private void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        try
        {
            // Handle navigation based on the route
            if (e.Target.Location.OriginalString.Contains("EasyGame"))
            {
                // Cancel the navigation to EasyGame and stay on MainPage
                e.Cancel();

                // Start a new Easy game
                StartNewGame(GameEnums.GameDifficulty.Easy);
            }
            else if (e.Target.Location.OriginalString.Contains("MediumGame"))
            {
                // Cancel the navigation to MediumGame and stay on MainPage
                e.Cancel();

                // Start a new Medium game
                StartNewGame(GameEnums.GameDifficulty.Medium);
            }
            else if (e.Target.Location.OriginalString.Contains("HardGame"))
            {
                // Cancel the navigation to HardGame and stay on MainPage
                e.Cancel();

                // Start a new Hard game
                StartNewGame(GameEnums.GameDifficulty.Hard);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnShellNavigating: {ex}");
        }
    }

    private void StartNewGame(GameEnums.GameDifficulty difficulty)
    {
        try
        {
            // Get the current page
            if (Current.CurrentPage is MainPage mainPage)
            {
                // Get the view model and start a new game
                var viewModel = mainPage.BindingContext as GameViewModel;

                // Execute the command with the difficulty parameter
                if (viewModel != null)
                {
                    Debug.WriteLine($"Starting new game with difficulty: {difficulty}");
                    viewModel.NewGameCommand.Execute(difficulty);
                    mainPage.SelectRandomGameAnimationStyle();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error starting new game: {ex}");
        }
    }
}