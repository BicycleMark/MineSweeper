// SquareImageGridAnimationExtensions.cs
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MineSweeper.Views.Controls;

namespace MineSweeper.Extensions
{
    public static class SquareImageGridAnimationExtensions
    {
        // Animation type enum
        public enum AnimationType
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
        public enum AnimationPattern
        {
            Sequential,
            WaveFromTopLeft,
            WaveFromCenter,
            Spiral,
            Random,
            Checkerboard
        }

        private static readonly Random _random = new();

        // Extension method to animate a cell
        public static Task AnimateCellAsync(this Image image, int row, int col, 
            AnimationType animationType, int totalRows, int totalColumns)
        {
            return animationType switch
            {
                AnimationType.FlipIn => FlipInAnimation(image, row, col),
                AnimationType.FadeIn => FadeInAnimation(image, row, col),
                AnimationType.ScaleIn => ScaleInAnimation(image, row, col),
                AnimationType.BounceIn => BounceInAnimation(image, row, col),
                AnimationType.SpinIn => SpinInAnimation(image, row, col),
                AnimationType.SlideIn => SlideInAnimation(image, row, col),
                AnimationType.RandomPerCell => RandomPerCellAnimation(image, row, col, totalRows, totalColumns),
                AnimationType.SwipeDiagonalTopLeftBottomRight => SwipeDiagonalTopLeftBottomRightAnimation(image, row, col),
                AnimationType.SwipeDiagonalTopRightBottomLeft => SwipeDiagonalTopRightBottomLeftAnimation(image, row, col, totalColumns),
                AnimationType.SwipeDiagonalBottomLeftTopRight => SwipeDiagonalBottomLeftTopRightAnimation(image, row, col, totalRows),
                AnimationType.SwipeDiagonalBottomRightTopLeft => SwipeDiagonalBottomRightTopLeftAnimation(image, row, col, totalRows, totalColumns),
                AnimationType.SwirlInnerToOuter => SwirlInnerToOuterAnimation(image, row, col, totalRows, totalColumns),
                AnimationType.SwirlOuterToInner => SwirlOuterToInnerAnimation(image, row, col, totalRows, totalColumns),
                AnimationType.LeftToRight => LeftToRightAnimation(image, row, col, totalColumns),
                AnimationType.RightToLeft => RightToLeftAnimation(image, row, col, totalColumns),
                AnimationType.TopToBottom => TopToBottomAnimation(image, row, col, totalRows),
                AnimationType.BottomToTop => BottomToTopAnimation(image, row, col, totalRows),
                _ => FadeInAnimation(image, row, col)
            };
        }

        public static int CalculateAnimationDelay(int row, int col, 
            AnimationPattern pattern, int totalRows, int totalCols)
        {
            const int baseDelay = 10;

            return pattern switch
            {
                AnimationPattern.WaveFromTopLeft => (row + col) * baseDelay,
                AnimationPattern.WaveFromCenter => {
                    int centerRow = totalRows / 2;
                    int centerCol = totalCols / 2;
                    int distanceFromCenter = Math.Abs(row - centerRow) + Math.Abs(col - centerCol);
                    return distanceFromCenter * baseDelay;
                },
                AnimationPattern.Spiral => {
                    int maxDist = Math.Max(Math.Max(row, totalRows - 1 - row),
                        Math.Max(col, totalCols - 1 - col));
                    return maxDist * baseDelay * 2;
                },
                AnimationPattern.Random => _random.Next(0, baseDelay * 10),
                AnimationPattern.Checkerboard => ((row + col) % 2 == 0) ? 0 : baseDelay * 5,
                AnimationPattern.Sequential => (row * totalCols + col) * baseDelay / 2,
                _ => (row * totalCols + col) * baseDelay / 2
            };
        }

        // Animation implementations
        private static async Task FlipInAnimation(Image image, int row, int col)
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

        private static async Task FadeInAnimation(Image image, int row, int col)
        {
            image.Opacity = 0;

            int delay = (row * 8) + (col * 8);
            await Task.Delay(delay);

            await image.FadeTo(1, 300, Easing.CubicOut);
        }

        // Add all other animation methods here, making them private static methods
        // Include all the animation methods from MainPage.Animations.cs
        
        private static async Task RandomPerCellAnimation(Image image, int row, int col, int totalRows, int totalColumns)
        {
            // Select a random animation type (excluding RandomPerCell)
            var animationTypes = Enum.GetValues<AnimationType>()
                .Where(t => t != AnimationType.RandomPerCell)
                .ToArray();

            var randomType = animationTypes[_random.Next(animationTypes.Length)];
            
            // Use the extension method recursively
            await image.AnimateCellAsync(row, col, randomType, totalRows, totalColumns);
        }
        
        // Add all the other animation methods from the original file...
    }
}