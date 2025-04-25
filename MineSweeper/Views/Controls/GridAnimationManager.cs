using System.Diagnostics;
using MineSweeper.Extensions;

namespace MineSweeper.Views.Controls;

/// <summary>
///     Manages animations for any grid that implements IAnimatableGrid.
/// </summary>
public class GridAnimationManager
{
    private readonly IAnimatableGrid _grid;
    private readonly Random _random = new();
    private GridAnimationExtensions.AnimationType _currentAnimationType;
    private GridAnimationExtensions.AnimationPattern _currentPattern;
    
    /// <summary>
    ///     Gets the current animation type.
    /// </summary>
    public GridAnimationExtensions.AnimationType CurrentAnimationType => _currentAnimationType;
    
    /// <summary>
    ///     Gets the current animation pattern.
    /// </summary>
    public GridAnimationExtensions.AnimationPattern CurrentAnimationPattern => _currentPattern;
    
    /// <summary>
    ///     Gets or sets the forced animation type. If set, this animation type will always be used.
    ///     If null, a random animation type will be selected.
    /// </summary>
    public GridAnimationExtensions.AnimationType? ForcedAnimationType { get; set; }
    
    /// <summary>
    ///     Forces a specific animation type to be used.
    /// </summary>
    /// <param name="animationType">The animation type to force.</param>
    public void ForceAnimationType(GridAnimationExtensions.AnimationType animationType)
    {
        // Set the forced animation type
        ForcedAnimationType = animationType;
        
        // Set the current animation type directly
        _currentAnimationType = animationType;
        
        Debug.WriteLine($"Animation type forced to: {animationType}");
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GridAnimationManager" /> class.
    /// </summary>
    /// <param name="grid">The grid to manage animations for.</param>
    public GridAnimationManager(IAnimatableGrid grid)
    {
        _grid = grid;
        SelectRandomAnimationStyle();
    }

    /// <summary>
    ///     Selects an animation style and pattern. If ForcedAnimationType is set,
    ///     that animation type will be used; otherwise, a random type will be selected.
    /// </summary>
    public void SelectRandomAnimationStyle()
    {
        if (ForcedAnimationType.HasValue)
        {
            // Use the forced animation type
            _currentAnimationType = ForcedAnimationType.Value;
        }
        else
        {
            // Choose random animation type for this game
            var animationTypes = Enum.GetValues<GridAnimationExtensions.AnimationType>();
            _currentAnimationType = animationTypes[_random.Next(animationTypes.Length)];
        }

        // Choose random pattern for this game
        var patterns = Enum.GetValues<GridAnimationExtensions.AnimationPattern>();
        _currentPattern = patterns[_random.Next(patterns.Length)];

        Debug.WriteLine($"Grid animation style: {_currentAnimationType}, Pattern: {_currentPattern}");
    }

    /// <summary>
    ///     Sets up animations for the grid.
    /// </summary>
    public void SetupAnimations()
    {
        _grid.GetCellImage += HandleGetCellImage;
    }

    /// <summary>
    ///     Handles the GetCellImage event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">The event arguments containing cell information and the image to be set.</param>
    private void HandleGetCellImage(object? sender, GetCellImageEventArgs args)
    {
        var image = new Image
        {
            Source = "unplayed.png",
            Aspect = Aspect.AspectFill,
            Opacity = 0
        };

        args.Image = image;

        // Schedule animation to run after layout
        Application.Current?.Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(50);

            // No special setup needed for SlideIn animation anymore
            // The new implementation handles the randomization internally

            // Calculate delay based on current pattern
            var delay = GridAnimationExtensions.CalculateAnimationDelay(
                args.Row,
                args.Column,
                _currentPattern,
                _grid.Rows,
                _grid.Columns);

            await Task.Delay(delay);
            await image.AnimateCellAsync(args.Row, args.Column, _currentAnimationType, _grid.Rows, _grid.Columns);
        });
    }

    /// <summary>
    ///     Cleans up resources used by the manager.
    /// </summary>
    public void Cleanup()
    {
        _grid.GetCellImage -= HandleGetCellImage;
    }
}
