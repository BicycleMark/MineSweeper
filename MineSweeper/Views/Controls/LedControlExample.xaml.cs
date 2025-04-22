namespace MineSweeper.Views.Controls;

public partial class LedControlExample : ContentPage
{
    private readonly Random _random = new();

    public LedControlExample()
    {
        InitializeComponent();

        // Initialize the color buttons with the correct colors
        backgroundColorButton.Text = "#222222";
        backgroundColorButton.BackgroundColor = Color.FromArgb("#222222");
        backgroundColorButton.TextColor = Colors.White;

        highlightColorButton.Text = "#CCCCCC";
        highlightColorButton.BackgroundColor = Color.FromArgb("#CCCCCC");
        highlightColorButton.TextColor = Colors.Black;

        shadowColorButton.Text = "#444444";
        shadowColorButton.BackgroundColor = Color.FromArgb("#444444");
        shadowColorButton.TextColor = Colors.White;

        // Initialize the borders around the active color selections
        UpdateBackgroundColorPickerBorders("#222222");
        UpdateHighlightColorPickerBorders("#CCCCCC");
        UpdateShadowColorPickerBorders("#444444");
    }

    /// <summary>
    ///     Override the OnAppearing method to ensure the back button is visible
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Ensure the navigation bar is visible
        Shell.SetNavBarIsVisible(this, true);

