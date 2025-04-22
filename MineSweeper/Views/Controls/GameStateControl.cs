using System.Windows.Input;
using MineSweeper.Models;

namespace MineSweeper.Views.Controls;

/// <summary>
///     A control that displays a smiley face representing the current game state.
///     The face changes based on the game status and user interactions.
/// </summary>
public class GameStateControl : ContentView
{
    /// <summary>
    ///     Bindable property for the game status.
    /// </summary>
    public static readonly BindableProperty StatusProperty = BindableProperty.Create(
        nameof(Status),
        typeof(GameEnums.GameStatus),
        typeof(GameStateControl),
        GameEnums.GameStatus.NotStarted,
        propertyChanged: OnStatusChanged);

    /// <summary>
    ///     Bindable property for the size scale factor of the smiley face.
    /// </summary>
    public static readonly BindableProperty SizeScaleProperty = BindableProperty.Create(
        nameof(SizeScale),
        typeof(double),
        typeof(GameStateControl),
        1.0,
        propertyChanged: OnSizeScaleChanged);

    /// <summary>
    ///     Bindable property for whether to show the 3D border effect.
    /// </summary>
    public static readonly BindableProperty ShowBorderProperty = BindableProperty.Create(
        nameof(ShowBorder),
        typeof(bool),
        typeof(GameStateControl),
        true,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the thickness of the border.
    /// </summary>
    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(int),
        typeof(GameStateControl),
        2,
        propertyChanged: OnBorderThicknessChanged);

    /// <summary>
    ///     Bindable property for the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor),
        typeof(Color),
        typeof(GameStateControl),
        Colors.DimGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
        nameof(HighlightColor),
        typeof(Color),
        typeof(GameStateControl),
        Colors.LightGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the background color of the display.
    /// </summary>
    public static readonly BindableProperty DisplayBackgroundColorProperty = BindableProperty.Create(
        nameof(DisplayBackgroundColor),
        typeof(Color),
        typeof(GameStateControl),
        Color.FromArgb("#222222"),
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the command to execute when the control is tapped.
    /// </summary>
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(GameStateControl),
        null);

