namespace MineSweeper.Views.Controls;

/// <summary>
///     A control that displays a number using LED-style digit images with a 3D recessed border effect.
/// </summary>
public class LedControl : ContentView
{
    /// <summary>
    ///     Bindable property for the value to display.
    /// </summary>
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(int),
        typeof(LedControl),
        0,
        propertyChanged: OnValueChanged);

    /// <summary>
    ///     Bindable property for the maximum number of digits to display.
    /// </summary>
    public static readonly BindableProperty MaxDigitsToDisplayProperty = BindableProperty.Create(
        nameof(MaxDigitsToDisplay),
        typeof(int),
        typeof(LedControl),
        3,
        propertyChanged: OnMaxDigitsToDisplayChanged);

    /// <summary>
    ///     Bindable property for whether to pad with zeros or spaces.
    /// </summary>
    public static readonly BindableProperty PadWithZerosProperty = BindableProperty.Create(
        nameof(PadWithZeros),
        typeof(bool),
        typeof(LedControl),
        false,
        propertyChanged: OnPadWithZerosChanged);

    /// <summary>
    ///     Bindable property for the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
        nameof(HighlightColor),
        typeof(Color),
        typeof(LedControl),
        Colors.LightGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor),
        typeof(Color),
        typeof(LedControl),
        Colors.DimGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for whether to show the 3D border effect.
    /// </summary>
    public static readonly BindableProperty ShowBorderProperty = BindableProperty.Create(
        nameof(ShowBorder),
        typeof(bool),
        typeof(LedControl),
        true,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the background color of the LED display.
    /// </summary>
    public static readonly BindableProperty DisplayBackgroundColorProperty = BindableProperty.Create(
        nameof(DisplayBackgroundColor),
        typeof(Color),
        typeof(LedControl),
        Color.FromArgb("#222222"),
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the thickness of the border.
    /// </summary>
    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(int),
        typeof(LedControl),
        6,
        propertyChanged: OnBorderThicknessChanged);

    /// <summary>
    ///     Bindable property for the size scale factor of the LED display.
    /// </summary>
    public static readonly BindableProperty SizeScaleProperty = BindableProperty.Create(
        nameof(SizeScale),
        typeof(double),
        typeof(LedControl),
        1.0,
        propertyChanged: OnSizeScaleChanged);

    private readonly ChiseledBorder _border;

    private readonly HorizontalStackLayout _digitContainer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LedControl" /> class.
    /// </summary>
    public LedControl()
    {
        // Create the container for the digit images
        _digitContainer = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 0,
            Padding = 5
        };

        // Create the chiseled border with the digit container as content
        _border = new ChiseledBorder
        {
            ShadowColor = ShadowColor,
            HighlightColor = HighlightColor,
            BorderThickness = BorderThickness,
            ContentBackgroundColor = DisplayBackgroundColor,
            IsRecessed = true,
            Content = _digitContainer,
            IsVisible = ShowBorder,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        // Set the content to the border
        Content = _border;

        // Set default properties
        BackgroundColor = Colors.Transparent;
        Padding = 0;
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;

        // Update the size based on the scale factor
        UpdateSize();

        // Update the border appearance
        UpdateBorderAppearance();

        // Update the display
        UpdateDisplay();
    }

    /// <summary>
    ///     Gets or sets the value to display.
    /// </summary>
    public int Value
    {
        get => (int) GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    ///     Gets or sets the maximum number of digits to display.
    /// </summary>
    public int MaxDigitsToDisplay
    {
        get => (int) GetValue(MaxDigitsToDisplayProperty);
        set => SetValue(MaxDigitsToDisplayProperty, value);
    }

    /// <summary>
    ///     Gets or sets whether to pad with zeros or spaces.
    /// </summary>
    public bool PadWithZeros
    {
        get => (bool) GetValue(PadWithZerosProperty);
        set => SetValue(PadWithZerosProperty, value);
    }

    /// <summary>
    ///     Gets or sets the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public Color HighlightColor
    {
        get => (Color) GetValue(HighlightColorProperty);
        set => SetValue(HighlightColorProperty, value);
    }

    /// <summary>
    ///     Gets or sets the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public Color ShadowColor
    {
        get => (Color) GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    ///     Gets or sets whether to show the 3D border effect.
    /// </summary>
    public bool ShowBorder
    {
        get => (bool) GetValue(ShowBorderProperty);
        set => SetValue(ShowBorderProperty, value);
    }

    /// <summary>
    ///     Gets or sets the background color of the LED display.
    /// </summary>
    public Color DisplayBackgroundColor
    {
        get => (Color) GetValue(DisplayBackgroundColorProperty);
        set => SetValue(DisplayBackgroundColorProperty, value);
    }

    /// <summary>
    ///     Gets or sets the thickness of the border.
    /// </summary>
    public int BorderThickness
    {
        get => (int) GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    /// <summary>
    ///     Gets or sets the size scale factor of the LED display.
    ///     This allows changing the size without affecting the aspect ratio.
    /// </summary>
    public double SizeScale
    {
        get => (double) GetValue(SizeScaleProperty);
        set => SetValue(SizeScaleProperty, value);
    }

    /// <summary>
    ///     Called when the Value property changes.
    /// </summary>
    /// <param name="bindable">The LedControl instance whose property changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control) control.UpdateDisplay();
    }

    /// <summary>
    ///     Called when the MaxDigitsToDisplay property changes.
    /// </summary>
    /// <param name="bindable">The LedControl instance whose property changed.</param>
    /// <param name="oldValue">The previous maximum digits value.</param>
    /// <param name="newValue">The new maximum digits value.</param>
    private static void OnMaxDigitsToDisplayChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
        {
            // Update the size since the number of digits affects the width calculation
            control.UpdateSize();

            // Update the display
            control.UpdateDisplay();
        }
    }

    /// <summary>
    ///     Called when the PadWithZeros property changes.
    /// </summary>
    /// <param name="bindable">The LedControl instance whose property changed.</param>
    /// <param name="oldValue">The previous padding value.</param>
    /// <param name="newValue">The new padding value.</param>
    private static void OnPadWithZerosChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control) control.UpdateDisplay();
    }

    /// <summary>
    ///     Called when any of the border properties change.
    /// </summary>
    /// <param name="bindable">The LedControl instance whose property changed.</param>
    /// <param name="oldValue">The previous property value.</param>
    /// <param name="newValue">The new property value.</param>
    private static void OnBorderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
            // For other border properties, just update the appearance
            control.UpdateBorderAppearance();
    }

    /// <summary>
    ///     Called when the BorderThickness property changes.
    /// </summary>
    /// <param name="bindable">The LedControl instance whose property changed.</param>
    /// <param name="oldValue">The previous border thickness value.</param>
    /// <param name="newValue">The new border thickness value.</param>
    private static void OnBorderThicknessChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
        {
            // Force a complete update when border thickness changes
            control.UpdateSize();
            control.UpdateBorderAppearance();

            // Force a layout update
            control.InvalidateMeasure();
        }
    }

    /// <summary>
    ///     Called when the size scale property changes.
    /// </summary>
    /// <param name="bindable">The LedControl instance whose property changed.</param>
    /// <param name="oldValue">The previous scale factor value.</param>
    /// <param name="newValue">The new scale factor value.</param>
    private static void OnSizeScaleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control) control.UpdateSize();
    }

    /// <summary>
    ///     Updates the size of the control based on the scale factor.
    /// </summary>
    private void UpdateSize()
    {
        // Base size for a single digit (these are reasonable defaults that maintain aspect ratio)
        const double baseWidth = 30;
        const double baseHeight = 50;

        // Calculate the size of each digit based on the scale factor
        var digitWidth = baseWidth * SizeScale;
        var digitHeight = baseHeight * SizeScale;

        // Calculate the total width based on the number of digits
        var totalWidth = digitWidth * MaxDigitsToDisplay + BorderThickness * 2 + 10; // Add padding and border thickness
        var totalHeight = digitHeight + BorderThickness * 2 + 10; // Add padding and border thickness

        // Explicitly set the size of the control and border to ensure it shrinks when scale decreases
        WidthRequest = totalWidth;
        HeightRequest = totalHeight;

        _border.WidthRequest = totalWidth;
        _border.HeightRequest = totalHeight;

        // Set the size of the individual digit images
        foreach (var child in _digitContainer.Children)
            if (child is Image image)
            {
                image.WidthRequest = digitWidth;
                image.HeightRequest = digitHeight;
            }

        // Update the border appearance since the size changed
        UpdateBorderAppearance();

        // Force a layout update
        InvalidateMeasure();
    }

    /// <summary>
    ///     Updates the border appearance based on the current property values.
    /// </summary>
    private void UpdateBorderAppearance()
    {
        if (_border == null)
            return;

        // Update the border properties
        _border.ShadowColor = ShadowColor;
        _border.HighlightColor = HighlightColor;
        _border.BorderThickness = BorderThickness;
        _border.ContentBackgroundColor = DisplayBackgroundColor;
        _border.IsVisible = ShowBorder;
    }

    /// <summary>
    ///     Updates the display based on the current property values.
    /// </summary>
    private void UpdateDisplay()
    {
        // Clear the container
        _digitContainer.Children.Clear();

        // Convert the value to a string
        var valueStr = Value.ToString();

        // If the value is larger than MaxDigitsToDisplay, truncate from the left
        if (valueStr.Length > MaxDigitsToDisplay)
        {
            valueStr = valueStr.Substring(valueStr.Length - MaxDigitsToDisplay);
        }
        else if (valueStr.Length < MaxDigitsToDisplay)
        {
            // Pad with zeros or spaces as specified
            var paddingChar = PadWithZeros ? '0' : ' ';
            valueStr = valueStr.PadLeft(MaxDigitsToDisplay, paddingChar);
        }

        // Base size for a single digit
        const double baseWidth = 30;
        const double baseHeight = 50;

        // Calculate the size of each digit based on the scale factor
        var digitWidth = baseWidth * SizeScale;
        var digitHeight = baseHeight * SizeScale;

        // Add an image for each digit
        foreach (var digit in valueStr)
        {
            var imageName = GetImageNameForDigit(digit);
            var image = new Image
            {
                Source = ImageSource.FromFile(imageName),
                Aspect = Aspect.AspectFit,
                WidthRequest = digitWidth,
                HeightRequest = digitHeight,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            _digitContainer.Children.Add(image);
        }

        // Update the size to ensure the control resizes properly
        UpdateSize();
    }

    /// <summary>
    ///     Gets the image name for a digit.
    /// </summary>
    /// <param name="digit">The digit character to get the image name for.</param>
    /// <returns>The filename of the image that corresponds to the digit.</returns>
    private string GetImageNameForDigit(char digit)
    {
        if (digit >= '0' && digit <= '9') return $"digit_{digit}_image.png";

        // For spaces or any other character, use the blank image
        return "digit_blank_image.png";
    }
}
