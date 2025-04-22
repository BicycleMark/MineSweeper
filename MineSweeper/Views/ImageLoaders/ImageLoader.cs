using MineSweeper.ViewModels;

namespace MineSweeper.Views.ImageLoaders;

/// <summary>
///     Base class for image loaders that load game piece images from resources
/// </summary>
public abstract class ImageLoader : IImageLoader
{
    protected readonly string[] imageFiles =
    {
        "digit_0_image.svg",
        "digit_1_image.svg",
        "digit_2_image.svg",
        "digit_3_image.svg",
        "digit_4_image.svg",
        "digit_5_image.svg",
        "digit_6_image.svg",
        "digit_7_image.svg",
        "digit_8_image.svg",
        "digit_9_image.svg",
        "digit_none_image.svg",
        "flagged.svg",
        "neighbor_0_image.svg",
        "neighbor_1_image.svg",
        "neighbor_2_image.svg",
        "neighbor_3_image.svg",
        "neighbor_4_image.svg",
        "neighbor_5_image.svg",
        "neighbor_6_image.svg",
        "neighbor_7_image.svg",
        "neighbor_8_image.svg",
        "pressed.svg",
        "revealed_mine.svg",
        "unplayed.svg",
        "wrong_guess.svg"
    };

    /// <summary>
    ///     The current theme path being used
    /// </summary>
    protected string currentTheme = string.Empty;

    // Base implementation of IImageLoader interface
    public virtual async Task InitializeAsync(string themePrefix = "Themes/default")
    {
        currentTheme = themePrefix;
        await ValidateThemeCompleteness(themePrefix);
        await LoadAllImagesAsync(themePrefix);
    }

    public virtual async Task ChangeThemeAsync(string themePrefix)
    {
        if (currentTheme == themePrefix)
            // Already using this theme - no need to reload
            return;

        // Validate new theme before changing
        await ValidateThemeCompleteness(themePrefix);
        currentTheme = themePrefix;
        await LoadAllImagesAsync(themePrefix);
    }

    // Abstract method that derived classes must implement
    public abstract object GetImageResource(GamePieceEnum.ThemedGamPieces piece);

    // Maps enum values to their corresponding filenames
    protected virtual string MapEnumToFileName(GamePieceEnum.ThemedGamPieces piece)
    {
        return piece switch
        {
            GamePieceEnum.ThemedGamPieces.Digit0 => "digit_0_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit1 => "digit_1_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit2 => "digit_2_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit3 => "digit_3_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit4 => "digit_4_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit5 => "digit_5_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit6 => "digit_6_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit7 => "digit_7_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit8 => "digit_8_image.svg",
            GamePieceEnum.ThemedGamPieces.Digit9 => "digit_9_image.svg",
            GamePieceEnum.ThemedGamPieces.DigitNone => "digit_none_image.svg",
            GamePieceEnum.ThemedGamPieces.Flagged => "flagged.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor0 => "neighbor_0_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor1 => "neighbor_1_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor2 => "neighbor_2_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor3 => "neighbor_3_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor4 => "neighbor_4_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor5 => "neighbor_5_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor6 => "neighbor_6_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor7 => "neighbor_7_image.svg",
            GamePieceEnum.ThemedGamPieces.Neighbor8 => "neighbor_8_image.svg",
            GamePieceEnum.ThemedGamPieces.Pressed => "pressed.svg",
            GamePieceEnum.ThemedGamPieces.RevealedMine => "revealed_mine.svg",
            GamePieceEnum.ThemedGamPieces.Unplayed => "unplayed.svg",
            GamePieceEnum.ThemedGamPieces.WrongGuess => "wrong_guess.svg",
            _ => throw new ArgumentOutOfRangeException(nameof(piece), piece, "Unknown themed game piece")
        };
    }

    // Protected methods for derived classes to use
    protected virtual async Task<string> LoadMauiAsset(string prefix, string assetName)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(
            Path.Combine(prefix, assetName));
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    protected virtual async Task ValidateThemeCompleteness(string themePrefix)
    {
        List<string> missingFiles = new();

        foreach (var fileName in imageFiles)
            try
            {
                // Try to open the file to verify it exists
                await FileSystem.OpenAppPackageFileAsync(Path.Combine(themePrefix, fileName));
            }
            catch (Exception)
            {
                // File doesn't exist
                missingFiles.Add(fileName);
            }

        if (missingFiles.Count > 0)
            throw new FileNotFoundException(
                $"The following required files are missing from theme '{themePrefix}': {string.Join(", ", missingFiles)}");
    }

    // Abstract method that derived classes must implement to load their specific resources
    protected abstract Task LoadAllImagesAsync(string themePrefix);
}