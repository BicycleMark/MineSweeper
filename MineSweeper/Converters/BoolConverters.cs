using System.Globalization;
using Microsoft.Maui.Graphics;

namespace MineSweeper.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string paramString)
        {
            var colorNames = paramString.Split(',');
            if (colorNames.Length >= 2)
            {
                return boolValue ? GetColorByName(colorNames[0].Trim()) : GetColorByName(colorNames[1].Trim());
            }
        }
        return Colors.Transparent;
    }
    
    private Color GetColorByName(string colorName)
    {
        return colorName.ToLower() switch
        {
            "lightgray" => Colors.LightGray,
            "darkgray" => Colors.DarkGray,
            "blue" => Colors.Blue,
            "red" => Colors.Red,
            "green" => Colors.Green,
            "yellow" => Colors.Yellow,
            "black" => Colors.Black,
            "white" => Colors.White,
            _ => Colors.Transparent
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolAndBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is bool paramBool)
        {
            return boolValue && paramBool;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
