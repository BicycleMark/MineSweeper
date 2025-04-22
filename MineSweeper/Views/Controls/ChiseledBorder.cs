namespace MineSweeper.Views.Controls;

/// <summary>
///     A reusable control that provides a 3D chiseled border effect around its content.
/// </summary>
public class ChiseledBorder : ContentView
{
    /// <summary>
    ///     Bindable property for the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor),
        typeof(Color),
        typeof(ChiseledBorder),
        Colors.DimGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
        nameof(HighlightColor),
        typeof(Color),
        typeof(ChiseledBorder),
        Colors.LightGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the thickness of the border.
    /// </summary>
    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(int),
        typeof(ChiseledBorder),
        6,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the background color of the content area.
    /// </summary>
    public static readonly BindableProperty ContentBackgroundColorProperty = BindableProperty.Create(
        nameof(ContentBackgroundColor),
        typeof(Color),
        typeof(ChiseledBorder),
        Colors.Black,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for whether the border appears recessed (true) or raised (false).
    /// </summary>
    public static readonly BindableProperty IsRecessedProperty = BindableProperty.Create(
        nameof(IsRecessed),
        typeof(bool),
        typeof(ChiseledBorder),
        true,
        propertyChanged: OnBorderPropertyChanged);

    private readonly BoxView _background;
    private readonly ChiseledBorderDrawable _borderDrawable;

    private readonly GraphicsView _borderGraphics;
    private readonly Grid _container;
    private View _userContent;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChiseledBorder" /> class.
    /// </summary>
    public ChiseledBorder()
    {
        // Create the border drawable
        _borderDrawable = new ChiseledBorderDrawable
        {
            ShadowColor = ShadowColor,
            HighlightColor = HighlightColor,
            BorderThickness = BorderThickness,
            IsRecessed = IsRecessed
        };

        // Create the graphics view for the border
        _borderGraphics = new GraphicsView
        {
            Drawable = _borderDrawable,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.Transparent
        };

        // Create the background
        _background = new BoxView
        {
            BackgroundColor = ContentBackgroundColor,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        // Create the container grid
        _container = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Padding = 0,
            RowSpacing = 0,
            ColumnSpacing = 0
        };

        // Add elements to the grid
        _container.Add(_borderGraphics); // Bottom layer (border)
        _container.Add(_background); // Middle layer (background)

        // Set the content
        base.Content = _container;

        // Set default properties
        BackgroundColor = Colors.Transparent;
        Padding = 0;
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;

        // Update appearance
        UpdateBorderAppearance();

        // Add a handler for the SizeChanged event to ensure the border is redrawn when the size changes
        SizeChanged += OnSizeChanged;
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
    ///     Gets or sets the thickness of the border.
    /// </summary>
    public int BorderThickness
    {
        get => (int) GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    /// <summary>
    ///     Gets or sets the background color of the content area.
    /// </summary>
    public Color ContentBackgroundColor
    {
        get => (Color) GetValue(ContentBackgroundColorProperty);
        set => SetValue(ContentBackgroundColorProperty, value);
    }

    /// <summary>
    ///     Gets or sets whether the border appears recessed (true) or raised (false).
    /// </summary>
    public bool IsRecessed
    {
        get => (bool) GetValue(IsRecessedProperty);
        set => SetValue(IsRecessedProperty, value);
    }

    /// <summary>
    ///     Gets or sets the content of the border.
    /// </summary>
    public new View Content
    {
        get => _userContent;
        set
        {
            _userContent = value;

            if (_container != null && value != null)
            {
                // If there's already a content view (not the container itself), remove it
                var existingContent = _container.Children.LastOrDefault();
                if (existingContent != null && existingContent != _background && existingContent != _borderGraphics)
                    _container.Remove(existingContent);

                // Add the new content on top
                _container.Add(value);

                // Configure the content margin
                value.Margin = new Thickness(BorderThickness);
            }
        }
    }

    /// <summary>
    ///     Handles the SizeChanged event.
    /// </summary>
    private void OnSizeChanged(object sender, EventArgs e)
    {
        // Force a redraw of the border when the size changes
        _borderGraphics.Invalidate();
    }

    /// <summary>
    ///     Called when any of the border properties change.
    /// </summary>
    private static void OnBorderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ChiseledBorder border) border.UpdateBorderAppearance();
    }

    /// <summary>
    ///     Updates the border appearance based on the current property values.
    /// </summary>
    private void UpdateBorderAppearance()
    {
        if (_borderDrawable == null || _background == null)
            return;

        _borderDrawable.ShadowColor = ShadowColor;
        _borderDrawable.HighlightColor = HighlightColor;
        _borderDrawable.BorderThickness = BorderThickness;
        _borderDrawable.IsRecessed = IsRecessed;

        _background.BackgroundColor = ContentBackgroundColor;
        _background.Margin = new Thickness(BorderThickness);

        // If we have content, update its margin too
        if (_userContent != null) _userContent.Margin = new Thickness(BorderThickness);

        // Request redraw
        _borderGraphics.Invalidate();

        // Force a layout update
        InvalidateMeasure();
        InvalidateLayout();
    }
}