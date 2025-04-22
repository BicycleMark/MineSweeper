using MineSweeper.ViewModels;

namespace MineSweeper.Views.ImageLoaders;

/// <summary>
///     Interface for image loaders that provide themed game piece images
/// </summary>
public interface IImageLoader
{
    /// <summary>
    ///     Initializes the loader with images from the specified theme
    /// </summary>
    /// <param name="themePrefix">Theme prefix path (default: "Themes/default")</param>
    /// <returns>Task that completes when initialization is done</returns>
    Task InitializeAsync(string themePrefix = "Themes/default");

    /// <summary>
    ///     Changes the current theme and loads all images for that theme
    /// </summary>
    /// <param name="themePrefix">Theme prefix path</param>
    /// <returns>Task that completes when theme change is done</returns>
    Task ChangeThemeAsync(string themePrefix);

    /// <summary>
    ///     Gets the image resource for the specified game piece
    /// </summary>
    /// <param name="piece">The game piece to get the image for</param>
    /// <returns>The image resource (string for SVG, Image for PNG)</returns>
    object GetImageResource(GamePieceEnum.ThemedGamPieces piece);
}