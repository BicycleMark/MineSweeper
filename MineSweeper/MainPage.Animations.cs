// MainPage.Animations.cs
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MineSweeper.Views.Controls;

namespace MineSweeper
{
    public partial class MainPage
    {
        // MainPage.Animations.cs - Add these properties
        private AnimationType _currentGameAnimationType;
        private AnimationPattern _currentGamePattern;
        
        // Animation type enum
        private enum AnimationType
        {
            FlipIn,
            FadeIn,
            ScaleIn,
            BounceIn,
            SpinIn,
            SlideIn,
            RandomPerCell,
            SwipeDiagonalTopLeftBottomRight,
            SwipeDiagonalTopRightBottomLeft,
            SwipeDiagonalBottomLeftTopRight,
            SwipeDiagonalBottomRightTopLeft,
            SwirlInnerToOuter,
            SwirlOuterToInner,
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop
        }
        
        // Animation pattern enum
        private enum AnimationPattern
        {
            Sequential,
            WaveFromTopLeft,
            WaveFromCenter,
            Spiral,
            Random,
            Checkerboard
        }
        
        // Animation dictionary to store our animation methods
        private readonly Dictionary<AnimationType, Func<Image, int, int, Task>> _animations = new();

        // Random for selecting animations
        private readonly Random _random = new();

        // Initialize animations in constructor
        partial void InitializeAnimations()
        {
            _animations.Clear();
            _animations.Add(AnimationType.FlipIn, FlipInAnimation);
            _animations.Add(AnimationType.FadeIn, FadeInAnimation);
            _animations.Add(AnimationType.ScaleIn, ScaleInAnimation);
            _animations.Add(AnimationType.BounceIn, BounceInAnimation);
            _animations.Add(AnimationType.SpinIn, SpinInAnimation);
            _animations.Add(AnimationType.SlideIn, SlideInAnimation);
            _animations.Add(AnimationType.RandomPerCell, RandomPerCellAnimation);
            _animations.Add(AnimationType.SwipeDiagonalTopLeftBottomRight, SwipeDiagonalTopLeftBottomRightAdapter);
            _animations.Add(AnimationType.SwipeDiagonalTopRightBottomLeft, SwipeDiagonalTopRightBottomLeftAdapter);
            _animations.Add(AnimationType.SwipeDiagonalBottomLeftTopRight, SwipeDiagonalBottomLeftTopRightAdapter);
            _animations.Add(AnimationType.SwipeDiagonalBottomRightTopLeft, SwipeDiagonalBottomRightTopLeftAdapter);
            _animations.Add(AnimationType.SwirlInnerToOuter, SwirlInnerToOuterAdapter);
            _animations.Add(AnimationType.SwirlOuterToInner, SwirlOuterToInnerAdapter);
            _animations.Add(AnimationType.LeftToRight, LeftToRightAdapter);
            _animations.Add(AnimationType.RightToLeft, RightToLeftAdapter);
            _animations.Add(AnimationType.TopToBottom, TopToBottomAdapter);
            _animations.Add(AnimationType.BottomToTop, BottomToTopAdapter);

        }
        // Gets a random animation for the cell
        private Func<Image, int, int, Task> GetRandomAnimation()
        {
            var animationTypes = Enum.GetValues<AnimationType>();
            var selectedType = animationTypes[_random.Next(animationTypes.Length)];
            return _animations[selectedType];
        }
        
        public void SelectRandomGameAnimationStyle()
        {
            // Choose random animation type for this game
            var animationTypes = Enum.GetValues<AnimationType>();
            _currentGameAnimationType = animationTypes[_random.Next(animationTypes.Length)];
    
            // Choose random pattern for this game
            var patterns = Enum.GetValues<AnimationPattern>();
            _currentGamePattern = patterns[_random.Next(patterns.Length)];
    
            System.Diagnostics.Debug.WriteLine($"Game animation style: {_currentGameAnimationType}, Pattern: {_currentGamePattern}");
        }

