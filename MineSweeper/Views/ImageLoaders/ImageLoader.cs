using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace MineSweeper.Views.ImageLoaders;

public class ImageLoader
{
    protected  async Task<string> LoadMauiAsset(string prefix, string assetName)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(
            Path.Combine(prefix, assetName));
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}