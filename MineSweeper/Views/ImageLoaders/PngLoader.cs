using MineSweeper.ViewModels;

namespace MineSweeper.Views.ImageLoaders;

/// <summary>
///     Loads PNG files and provides them as Image objects for game pieces
/// </summary>
public class PngLoader : ImageLoader
{
    private readonly Dictionary<string, string> _imagePathCache = new();
    private readonly Dictionary<GamePieceEnum.ThemedGamPieces, Image> _themeDictionary = new();

    public override object GetImageResource(GamePieceEnum.ThemedGamPieces piece)
    {
        if (_themeDictionary.TryGetValue(piece, out var image)) return image;

        throw new KeyNotFoundException($"No Image found for game piece: {piece}");
    }

    // Helper method to get Image directly (type-safe wrapper)
    public Image GetImage(GamePieceEnum.ThemedGamPieces piece)
    {
        return (Image) GetImageResource(piece);
    }

    protected override async Task LoadAllImagesAsync(string themePrefix)
    {
        // Clear existing data
        _themeDictionary.Clear();
        _imagePathCache.Clear();

        // Use a semaphore to limit concurrent file operations
        SemaphoreSlim semaphoreSlim = new(5, 5);

        try
        {
            await semaphoreSlim.WaitAsync();

            var pngPrefix = GetPngPathFromSvgPath(themePrefix);

            // First cache all image paths
            foreach (var fileName in imageFiles)
            {
                // Convert SVG filename to PNG filename
                var pngFileName = fileName.Replace(".svg", ".png");
                var imagePath = Path.Combine(pngPrefix, pngFileName);

                // Verify the PNG exists by attempting to access it
                try
                {
                    // We're not actually reading the content, just verifying existence
                    _ = await FileSystem.OpenAppPackageFileAsync(imagePath);
                    _imagePathCache[fileName] = imagePath;
                }
                catch (Exception ex)
                {
                    throw new FileNotFoundException($"PNG file not found: {imagePath}", ex);
                }
            }

            // Then map all enum values to their corresponding Image objects
            foreach (GamePieceEnum.ThemedGamPieces piece in Enum.GetValues(typeof(GamePieceEnum.ThemedGamPieces)))
            {
                var svgFileName = MapEnumToFileName(piece);
                if (_imagePathCache.TryGetValue(svgFileName, out var imagePath) && imagePath != null)
                {
                    // Create an Image object for this resource
                    var image = new Image
                    {
                        Source = ImageSource.FromFile(imagePath),
                        Aspect = Aspect.AspectFit
                    };

                    _themeDictionary[piece] = image;
                }
                else
                {
                    throw new Exception($"PNG file not loaded for game piece: {piece}");
                }
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    // Converts an SVG theme path to the equivalent PNG theme path
    protected virtual string GetPngPathFromSvgPath(string svgThemePath)
    {
        if (svgThemePath.EndsWith("/svg", StringComparison.OrdinalIgnoreCase))
            return svgThemePath.Substring(0, svgThemePath.Length - 3) + "png";

        return svgThemePath.Replace("/svg", "/png");
    }
}