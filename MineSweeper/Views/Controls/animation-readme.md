# MineSweeper Animation System

## Overview
The MineSweeper application includes a flexible animation system for grid cells. This document explains how the animation system works and how to customize it.

## Animation Types
The following animation types are available:

- **FlipIn**: Cells flip into view along the Y-axis
- **FadeIn**: Cells fade into view
- **ScaleIn**: Cells scale from small to full size
- **BounceIn**: Cells bounce into view with an elastic effect
- **SpinIn**: Cells spin into view
- **SlideIn**: Cells slide into view
- **RandomPerCell**: Each cell uses a randomly selected animation
- **SwipeDiagonal...**: Various diagonal swipe animations
- **SwirlInnerToOuter**: Animation spirals from the center outward
- **SwirlOuterToInner**: Animation spirals from outside toward the center
- **LeftToRight**: Animation sweeps from left to right
- **RightToLeft**: Animation sweeps from right to left
- **TopToBottom**: Animation sweeps from top to bottom
- **BottomToTop**: Animation sweeps from bottom to top
- **Pixelated**: Cells appear pixelated and gradually sharpen into view
- **BreathInBreathOut**: Cells scale in and out like breathing
- **AttenuatedVibration**: Cells vibrate with decreasing intensity
- **SwirlLikeADrainIntoPlace**: Cells swirl into place like water going down a drain
- **RadarPaternWitRadarLikeOpacity**: Cells appear with a radar-like sweeping pattern with varying opacity
- **ConstructFromPile**: Cells are pulled one by one from a pile and placed into position
- **LikeAShadePulled**: Cells fall into place from above like a window shade being pulled down
- **ABrickLayerfromAbove**: Cells fall from the absolute top, building from the bottom up with alternating left-to-right and right-to-left patterns
- **SineWaveBuilder**: Cells move in a sine wave pattern, building from the bottom up, with rows alternating between left-to-right and right-to-left

## Animation Patterns
Animation patterns control the sequence in which cells are animated:

- **Sequential**: Cell animations follow a sequential order based on row and column indices
- **WaveFromTopLeft**: Animation starts from top-left corner and spreads like a wave
- **WaveFromCenter**: Animation spreads outward from the center
- **Spiral**: Animation follows a spiral pattern
- **Random**: Cells animate in random order
- **Checkerboard**: Cells animate in a checkerboard pattern

## Forcing a Specific Animation Type
By default, the animation system randomly selects an animation type for each game. However, you can force a specific animation type using the `ForcedAnimationType` property of the `GridAnimationManager` class:

```csharp
// Get a reference to the GridAnimationManager
var animationManager = /* get your animation manager instance */;

// Force the Pixelated animation type
animationManager.ForcedAnimationType = GridAnimationExtensions.AnimationType.Pixelated;

// Revert to random selection
animationManager.ForcedAnimationType = null;
```

### Example: Using Only the Pixelated Animation
To use only the Pixelated animation:

```csharp
// In your game initialization code
var grid = new SquareImageGrid();
var animationManager = new GridAnimationManager(grid);
animationManager.ForcedAnimationType = GridAnimationExtensions.AnimationType.Pixelated;
animationManager.SetupAnimations();
```

### Example: Restoring Random Animation Selection
To revert back to using random animations:

```csharp
// When you want to restore random selection
animationManager.ForcedAnimationType = null;
animationManager.SelectRandomAnimationStyle(); // Re-select a random style
```

## Creating Custom Animations
To add a new animation type:

1. Add a new value to the `AnimationType` enum in `GridAnimationExtensions.cs`
2. Implement a new animation method in the same file
3. Add a case for the new animation type in the `AnimateCellAsync` switch statement

### ConstructFromPile Animation

The `ConstructFromPile` animation is a new animation that pulls one tile at a time from a pile and places each tile into a random location until the board is constructed. This creates a visual effect of the game board being built piece by piece.

