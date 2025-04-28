using System.Diagnostics;
using Microsoft.Maui.Controls.Shapes;
using MineSweeper.Extensions;
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
    private GridAnimationExtensions.AnimationType _selectedAnimationType;

    public MainPage(GameViewModel viewModel, ILogger logger)
    {
        InitializeComponent();
        _logger = logger;
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Initialize animation manager
        _animationManager = new GridAnimationManager(GameGrid);
        
        // Set up the chiseled border for the top panel
        SetupTopPanelBorder();
        
        // Set up the status bar border
        SetupStatusBarBorder();
        
        // Initialize the animation picker
        InitializeAnimationPicker();

        // Start a new game when the page is loaded
        Loaded += OnPageLoaded;
        
        // Subscribe to the GameTileTapped event instead of TileTapped
        // GameTileTapped is guaranteed to only fire for actual game tiles, never for whitespace
        GameGrid.GameTileTapped += OnTileTapped;
        
        // Log that we've subscribed to the event
        _logger.Log("Subscribed to GameTileTapped event (guaranteed no whitespace clicks)");
    }
    
    /// <summary>
    ///     Sets up the chiseled border for the status bar.
    /// </summary>
    private void SetupStatusBarBorder()
    {
        try
        {
            // Get the current app theme
            var isDarkTheme = Application.Current?.RequestedTheme == AppTheme.Dark;

            // Create a new ChiseledBorderDrawable for the status bar
            var borderDrawable = new ChiseledBorderDrawable
            {
                BorderThickness = 6,
                // Match the colors used in the game grid's ChiseledBorder
                ShadowColor = isDarkTheme ? Colors.Black : Colors.DimGray,
                HighlightColor = isDarkTheme ? Color.FromArgb("#444444") : Colors.LightGray,
                IsRecessed = true
            };

            // Set the drawable for the status bar
            StatusBar.Drawable = borderDrawable;

            // Force a redraw
            StatusBar.Invalidate();

            _logger.Log("Status bar border set up successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error setting up status bar border: {ex.Message}");
        }
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
    ///     Applies the selected animation style for the game grid.
    /// </summary>
    public void SelectRandomGameAnimationStyle()
    {
        try
        {
            // Use the selected animation type if available, otherwise use random
            if (_selectedAnimationType != null)
            {
                _animationManager.ForceAnimationType(_selectedAnimationType);
                _logger.Log($"Selected animation style applied: {_selectedAnimationType}");
            }
            else
            {
                // Clear any forced animation type to allow random selection
                _animationManager.ForcedAnimationType = null;
                
                // Select a random animation style
                _animationManager.SelectRandomAnimationStyle();
                _logger.Log($"Random animation style selected: {_animationManager.CurrentAnimationType}");
            }
            
            // Update the animation picker with the current animation type
            UpdateStatusLabel();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error selecting animation style: {ex.Message}");
        }
    }
    
    /// <summary>
    ///     Forces a random animation style for continuous animations mode.
    /// </summary>
    public void ForceRandomAnimationStyle()
    {
        try
        {
            // Clear any forced animation type to allow random selection
            _animationManager.ForcedAnimationType = null;
            
            // Get all animation types
            var animationTypes = Enum.GetValues<GridAnimationExtensions.AnimationType>();
            
            // Select a random animation type directly
            var random = new Random();
            var randomIndex = random.Next(animationTypes.Length);
            var randomAnimationType = animationTypes[randomIndex];
            
            // Force this random animation type
            _animationManager.ForceAnimationType(randomAnimationType);
            
            _logger.Log($"Random animation style forced: {randomAnimationType}");
            
            // Update the animation picker with the current animation type
            UpdateStatusLabel();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error forcing random animation style: {ex.Message}");
        }
    }
    
    /// <summary>
    ///     Initializes the animation picker with all available animation types.
    /// </summary>
    private void InitializeAnimationPicker()
    {
        try
        {
            // Set the default selected animation type
            _selectedAnimationType = _animationManager.CurrentAnimationType;
            
            // Update the animation label
            AnimationLabel.Text = _selectedAnimationType.ToString();
            
            // Initialize the tile status label
            TileStatus.Text = "Click a tile";
            
            _logger.Log("Animation picker and tile status initialized");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error initializing animation picker: {ex.Message}");
        }
    }
    
    /// <summary>
    ///     Handles the animation label tap event.
    /// </summary>
    private async void OnAnimationLabelTapped(object sender, EventArgs e)
    {
        try
        {
            // Create a modal page for animation selection
            var modalPage = new ContentPage
            {
                Title = "Select Animation"
            };
            
            // Create a scrollable container for the animation options
            var scrollView = new ScrollView();
            var stackLayout = new VerticalStackLayout
            {
                Spacing = 10,
                Padding = new Thickness(20)
            };
            
            // Get all animation types
            var animationTypes = Enum.GetValues<GridAnimationExtensions.AnimationType>();
            
            // Create a radio button for each animation type
            foreach (var animationType in animationTypes)
            {
                var animationName = animationType.ToString();
                
                // Create a horizontal layout for each option
                var optionLayout = new HorizontalStackLayout
                {
                    Spacing = 10
                };
                
                // Create a checkbox (using a custom renderer for simplicity)
                var checkBox = new CheckBox
                {
                    IsChecked = animationType.Equals(_selectedAnimationType),
                    Color = Colors.Blue
                };
                
                // Create a label for the animation name
                var label = new Label
                {
                    Text = animationName,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 16
                };
                
                // Add tap gesture to the entire layout
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, args) =>
                {
                    // Uncheck all checkboxes
                    foreach (var layout in stackLayout.Children)
                    {
                        if (layout is HorizontalStackLayout horizontalLayout && 
                            horizontalLayout.Children.FirstOrDefault() is CheckBox cb)
                        {
                            cb.IsChecked = false;
                        }
                    }
                    
                    // Check this checkbox
                    checkBox.IsChecked = true;
                    
                    // Set the selected animation type
                    if (Enum.TryParse<GridAnimationExtensions.AnimationType>(animationName, out var selectedType))
                    {
                        _selectedAnimationType = selectedType;
                        _logger.Log($"Animation type selected: {_selectedAnimationType}");
                        
                        // Update the animation label
                        AnimationLabel.Text = _selectedAnimationType.ToString();
                        
                        // Apply the selected animation type
                        _animationManager.ForceAnimationType(_selectedAnimationType);
                        
                        // Close the modal
                        modalPage.Navigation.PopModalAsync();
                    }
                };
                
                // Add the tap gesture to both the checkbox and label
                optionLayout.GestureRecognizers.Add(tapGesture);
                
                // Add the checkbox and label to the option layout
                optionLayout.Children.Add(checkBox);
                optionLayout.Children.Add(label);
                
                // Add the option layout to the stack layout
                stackLayout.Children.Add(optionLayout);
            }
            
            // Add a cancel button
            var cancelButton = new Button
            {
                Text = "Cancel",
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            
            cancelButton.Clicked += (s, args) =>
            {
                modalPage.Navigation.PopModalAsync();
            };
            
            stackLayout.Children.Add(cancelButton);
            
            // Set the content of the scroll view
            scrollView.Content = stackLayout;
            
            // Set the content of the modal page
            modalPage.Content = scrollView;
            
            // Show the modal page
            await Navigation.PushModalAsync(modalPage);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling animation label tap: {ex.Message}");
        }
    }
    
    /// <summary>
    ///     Gets the currently selected animation type.
    /// </summary>
    /// <returns>The selected animation type, or null if none is selected.</returns>
    public GridAnimationExtensions.AnimationType? GetSelectedAnimationType()
    {
        return _selectedAnimationType;
    }
    
    /// <summary>
    ///     Updates the animation label with the current animation type.
    /// </summary>
    private void UpdateStatusLabel()
    {
        try
        {
            // Get the current animation type
            var animationType = _animationManager.CurrentAnimationType;
            
            // Update the animation label
            AnimationLabel.Text = animationType.ToString();
            
            _logger.Log($"Animation label updated with animation type: {animationType}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating animation label: {ex.Message}");
        }
    }
    
    /// <summary>
    ///     Handles the tile tapped event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnTileTapped(object? sender, TileTappedEventArgs e)
    {
        try
        {
            
            // Update the tile status label with the row and column information
            TileStatus.Text = $"Tile: Row {e.Row}, Col {e.Column}";
            
            // Add a timestamp to help identify if this is a new event or a cached one
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            Debug.WriteLine($"[{timestamp}] OnTileTapped called - Row {e.Row}, Col {e.Column}, IsDefaultTile: {e.IsLongHold}");
            
            
            _logger.Log($"Tile tapped at row {e.Row}, column {e.Column}, isLongHold {e.IsLongHold}");
            if (!e.IsLongHold)
            {
                // If this is not a long hold, we can Play the tile
                // and remove the tap handler
                // This ensures it only fires once
                e.DoRemove = true;
            }
            else
            {
                // Toggle Flag on the tile
            }
            // Force the UI to update immediately
            MainThread.BeginInvokeOnMainThread(() => {
                TileStatus.Text = $"Tile: Row {e.Row}, Col {e.Column} {e.IsLongHold}";
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling tile tapped event: {ex.Message}");
            Debug.WriteLine($"Error in OnTileTapped: {ex.Message}\n{ex.StackTrace}");
        }
    }
    
    /// <summary>
    ///     Handles direct taps on the grid.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnGridDirectTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            // Get the tapped position
            if (e.GetPosition(GameGrid) is Point position)
            {
                // Calculate the cell size
                var cellWidth = GameGrid.Width / GameGrid.Columns;
                var cellHeight = GameGrid.Height / GameGrid.Rows;

                // Calculate the row and column
                var row = (int)(position.Y / cellHeight);
                var col = (int)(position.X / cellWidth);

                // Add a timestamp to help identify if this is a new event or a cached one
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                Debug.WriteLine($"[{timestamp}] OnGridDirectTapped called - Position: ({position.X}, {position.Y}), calculated Row {row}, Col {col}");
                
                // Ensure the row and column are within bounds
                if (row >= 0 && row < GameGrid.Rows && col >= 0 && col < GameGrid.Columns)
                {
                    // Update the tile status label with the row and column information
                    MainThread.BeginInvokeOnMainThread(() => {
                        TileStatus.Text = $"Direct Tap: Row {row}, Col {col} @ {timestamp}";
                    });
                    
                    _logger.Log($"Direct tap at row {row}, column {col}");
                }
                else
                {
                    Debug.WriteLine($"Tap position out of bounds: Row {row}, Col {col}");
                }
            }
            else
            {
                Debug.WriteLine("Could not get tap position");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling direct grid tap: {ex.Message}");
            Debug.WriteLine($"Error in OnGridDirectTapped: {ex.Message}\n{ex.StackTrace}");
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

            // Start a new game with Easy difficulty
            await _viewModel.NewGameCommand.ExecuteAsync(GameEnums.GameDifficulty.Easy);
            _logger.Log("New game created with Easy difficulty");

            // Set up animations with the selected animation type
            SelectRandomGameAnimationStyle();
            _animationManager.SetupAnimations();
            
            // Create the grid
            GameGrid.CreateGrid(_viewModel.Rows, _viewModel.Columns);
            _logger.Log($"Grid created with {_viewModel.Rows} rows and {_viewModel.Columns} columns");
            
            // Re-subscribe to the GameTileTapped event
            GameGrid.GameTileTapped -= OnTileTapped; // Remove any existing subscription
            GameGrid.GameTileTapped += OnTileTapped; // Re-subscribe
            _logger.Log("Re-subscribed to GameTileTapped event (guaranteed no whitespace clicks)");
            
            // Remove any direct tap gesture recognizers from the GameGrid
            foreach (var gesture in GameGrid.GestureRecognizers.OfType<TapGestureRecognizer>().ToList())
            {
                GameGrid.GestureRecognizers.Remove(gesture);
            }
            _logger.Log("Removed direct tap gesture recognizers from GameGrid");
            
            // Remove any overlay grids that might have been added previously
            RemoveOverlayGrids();
            _logger.Log("Removed any overlay grids");
            
            // Update the status label with the current animation type
            UpdateStatusLabel();
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
    
    /// <summary>
    ///     Removes any overlay grids that might have been added previously.
    /// </summary>
    private void RemoveOverlayGrids()
    {
        try
        {
            // Get the parent of the game grid
            var gameGridParent = GameGrid.Parent as Layout;
            if (gameGridParent != null)
            {
                // Find and remove any existing overlay grids
                var isOverlayProperty = BindableProperty.Create("IsOverlay", typeof(bool), typeof(Grid), false);
                var existingOverlays = gameGridParent.Children.Where(c => c is Grid && (c as BindableObject)?.GetValue(isOverlayProperty) as bool? == true).ToList();
                foreach (var overlay in existingOverlays)
                {
                    gameGridParent.Children.Remove(overlay);
                    Debug.WriteLine("Removed existing overlay grid");
                }
            }
            
            _logger.Log("Removed any existing overlay grids");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error removing overlay grids: {ex.Message}");
            Debug.WriteLine($"Error in RemoveOverlayGrids: {ex.Message}\n{ex.StackTrace}");
        }
    }
    
    /// <summary>
    ///     Handles taps on individual cells.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnCellTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            // Get the tap gesture recognizer that was tapped
            if (sender is TapGestureRecognizer tapGesture)
            {
                // Get the row and column from the attached properties
                var row = (int)tapGesture.GetValue(BindableProperty.Create("Row", typeof(int), typeof(TapGestureRecognizer), 0));
                var col = (int)tapGesture.GetValue(BindableProperty.Create("Column", typeof(int), typeof(TapGestureRecognizer), 0));
                
                // Add a timestamp to help identify if this is a new event or a cached one
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                Debug.WriteLine($"[{timestamp}] OnCellTapped called - Row {row}, Col {col}");
                
                // Update the tile status label with the row and column information
                MainThread.BeginInvokeOnMainThread(() => {
                    TileStatus.Text = $"Cell Tap: Row {row}, Col {col} @ {timestamp}";
                });
                
                _logger.Log($"Cell tapped at row {row}, column {col}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling cell tap: {ex.Message}");
            Debug.WriteLine($"Error in OnCellTapped: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
