using System.Diagnostics;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Services.Logging;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class MainPage : ContentPage
{
    private readonly GridAnimationManager _animationManager;
    private readonly ILogger _logger;
    private readonly GameViewModel _viewModel;

    public MainPage(GameViewModel viewModel, ILogger logger)
    {
        InitializeComponent();
        _logger = logger;
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Initialize animation manager
        _animationManager = new GridAnimationManager(GameGrid);
        
        // Force the SwirlLikeADrainIntoPlace animation type
        //_animationManager.ForcedAnimationType = MineSweeper.Extensions.GridAnimationExtensions.AnimationType.SwirlLikeADrainIntoPlace;

        // Set up the chiseled border for the top panel
        SetupTopPanelBorder();

        // Start a new game when the page is loaded
        Loaded += OnPageLoaded;
    }

    /// <summary>
    ///     Sets up the chiseled border for the top panel.
    /// </summary>
    private void SetupTopPanelBorder()
    {
        try
        {
            // Get the current app theme
            var isDarkTheme = Application.Current?.RequestedTheme == AppTheme.Dark;

            // Create a new ChiseledBorderDrawable for the top panel
            var borderDrawable = new ChiseledBorderDrawable
            {
                BorderThickness = 6,
                // Match the colors used in the game grid's ChiseledBorder
                ShadowColor = isDarkTheme ? Colors.Black : Colors.DimGray,
                HighlightColor = isDarkTheme ? Color.FromArgb("#444444") : Colors.LightGray,
                IsRecessed = true
            };

            // Set the drawable for the top panel border
            TopPanelBorder.Drawable = borderDrawable;

            // Force a redraw
            TopPanelBorder.Invalidate();

            _logger.Log("Top panel border set up successfully");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Invalid argument for border setup: {ex.Message}");
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError($"Null reference in border setup: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error setting up top panel border: {ex.Message}");
        }
    }


    /// <summary>
    ///     Selects a random animation style for the game grid.
    /// </summary>
    public void SelectRandomGameAnimationStyle()
    {
        try
        {
            _animationManager.SelectRandomAnimationStyle();
            _logger.Log("Random animation style selected");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error selecting animation style: {ex.Message}");
        }
    }

    /// <summary>
    ///     Handles the page loaded event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        _logger.Log("Page loaded, initializing game");
        
        try
        {
            // Delay the game initialization to improve navigation performance
            await Task.Delay(200);

            // Select a random animation style for this game
            _animationManager.SelectRandomAnimationStyle();
            _logger.Log("Animation style selected");

            // Start a new game with Easy difficulty
            await _viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
            _logger.Log("New game created with Easy difficulty");

            // Set up animations
            _animationManager.SetupAnimations();
            
            // Create the grid
            GameGrid.CreateGrid(_viewModel.Rows, _viewModel.Columns);
            _logger.Log($"Grid created with {_viewModel.Rows} rows and {_viewModel.Columns} columns");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"UI operation error in OnPageLoaded: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError($"Task canceled in OnPageLoaded: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error in OnPageLoaded: {ex.Message}");
        }
    }


    /// <summary>
    ///     Called when the page appears.
    /// </summary>
    protected override void OnAppearing()
    {
        _logger.Log("MainPage: OnAppearing starting");

        try
        {
            base.OnAppearing();
            _logger.Log("MainPage: OnAppearing completed successfully");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"UI operation error in OnAppearing: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error in OnAppearing: {ex.Message}");
        }
    }

    /// <summary>
    ///     Called when the page disappears.
    /// </summary>
    protected override void OnDisappearing()
    {
        _logger.Log("MainPage: OnDisappearing starting");

        try
        {
            // Clean up animation manager
            _animationManager.Cleanup();
            base.OnDisappearing();
            _logger.Log("MainPage: OnDisappearing completed successfully");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"UI operation error in OnDisappearing: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error in OnDisappearing: {ex.Message}");
        }
    }
}
