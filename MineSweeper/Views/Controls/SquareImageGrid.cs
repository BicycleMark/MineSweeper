using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Graphics;

namespace MineSweeper.Views.Controls;

/// <summary>
/// A square grid control that displays a grid of images.
/// The grid will always maintain a square aspect ratio and
/// will fill the maximum square area possible within its container.
/// </summary>
public class SquareImageGrid : ContentView
{
    /// <summary>
    /// Bindable property for the grid size (number of rows and columns).
    /// </summary>
    public static readonly BindableProperty GridSizeProperty = BindableProperty.Create(
        nameof(GridSize),
        typeof(int),
        typeof(SquareImageGrid),
        defaultValue: 5,
        propertyChanged: OnGridSizeChanged);
        
    /// <summary>
    /// Bindable property for the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
        nameof(HighlightColor),
        typeof(Color),
        typeof(SquareImageGrid),
        defaultValue: Colors.LightGray,
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor),
        typeof(Color),
        typeof(SquareImageGrid),
        defaultValue: Colors.DimGray,
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for whether to show the 3D border effect.
    /// </summary>
    public static readonly BindableProperty ShowBorderProperty = BindableProperty.Create(
        nameof(ShowBorder),
        typeof(bool),
        typeof(SquareImageGrid),
        defaultValue: true,
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for the thickness of the border.
    /// </summary>
    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(int),
        typeof(SquareImageGrid),
        defaultValue: 6,
        propertyChanged: OnBorderPropertyChanged);
        
    /// <summary>
    /// Bindable property for whether the border appears recessed (true) or raised (false).
    /// </summary>
    public static readonly BindableProperty IsRecessedProperty = BindableProperty.Create(
        nameof(IsRecessed),
        typeof(bool),
        typeof(SquareImageGrid),
        defaultValue: true,
        propertyChanged: OnBorderPropertyChanged);
    
    /// <summary>
    /// Gets or sets the size of the grid (number of rows and columns).
    /// </summary>
    public int GridSize
    {
        get => (int)GetValue(GridSizeProperty);
        set => SetValue(GridSizeProperty, value);
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
    /// Gets or sets the thickness of the border.
    /// </summary>
    public int BorderThickness
    {
        get => (int)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }
    
    /// <summary>
    /// Gets or sets whether the border appears recessed (true) or raised (false).
    /// </summary>
    public bool IsRecessed
    {
        get => (bool)GetValue(IsRecessedProperty);
        set => SetValue(IsRecessedProperty, value);
    }
    
    /// <summary>
    /// Called when the GridSize property changes.
    /// </summary>
    private static void OnGridSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SquareImageGrid grid && newValue is int size)
        {
            // Create a new grid with the specified number of rows and columns
            grid.CreateGrid(size, size);
        }
    }
    
    /// <summary>
    /// Called when any of the border properties change.
    /// </summary>
    private static void OnBorderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SquareImageGrid grid)
        {
            // Update the border appearance
            grid.UpdateBorderAppearance();
        }
    }
    
    private readonly Grid _grid;
    private readonly AbsoluteLayout _container;
    private readonly ChiseledBorder _border;
    private View[,]? _images;
    
    /// <summary>
    /// Gets or sets the number of rows in the grid.
    /// </summary>
    public int Rows { get; private set; }
    
    /// <summary>
    /// Gets or sets the number of columns in the grid.
    /// </summary>
    public int Columns { get; private set; }
    
    /// <summary>
    /// Indexer to get or set the image at a specific row and column.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="col">The column index.</param>
    /// <returns>The image at the specified position.</returns>
    public View this[int row, int col]
    {
        get
        {
            ValidateIndices(row, col);
            if (_images == null)
            {
                throw new InvalidOperationException("Grid has not been initialized yet.");
            }
            return _images[row, col];
        }
        set
        {
            ValidateIndices(row, col);
            if (_images == null)
            {
                throw new InvalidOperationException("Grid has not been initialized yet.");
            }
            _images[row, col] = value;
            UpdateCellImage(row, col, value);
        }
    }
    
    /// <summary>
    /// Event triggered to get the image for a specific cell at [row, column].
    /// </summary>
    public event EventHandler<GetCellImageEventArgs>? GetCellImage;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SquareImageGrid"/> class.
    /// </summary>
    public SquareImageGrid()
    {
        // Create the internal grid
        _grid = new Grid
        {
            BackgroundColor = Colors.Transparent,
            Padding = 0,
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        
        // Create a container to center the grid
        _container = new AbsoluteLayout
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        
        // Add the grid to the container
        AbsoluteLayout.SetLayoutBounds(_grid, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(_grid, AbsoluteLayoutFlags.All);
        _container.Children.Add(_grid);
        
        // Create the chiseled border with the container as content
        _border = new ChiseledBorder
        {
            ShadowColor = ShadowColor,
            HighlightColor = HighlightColor,
            BorderThickness = BorderThickness,
            ContentBackgroundColor = Colors.Transparent,
            IsRecessed = IsRecessed,
            Content = _container,
            IsVisible = ShowBorder,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        
        // Set the content to the border
        Content = _border;
        
        // Set default properties
        BackgroundColor = Colors.Transparent;
        Padding = 0;
        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;
        
        // Create a default 5x5 grid
        CreateGrid(5, 5);
        
        // Add event handlers
        SizeChanged += OnSizeChanged;
        Loaded += OnLoaded;
    }
    
    /// <summary>
    /// Updates the border appearance based on the current property values.
    /// </summary>
    private void UpdateBorderAppearance()
    {
        if (_border == null)
            return;
            
        // Update the border properties
        _border.ShadowColor = ShadowColor;
        _border.HighlightColor = HighlightColor;
        _border.BorderThickness = BorderThickness;
        _border.IsRecessed = IsRecessed;
        _border.IsVisible = ShowBorder;
    }
    
    /// <summary>
    /// Validates that the row and column indices are within bounds.
    /// </summary>
    /// <param name="row">The row index to validate.</param>
    /// <param name="col">The column index to validate.</param>
    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= Rows)
            throw new ArgumentOutOfRangeException(nameof(row), $"Row index must be between 0 and {Rows - 1}");
        if (col < 0 || col >= Columns)
            throw new ArgumentOutOfRangeException(nameof(col), $"Column index must be between 0 and {Columns - 1}");
    }
    
    /// <summary>
    /// Updates the image for a specific cell in the grid.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="image">The new image for the cell.</param>
    private void UpdateCellImage(int row, int col, View image)
    {
        // Find the existing cell in the grid
        var existingCell = GetCellFromGrid(row, col);
        if (existingCell != null)
        {
            // Remove the existing cell
            _grid.Children.Remove(existingCell);
        }
        
        // Prepare the new image
        if (image != null)
        {
            image.Margin = new Thickness(1);
            image.HorizontalOptions = LayoutOptions.Fill;
            image.VerticalOptions = LayoutOptions.Fill;
            
            // Set the cell's position in the grid
            Grid.SetRow(image, row);
            Grid.SetColumn(image, col);
            
            // Add the new image to the grid
            _grid.Children.Add(image);
        }
        else
        {
            // If no image is provided, create a default cell
            var defaultCell = new Rectangle
            {
                Stroke = Colors.Black,
                StrokeThickness = 1,
                Fill = Colors.Transparent,
                Margin = new Thickness(1),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
            
            // Set the cell's position in the grid
            Grid.SetRow(defaultCell, row);
            Grid.SetColumn(defaultCell, col);
            
            // Add the default cell to the grid
            _grid.Children.Add(defaultCell);
            
            // Update the images array
            if (_images != null)
            {
                _images[row, col] = defaultCell;
            }
        }
    }
    
    /// <summary>
    /// Gets the cell at the specified row and column from the grid.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="col">The column index.</param>
    /// <returns>The cell at the specified position, or null if not found.</returns>
    private View? GetCellFromGrid(int row, int col)
    {
        // Since we're maintaining our own array of images, we can just return the image from there
        if (row >= 0 && row < Rows && col >= 0 && col < Columns && _images != null)
        {
            return _images[row, col];
        }
        return null;
    }
    
    /// <summary>
    /// Handles the Loaded event.
    /// </summary>
    private void OnLoaded(object? sender, EventArgs e)
    {
        // Ensure the grid is properly sized when it's first loaded
        UpdateLayout();
    }
    
    /// <summary>
    /// Handles the SizeChanged event.
    /// </summary>
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        // Update the layout when the size changes
        UpdateLayout();
    }
    
    /// <summary>
    /// Updates the layout to ensure the grid is square and centered, with tighter spacing to the border.
    /// </summary>
    private void UpdateLayout()
    {
        // Get the available width and height
        double availableWidth = Width;
        double availableHeight = Height;
        
        // If we have valid dimensions, update the layout
        if (availableWidth > 0 && availableHeight > 0)
        {
            // Use the minimum dimension to ensure the grid is square
            double size = Math.Min(availableWidth, availableHeight);
            
            // Adjust the size to be closer to the border (95% of available space)
            // This makes the grid take up more space, bringing it closer to the border
            double adjustedSize = size * 0.95;
            
            // Center the grid using proportional positioning
            AbsoluteLayout.SetLayoutBounds(_grid, new Rect(0.5, 0.5, adjustedSize, adjustedSize));
            AbsoluteLayout.SetLayoutFlags(_grid, AbsoluteLayoutFlags.PositionProportional);
            
            // Log the size for debugging
            System.Diagnostics.Debug.WriteLine($"SquareImageGrid: Setting size to {adjustedSize}x{adjustedSize} centered proportionally");
        }
        else
        {
            // Log that we don't have valid dimensions yet
            System.Diagnostics.Debug.WriteLine($"SquareImageGrid: Invalid dimensions - Width: {availableWidth}, Height: {availableHeight}");
        }
    }
    
    /// <summary>
    /// Creates the grid with the specified number of rows and columns.
    /// </summary>
    /// <param name="rows">The number of rows.</param>
    /// <param name="columns">The number of columns.</param>
    public void CreateGrid(int rows, int columns)
    {
        // Store the dimensions
        Rows = rows;
        Columns = columns;

        // Initialize the images array
        _images = new View[rows, columns];

        // Clear the grid
        _grid.Children.Clear();
        _grid.RowDefinitions.Clear();
        _grid.ColumnDefinitions.Clear();

        // Add row and column definitions
        for (int i = 0; i < rows; i++)
        {
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }

        for (int j = 0; j < columns; j++)
        {
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // Add cells to the grid
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Create event args for this cell
                var args = new GetCellImageEventArgs(i, j);
            
                // Raise the event to get the image
                GetCellImage?.Invoke(this, args);
            
                // Use the provided image or create a default one
                View cell = args.Image;
                if (cell != null)
                {
                    // If an image is provided, use it
                    cell.Margin = new Thickness(1);
                }
                else
                {
                    // Default cell if no image is provided
                    cell = new Rectangle
                    {
                        Stroke = Colors.Black,
                        StrokeThickness = 1,
                        Fill = Colors.Transparent,
                        Margin = new Thickness(1)
                    };
                }

                // Store the cell in the images array
                _images[i, j] = cell;

                cell.HorizontalOptions = LayoutOptions.Fill;
                cell.VerticalOptions = LayoutOptions.Fill;
            
                // Set the cell's position in the grid
                Grid.SetRow(cell, i);
                Grid.SetColumn(cell, j);

                // Add the cell to the grid
                _grid.Children.Add(cell);
            }
        }

        // Update the layout
        UpdateLayout();
    }
    
    /// <summary>
    /// Override the OnParentSet method to update the layout when the parent changes.
    /// </summary>
    protected override void OnParentSet()
    {
        base.OnParentSet();
        
        // Update the layout when the parent changes
        UpdateLayout();
    }
}