    /// <summary>
    ///     Bindable property for the command parameter.
    /// </summary>
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter),
        typeof(object),
        typeof(GameStateControl),
        null);

    private readonly ChiseledBorder _border;

    private readonly Image _smileyImage;
    private bool _isPressed;
    private GameEnums.GameStatus _previousStatus;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameStateControl" /> class.
    /// </summary>
    public GameStateControl()
    {
        // Create the smiley image
        _smileyImage = new Image
        {
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        // Create the chiseled border with the smiley image as content
        _border = new ChiseledBorder
        {
            ShadowColor = ShadowColor,
            HighlightColor = HighlightColor,
            BorderThickness = BorderThickness,
            ContentBackgroundColor = DisplayBackgroundColor,
            IsRecessed = true,
            Content = _smileyImage,
            IsVisible = ShowBorder,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Padding = 5
        };

        // Set the content to the border
        Content = _border;

        // Set default properties
        BackgroundColor = Colors.Transparent;
        Padding = 0;
        HorizontalOptions = LayoutOptions.Center;
        VerticalOptions = LayoutOptions.Center;

        // Add tap gesture recognizers for press/release events
        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += OnTapped;
        GestureRecognizers.Add(tapGestureRecognizer);

        // Add press/release gesture recognizers
        var pressedGestureRecognizer = new PressedGestureRecognizer();
        pressedGestureRecognizer.Pressed += OnPressed;
        GestureRecognizers.Add(pressedGestureRecognizer);

        var releasedGestureRecognizer = new ReleasedGestureRecognizer();
        releasedGestureRecognizer.Released += OnReleased;
        GestureRecognizers.Add(releasedGestureRecognizer);

        // Update the size based on the scale factor
        UpdateSize();

        // Update the border appearance
        UpdateBorderAppearance();

        // Update the display
        UpdateDisplay();
    }

    /// <summary>
    ///     Gets or sets the game status.
    /// </summary>
    public GameEnums.GameStatus Status
    {
        get => (GameEnums.GameStatus) GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    ///     Gets or sets the size scale factor of the smiley face.
    ///     This allows changing the size without affecting the aspect ratio.
    /// </summary>
    public double SizeScale
    {
        get => (double) GetValue(SizeScaleProperty);
        set => SetValue(SizeScaleProperty, value);
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
    ///     Gets or sets the thickness of the border.
    /// </summary>
    public int BorderThickness
    {
        get => (int) GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
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
    ///     Gets or sets the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public Color HighlightColor
    {
        get => (Color) GetValue(HighlightColorProperty);
        set => SetValue(HighlightColorProperty, value);
    }

    /// <summary>
    ///     Gets or sets the background color of the display.
    /// </summary>
    public Color DisplayBackgroundColor
    {
        get => (Color) GetValue(DisplayBackgroundColorProperty);
        set => SetValue(DisplayBackgroundColorProperty, value);
    }

    /// <summary>
    ///     Gets or sets the command to execute when the control is tapped.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    ///     Gets or sets the command parameter.
    /// </summary>
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    ///     Called when the Status property changes.
    /// </summary>
    /// <param name="bindable">The GameStateControl instance whose property changed.</param>
    /// <param name="oldValue">The previous status value.</param>
    /// <param name="newValue">The new status value.</param>
    private static void OnStatusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GameStateControl control) control.UpdateDisplay();
    }

    /// <summary>
    ///     Called when the size scale property changes.
    /// </summary>
    /// <param name="bindable">The GameStateControl instance whose property changed.</param>
    /// <param name="oldValue">The previous scale factor value.</param>
    /// <param name="newValue">The new scale factor value.</param>
    private static void OnSizeScaleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GameStateControl control) control.UpdateSize();
    }

    /// <summary>
    ///     Called when any of the border properties change.
    /// </summary>
    /// <param name="bindable">The GameStateControl instance whose property changed.</param>
    /// <param name="oldValue">The previous property value.</param>
    /// <param name="newValue">The new property value.</param>
    private static void OnBorderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GameStateControl control)
            // For other border properties, just update the appearance
            control.UpdateBorderAppearance();
    }

    /// <summary>
    ///     Called when the BorderThickness property changes.
    /// </summary>
    /// <param name="bindable">The GameStateControl instance whose property changed.</param>
    /// <param name="oldValue">The previous border thickness value.</param>
    /// <param name="newValue">The new border thickness value.</param>
    private static void OnBorderThicknessChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is GameStateControl control)
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
    ///     Updates the size of the control based on the scale factor.
    /// </summary>
    private void UpdateSize()
    {
        // Base size for the smiley face (these are reasonable defaults that maintain aspect ratio)
        const double baseWidth = 40;
        const double baseHeight = 40;

        // Calculate the size based on the scale factor
        var width = baseWidth * SizeScale;
        var height = baseHeight * SizeScale;

        // Calculate the total size including border and padding
        var totalWidth = width + BorderThickness * 2 + 10; // Add padding and border thickness
        var totalHeight = height + BorderThickness * 2 + 10; // Add padding and border thickness

        // Explicitly set the size of the control and border to ensure it shrinks when scale decreases
        WidthRequest = totalWidth;
        HeightRequest = totalHeight;

        _border.WidthRequest = totalWidth;
        _border.HeightRequest = totalHeight;

        // Set the size of the smiley image
        _smileyImage.WidthRequest = width;
        _smileyImage.HeightRequest = height;

        // Update the border appearance since the size changed
        UpdateBorderAppearance();

        // Force a layout update
        InvalidateMeasure();
        InvalidateLayout();
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
    ///     Updates the display based on the current status.
    /// </summary>
    private void UpdateDisplay()
    {
        // Store the current status for press/release handling
        if (!_isPressed) _previousStatus = Status;

        // Update the image source based on the current status
        var imageName = GetImageNameForStatus(Status);
        _smileyImage.Source = ImageSource.FromFile(imageName);
    }

    /// <summary>
    ///     Handles the tap event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments containing tap information.</param>
    private void OnTapped(object? sender, TappedEventArgs e)
    {
        // For tap events, we'll just reset to the previous status
        // This ensures that if the user taps quickly, the face will still reset
        if (_isPressed)
        {
            _isPressed = false;
            Status = _previousStatus;
        }

        // Execute the command if one is set
        if (Command != null && Command.CanExecute(CommandParameter)) Command.Execute(CommandParameter);
    }

    /// <summary>
    ///     Handles the pressed event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnPressed(object? sender, EventArgs e)
    {
        // Only change to pressed state if the game is not over
        if (Status != GameEnums.GameStatus.Won && Status != GameEnums.GameStatus.Lost)
        {
            _isPressed = true;
            _previousStatus = Status;
            Status = GameEnums.GameStatus.InPress;
        }
    }

    /// <summary>
    ///     Handles the released event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnReleased(object? sender, EventArgs e)
    {
        // Only reset if we're in the pressed state
        if (_isPressed)
        {
            _isPressed = false;
            Status = _previousStatus;
        }
    }

    /// <summary>
    ///     Gets the image name for a specific game status.
    /// </summary>
    /// <param name="status">The game status to get the image name for.</param>
    /// <returns>The filename of the image that corresponds to the given status.</returns>
    private string GetImageNameForStatus(GameEnums.GameStatus status)
    {
        return status switch
        {
            GameEnums.GameStatus.Won => "smiley_cool.png",
            GameEnums.GameStatus.Lost => "smiley_sad.png",
            GameEnums.GameStatus.InPress => "smiley_tense.png",
            _ => "smiley_normal.png" // Default for NotStarted and InProgress
        };
    }
}

/// <summary>
///     A gesture recognizer for press events.
/// </summary>
public class PressedGestureRecognizer : Element, IGestureRecognizer
{
    /// <summary>
    ///     Event that is triggered when the element is pressed.
    /// </summary>
    public event EventHandler? Pressed;

    /// <summary>
    ///     Raises the Pressed event.
    /// </summary>
    public void SendPressed()
    {
        Pressed?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
///     A gesture recognizer for release events.
/// </summary>
public class ReleasedGestureRecognizer : Element, IGestureRecognizer
{
    /// <summary>
    ///     Event that is triggered when the element is released.
    /// </summary>
    public event EventHandler? Released;

    /// <summary>
    ///     Raises the Released event.
    /// </summary>
    public void SendReleased()
    {
        Released?.Invoke(this, EventArgs.Empty);
    }
}