using System.Threading;
using System.Threading.Tasks;
using MineSweeper.ViewModels;

namespace MineSweeper.Views.ImageLoaders;

public class SvgLoader : ImageLoader
{
    public SvgLoader()
    {
        _themeDictionary = new Dictionary<GamePieceEnum.ThemedGamPieces, string>();
    }
    private string[] imageFiles =
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
    
    private readonly Dictionary<string, string> svgDictionary = new();
    public async Task InitializeAsync(string prefix = "Themes/default")
    {
        _themeDictionary.Clear();
        await LoadAllImages(prefix);
        return ;
    }
    
    private  async Task LoadAllImages(string prefix)
    {
        SemaphoreSlim semaphoreSlim = new(5, 5);
        await semaphoreSlim.WaitAsync();
        try
        {
            foreach (var image in imageFiles)
            {
                var svg = await LoadMauiAsset(prefix, image);
                if (svg == null)
                {
                    throw new Exception($"Failed to load SVG: {image}");
                }

                svgDictionary.Add(image, svg);
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
    
    Dictionary<GamePieceEnum.ThemedGamPieces, string> _themeDictionary = new();
    public string this[GamePieceEnum.ThemedGamPieces index]
    {
        get => _themeDictionary[index];
        
    }
    
} // SvgLoader
