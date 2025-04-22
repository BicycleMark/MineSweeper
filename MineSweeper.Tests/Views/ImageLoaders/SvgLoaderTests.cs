using MineSweeper.ViewModels;
using MineSweeper.Views.ImageLoaders;

namespace MineSweeper.Tests.Views.ImageLoaders;

/// <summary>
///     Tests for the SvgLoader implementation
/// </summary>
public class SvgLoaderTests
{
    [Fact]
    public async Task GetSvg_AfterInitialization_ReturnsCorrectContent()
    {
        // Arrange
        var loader = new TestSvgLoader();

        // Setup specific content for a few files
        loader.SetupMockSvgContent("digit_0_image.svg", "<svg>Digit 0 custom content</svg>");
        loader.SetupMockSvgContent("flagged.svg", "<svg>Flagged custom content</svg>");

        // Act
        await loader.InitializeAsync("Themes/test");
        var svg1 = loader.GetSvg(GamePieceEnum.ThemedGamPieces.Digit0);
        var svg2 = loader.GetSvg(GamePieceEnum.ThemedGamPieces.Flagged);

        // Assert
        Assert.Equal("<svg>Digit 0 custom content</svg>", svg1);
        Assert.Equal("<svg>Flagged custom content</svg>", svg2);
    }

    [Fact]
    public void GetImageResource_WithMissingPiece_ThrowsKeyNotFoundException()
    {
        // Arrange 
        var loader = new SvgLoader();
        // Not initialized, so dictionary is empty

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => loader.GetImageResource(GamePieceEnum.ThemedGamPieces.Digit0));
    }

    [Fact]
    public async Task InitializeAsync_LoadsAllGamePieces()
    {
        // Arrange
        var loader = new TestSvgLoader();

        // Act
        await loader.InitializeAsync("Themes/test");

        // Assert - try to get each enum value, none should throw
        foreach (GamePieceEnum.ThemedGamPieces piece in Enum.GetValues(typeof(GamePieceEnum.ThemedGamPieces)))
        {
            var svg = loader.GetSvg(piece);
            Assert.NotNull(svg);
            Assert.Contains("svg", svg); // Should contain svg tag
        }
    }

    [Fact]
    public async Task ChangeThemeAsync_UpdatesThemeContent()
    {
        // Arrange
        var loader = new TestSvgLoader();

        // Setup specific content
        loader.SetupMockSvgContent("unplayed.svg", "<svg>Default theme unplayed</svg>");

        // Act - initialize with default theme
        await loader.InitializeAsync();
        var defaultSvg = loader.GetSvg(GamePieceEnum.ThemedGamPieces.Unplayed);

        // Update mock content for tropical theme
        loader.SetupMockSvgContent("unplayed.svg", "<svg>Tropical theme unplayed</svg>");

        // Change to tropical theme
        await loader.ChangeThemeAsync("Themes/tropical");
        var tropicalSvg = loader.GetSvg(GamePieceEnum.ThemedGamPieces.Unplayed);

        // Assert
        Assert.Equal("<svg>Default theme unplayed</svg>", defaultSvg);
        Assert.Equal("<svg>Tropical theme unplayed</svg>", tropicalSvg);
    }

    [Fact]
    public async Task ValidateThemeCompleteness_WhenFails_ThrowsException()
    {
        // Arrange
        var loader = new TestSvgLoader();
        loader.SetValidateResult(false);

        // Act & Assert
        // The exception needs to be thrown in ValidateThemeCompleteness, which is called by InitializeAsync
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
        {
            await loader.ChangeThemeAsync("Themes/invalid");
        });
    }

    /// <summary>
    ///     Helper class to test SvgLoader without file system dependencies
    /// </summary>
    private class TestSvgLoader : SvgLoader
    {
        private readonly Dictionary<string, string> _mockSvgFiles = new();
        private bool _validateAlwaysSucceeds = true;

        public void SetupMockSvgContent(string fileName, string content)
        {
            _mockSvgFiles[fileName] = content;
        }

        public void SetValidateResult(bool succeeds)
        {
            _validateAlwaysSucceeds = succeeds;
        }

        // Override protected methods to avoid file system access
        protected override async Task<string> LoadMauiAsset(string prefix, string assetName)
        {
            if (_mockSvgFiles.TryGetValue(assetName, out var content)) return content;

            // Return a default mock SVG if no specific content was set
            return $"<svg>{assetName} mock content</svg>";
        }

        protected override Task ValidateThemeCompleteness(string themePrefix)
        {
            if (!_validateAlwaysSucceeds) throw new FileNotFoundException("Validation failed for testing");

            return Task.CompletedTask;
        }

        // Override to bypass file system validation
        public override async Task InitializeAsync(string themePrefix = "Themes/default")
        {
            // Skip the base initialization that requires file system access
            currentTheme = themePrefix;
            await LoadAllImagesAsync(themePrefix);
        }
    }
}