        // Handler for GetCellImage event
        private void HandleGetCellImage(object? sender, GetCellImageEventArgs getCellImageEventArgs)
        {
            var image = CreateCellImage();
            getCellImageEventArgs.Image = image;

            // Schedule animation to run after layout
            Dispatcher.Dispatch(async () => {
                await Task.Delay(50);
        
                // Get the animation function for the current game's style
                var animation = _animations[_currentGameAnimationType];
        
                // Calculate delay based on current pattern
                int delay = CalculateAnimationDelay(
                    getCellImageEventArgs.Row, 
                    getCellImageEventArgs.Column, 
                    _viewModel.Rows, 
                    _viewModel.Columns);
            
                await Task.Delay(delay);
                await animation(image, getCellImageEventArgs.Row, getCellImageEventArgs.Column);
            });
        }
        // Creates a basic cell image
        private Image CreateCellImage()
        {
            return new Image
            {
                Source = "unplayed.png",
                Aspect = Aspect.AspectFill,
                Opacity = 0,
            };
        }


        private async Task RandomPerCellAnimation(Image image, int row, int col)
        {
            // Select a random animation type for this specific cell
            // (excluding RandomPerCell itself to avoid recursion)
            var animationTypes = Enum.GetValues<AnimationType>()
                .Where(t => t != AnimationType.RandomPerCell)
                .ToArray();
        
            var randomCellAnimation = animationTypes[_random.Next(animationTypes.Length)];
    
            // Execute the randomly chosen animation
            await _animations[randomCellAnimation](image, row, col);
        }
        
        // Animation implementations
        private async Task FlipInAnimation(Image image, int row, int col)
        {
            image.Opacity = 0.3;
            image.RotationY = 90;
            image.AnchorX = 0.5;
            
            int delay = (row * 5) + (col * 5);
            await Task.Delay(delay);
            
            await Task.WhenAll(
                image.RotateYTo(0, 250, Easing.BounceOut),
                image.FadeTo(1, 200)
            );
        }
        
        private int CalculateAnimationDelay(int row, int col, int totalRows, int totalCols)
        {
            const int baseDelay = 10;
    
            switch (_currentGamePattern)
            {
                case AnimationPattern.WaveFromTopLeft:
                    return (row + col) * baseDelay;
            
                case AnimationPattern.WaveFromCenter:
                    int centerRow = totalRows / 2;
                    int centerCol = totalCols / 2;
                    int distanceFromCenter = Math.Abs(row - centerRow) + Math.Abs(col - centerCol);
                    return distanceFromCenter * baseDelay;
            
                case AnimationPattern.Spiral:
                    // Approximation of spiral ordering
                    int maxDist = Math.Max(Math.Max(row, totalRows - 1 - row), 
                        Math.Max(col, totalCols - 1 - col));
                    return maxDist * baseDelay * 2;
            
                case AnimationPattern.Random:
                    return _random.Next(0, baseDelay * 10);
            
                case AnimationPattern.Checkerboard:
                    return ((row + col) % 2 == 0) ? 0 : baseDelay * 5;
            
                case AnimationPattern.Sequential:
                default:
                    return (row * totalCols + col) * baseDelay / 2;
            }
        }
        
        private async Task FadeInAnimation(Image image, int row, int col)
        {
            image.Opacity = 0;
            
            int delay = (row * 8) + (col * 8);
            await Task.Delay(delay);
            
            await image.FadeTo(1, 300, Easing.CubicOut);
        }
        
        private async Task ScaleInAnimation(Image image, int row, int col)
        {
            image.Opacity = 0;
            image.Scale = 0.5;
            
            int delay = (row * 7) + (col * 7);
            await Task.Delay(delay);
            
            await Task.WhenAll(
                image.ScaleTo(1.0, 300, Easing.SpringOut),
                image.FadeTo(1, 200)
            );
        }
        
        private async Task BounceInAnimation(Image image, int row, int col)
        {
            image.Opacity = 0.5;
            image.Scale = 1.5;
            
            int delay = (row * 6) + (col * 6);
            await Task.Delay(delay);
            
            await Task.WhenAll(
                image.ScaleTo(1.0, 400, Easing.BounceOut),
                image.FadeTo(1, 250)
            );
        }
        
        private async Task SpinInAnimation(Image image, int row, int col)
        {
            image.Opacity = 0;
            image.Rotation = 180;
            
            int delay = (row + col) * 4;
            await Task.Delay(delay);
            
            await Task.WhenAll(
                image.RotateTo(0, 350, Easing.CubicOut),
                image.FadeTo(1, 250)
            );
        }
        