```csharp
/// <summary>
/// Performs an animation where cells are pulled one by one from a pile and placed into position.
/// </summary>
/// <param name="image">The image to animate.</param>
/// <param name="row">The row index of the cell.</param>
/// <param name="col">The column index of the cell.</param>
/// <param name="totalRows">The total number of rows in the grid.</param>
/// <param name="totalColumns">The total number of columns in the grid.</param>
/// <returns>A task representing the animation operation.</returns>
public static async Task ConstructFromPileAnimation(Image image, int row, int col, int totalRows, int totalColumns)
{
    // Initial state: invisible
    image.Opacity = 0;
    
    // Define the pile location (bottom of the screen, centered horizontally)
    var pileX = totalColumns * 20; // Center of the grid horizontally
    var pileY = totalRows * 40;    // Below the grid
    
    // Set initial position at the pile
    image.TranslationX = pileX;
    image.TranslationY = pileY;
    image.Scale = 0.8;
    image.Rotation = _random.Next(-30, 31); // Random initial rotation
    
    // Calculate a random order for placing tiles
    var cellIndex = row * totalColumns + col;
    var totalCells = totalRows * totalColumns;
    
    // Create a random placement order (0 to totalCells-1)
    var placementOrder = new List<int>();
    for (int i = 0; i < totalCells; i++)
        placementOrder.Add(i);
    
    // Shuffle the placement order
    for (int i = 0; i < placementOrder.Count; i++)
    {
        int j = _random.Next(i, placementOrder.Count);
        int temp = placementOrder[i];
        placementOrder[i] = placementOrder[j];
        placementOrder[j] = temp;
    }
    
    // Find the position of this cell in the random order
    var placementIndex = placementOrder.IndexOf(cellIndex);
    
    // Calculate delay based on placement order
    var delay = placementIndex * 8; // 8ms between each tile placement
    await Task.Delay(delay);
    
    // Make the image visible
    image.Opacity = 1;
    
    // Animate the tile from the pile to its final position
    // Use a cubic easing for faster animation with shorter duration
    await Task.WhenAll(
        image.TranslateTo(0, 0, 80, Easing.CubicOut),
        image.RotateTo(0, 60, Easing.CubicOut),
        image.ScaleTo(1.0, 60, Easing.CubicOut)
    );
}
```

### Example: Using the ConstructFromPile Animation
To use the ConstructFromPile animation:

```csharp
// In your game initialization code
var grid = new SquareImageGrid();
var animationManager = new GridAnimationManager(grid);
animationManager.ForcedAnimationType = GridAnimationExtensions.AnimationType.ConstructFromPile;
animationManager.SetupAnimations();
```

Here are examples of animation implementations:

### Pixelated Animation

```csharp
/// <summary>
///     Performs a pixelation animation where the cell starts blurry and gradually sharpens.
/// </summary>
private static async Task PixelatedAnimation(Image image, int row, int col)
{
    // Initial state: visible but blurry/pixelated
    image.Opacity = 1;
    image.Scale = 1.0;
    
    // Apply a blur effect or pixelation effect
    // We can simulate this with a combination of scaling and opacity
    image.ScaleX = 0.5;
    image.ScaleY = 0.5;
    image.Opacity = 0.7;
    
    // Calculate delay based on position
    var delay = row * 6 + col * 6;
    await Task.Delay(delay);
    
    // Animate from pixelated to clear in steps
    // First step - increase scale slightly and opacity
    await Task.WhenAll(
        image.ScaleXTo(0.7, 100),
        image.ScaleYTo(0.7, 100),
        image.FadeTo(0.8, 100)
    );
    
    // Second step - increase scale more and opacity
    await Task.WhenAll(
        image.ScaleXTo(0.9, 100),
        image.ScaleYTo(0.9, 100),
        image.FadeTo(0.9, 100)
    );
    
    // Final step - full scale and opacity
    await Task.WhenAll(
        image.ScaleXTo(1.0, 150, Easing.CubicOut),
        image.ScaleYTo(1.0, 150, Easing.CubicOut),
        image.FadeTo(1.0, 150)
    );
}
```

