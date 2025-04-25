﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.Dispatching;
using MineSweeper.Features.Game.Models;
using MineSweeper.Features.Game.Pages;
using MineSweeper.Features.Game.ViewModels;
using MineSweeper.Views.Controls;

namespace MineSweeper;

public partial class AppShell : Shell, IDisposable
{
    private bool _isContinuousAnimationsEnabled;
    private IDispatcherTimer? _continuousAnimationsTimer;
    private IDispatcherTimer? _countdownTimer;
    private GameEnums.GameDifficulty _currentDifficulty = GameEnums.GameDifficulty.Easy;
    private int _countdownSeconds = 10;
    
    public ICommand ToggleContinuousAnimationsCommand { get; private set; }
    
    public AppShell()
    {
        InitializeComponent();
        
        // Initialize the toggle command and set binding context
        ToggleContinuousAnimationsCommand = new Command(ToggleContinuousAnimations);
        BindingContext = this;

        // Register routes for navigation
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("GamePage", typeof(GamePage));
        Routing.RegisterRoute("EasyGame", typeof(GamePage));
        Routing.RegisterRoute("MediumGame", typeof(GamePage));
        Routing.RegisterRoute("HardGame", typeof(GamePage));


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

    /// <summary>
    /// Toggles the continuous animations feature on or off
    /// </summary>
    private void ToggleContinuousAnimations()
    {
        _isContinuousAnimationsEnabled = !_isContinuousAnimationsEnabled;
        
        if (_isContinuousAnimationsEnabled)
        {
            // Update menu item text
            ContinuousAnimationsMenuItem.Text = "★ DISABLE CONTINUOUS ANIMATIONS ★";
            
            // Initialize and start the timer
            InitializeContinuousAnimationsTimer();
            
            // Immediately advance to a random difficulty
            var random = new Random();
            _currentDifficulty = (GameEnums.GameDifficulty)random.Next(0, 3); // Random difficulty: 0=Easy, 1=Medium, 2=Hard
            Debug.WriteLine($"Immediately advanced to random difficulty: {_currentDifficulty}");
            
            // Start a new game with the random difficulty
            StartNewGame(_currentDifficulty);
            
            // The StartNewGame method will apply a random animation since _isContinuousAnimationsEnabled is true
            Debug.WriteLine("Immediately started new game with random difficulty and animation");
        }
        else
        {
            // Update menu item text
            ContinuousAnimationsMenuItem.Text = "★ ENABLE CONTINUOUS ANIMATIONS ★";
            
            // Stop the timers
            _continuousAnimationsTimer?.Stop();
            _continuousAnimationsTimer = null;
            
            _countdownTimer?.Stop();
            _countdownTimer = null;
            
            // Reset the game timer display to 0
            try
            {
                if (Current.CurrentPage is MainPage mainPage)
                {
                    var viewModel = mainPage.BindingContext as GameViewModel;
                    if (viewModel != null)
                    {
                        viewModel.GameTime = 0;
                    }
                    
                    // Restore the user's selected animation
                    mainPage.SelectRandomGameAnimationStyle();
                    Debug.WriteLine("Restored user's selected animation style");
                }
                else if (Current.CurrentPage is GamePage gamePage)
                {
                    // Restore the user's selected animation
                    gamePage.SelectRandomGameAnimationStyle();
                    Debug.WriteLine("Restored user's selected animation style");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error resetting game timer display or restoring animation: {ex}");
            }
        }
        
        // Close the flyout menu
        FlyoutIsPresented = false;
    }
    
    /// <summary>
    /// Initializes the timer for continuous animations
    /// </summary>
    private void InitializeContinuousAnimationsTimer()
    {
        // Initialize countdown
        _countdownSeconds = 10;
        
        // Create a countdown timer that updates every second
        _countdownTimer = Dispatcher.CreateTimer();
        _countdownTimer.Interval = TimeSpan.FromSeconds(1);
        _countdownTimer.Tick += OnCountdownTimerTick;
        _countdownTimer.Start();
        
        // Update the game timer display with the initial countdown value
        UpdateGameTimerDisplay();
        
        // We don't need a separate timer for cycling through difficulties
        // The countdown timer will handle this when it reaches zero
    }
    
    /// <summary>
    /// Handles the countdown timer tick event
    /// </summary>
    private void OnCountdownTimerTick(object? sender, EventArgs e)
    {
        // Decrement the countdown
        _countdownSeconds--;
        
        // Update the game timer display
        UpdateGameTimerDisplay();
        
        // If countdown reaches 0, start a new game and reset the countdown
        if (_countdownSeconds <= 0)
        {
            // Cycle to the next difficulty
            _currentDifficulty = _currentDifficulty switch
            {
                GameEnums.GameDifficulty.Easy => GameEnums.GameDifficulty.Medium,
                GameEnums.GameDifficulty.Medium => GameEnums.GameDifficulty.Hard,
                GameEnums.GameDifficulty.Hard => GameEnums.GameDifficulty.Easy,
                _ => GameEnums.GameDifficulty.Easy
            };
            
            // Start a new game with the next difficulty
            StartNewGame(_currentDifficulty);
            
            // Reset the countdown
            _countdownSeconds = 10;
        }
        // Force a random animation style every 3 seconds during continuous animations
        else if (_countdownSeconds % 3 == 0)
        {
            try
            {
                // Get the current page
                if (Current.CurrentPage is MainPage mainPage)
                {
                    // Force a random animation style
                    mainPage.ForceRandomAnimationStyle();
                    Debug.WriteLine($"Forced random animation style at countdown: {_countdownSeconds}");
                }
                else if (Current.CurrentPage is GamePage gamePage)
                {
                    // Force a random animation style
                    gamePage.ForceRandomAnimationStyle();
                    Debug.WriteLine($"Forced random animation style at countdown: {_countdownSeconds}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error forcing random animation style during countdown: {ex}");
            }
        }
    }
    
    /// <summary>
    /// Updates the game timer display with the countdown value
    /// </summary>
    private void UpdateGameTimerDisplay()
    {
        try
        {
            // Get the current page
            if (Current.CurrentPage is MainPage mainPage)
            {
                // Get the view model
                var viewModel = mainPage.BindingContext as GameViewModel;
                
                // Update the GameTime property to show the countdown
                if (viewModel != null)
                {
                    viewModel.GameTime = _countdownSeconds;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating game timer display: {ex}");
        }
    }
    
    /// <summary>
    /// Handles the timer tick event for continuous animations
    /// </summary>
    private void OnContinuousAnimationsTimerTick(object? sender, EventArgs e)
    {
        // Cycle to the next difficulty
        _currentDifficulty = _currentDifficulty switch
        {
            GameEnums.GameDifficulty.Easy => GameEnums.GameDifficulty.Medium,
            GameEnums.GameDifficulty.Medium => GameEnums.GameDifficulty.Hard,
            GameEnums.GameDifficulty.Hard => GameEnums.GameDifficulty.Easy,
            _ => GameEnums.GameDifficulty.Easy
        };
        
        // Start a new game with the next difficulty
        StartNewGame(_currentDifficulty);
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
                    
                    // If continuous animations are enabled, force a random animation style
                    // Otherwise, use the selected animation style
                    if (_isContinuousAnimationsEnabled)
                    {
                        mainPage.ForceRandomAnimationStyle();
                        Debug.WriteLine("Forced random animation style for continuous animations");
                    }
                    else
                    {
                        mainPage.SelectRandomGameAnimationStyle();
                        Debug.WriteLine("Using selected animation style");
                    }
                }
            }
            else if (Current.CurrentPage is GamePage gamePage)
            {
                // Get the view model from the GamePage
                var viewModel = gamePage.BindingContext as GameViewModel;

                // Execute the command with the difficulty parameter
                if (viewModel != null)
                {
                    Debug.WriteLine($"Starting new game with difficulty: {difficulty}");
                    viewModel.NewGameCommand.Execute(difficulty);
                    
                    // If continuous animations are enabled, force a random animation style
                    // Otherwise, use the selected animation style
                    if (_isContinuousAnimationsEnabled)
                    {
                        gamePage.ForceRandomAnimationStyle();
                        Debug.WriteLine("Forced random animation style for continuous animations");
                    }
                    else
                    {
                        gamePage.SelectRandomGameAnimationStyle();
                        Debug.WriteLine("Using selected animation style");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error starting new game: {ex}");
        }
    }
    
    #region IDisposable Implementation
    
    private bool _disposed;
    
    /// <summary>
    /// Disposes resources used by the AppShell
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Disposes resources used by the AppShell
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            // Clean up managed resources
            if (_continuousAnimationsTimer != null)
            {
                _continuousAnimationsTimer.Stop();
                _continuousAnimationsTimer.Tick -= OnContinuousAnimationsTimerTick;
                _continuousAnimationsTimer = null;
            }
            
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Tick -= OnCountdownTimerTick;
                _countdownTimer = null;
            }
            
            // Unsubscribe from events
            Navigating -= OnShellNavigating;
        }
        
        _disposed = true;
    }
    
    /// <summary>
    /// Finalizer
    /// </summary>
    ~AppShell()
    {
        Dispose(false);
    }
    
    #endregion
}
