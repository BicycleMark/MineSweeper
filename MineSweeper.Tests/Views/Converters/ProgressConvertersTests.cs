using System.Globalization;
using MineSweeper.Views.Converters;

namespace MineSweeper.Tests.Views.Converters;

public class ProgressConvertersTests
{
    [Fact]
    public void SimpleRevealedConverter_Convert()
    {
        // Arrange
        var converter = new SimpleRevealedConverter();

        // Act
        var result = converter.Convert(true, typeof(Color), null, CultureInfo.CurrentCulture);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Color>(result);
    }

    [Fact]
    public void BoolToOpacityConverter_Convert()
    {
        // Arrange
        var converter = new BoolToOpacityConverter();

        // Act
        var result = converter.Convert(true, typeof(double), null, CultureInfo.CurrentCulture);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<double>(result);
    }

    [Fact]
    public void InverseBoolToOpacityConverter_Convert()
    {
        // Arrange
        var converter = new InverseBoolToOpacityConverter();

        // Act
        var result = converter.Convert(true, typeof(double), null, CultureInfo.CurrentCulture);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<double>(result);
    }
}