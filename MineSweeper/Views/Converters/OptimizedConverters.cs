using System.Globalization;
using Microsoft.Maui.Graphics;

namespace MineSweeper.Views.Converters;

/// <summary>
/// Converts a boolean to a color for cell background (revealed/unrevealed)
/// </summary>
public class SimpleRevealedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isRevealed)
        {
            // Get colors from application resources
            if (Application.Current?.Resources != null)
            {
                var revealedKey = "CellRevealed";
                var unrevealedKey = "CellUnrevealed";
                
                if (Application.Current.Resources.TryGetValue(isRevealed ? revealedKey : unrevealedKey, out var color) && color is Color)
                {
                    return color;
                }
            }
            
            // Fallback colors
            return isRevealed ? Colors.LightGray : Colors.DarkGray;
        }
        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean to opacity (1.0 for true, 0.0 for false)
/// </summary>
public class BoolToOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue ? 1.0 : 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean to opacity (0.0 for true, 1.0 for false)
/// </summary>
public class InverseBoolToOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue ? 0.0 : 1.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a mine count to a color
/// </summary>
public class MineCountToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int mineCount)
        {
            // Get color from application resources if available
            var resourceKey = $"MineCount{mineCount}Color";
            if (Application.Current?.Resources != null && 
                Application.Current.Resources.TryGetValue(resourceKey, out var color) && 
                color is Color)
            {
                return color;
            }
            
            // Fallback colors
            return mineCount switch
            {
                1 => Colors.Blue,
                2 => Colors.Green,
                3 => Colors.Red,
                4 => Colors.DarkBlue,
                5 => Colors.DarkRed,
                6 => Colors.Teal,
                7 => Colors.Black,
                8 => Colors.Gray,
                _ => Colors.Transparent
            };
        }
        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a numeric value to a boolean (true if > 0)
/// </summary>
public class NonZeroConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is int intValue && intValue > 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
