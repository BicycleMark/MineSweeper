using MineSweeper.ViewModels;

namespace MineSweeper.Views.ImageLoaders;

/// <summary>
///     Loads SVG files and provides them as string content for game pieces
/// </summary>
public class SvgLoader : ImageLoader
{
    private readonly Dictionary<string, string> _svgContentCache = new();
    private readonly Dictionary<GamePieceEnum.ThemedGamPieces, string> _themeDictionary = new();


    public override object GetImageResource(GamePieceEnum.ThemedGamPieces piece)
    {
        if (_themeDictionary.TryGetValue(piece, out var svgContent)) return svgContent;

        throw new KeyNotFoundException($"No SVG content found for game piece: {piece}");
    }

    // Helper method to get SVG content directly as string (type-safe wrapper)
    public string GetSvg(GamePieceEnum.ThemedGamPieces piece)
    {
        return (string) GetImageResource(piece);
    }

    protected override async Task LoadAllImagesAsync(string themePrefix)
    {
        // Clear existing data
        _themeDictionary.Clear();
        _svgContentCache.Clear();

        // Use a semaphore to limit concurrent file operations
        SemaphoreSlim semaphoreSlim = new(5, 5);

        try
        {
            await semaphoreSlim.WaitAsync();

            // First load all SVG content into the content cache
            foreach (var fileName in imageFiles)
            {
                var svgContent = await LoadMauiAsset(themePrefix, fileName);
                if (string.IsNullOrEmpty(svgContent)) throw new Exception($"Failed to load SVG: {fileName}");

                _svgContentCache[fileName] = svgContent;
            }

            // Then map all enum values to their corresponding SVG content
            foreach (GamePieceEnum.ThemedGamPieces piece in Enum.GetValues(typeof(GamePieceEnum.ThemedGamPieces)))
            {
                var fileName = MapEnumToFileName(piece);
                if (_svgContentCache.TryGetValue(fileName, out var svgContent) && svgContent != null)
                    _themeDictionary[piece] = svgContent;
                else
                    throw new Exception($"SVG file not loaded for game piece: {piece} (file: {fileName})");
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
}