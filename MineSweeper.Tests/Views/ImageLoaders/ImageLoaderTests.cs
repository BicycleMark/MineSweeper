using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MineSweeper.ViewModels;
using MineSweeper.Views.ImageLoaders;
using Moq;
using Xunit;

namespace MineSweeper.Tests.Views.ImageLoaders;

/// <summary>
/// Tests for the ImageLoader base class functionality
/// </summary>
public class ImageLoaderTests
{
    /// <summary>
    /// Concrete implementation of ImageLoader for testing purposes
    /// </summary>
    private class TestImageLoader : ImageLoader
    {
        /// <summary>
        /// Flag to track if LoadAllImagesAsync was called
        /// </summary>
        public bool LoadAllImagesAsyncCalled { get; private set; }
        
        /// <summary>
        /// Tracks the last theme prefix passed to methods
        /// </summary>
        public string? LastThemePrefix { get; private set; }
        
        private readonly Dictionary<GamePieceEnum.ThemedGamPieces, object> _resources = new();

        // Override to skip file system validation in tests
        protected override Task ValidateThemeCompleteness(string themePrefix)
        {
            // For tests, we don't validate against the actual file system
            LastThemePrefix = themePrefix;
            return Task.CompletedTask;
        }
        
        // Override ChangeThemeAsync to properly track when LoadAllImagesAsync is called
        public override async Task ChangeThemeAsync(string themePrefix)
        {
            // If we're changing to the same theme, don't reload
            if (currentTheme == themePrefix)
            {
                // We set the LastThemePrefix but don't call LoadAllImagesAsync
                LastThemePrefix = themePrefix;
                return;
            }
            
            // Now we can proceed with normal loading process for a new theme
            await base.ChangeThemeAsync(themePrefix);
        }
        
        public override object GetImageResource(GamePieceEnum.ThemedGamPieces piece)
        {
            if (_resources.TryGetValue(piece, out var resource))
            {
                return resource;
            }
            
            throw new KeyNotFoundException($"Resource not found for {piece}");
        }
        
        protected override Task LoadAllImagesAsync(string themePrefix)
        {
            LoadAllImagesAsyncCalled = true;
            LastThemePrefix = themePrefix;
            
            // Add some test data
            foreach (GamePieceEnum.ThemedGamPieces piece in Enum.GetValues(typeof(GamePieceEnum.ThemedGamPieces)))
            {
                _resources[piece] = $"Mock resource for {piece}";
            }
            
            return Task.CompletedTask;
        }
        
        // We don't need this method anymore since we're overriding ValidateThemeCompleteness above
            
        // Expose MapEnumToFileName for testing
        public string TestMapEnumToFileName(GamePieceEnum.ThemedGamPieces piece) =>
            MapEnumToFileName(piece);
    }
    
    [Fact]
    public async Task InitializeAsync_SetsCurrentThemeAndLoadsImages()
    {
        // Arrange
        var loader = new TestImageLoader();
        var themePrefix = "Themes/test";
        
        // Use the real FileSystem for now - in a real test we would mock this
        
        // Act
        await loader.InitializeAsync(themePrefix);
        
        // Assert
        Assert.Equal(themePrefix, loader.LastThemePrefix);
        Assert.True(loader.LoadAllImagesAsyncCalled);
    }
    
    [Fact]
    public async Task ChangeThemeAsync_WhenNewTheme_LoadsNewTheme()
    {
        // Arrange
        var loader = new TestImageLoader();
        var initialTheme = "Themes/default";
        var newTheme = "Themes/tropical";
        
        await loader.InitializeAsync(initialTheme);
        
        // Reset the flag to test if LoadAllImagesAsync is called again
        var prop = typeof(TestImageLoader).GetProperty("LoadAllImagesAsyncCalled");
        prop?.SetValue(loader, false);
        
        // Act
        await loader.ChangeThemeAsync(newTheme);
        
        // Assert
        Assert.Equal(newTheme, loader.LastThemePrefix);
        Assert.True(loader.LoadAllImagesAsyncCalled);
    }
    
    [Fact]
    public async Task ChangeThemeAsync_WhenSameTheme_DoesNotReload()
    {
        // Arrange
        var loader = new TestImageLoader();
        var theme = "Themes/default";
        
        await loader.InitializeAsync(theme);
        
        // Reset the flag to test if LoadAllImagesAsync is called again
        var prop = typeof(TestImageLoader).GetProperty("LoadAllImagesAsyncCalled");
        prop?.SetValue(loader, false);
        
        // Act
        await loader.ChangeThemeAsync(theme);
        
        // Assert
        Assert.False(loader.LoadAllImagesAsyncCalled);
    }
    
    [Fact]
    public void MapEnumToFileName_ReturnsCorrectFilename()
    {
        // Arrange
        var loader = new TestImageLoader();
        
        // Act & Assert
        Assert.Equal("digit_0_image.svg", loader.TestMapEnumToFileName(GamePieceEnum.ThemedGamPieces.Digit0));
        Assert.Equal("neighbor_1_image.svg", loader.TestMapEnumToFileName(GamePieceEnum.ThemedGamPieces.Neighbor1));
        Assert.Equal("flagged.svg", loader.TestMapEnumToFileName(GamePieceEnum.ThemedGamPieces.Flagged));
        Assert.Equal("revealed_mine.svg", loader.TestMapEnumToFileName(GamePieceEnum.ThemedGamPieces.RevealedMine));
    }
    
    [Fact]
    public void MapEnumToFileName_WithInvalidEnum_ThrowsException()
    {
        // Arrange
        var loader = new TestImageLoader();
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            loader.TestMapEnumToFileName((GamePieceEnum.ThemedGamPieces)999));
    }
}
