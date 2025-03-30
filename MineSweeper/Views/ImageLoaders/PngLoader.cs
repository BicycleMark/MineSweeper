using Microsoft.Maui.Controls;
using MineSweeper.ViewModels;

namespace MineSweeper.Views.ImageLoaders;

public class PngLoader : ImageLoader
{
    Dictionary<GamePieceEnum.ThemedGamPieces, Image> _themeDictionary = new();
    public Image this[GamePieceEnum.ThemedGamPieces index]
    {
        get => _themeDictionary[index];
        
    }
}