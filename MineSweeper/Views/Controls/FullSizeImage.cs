using Microsoft.Maui.Controls.Shapes;

namespace MineSweeper.Views.Controls;

public class FullSizeImage : ContentView
{
    public Point Point { set; get; }
    
    private readonly Image _image;
    
    public FullSizeImage()
    {
        // Create a solid rectangle that fills the entire space
        var background = new Rectangle
        {
            Fill = new SolidColorBrush(Colors.White),
            StrokeThickness = 0
        };
        
        // Create the image 
        _image = new Image
        {
            Aspect = Aspect.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        
        // Create a Grid to hold both
        var grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Padding = 0,
            Margin = 0
        };
        
        grid.Children.Add(background);
        grid.Children.Add(_image);
        
        // Set the grid as the content
        Content = grid;
        
        // Make sure the ContentView itself fills its container
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;
        Padding = 0;
        Margin = 0;
    }
    
    // Add a Source property that passes through to the Image
    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source), 
        typeof(ImageSource), 
        typeof(FullSizeImage),
        propertyChanged: OnSourceChanged);
        
    public ImageSource Source
    {
        get => (ImageSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }
    
    private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is FullSizeImage control && newValue is ImageSource source)
        {
            control._image.Source = source;
        }
    }
}
