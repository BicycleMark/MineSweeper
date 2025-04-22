using System.Globalization;

namespace MineSweeper.Views.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // If the target type is Boolean, use the BoolToBoolConverter logic
        if (targetType == typeof(bool)) return new BoolToBoolConverter().Convert(value, targetType, parameter, culture);

        // Otherwise, use the original color conversion logic
        if (value is bool boolValue && parameter is string paramString)
        {
            var colorNames = paramString.Split(',');
            if (colorNames.Length >= 2)
                return boolValue ? GetColorByName(colorNames[0].Trim()) : GetColorByName(colorNames[1].Trim());
        }

        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private Color GetColorByName(string colorName)
    {
        // Check if the color name is a resource reference
        if (colorName.StartsWith("{StaticResource "))
        {
            var resourceName = colorName.Substring("{StaticResource ".Length,
                colorName.Length - "{StaticResource ".Length - 1);

            // Try to get the color from the application resources
            if (Application.Current?.Resources?.TryGetValue(resourceName, out var resource) == true &&
                resource is Color color) return color;
        }

        // Fallback to predefined colors
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
}

public class BoolToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string paramString)
        {
            var boolValues = paramString.Split(',');
            if (boolValues.Length >= 2)
            {
                var trueValue = bool.Parse(boolValues[0].Trim());
                var falseValue = bool.Parse(boolValues[1].Trim());
                return boolValue ? trueValue : falseValue;
            }
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolAndBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is bool paramBool) return boolValue && paramBool;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}