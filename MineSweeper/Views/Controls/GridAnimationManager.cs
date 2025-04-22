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
    ///     Initializes a new instance of the <see cref="GridAnimationManager" /> class.
    /// </summary>
    /// <param name="grid">The grid to manage animations for.</param>
    public GridAnimationManager(IAnimatableGrid grid)
    {
        _grid = grid;
        SelectRandomAnimationStyle();
    }

    /// <summary>
    ///     Selects a random animation style and pattern.
    /// </summary>
    public void SelectRandomAnimationStyle()
    {
        // Choose random animation type for this game
        var animationTypes = Enum.GetValues<GridAnimationExtensions.AnimationType>();
        _currentAnimationType = animationTypes[_random.Next(animationTypes.Length)];

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