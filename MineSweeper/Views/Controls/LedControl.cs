using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace MineSweeper.Views.Controls;

/// <summary>
/// A control that displays a number using LED-style digit images with a 3D recessed border effect.
/// </summary>
public class LedControl : ContentView
{
    /// <summary>
    /// Bindable property for the value to display.
    /// </summary>
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(int),
        typeof(LedControl),
        defaultValue: 0,
        propertyChanged: OnValueChanged);

    /// <summary>
    /// Bindable property for the maximum number of digits to display.
    /// </summary>
    public static readonly BindableProperty MaxDigitsToDisplayProperty = BindableProperty.Create(
        nameof(MaxDigitsToDisplay),
        typeof(int),
        typeof(LedControl),
        defaultValue: 3,
        propertyChanged: OnMaxDigitsToDisplayChanged);

    /// <summary>
    /// Bindable property for whether to pad with zeros or spaces.
    /// </summary>
    public static readonly BindableProperty PadWithZerosProperty = BindableProperty.Create(
        nameof(PadWithZeros),
        typeof(bool),
        typeof(LedControl),
        defaultValue: false,
        propertyChanged: OnPadWithZerosChanged);
        
    /// <summary>
    /// Bindable property for the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
        nameof(HighlightColor),
        typeof(Color),
        typeof(LedControl),
        defaultValue: Colors.LightGray,
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor),
        typeof(Color),
        typeof(LedControl),
        defaultValue: Colors.DimGray,
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for whether to show the 3D border effect.
    /// </summary>
    public static readonly BindableProperty ShowBorderProperty = BindableProperty.Create(
        nameof(ShowBorder),
        typeof(bool),
        typeof(LedControl),
        defaultValue: true,
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for the background color of the LED display.
    /// </summary>
    public static readonly BindableProperty DisplayBackgroundColorProperty = BindableProperty.Create(
        nameof(DisplayBackgroundColor),
        typeof(Color),
        typeof(LedControl),
        defaultValue: Color.FromArgb("#222222"),
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for the thickness of the border.
    /// </summary>
    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(int),
        typeof(LedControl),
        defaultValue: 6,
        propertyChanged: OnBorderThicknessChanged);
        
    /// <summary>
    /// Bindable property for the size scale factor of the LED display.
    /// </summary>
    public static readonly BindableProperty SizeScaleProperty = BindableProperty.Create(
        nameof(SizeScale),
        typeof(double),
        typeof(LedControl),
        defaultValue: 1.0,
        propertyChanged: OnSizeScaleChanged);

    /// <summary>
    /// Gets or sets the value to display.
    /// </summary>
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of digits to display.
    /// </summary>
    public int MaxDigitsToDisplay
    {
        get => (int)GetValue(MaxDigitsToDisplayProperty);
        set => SetValue(MaxDigitsToDisplayProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to pad with zeros or spaces.
    /// </summary>
    public bool PadWithZeros
    {
        get => (bool)GetValue(PadWithZerosProperty);
        set => SetValue(PadWithZerosProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public Color HighlightColor
    {
        get => (Color)GetValue(HighlightColorProperty);
        set => SetValue(HighlightColorProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }
    
    /// <summary>
    /// Gets or sets whether to show the 3D border effect.
    /// </summary>
    public bool ShowBorder
    {
        get => (bool)GetValue(ShowBorderProperty);
        set => SetValue(ShowBorderProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the background color of the LED display.
    /// </summary>
    public Color DisplayBackgroundColor
    {
        get => (Color)GetValue(DisplayBackgroundColorProperty);
        set => SetValue(DisplayBackgroundColorProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the thickness of the border.
    /// </summary>
    public int BorderThickness
    {
        get => (int)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the size scale factor of the LED display.
    /// This allows changing the size without affecting the aspect ratio.
    /// </summary>
    public double SizeScale
    {
        get => (double)GetValue(SizeScaleProperty);
        set => SetValue(SizeScaleProperty, value);
    }

    private readonly HorizontalStackLayout _digitContainer;
    private readonly Grid _container;
    private readonly BoxView _recessedArea;
    
    // Outer border lines
    private readonly List<Line> _topOuterLines = new();
    private readonly List<Line> _leftOuterLines = new();
    private readonly List<Line> _bottomOuterLines = new();
    private readonly List<Line> _rightOuterLines = new();
    
    // Inner border lines
    private readonly List<Line> _topInnerLines = new();
    private readonly List<Line> _leftInnerLines = new();
    private readonly List<Line> _bottomInnerLines = new();
    private readonly List<Line> _rightInnerLines = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LedControl"/> class.
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
        
        // Recessed area (background for the digits)
        // This is the main background area where the digits will be displayed
        _recessedArea = new BoxView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = DisplayBackgroundColor
        };
        
        // Create the main container grid
        _container = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Padding = 0
        };
        
        // Add the recessed area and digit container to the grid
        _container.Add(_recessedArea);         // Base layer (background)
        _container.Add(_digitContainer);       // Top layer (content)
        
        // Create the border lines
        CreateBorderLines();
        
        // Set the content to the container
        Content = _container;

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
    /// Creates the border lines for the 3D chiseled effect.
    /// </summary>
    private void CreateBorderLines()
    {
        // Create the outer border lines - create 8 lines (maximum possible thickness)
        for (int i = 0; i < 8; i++)
        {
            // Top outer line (shadow) - thin line with 1px thickness
            var topLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(ShadowColor),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start
            };
            _topOuterLines.Add(topLine);
            _container.Add(topLine);
            
            // Left outer line (shadow) - thin line with 1px thickness
            var leftLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(ShadowColor),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Fill
            };
            _leftOuterLines.Add(leftLine);
            _container.Add(leftLine);
            
            // Bottom outer line (highlight) - thin line with 1px thickness
            var bottomLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(HighlightColor),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End
            };
            _bottomOuterLines.Add(bottomLine);
            _container.Add(bottomLine);
            
            // Right outer line (highlight) - thin line with 1px thickness
            var rightLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(HighlightColor),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Fill
            };
            _rightOuterLines.Add(rightLine);
            _container.Add(rightLine);
        }
        
        // Create the inner border lines - create 8 lines (maximum possible thickness)
        for (int i = 0; i < 8; i++)
        {
            // Top inner line (shadow) - thin line with 1px thickness
            var topLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(ShadowColor),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start
            };
            _topInnerLines.Add(topLine);
            _container.Add(topLine);
            
            // Left inner line (shadow) - thin line with 1px thickness
            var leftLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(ShadowColor),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Fill
            };
            _leftInnerLines.Add(leftLine);
            _container.Add(leftLine);
            
            // Bottom inner line (highlight) - thin line with 1px thickness
            var bottomLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(HighlightColor),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.End
            };
            _bottomInnerLines.Add(bottomLine);
            _container.Add(bottomLine);
            
            // Right inner line (highlight) - thin line with 1px thickness
            var rightLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(HighlightColor),
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Fill
            };
            _rightInnerLines.Add(rightLine);
            _container.Add(rightLine);
        }
    }

    /// <summary>
    /// Called when the Value property changes.
    /// </summary>
    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
        {
            control.UpdateDisplay();
        }
    }

    /// <summary>
    /// Called when the MaxDigitsToDisplay property changes.
    /// </summary>
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
    /// Called when the PadWithZeros property changes.
    /// </summary>
    private static void OnPadWithZerosChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
        {
            control.UpdateDisplay();
        }
    }
    
    /// <summary>
    /// Called when any of the border properties change.
    /// </summary>
    private static void OnBorderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
        {
            // For other border properties, just update the appearance
            control.UpdateBorderAppearance();
        }
    }
    
    /// <summary>
    /// Called when the BorderThickness property changes.
    /// </summary>
    private static void OnBorderThicknessChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
        {
            // Force a complete update when border thickness changes
            control.UpdateSize();
            control.UpdateBorderAppearance();
            
            // Force a layout update
            control.InvalidateMeasure();
            control.InvalidateLayout();
        }
    }
    
    /// <summary>
    /// Called when the size scale property changes.
    /// </summary>
    private static void OnSizeScaleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LedControl control)
        {
            control.UpdateSize();
        }
    }
    
    /// <summary>
    /// Updates the size of the control based on the scale factor.
    /// </summary>
    private void UpdateSize()
    {
        // Base size for a single digit (these are reasonable defaults that maintain aspect ratio)
        const double baseWidth = 30;
        const double baseHeight = 50;
        
        // Calculate the size of each digit based on the scale factor
        double digitWidth = baseWidth * SizeScale;
        double digitHeight = baseHeight * SizeScale;
        
        // Calculate the total width based on the number of digits
        double totalWidth = digitWidth * MaxDigitsToDisplay + (BorderThickness * 2) + 10; // Add padding and border thickness
        double totalHeight = digitHeight + (BorderThickness * 2) + 10; // Add padding and border thickness
        
        // Explicitly set the size of the control and container to ensure it shrinks when scale decreases
        WidthRequest = totalWidth;
        HeightRequest = totalHeight;
        
        // Set the size of the container grid
        _container.WidthRequest = totalWidth;
        _container.HeightRequest = totalHeight;
        
        // Set the size of the recessed area
        _recessedArea.WidthRequest = totalWidth - (BorderThickness * 2);
        _recessedArea.HeightRequest = totalHeight - (BorderThickness * 2);
        
        // Set the size of the individual digit images
        foreach (var child in _digitContainer.Children)
        {
            if (child is Image image)
            {
                image.WidthRequest = digitWidth;
                image.HeightRequest = digitHeight;
            }
        }
        
        // Update the border appearance since the size changed
        UpdateBorderAppearance();
        
        // Force a layout update
        InvalidateMeasure();
        
        // Request a new layout pass
        InvalidateLayout();
    }
    
    /// <summary>
    /// Updates the border appearance based on the current property values.
    /// </summary>
    private void UpdateBorderAppearance()
    {
        if (_container == null)
            return;
            
        // Update the recessed area background color
        _recessedArea.BackgroundColor = DisplayBackgroundColor;
        
        if (ShowBorder)
        {
            // Set up the container with no padding to eliminate any extra space
            _container.Padding = new Thickness(0);
            
            // Calculate the inner border offset (where the recessed area begins)
            // Use BorderThickness directly without additional padding to eliminate empty space
            int innerBorderOffset = BorderThickness;
            
            // Configure the recessed area - make it exactly match the inner border
            // to eliminate empty space around the chiseled frame
            _recessedArea.Margin = new Thickness(innerBorderOffset);
            
            // Configure the digit container - position it with minimal margin
            // Just enough space to not overlap with the inner border lines
            _digitContainer.Margin = new Thickness(innerBorderOffset + 1);
            _digitContainer.Padding = new Thickness(0);
            
            // Update the outer border lines
            for (int i = 0; i < _topOuterLines.Count; i++)
            {
                // Make all lines visible
                _topOuterLines[i].IsVisible = i < BorderThickness;
                _leftOuterLines[i].IsVisible = i < BorderThickness;
                _bottomOuterLines[i].IsVisible = i < BorderThickness;
                _rightOuterLines[i].IsVisible = i < BorderThickness;
                
                if (i < BorderThickness)
                {
                    // Position the lines with appropriate offsets to create the mitered corners
                    
                    // Top line - starts at (i,i) and ends at (width-i-1,i)
                    _topOuterLines[i].X1 = i;
                    _topOuterLines[i].Y1 = i;
                    _topOuterLines[i].X2 = -1; // Will be set in SizeChanged event
                    _topOuterLines[i].Y2 = i;
                    
                    // Left line - starts at (i,i) and ends at (i,height-i-1)
                    _leftOuterLines[i].X1 = i;
                    _leftOuterLines[i].Y1 = i;
                    _leftOuterLines[i].X2 = i;
                    _leftOuterLines[i].Y2 = -1; // Will be set in SizeChanged event
                    
                    // Bottom line - starts at (i,height-i-1) and ends at (width-i-1,height-i-1)
                    _bottomOuterLines[i].X1 = i;
                    _bottomOuterLines[i].Y1 = -1; // Will be set in SizeChanged event
                    _bottomOuterLines[i].X2 = -1; // Will be set in SizeChanged event
                    _bottomOuterLines[i].Y2 = -1; // Will be set in SizeChanged event
                    
                    // Right line - starts at (width-i-1,i) and ends at (width-i-1,height-i-1)
                    _rightOuterLines[i].X1 = -1; // Will be set in SizeChanged event
                    _rightOuterLines[i].Y1 = i;
                    _rightOuterLines[i].X2 = -1; // Will be set in SizeChanged event
                    _rightOuterLines[i].Y2 = -1; // Will be set in SizeChanged event
                    
                    // Update the colors - for recessed look, shadow on top/left, highlight on bottom/right
                    (_topOuterLines[i].Stroke as SolidColorBrush).Color = ShadowColor;
                    (_leftOuterLines[i].Stroke as SolidColorBrush).Color = ShadowColor;
                    (_bottomOuterLines[i].Stroke as SolidColorBrush).Color = HighlightColor;
                    (_rightOuterLines[i].Stroke as SolidColorBrush).Color = HighlightColor;
                }
            }
            
            // Update the inner border lines
            for (int i = 0; i < _topInnerLines.Count; i++)
            {
                // Make all lines visible
                _topInnerLines[i].IsVisible = i < BorderThickness;
                _leftInnerLines[i].IsVisible = i < BorderThickness;
                _bottomInnerLines[i].IsVisible = i < BorderThickness;
                _rightInnerLines[i].IsVisible = i < BorderThickness;
                
                if (i < BorderThickness)
                {
                    // Position the lines with appropriate offsets to create the mitered corners
                    int offset = innerBorderOffset + i;
                    
                    // Top inner line - starts at (offset,offset) and ends at (width-offset-1,offset)
                    _topInnerLines[i].X1 = offset;
                    _topInnerLines[i].Y1 = offset;
                    _topInnerLines[i].X2 = -1; // Will be set in SizeChanged event
                    _topInnerLines[i].Y2 = offset;
                    
                    // Left inner line - starts at (offset,offset) and ends at (offset,height-offset-1)
                    _leftInnerLines[i].X1 = offset;
                    _leftInnerLines[i].Y1 = offset;
                    _leftInnerLines[i].X2 = offset;
                    _leftInnerLines[i].Y2 = -1; // Will be set in SizeChanged event
                    
                    // Bottom inner line - starts at (offset,height-offset-1) and ends at (width-offset-1,height-offset-1)
                    _bottomInnerLines[i].X1 = offset;
                    _bottomInnerLines[i].Y1 = -1; // Will be set in SizeChanged event
                    _bottomInnerLines[i].X2 = -1; // Will be set in SizeChanged event
                    _bottomInnerLines[i].Y2 = -1; // Will be set in SizeChanged event
                    
                    // Right inner line - starts at (width-offset-1,offset) and ends at (width-offset-1,height-offset-1)
                    _rightInnerLines[i].X1 = -1; // Will be set in SizeChanged event
                    _rightInnerLines[i].Y1 = offset;
                    _rightInnerLines[i].X2 = -1; // Will be set in SizeChanged event
                    _rightInnerLines[i].Y2 = -1; // Will be set in SizeChanged event
                    
                    // Update the colors - use full opacity for better visibility
                    (_topInnerLines[i].Stroke as SolidColorBrush).Color = ShadowColor;
                    (_leftInnerLines[i].Stroke as SolidColorBrush).Color = ShadowColor;
                    (_bottomInnerLines[i].Stroke as SolidColorBrush).Color = HighlightColor;
                    (_rightInnerLines[i].Stroke as SolidColorBrush).Color = HighlightColor;
                }
            }
            
            // Subscribe to the SizeChanged event to update the line coordinates
            SizeChanged += OnSizeChanged;
        }
        else
        {
            // Hide all the border lines
            foreach (var line in _topOuterLines.Concat(_leftOuterLines).Concat(_bottomOuterLines).Concat(_rightOuterLines)
                                .Concat(_topInnerLines).Concat(_leftInnerLines).Concat(_bottomInnerLines).Concat(_rightInnerLines))
            {
                line.IsVisible = false;
            }
            
            // Keep the recessed area and digit container visible
            _recessedArea.Margin = new Thickness(0);
            _digitContainer.Margin = new Thickness(6);
            _digitContainer.Padding = new Thickness(4);
            
            // Unsubscribe from the SizeChanged event
            SizeChanged -= OnSizeChanged;
        }
    }
    
    /// <summary>
    /// Called when the control's size changes.
    /// </summary>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (!ShowBorder)
            return;
            
        double width = Width;
        double height = Height;
        
        if (width <= 0 || height <= 0)
            return;
            
        // Calculate the inner border offset (where the recessed area begins)
        int innerBorderOffset = BorderThickness;
        
        // Update the outer border lines
        for (int i = 0; i < _topOuterLines.Count && i < BorderThickness; i++)
        {
            if (i < _topOuterLines.Count)
            {
                // Top line - ends at (width-i-1,i)
                _topOuterLines[i].X2 = width - i - 1;
                
                // Left line - ends at (i,height-i-1)
                _leftOuterLines[i].Y2 = height - i - 1;
                
                // Bottom line - starts at (i,height-i-1) and ends at (width-i-1,height-i-1)
                _bottomOuterLines[i].Y1 = height - i - 1;
                _bottomOuterLines[i].X2 = width - i - 1;
                _bottomOuterLines[i].Y2 = height - i - 1;
                
                // Right line - starts at (width-i-1,i) and ends at (width-i-1,height-i-1)
                _rightOuterLines[i].X1 = width - i - 1;
                _rightOuterLines[i].X2 = width - i - 1;
                _rightOuterLines[i].Y2 = height - i - 1;
            }
        }
        
        // Update the inner border lines
        for (int i = 0; i < _topInnerLines.Count && i < BorderThickness; i++)
        {
            if (i < _topInnerLines.Count)
            {
                int offset = innerBorderOffset + i;
                
                // Top inner line - ends at (width-offset-1,offset)
                _topInnerLines[i].X2 = width - offset - 1;
                
                // Left inner line - ends at (offset,height-offset-1)
                _leftInnerLines[i].Y2 = height - offset - 1;
                
                // Bottom inner line - starts at (offset,height-offset-1) and ends at (width-offset-1,height-offset-1)
                _bottomInnerLines[i].Y1 = height - offset - 1;
                _bottomInnerLines[i].X2 = width - offset - 1;
                _bottomInnerLines[i].Y2 = height - offset - 1;
                
                // Right inner line - starts at (width-offset-1,offset) and ends at (width-offset-1,height-offset-1)
                _rightInnerLines[i].X1 = width - offset - 1;
                _rightInnerLines[i].X2 = width - offset - 1;
                _rightInnerLines[i].Y2 = height - offset - 1;
            }
        }
    }

    /// <summary>
    /// Updates the display based on the current property values.
    /// </summary>
    private void UpdateDisplay()
    {
        // Clear the container
        _digitContainer.Children.Clear();

        // Convert the value to a string
        string valueStr = Value.ToString();

        // If the value is larger than MaxDigitsToDisplay, truncate from the left
        if (valueStr.Length > MaxDigitsToDisplay)
        {
            valueStr = valueStr.Substring(valueStr.Length - MaxDigitsToDisplay);
        }
        else if (valueStr.Length < MaxDigitsToDisplay)
        {
            // Pad with zeros or spaces as specified
            char paddingChar = PadWithZeros ? '0' : ' ';
            valueStr = valueStr.PadLeft(MaxDigitsToDisplay, paddingChar);
        }

        // Base size for a single digit
        const double baseWidth = 30;
        const double baseHeight = 50;
        
        // Calculate the size of each digit based on the scale factor
        double digitWidth = baseWidth * SizeScale;
        double digitHeight = baseHeight * SizeScale;

        // Add an image for each digit
        foreach (char digit in valueStr)
        {
            string imageName = GetImageNameForDigit(digit);
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
    /// Gets the image name for a digit.
    /// </summary>
    private string GetImageNameForDigit(char digit)
    {
        if (digit >= '0' && digit <= '9')
        {
            return $"digit_{digit}_image.png";
        }
        else
        {
            // For spaces or any other character, use the blank image
            return "digit_blank_image.png";
        }
    }
}