        // Set up the back button behavior
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            Command = new Command(async () => await Shell.Current.GoToAsync("..")),
            IsVisible = true,
            IsEnabled = true
        });
    }

    /// <summary>
    ///     Handles the value slider value changed event.
    /// </summary>
    private void OnValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Get the new value
        var value = (int) Math.Round(e.NewValue);

        // Update the LED display
        ledDisplay.Value = value;

        // Update the label
        valueLabel.Text = value.ToString();

        // Update the status label
        statusLabel.Text = $"Value changed to {value}";
    }

    /// <summary>
    ///     Handles the max digits slider value changed event.
    /// </summary>
    private void OnMaxDigitsChanged(object sender, ValueChangedEventArgs e)
    {
        // Get the new max digits
        var maxDigits = (int) Math.Round(e.NewValue);

        // Update the LED display
        ledDisplay.MaxDigitsToDisplay = maxDigits;

        // Update the label
        maxDigitsLabel.Text = maxDigits.ToString();

        // Update the status label
        statusLabel.Text = $"Max digits changed to {maxDigits}";
    }

    /// <summary>
    ///     Handles the size scale slider value changed event.
    /// </summary>
    private void OnSizeScaleChanged(object sender, ValueChangedEventArgs e)
    {
        // Get the new size scale (round to 1 decimal place for display)
        var sizeScale = Math.Round(e.NewValue, 1);

        // Update the LED display
        ledDisplay.SizeScale = sizeScale;

        // Update the label
        sizeScaleLabel.Text = sizeScale.ToString("F1");

        // Update the status label
        statusLabel.Text = $"Size scale changed to {sizeScale:F1}";
    }

    /// <summary>
    ///     Handles the border thickness slider value changed event.
    /// </summary>
    private void OnBorderThicknessChanged(object sender, ValueChangedEventArgs e)
    {
        // Get the new border thickness (round to nearest integer)
        var borderThickness = (int) Math.Round(e.NewValue);

        // Update the LED display
        ledDisplay.BorderThickness = borderThickness;

        // Update the label
        borderThicknessLabel.Text = borderThickness.ToString();

        // Update the status label
        statusLabel.Text = $"Border thickness changed to {borderThickness}";
    }

    /// <summary>
    ///     Handles the pad with zeros switch toggled event.
    /// </summary>
    private void OnPadWithZerosToggled(object sender, ToggledEventArgs e)
    {
        // Update the LED display
        ledDisplay.PadWithZeros = e.Value;

        // Update the status label
        statusLabel.Text = $"Pad with zeros set to {e.Value}";
    }

    /// <summary>
    ///     Handles the show border switch toggled event.
    /// </summary>
    private void OnShowBorderToggled(object sender, ToggledEventArgs e)
    {
        // Update the LED display
        ledDisplay.ShowBorder = e.Value;

        // Update the status label
        statusLabel.Text = $"Show border set to {e.Value}";
    }

    /// <summary>
    ///     Handles the background color button click event.
    /// </summary>
    private void OnBackgroundColorClicked(object sender, EventArgs e)
    {
        // Rotate through some common background colors
        Color[] colors = new[]
        {
            Color.FromArgb("#222222"), // Dark gray (default)
            Color.FromArgb("#000000"), // Black
            Color.FromArgb("#0A0A0A"), // Very dark gray
            Color.FromArgb("#1A1A1A"), // Dark gray
            Color.FromArgb("#333333") // Medium dark gray
        };

        // Find the current color in the array
        var currentIndex = -1;
        for (var i = 0; i < colors.Length; i++)
            if (ledDisplay.DisplayBackgroundColor.ToHex() == colors[i].ToHex())
            {
                currentIndex = i;
                break;
            }

        // Move to the next color
        var nextIndex = (currentIndex + 1) % colors.Length;
        var newColor = colors[nextIndex];

        // Update the LED display
        ledDisplay.DisplayBackgroundColor = newColor;

        // Update the button
        backgroundColorButton.Text = newColor.ToHex();
        backgroundColorButton.BackgroundColor = newColor;
        backgroundColorButton.TextColor = Colors.White; // Always white text for dark backgrounds

        // Update the borders around the active color selection
        UpdateBackgroundColorPickerBorders(newColor.ToHex());

        // Update the status label
        statusLabel.Text = $"Background color changed to {newColor.ToHex()}";
    }

    /// <summary>
    ///     Handles the highlight color button click event.
    /// </summary>
    private void OnHighlightColorClicked(object sender, EventArgs e)
    {
        // Rotate through some common highlight colors
        Color[] colors = new[]
        {
            Colors.LightGray, // Default
            Colors.White,
            Colors.Silver,
            Colors.LightBlue,
            Colors.LightGreen,
            Colors.LightYellow
        };

        // Find the current color in the array
        var currentIndex = -1;
        for (var i = 0; i < colors.Length; i++)
            if (ledDisplay.HighlightColor.ToHex() == colors[i].ToHex())
            {
                currentIndex = i;
                break;
            }

        // Move to the next color
        var nextIndex = (currentIndex + 1) % colors.Length;
        var newColor = colors[nextIndex];

        // Update the LED display
        ledDisplay.HighlightColor = newColor;

        // Update the button
        highlightColorButton.Text = newColor.ToHex();
        highlightColorButton.BackgroundColor = newColor;
        highlightColorButton.TextColor = Colors.Black; // Always black text for light backgrounds

        // Update the borders around the active color selection
        UpdateHighlightColorPickerBorders(newColor.ToHex());

        // Update the status label
        statusLabel.Text = $"Highlight color changed to {newColor.ToHex()}";
    }

    /// <summary>
    ///     Handles the shadow color button click event.
    /// </summary>
    private void OnShadowColorClicked(object sender, EventArgs e)
    {
        // Rotate through some common shadow colors
        Color[] colors = new[]
        {
            Colors.DimGray, // Default
            Colors.Black,
            Colors.DarkGray,
            Colors.DarkBlue,
            Colors.DarkGreen,
            Colors.DarkRed
        };

        // Find the current color in the array
        var currentIndex = -1;
        for (var i = 0; i < colors.Length; i++)
            if (ledDisplay.ShadowColor.ToHex() == colors[i].ToHex())
            {
                currentIndex = i;
                break;
            }

        // Move to the next color
        var nextIndex = (currentIndex + 1) % colors.Length;
        var newColor = colors[nextIndex];

        // Update the LED display
        ledDisplay.ShadowColor = newColor;

        // Update the button
        shadowColorButton.Text = newColor.ToHex();
        shadowColorButton.BackgroundColor = newColor;
        shadowColorButton.TextColor = Colors.White; // Always white text for dark backgrounds

        // Update the borders around the active color selection
        UpdateShadowColorPickerBorders(newColor.ToHex());

        // Update the status label
        statusLabel.Text = $"Shadow color changed to {newColor.ToHex()}";
    }

    /// <summary>
    ///     Event handler for the 0 button.
    /// </summary>
    private void OnValue0Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        valueSlider.Value = 0;
    }

    /// <summary>
    ///     Event handler for the 123 button.
    /// </summary>
    private void OnValue123Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        valueSlider.Value = 123;
    }

    /// <summary>
    ///     Event handler for the 9999 button.
    /// </summary>
    private void OnValue9999Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        valueSlider.Value = 9999;
    }

    /// <summary>
    ///     Event handler for the 12345 button.
    /// </summary>
    private void OnValue12345Clicked(object sender, EventArgs e)
    {
        // Update the slider value
        valueSlider.Value = 12345;
    }

    /// <summary>
    ///     Event handler for the Random button.
    /// </summary>
    private void OnRandomValueClicked(object sender, EventArgs e)
    {
        // Generate a random value between 0 and 99999
        var randomValue = _random.Next(100000);

        // Update the slider value
        valueSlider.Value = randomValue;
    }

    /// <summary>
    ///     Handles the background color picker button click event.
    /// </summary>
    private void OnBackgroundColorPickerClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string colorHex)
        {
            // Parse the color from the command parameter
            var newColor = Color.FromArgb(colorHex);

            // Update the LED display
            ledDisplay.DisplayBackgroundColor = newColor;

            // Update the main color button
            backgroundColorButton.Text = colorHex;
            backgroundColorButton.BackgroundColor = newColor;
            backgroundColorButton.TextColor = IsLightColor(newColor) ? Colors.Black : Colors.White;

            // Update the borders around the active color selection
            UpdateBackgroundColorPickerBorders(colorHex);

            // Update the status label
            statusLabel.Text = $"Background color changed to {colorHex}";
        }
    }

    /// <summary>
    ///     Handles the highlight color picker button click event.
    /// </summary>
    private void OnHighlightColorPickerClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string colorHex)
        {
            // Parse the color from the command parameter
            var newColor = Color.FromArgb(colorHex);

            // Update the LED display
            ledDisplay.HighlightColor = newColor;

            // Update the main color button
            highlightColorButton.Text = colorHex;
            highlightColorButton.BackgroundColor = newColor;
            highlightColorButton.TextColor = IsLightColor(newColor) ? Colors.Black : Colors.White;

            // Update the borders around the active color selection
            UpdateHighlightColorPickerBorders(colorHex);

            // Update the status label
            statusLabel.Text = $"Highlight color changed to {colorHex}";
        }
    }

    /// <summary>
    ///     Handles the shadow color picker button click event.
    /// </summary>
    private void OnShadowColorPickerClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string colorHex)
        {
            // Parse the color from the command parameter
            var newColor = Color.FromArgb(colorHex);

            // Update the LED display
            ledDisplay.ShadowColor = newColor;

            // Update the main color button
            shadowColorButton.Text = colorHex;
            shadowColorButton.BackgroundColor = newColor;
            shadowColorButton.TextColor = IsLightColor(newColor) ? Colors.Black : Colors.White;

            // Update the borders around the active color selection
            UpdateShadowColorPickerBorders(colorHex);

            // Update the status label
            statusLabel.Text = $"Shadow color changed to {colorHex}";
        }
    }

    /// <summary>
    ///     Determines if a color is light or dark to choose appropriate text color.
    /// </summary>
    private bool IsLightColor(Color color)
    {
        // Calculate the perceived brightness using the formula:
        // (0.299*R + 0.587*G + 0.114*B)
        var brightness = 0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue;

        // If brightness is greater than 0.5, it's a light color
        return brightness > 0.5;
    }

    /// <summary>
    ///     Updates the borders around the active background color selection.
    /// </summary>
    private void UpdateBackgroundColorPickerBorders(string activeColorHex)
    {
        // Loop through all buttons in the background color picker
        foreach (var child in backgroundColorPicker.Children)
            if (child is Button button)
            {
                // Check if this is the active color
                var isActive = button.CommandParameter is string colorHex && colorHex == activeColorHex;

                // Set the border for the active color, remove it for others
                button.BorderWidth = isActive ? 2 : 0;
                button.BorderColor = Colors.White;
                button.Margin = isActive ? new Thickness(0) : new Thickness(2);
            }
    }

    /// <summary>
    ///     Updates the borders around the active highlight color selection.
    /// </summary>
    private void UpdateHighlightColorPickerBorders(string activeColorHex)
    {
        // Loop through all buttons in the highlight color picker
        foreach (var child in highlightColorPicker.Children)
            if (child is Button button)
            {
                // Check if this is the active color
                var isActive = button.CommandParameter is string colorHex && colorHex == activeColorHex;

                // Set the border for the active color, remove it for others
                button.BorderWidth = isActive ? 2 : 0;
                button.BorderColor = Colors.Black;
                button.Margin = isActive ? new Thickness(0) : new Thickness(2);
            }
    }

    /// <summary>
    ///     Updates the borders around the active shadow color selection.
    /// </summary>
    private void UpdateShadowColorPickerBorders(string activeColorHex)
    {
        // Loop through all buttons in the shadow color picker
        foreach (var child in shadowColorPicker.Children)
            if (child is Button button)
            {
                // Check if this is the active color
                var isActive = button.CommandParameter is string colorHex && colorHex == activeColorHex;

                // Set the border for the active color, remove it for others
                button.BorderWidth = isActive ? 2 : 0;
                button.BorderColor = Colors.White;
                button.Margin = isActive ? new Thickness(0) : new Thickness(2);
            }
    }
}