### BreathInBreathOut Animation

```csharp
/// <summary>
///     Performs a breathing animation where the cell scales in and out like breathing.
/// </summary>
private static async Task BreathInBreathOutAnimation(Image image, int row, int col)
{
    // Initial state: fully visible but slightly smaller
    image.Opacity = 1;
    image.Scale = 0.9;
    
    // Calculate delay based on position
    var delay = row * 5 + col * 5;
    await Task.Delay(delay);
    
    // First breath in - scale up
    await image.ScaleTo(1.05, 800, Easing.SinInOut);
    
    // First breath out - scale down
    await image.ScaleTo(0.95, 800, Easing.SinInOut);
    
    // Second breath in - scale up
    await image.ScaleTo(1.03, 700, Easing.SinInOut);
    
    // Second breath out - scale down
    await image.ScaleTo(0.97, 700, Easing.SinInOut);
    
    // Final breath in - settle to normal size
    await image.ScaleTo(1.0, 500, Easing.SinOut);
}
```

### AttenuatedVibration Animation

```csharp
/// <summary>
///     Performs a vibration animation with decreasing intensity.
/// </summary>
private static async Task AttenuatedVibrationAnimation(Image image, int row, int col)
{
    // Initial state: fully visible
    image.Opacity = 1;
    image.Scale = 1.0;
    
    // Calculate delay based on position
    var delay = row * 4 + col * 4;
    await Task.Delay(delay);
    
    // First vibration - high intensity
    await image.TranslateTo(-5, 0, 50, Easing.CubicOut);
    await image.TranslateTo(5, 0, 100, Easing.CubicInOut);
    await image.TranslateTo(-5, 0, 100, Easing.CubicInOut);
    await image.TranslateTo(5, 0, 100, Easing.CubicInOut);
    
    // Second vibration - medium intensity
    await image.TranslateTo(-3, 0, 80, Easing.CubicInOut);
    await image.TranslateTo(3, 0, 80, Easing.CubicInOut);
    await image.TranslateTo(-3, 0, 80, Easing.CubicInOut);
    
    // Third vibration - low intensity
    await image.TranslateTo(2, 0, 60, Easing.CubicInOut);
    await image.TranslateTo(-2, 0, 60, Easing.CubicInOut);
    
    // Return to original position
    await image.TranslateTo(0, 0, 50, Easing.CubicOut);
}
```

### SwirlLikeADrainIntoPlace Animation

```csharp
/// <summary>
///     Performs a swirling animation like water going down a drain.
/// </summary>
private static async Task SwirlLikeADrainIntoPlaceAnimation(Image image, int row, int col, int totalRows, int totalColumns)
{
    // Initial state: invisible and positioned off-center
    image.Opacity = 0;
    image.Scale = 0.5;
    
    // Calculate center coordinates
    var centerRow = totalRows / 2.0;
    var centerCol = totalColumns / 2.0;
    
    // Calculate distance from center
    var rowDistance = row - centerRow;
    var colDistance = col - centerCol;
    
    // Calculate angle from center (in radians)
    var angle = Math.Atan2(rowDistance, colDistance);
    
    // Convert angle to degrees for rotation
    var angleDegrees = angle * (180 / Math.PI);
    
    // Calculate distance from center
    var distance = Math.Sqrt(rowDistance * rowDistance + colDistance * colDistance);
    
    // Calculate initial position (further out from final position)
    var initialDistance = distance * 2.5;
    var initialX = Math.Cos(angle) * initialDistance * 20; // Scale for visual effect
    var initialY = Math.Sin(angle) * initialDistance * 20;
    
    // Set initial position and rotation
    image.TranslationX = initialX;
    image.TranslationY = initialY;
    image.Rotation = angleDegrees;
    
    // Calculate delay based on distance from center
    var delay = (int)(distance * 15);
    await Task.Delay(delay);
    
    // Make visible
    await image.FadeTo(1, 50);
    
    // Number of rotations based on distance from center
    var rotations = 1 + distance;
    
    // Duration based on distance from center
    var duration = 700 + (int)(distance * 100);
    
    // First animate with rotation and translation
    await Task.WhenAll(
        image.RotateTo(360 * rotations, (uint)(duration * 0.8), Easing.CubicOut),
        image.TranslateTo(0, 0, (uint)duration, Easing.CubicOut),
        image.ScaleTo(1.0, (uint)duration, Easing.CubicOut)
    );
    
    // Then ensure final rotation is 0 (square orientation)
    await image.RotateTo(0, 200, Easing.CubicOut);
}
```

