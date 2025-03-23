using Microsoft.Maui.Controls.Shapes;

namespace MineSweeper.Views.Controls;

public class ImageBrushCell : ContentView
{
    public Point Point { get; set; }
    
    private readonly Rectangle _background;
    private readonly Image _foreground;
    
    public ImageBrushCell()
    {
        // Set ContentView properties to ensure it fills its container
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;
        Padding = 0;
        Margin = 0;
        
        // Create a grid to hold both the rectangle and image
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Padding = 0,
            Margin = 0
        };
        
        // Create the rectangle that provides the seamless background
        _background = new Rectangle
        {
            StrokeThickness = 2,
            Fill = new SolidColorBrush(Colors.LightBlue), // Keep the same color for continuity
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        
        // Create the image that will display on top of the background
        _foreground = new Image
        {
            Aspect = Aspect.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(-1, -4, -1, -4), // Even more negative margin vertically
            Scale = 1.08, // Increase scale further to cover gaps completely
            InputTransparent = true // So the touch events pass through to the parent
        };
        
        // Add both elements to the grid
        grid.Children.Add(_background);
        grid.Children.Add(_foreground);
        
        // Set the grid as the content
        Content = grid;
    }
    
    // Add a Source property that updates the image source
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source), 
        typeof(string), 
        typeof(ImageBrushCell),
        propertyChanged: OnSourceChanged);
        
    public string Source
    {
        get => (string)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ImageBrushCell control && newValue is string source)
        {
            // Set the image source
            control._foreground.Source = ImageSource.FromFile(source);
        }
    }
}
