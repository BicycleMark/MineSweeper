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
        // Animation type enum
        private enum AnimationType
        {
            FlipIn,
            FadeIn,
            ScaleIn,
            BounceIn,
            SpinIn,
            SlideIn,
        }

        // Animation dictionary to store our animation methods
        private readonly Dictionary<AnimationType, Func<Image, int, int, Task>> _animations = new();

        // Random for selecting animations
        private readonly Random _random = new();

        // Initialize animations in constructor
        partial void InitializeAnimations()
        {
            _animations.Clear(); // Clear first since we can't reassign the readonly field
            _animations.Add(AnimationType.FlipIn, FlipInAnimation);
            _animations.Add(AnimationType.FadeIn, FadeInAnimation);
            _animations.Add(AnimationType.ScaleIn, ScaleInAnimation);
            _animations.Add(AnimationType.BounceIn, BounceInAnimation);
            _animations.Add(AnimationType.SpinIn, SpinInAnimation);
            _animations.Add(AnimationType.SlideIn, SlideInAnimation);
        }

        // Gets a random animation for the cell
        private Func<Image, int, int, Task> GetRandomAnimation()
        {
            var animationTypes = Enum.GetValues<AnimationType>();
            var selectedType = animationTypes[_random.Next(animationTypes.Length)];
            return _animations[selectedType];
        }

        // Handler for GetCellImage event
        private void HandleGetCellImage(object? sender, GetCellImageEventArgs getCellImageEventArgs)
        {
            var image = CreateCellImage();
            getCellImageEventArgs.Image = image;

            // Schedule animation to run after layout
            Dispatcher.Dispatch(async () => {
                await Task.Delay(50);
                var animation = GetRandomAnimation();
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
    }
}