### RadarPaternWitRadarLikeOpacity Animation

```csharp
/// <summary>
///     Performs a radar-like sweeping animation with varying opacity.
///     Tiles become visible briefly during first two sweeps, then stay visible after third sweep.
/// </summary>
private static async Task RadarPaternWitRadarLikeOpacityAnimation(Image image, int row, int col, int totalRows, int totalColumns)
{
    // Initial state: invisible
    image.Opacity = 0;
    image.Scale = 1.0;
    
    // Calculate center coordinates
    var centerRow = totalRows / 2.0;
    var centerCol = totalColumns / 2.0;
    
    // Calculate distance and angle from center
    var rowDistance = row - centerRow;
    var colDistance = col - centerCol;
    var angle = Math.Atan2(rowDistance, colDistance);
    var angleDegrees = angle * (180 / Math.PI);
    var distance = Math.Sqrt(rowDistance * rowDistance + colDistance * colDistance);
    
    // Normalize angle to 0-360 degrees
    var normalizedAngle = (angleDegrees + 360) % 360;
    
    // Define animation parameters
    const int sweeps = 3; // Exactly three radar sweeps
    const int totalDuration = 4500; // Total duration for all sweeps
    const int singleSweepDuration = totalDuration / sweeps;
    
    // Calculate the time at which this cell should be "hit" by the radar in each sweep
    // based on its angle from the center
    var sweepHitTime = normalizedAngle / 360.0;
    
    // For each of the three sweeps
    for (int sweep = 0; sweep < sweeps; sweep++)
    {
        // Calculate when this cell should be hit in this sweep
        var sweepStartTime = sweep * singleSweepDuration;
        var cellHitTime = sweepStartTime + (int)(sweepHitTime * singleSweepDuration);
        
        // Wait until it's time for this cell to be hit by the radar
        var waitTime = cellHitTime - (sweep > 0 ? 0 : 0); // No initial delay for subsequent sweeps
        if (waitTime > 0)
        {
            await Task.Delay(waitTime);
        }
        
        if (sweep < 2)
        {
            // For first two sweeps, briefly show the cell then hide it
            await image.FadeTo(0.8, 150, Easing.CubicOut);
            await image.FadeTo(0, 350, Easing.CubicIn);
        }
        else
        {
            // For the third sweep, make the cell fully visible and keep it that way
            await image.FadeTo(1.0, 200, Easing.CubicOut);
        }
    }
}
```

### LikeAShadePulled Animation

```csharp
/// <summary>
/// Performs an animation where cells fall into place from above like a window shade being pulled down.
/// </summary>
/// <param name="image">The image to animate.</param>
/// <param name="row">The row index of the cell.</param>
/// <param name="col">The column index of the cell.</param>
/// <param name="totalRows">The total number of rows in the grid.</param>
/// <param name="totalColumns">The total number of columns in the grid.</param>
/// <returns>A task representing the animation operation.</returns>
public static async Task LikeAShadePulledAnimation(Image image, int row, int col, int totalRows, int totalColumns)
{
    // Initial state: invisible and positioned above its final position
    image.Opacity = 0;
    
    // Set initial position above the grid
    var initialY = -100 - (row * 20); // Higher rows start higher up
    image.TranslationY = initialY;
    
    // Add a slight rotation for a more dynamic effect
    image.Rotation = _random.Next(-10, 11);
    
    // Make the image visible
    image.Opacity = 1;
    
    // Calculate delay based on row (top rows fall first)
    var delay = row * 50; // 50ms between each row
    await Task.Delay(delay);
    
    // Animate the tile falling into place
    // Use a bounce effect to simulate hitting the ground
    await Task.WhenAll(
        image.TranslateTo(0, 0, 300, Easing.BounceOut),
        image.RotateTo(0, 250, Easing.CubicOut)
    );
}
```

