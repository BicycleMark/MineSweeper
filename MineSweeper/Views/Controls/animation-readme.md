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