        private async Task SlideInAnimation(Image image, int row, int col)
        {
            image.Opacity = 0.3;
            image.TranslationX = (col % 2 == 0) ? -40 : 40;
            
            int delay = row * 10;
            await Task.Delay(delay);
            
            await Task.WhenAll(
                image.TranslateTo(0, 0, 300, Easing.CubicOut),
                image.FadeTo(1, 250)
            );
        }
        
        private async Task SwipeDiagonalTopLeftBottomRightAnimation(Image image, int row, int col)
        {
            image.Opacity = 0;
            image.TranslationX = -30;
            image.TranslationY = -30;

            // Delay based on diagonal distance from top-left
            int delay = (row + col) * 15;
            await Task.Delay(delay);

            await Task.WhenAll(
                image.TranslateTo(0, 0, 300, Easing.CubicOut),
                image.FadeTo(1, 250)
            );
        }

        private async Task SwipeDiagonalTopRightBottomLeftAnimation(Image image, int row, int col, int totalCols)
        {
            image.Opacity = 0;
            image.TranslationX = 30;
            image.TranslationY = -30;

            // Delay based on diagonal distance from top-right
            int delay = (row + (totalCols - 1 - col)) * 15;
            await Task.Delay(delay);

            await Task.WhenAll(
                image.TranslateTo(0, 0, 300, Easing.CubicOut),
                image.FadeTo(1, 250)
            );
        }

        private async Task SwipeDiagonalBottomLeftTopRightAnimation(Image image, int row, int col, int totalRows)
        {
            image.Opacity = 0;
            image.TranslationX = -30;
            image.TranslationY = 30;

            // Delay based on diagonal distance from bottom-left
            int delay = ((totalRows - 1 - row) + col) * 15;
            await Task.Delay(delay);

            await Task.WhenAll(
                image.TranslateTo(0, 0, 300, Easing.CubicOut),
                image.FadeTo(1, 250)
            );
        }

        private async Task SwipeDiagonalBottomRightTopLeftAnimation(Image image, int row, int col, int totalRows, int totalCols)
        {
            image.Opacity = 0;
            image.TranslationX = 30;
            image.TranslationY = 30;

            // Delay based on diagonal distance from bottom-right
            int delay = ((totalRows - 1 - row) + (totalCols - 1 - col)) * 15;
            await Task.Delay(delay);

            await Task.WhenAll(
                image.TranslateTo(0, 0, 300, Easing.CubicOut),
                image.FadeTo(1, 250)
            );
        }
        