### Example: Using the LikeAShadePulled Animation
To use the LikeAShadePulled animation:

```csharp
// In your game initialization code
var grid = new SquareImageGrid();
var animationManager = new GridAnimationManager(grid);
animationManager.ForcedAnimationType = GridAnimationExtensions.AnimationType.LikeAShadePulled;
animationManager.SetupAnimations();
```

### ABrickLayerfromAbove Animation

```csharp
/// <summary>
/// Performs an animation where cells fall from the absolute top, building from the bottom up.
/// </summary>
/// <param name="image">The image to animate.</param>
/// <param name="row">The row index of the cell.</param>
/// <param name="col">The column index of the cell.</param>
/// <param name="totalRows">The total number of rows in the grid.</param>
/// <param name="totalColumns">The total number of columns in the grid.</param>
/// <returns>A task representing the animation operation.</returns>
public static async Task ABrickLayerfromAboveAnimation(Image image, int row, int col, int totalRows, int totalColumns)
{
    try
    {
        // Initial state: invisible and positioned at the absolute top
        image.Opacity = 0;
        
        // Set initial position at the very top of the grid based on grid size
        var initialY = -(totalRows * 40); // Calculate based on grid size
        image.TranslationY = initialY;
        
        // Add a slight rotation for a more dynamic effect
        image.Rotation = _random.Next(-5, 6);
        
        // Calculate a unique index for each cell
        // We want to build from the bottom up, so reverse the row index
        int reversedRow = totalRows - 1 - row;
        
        // Determine if this row goes left-to-right or right-to-left
        bool leftToRight = (reversedRow % 2 == 0);
        
        // Calculate cell index (bottom rows first, then moving up)
        int cellIndex;
        if (leftToRight)
        {
            // Left to right rows
            cellIndex = (reversedRow * totalColumns) + col;
        }
        else
        {
            // Right to left rows
            cellIndex = (reversedRow * totalColumns) + (totalColumns - 1 - col);
        }
        
        // Calculate delay based on the cell index
        // Each cell waits for its turn, but with a much shorter delay
        var delay = cellIndex * 30; // 30ms between each cell (was 80ms)
        await Task.Delay(delay);
        
        // Make the image visible immediately before animation
        image.Opacity = 1;
        
        // Animate the brick falling into place
        // Fast drop from the top
        await image.TranslateTo(0, 5, 100, Easing.CubicIn);
        
        // Then settle into final position with a small bounce
        await Task.WhenAll(
            image.TranslateTo(0, 0, 80, Easing.BounceOut),
            image.RotateTo(0, 80, Easing.CubicOut)
        );
        
        // Add a small "settling" effect (faster)
        await image.ScaleTo(1.05, 30, Easing.CubicOut);
        await image.ScaleTo(1.0, 30, Easing.CubicIn);
    }
    catch (Exception ex)
    {
        // Fallback in case of any errors - just make the cell visible
        image.Opacity = 1;
        image.TranslationY = 0;
        image.Rotation = 0;
        image.Scale = 1.0;
        
        // Log the error (if logging is available)
        System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
    }
}
```

### Example: Using the ABrickLayerfromAbove Animation
To use the ABrickLayerfromAbove animation:

```csharp
// In your game initialization code
var grid = new SquareImageGrid();
var animationManager = new GridAnimationManager(grid);
animationManager.ForcedAnimationType = GridAnimationExtensions.AnimationType.ABrickLayerfromAbove;
animationManager.SetupAnimations();
```

### SineWaveBuilder Animation

```csharp
/// <summary>
/// Performs an animation where cells move in a sine wave pattern, building from the bottom up,
/// with rows alternating between left-to-right and right-to-left.
/// </summary>
/// <param name="image">The image to animate.</param>
/// <param name="row">The row index of the cell.</param>
/// <param name="col">The column index of the cell.</param>
/// <param name="totalRows">The total number of rows in the grid.</param>
/// <param name="totalColumns">The total number of columns in the grid.</param>
/// <returns>A task representing the animation operation.</returns>
public static async Task SineWaveBuilderAnimation(Image image, int row, int col, int totalRows, int totalColumns)
{
    try
    {
        // Initial state: invisible
        image.Opacity = 0;
        
        // Determine if this row goes left-to-right or right-to-left
        bool leftToRight = (row % 2 == 0);
        
        // Calculate the actual column position based on direction
        int actualCol = leftToRight ? col : (totalColumns - 1 - col);
        
        // Calculate a unique index for each cell
        // We want to build from the bottom up, so reverse the row index
        int reversedRow = totalRows - 1 - row;
        
        // Determine if this row goes left-to-right or right-to-left
        bool reversedLeftToRight = (reversedRow % 2 == 0);
        
        // Calculate cell index (bottom rows first, then moving up)
        int cellIndex;
        if (reversedLeftToRight)
        {
            // Left to right rows
            cellIndex = (reversedRow * totalColumns) + col;
        }
        else
        {
            // Right to left rows
            cellIndex = (reversedRow * totalColumns) + (totalColumns - 1 - col);
        }
        
        // Calculate delay based on the cell index
        var delay = cellIndex * 50; // 50ms between each cell
        
        // Calculate sine wave parameters
        double amplitude = 50; // Height of the sine wave
        double frequency = 0.3; // Frequency of the sine wave
        
        // Set initial position based on sine wave
        // The sine wave is applied horizontally (X-axis)
        double initialX = amplitude * Math.Sin(frequency * actualCol);
        double initialY = -100; // Start above the grid
        
        image.TranslationX = initialX;
        image.TranslationY = initialY;
        
        // Wait for the calculated delay
        await Task.Delay(delay);
        
        // Make the image visible
        image.Opacity = 1;
        
        // Animate the cell moving down in a sine wave pattern
        uint duration = 500; // Animation duration in milliseconds
        uint steps = 20; // Number of animation steps
        uint stepDuration = duration / steps;
        
        for (int i = 0; i < steps; i++)
        {
            // Calculate progress (0 to 1)
            double progress = (double)i / steps;
            
            // Calculate current Y position (linear from top to bottom)
            double currentY = initialY + (progress * (Math.Abs(initialY)));
            
            // Calculate current X position (sine wave that diminishes as it approaches the bottom)
            double waveAmplitude = amplitude * (1 - progress); // Diminishing amplitude
            double currentX = waveAmplitude * Math.Sin(frequency * actualCol + (progress * Math.PI * 2));
            
            // Update position
            await image.TranslateTo(currentX, currentY, stepDuration);
        }
        
        // Final animation to settle into place
        await Task.WhenAll(
            image.TranslateTo(0, 0, 100, Easing.BounceOut),
            image.RotateTo(0, 100)
        );
    }
    catch (Exception ex)
    {
        // Fallback in case of any errors - just make the cell visible
        image.Opacity = 1;
        image.TranslationX = 0;
        image.TranslationY = 0;
        
        // Log the error (if logging is available)
        System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
    }
}
```

### Example: Using the SineWaveBuilder Animation
To use the SineWaveBuilder animation:

```csharp
// In your game initialization code
var grid = new SquareImageGrid();
var animationManager = new GridAnimationManager(grid);
animationManager.ForcedAnimationType = GridAnimationExtensions.AnimationType.SineWaveBuilder;
animationManager.SetupAnimations();
```