        private async Task SwirlInnerToOuterAnimation(Image image, int row, int col, int totalRows, int totalCols)
{
    image.Opacity = 0;
    image.Scale = 0.7;
    
    // Calculate center coordinates
    int centerRow = totalRows / 2;
    int centerCol = totalCols / 2;
    
    // Calculate layer (distance from center in a spiral pattern)
    int rowDistance = Math.Abs(row - centerRow);
    int colDistance = Math.Abs(col - centerCol);
    int layer = Math.Max(rowDistance, colDistance);
    
    // Calculate position within the layer (for proper sequencing)
    int position = 0;
    
    if (row == centerRow - layer) // Top edge of the layer
        position = col - (centerCol - layer);
    else if (col == centerCol + layer) // Right edge
        position = 2 * layer + (row - (centerRow - layer));
    else if (row == centerRow + layer) // Bottom edge
        position = 4 * layer - (col - (centerCol - layer));
    else // Left edge
        position = 6 * layer - (row - (centerRow - layer));
    
    // Base delay per layer plus position in layer
    int delay = layer * 80 + position * 20;
    await Task.Delay(delay);
    
    await Task.WhenAll(
        image.ScaleTo(1.0, 350, Easing.CubicOut),
        image.FadeTo(1, 300)
    );
}

private async Task SwirlOuterToInnerAnimation(Image image, int row, int col, int totalRows, int totalCols)
{
    image.Opacity = 0;
    image.Scale = 0.7;
    
    // Calculate center coordinates
    int centerRow = totalRows / 2;
    int centerCol = totalCols / 2;
    
    // Calculate max possible layer
    int maxLayer = Math.Max(Math.Max(centerRow, totalRows - centerRow - 1),
                           Math.Max(centerCol, totalCols - centerCol - 1));
    
    // Calculate current layer (distance from center in a spiral pattern)
    int rowDistance = Math.Abs(row - centerRow);
    int colDistance = Math.Abs(col - centerCol);
    int layer = Math.Max(rowDistance, colDistance);
    
    // Calculate position within the layer (for proper sequencing)
    int position = 0;
    
    if (row == centerRow - layer) // Top edge of the layer
        position = col - (centerCol - layer);
    else if (col == centerCol + layer) // Right edge
        position = 2 * layer + (row - (centerRow - layer));
    else if (row == centerRow + layer) // Bottom edge
        position = 4 * layer - (col - (centerCol - layer));
    else // Left edge
        position = 6 * layer - (row - (centerRow - layer));
    
    // Reverse the delay so outer layers animate first
    int delay = (maxLayer - layer) * 80 + position * 20;
    await Task.Delay(delay);
    
    await Task.WhenAll(
        image.ScaleTo(1.0, 350, Easing.CubicOut),
        image.FadeTo(1, 300)
    );
}
private async Task LeftToRightAnimation(Image image, int row, int col, int totalCols)
{
    image.Opacity = 0;
    image.TranslationX = -40;

    // Calculate delay based on column position
    int delay = col * 20;
    await Task.Delay(delay);

    await Task.WhenAll(
        image.TranslateTo(0, 0, 300, Easing.CubicOut),
        image.FadeTo(1, 250)
    );
}

private async Task RightToLeftAnimation(Image image, int row, int col, int totalCols)
{
    image.Opacity = 0;
    image.TranslationX = 40;

    // Calculate delay based on reverse column position
    int delay = (totalCols - 1 - col) * 20;
    await Task.Delay(delay);

    await Task.WhenAll(
        image.TranslateTo(0, 0, 300, Easing.CubicOut),
        image.FadeTo(1, 250)
    );
}

private async Task TopToBottomAnimation(Image image, int row, int col, int totalRows)
{
    image.Opacity = 0;
    image.TranslationY = -40;

    // Calculate delay based on row position
    int delay = row * 20;
    await Task.Delay(delay);

    await Task.WhenAll(
        image.TranslateTo(0, 0, 300, Easing.CubicOut),
        image.FadeTo(1, 250)
    );
}

private async Task BottomToTopAnimation(Image image, int row, int col, int totalRows)
{
    image.Opacity = 0;
    image.TranslationY = 40;

    // Calculate delay based on reverse row position
    int delay = (totalRows - 1 - row) * 20;
    await Task.Delay(delay);

    await Task.WhenAll(
        image.TranslateTo(0, 0, 300, Easing.CubicOut),
        image.FadeTo(1, 250)
    );
}
        
        // Adapter methods to match the required signature
        
        private Task LeftToRightAdapter(Image image, int row, int col)
        {
            return LeftToRightAnimation(image, row, col, _viewModel.Columns);
        }

        private Task RightToLeftAdapter(Image image, int row, int col)
        {
            return RightToLeftAnimation(image, row, col, _viewModel.Columns);
        }

        private Task TopToBottomAdapter(Image image, int row, int col)
        {
            return TopToBottomAnimation(image, row, col, _viewModel.Rows);
        }

        private Task BottomToTopAdapter(Image image, int row, int col)
        {
            return BottomToTopAnimation(image, row, col, _viewModel.Rows);
        }
        private Task SwipeDiagonalTopLeftBottomRightAdapter(Image image, int row, int col)
        {
            return SwipeDiagonalTopLeftBottomRightAnimation(image, row, col);
        }

        private Task SwipeDiagonalTopRightBottomLeftAdapter(Image image, int row, int col)
        {
            return SwipeDiagonalTopRightBottomLeftAnimation(image, row, col, _viewModel.Columns);
        }

        private Task SwipeDiagonalBottomLeftTopRightAdapter(Image image, int row, int col)
        {
            return SwipeDiagonalBottomLeftTopRightAnimation(image, row, col, _viewModel.Rows);
        }

        private Task SwipeDiagonalBottomRightTopLeftAdapter(Image image, int row, int col)
        {
            return SwipeDiagonalBottomRightTopLeftAnimation(image, row, col, _viewModel.Rows, _viewModel.Columns);
        }
        
        private Task SwirlInnerToOuterAdapter(Image image, int row, int col)
        {
            return SwirlInnerToOuterAnimation(image, row, col, _viewModel.Rows, _viewModel.Columns);
        }

        private Task SwirlOuterToInnerAdapter(Image image, int row, int col)
        {
            return SwirlOuterToInnerAnimation(image, row, col, _viewModel.Rows, _viewModel.Columns);
        }
    }
    
}