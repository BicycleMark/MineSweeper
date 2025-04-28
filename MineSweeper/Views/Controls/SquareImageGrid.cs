using System.Diagnostics;
using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

namespace MineSweeper.Views.Controls;

/// <summary>
///     A square grid control that displays a grid of images.
///     The grid will always maintain a square aspect ratio and
///     will fill the maximum square area possible within its container.
/// </summary>
public class SquareImageGrid : ContentView, IAnimatableGrid
{
    /// <summary>
    ///     Bindable property for the grid size (number of rows and columns).
    /// </summary>
    public static readonly BindableProperty GridSizeProperty = BindableProperty.Create(
        nameof(GridSize),
        typeof(int),
        typeof(SquareImageGrid),
        5,
        propertyChanged: OnGridSizeChanged);

    /// <summary>
    ///     Bindable property for the highlight color of the 3D border (bottom and right edges for recessed look).
    /// </summary>
    public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
        nameof(HighlightColor),
        typeof(Color),
        typeof(SquareImageGrid),
        Colors.LightGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the shadow color of the 3D border (top and left edges for recessed look).
    /// </summary>
    public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
        nameof(ShadowColor),
        typeof(Color),
        typeof(SquareImageGrid),
        Colors.DimGray,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for whether to show the 3D border effect.
    /// </summary>
    public static readonly BindableProperty ShowBorderProperty = BindableProperty.Create(
        nameof(ShowBorder),
        typeof(bool),
        typeof(SquareImageGrid),
        true,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for the thickness of the border.
    /// </summary>
    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(int),
        typeof(SquareImageGrid),
        6,
        propertyChanged: OnBorderPropertyChanged);

    /// <summary>
    ///     Bindable property for whether the border appears recessed (true) or raised (false).
    /// </summary>
    public static readonly BindableProperty IsRecessedProperty = BindableProperty.Create(
        nameof(IsRecessed),
        typeof(bool),
        typeof(SquareImageGrid),
        true,
        propertyChanged: OnBorderPropertyChanged);
    
    /// <summary>
    /// Attached property for storing the row index on a gesture recognizer
    /// </summary>
    private static readonly BindableProperty RowProperty = BindableProperty.CreateAttached(
        "Row",
        typeof(int),
        typeof(SquareImageGrid),
        0);

    /// <summary>
    /// Attached property for storing the column index on a gesture recognizer
    /// </summary>
    private static readonly BindableProperty ColumnProperty = BindableProperty.CreateAttached(
        "Column",
        typeof(int),
        typeof(SquareImageGrid),
        0);
    
    /// <summary>
    /// Gets the row index from a bindable object
    /// </summary>
    private static int GetRow(BindableObject obj)
    {
        return (int)obj.GetValue(RowProperty);
    }
    
    /// <summary>
    /// Sets the row index on a bindable object
    /// </summary>
    private static void SetRow(BindableObject obj, int value)
    {
        obj.SetValue(RowProperty, value);
    }
    
    /// <summary>
    /// Gets the column index from a bindable object
    /// </summary>
    private static int GetColumn(BindableObject obj)
    {
        return (int)obj.GetValue(ColumnProperty);
    }
    
    /// <summary>
    /// Sets the column index on a bindable object
    /// </summary>
    private static void SetColumn(BindableObject obj, int value)
    {
        obj.SetValue(ColumnProperty, value);
    }
    
    private readonly ChiseledBorder _border;
    private readonly AbsoluteLayout _container;

    private readonly Grid _grid;
    private View[,]? _images;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SquareImageGrid" /> class.
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
        
        // We're not adding a grid-wide tap gesture recognizer anymore
        // Instead, we'll rely on the buttons added to each cell in CreateGrid
        
        Debug.WriteLine("Grid-wide tap gesture recognizer disabled to prevent whitespace clicks");
    }

    /// <summary>
    ///     Gets or sets the size of the grid (number of rows and columns).
    /// </summary>
    public int GridSize
    {
        get => (int) GetValue(GridSizeProperty);
        set => SetValue(GridSizeProperty, value);
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
    ///     Gets or sets the thickness of the border.
    /// </summary>
    public int BorderThickness
    {
        get => (int) GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
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
    ///     Indexer to get or set the image at a specific row and column.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="col">The column index.</param>
    /// <returns>The image at the specified position.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is outside valid range.</exception>
    /// <exception cref="InvalidOperationException">Thrown when grid has not been initialized.</exception>
    public View this[int row, int col]
    {
        get
        {
            ValidateIndices(row, col);
            if (_images == null) throw new InvalidOperationException("Grid has not been initialized yet.");

            return _images[row, col];
        }
        set
        {
            ValidateIndices(row, col);
            if (_images == null) throw new InvalidOperationException("Grid has not been initialized yet.");

            _images[row, col] = value;
            UpdateCellImage(row, col, value);
        }
    }

    /// <summary>
    ///     Gets the number of rows in the grid.
    /// </summary>
    public int Rows { get; private set; }

    /// <summary>
    ///     Gets the number of columns in the grid.
    /// </summary>
    public int Columns { get; private set; }

    /// <summary>
    ///     Event triggered to get the image for a specific cell at [row, column].
    /// </summary>
    public event EventHandler<GetCellImageEventArgs>? GetCellImage;

    /// <summary>
    ///     Event triggered when a tile is tapped.
    ///     Only fires for actual tiles, not for whitespace.
    /// </summary>
    /// <remarks>
    ///     The event provides row, column, the tapped view, and whether it's a default tile
    ///     through the <see cref="TileTappedEventArgs"/> parameter.
    /// </remarks>
    public event EventHandler<TileTappedEventArgs>? TileTapped;
    
    /// <summary>
    ///     Event triggered when a non-whitespace tile is tapped.
    ///     This event is guaranteed to only fire for actual game tiles, never for whitespace.
    /// </summary>
    public event EventHandler<TileTappedEventArgs>? GameTileTapped;

    /// <summary>
    ///     Event triggered when a non-whitespace tile is LongHold Tapped.
    ///     This event is guaranteed to only fire for actual game tiles, never for whitespace.
    /// </summary>
    public event EventHandler<TileTappedEventArgs>? GameTileLongHoldTapped;

    /// <summary>
    ///     Creates the grid with the specified number of rows and columns.
    ///     Clears any existing grid elements and regenerates the entire structure.
    /// </summary>
    /// <param name="rows">The number of rows to create.</param>
    /// <param name="columns">The number of columns to create.</param>
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
        for (var i = 0; i < rows; i++)
            _grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});

        for (var j = 0; j < columns; j++)
            _grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});

        //_grid.RowSpacing = 5;
        //_grid.ColumnSpacing = 5;
            // Add cells to the grid
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
            {
                // Create event args for this cell
                var args = new GetCellImageEventArgs(i, j);

                // Raise the event to get the image
                GetCellImage?.Invoke(this, args);

                // Use the provided image or create a default one
                var cell = args.Image;
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
                
                // Make the cell input transparent so clicks pass through to the grid
                cell.InputTransparent = true;

                // Set the cell's position in the grid
                Grid.SetRow(cell, i);
                Grid.SetColumn(cell, j);
                
                // Check if this is a default tile (whitespace)
                bool isDefaultTile = IsDefaultTile(cell);
                
                if (!isDefaultTile)
                {
                    // This is an actual tile, add a tap gesture recognizer directly to the image
                    var tapGesture = new TapGestureRecognizer();
                    
                    // Store the row and column as local variables for the closure
                    int capturedRow = i;
                    int capturedCol = j;
                    
                    // Add the tap handler
                    tapGesture.Tapped += (sender, e) => {
                        Debug.WriteLine($"Image tapped at row {capturedRow}, column {capturedCol}");
                        
                        // Get the tapped view
                        var tileView = _images[capturedRow, capturedCol];
                        
                        // Create a new event args instance for each tap
                        var eventArgs = new TileTappedEventArgs(capturedRow, capturedCol, tileView, false);
                        
                        // Invoke the GameTileTapped event
                        // This event is guaranteed to only fire for actual game tiles
                        GameTileTapped?.Invoke(this, eventArgs);
                        
                        Debug.WriteLine($"GameTileTapped event raised for row {capturedRow}, column {capturedCol}");
                    };
                    
                    // Get the image for this cell from the _images array
                    var image = _images[i, j];
                    
                    // Make sure the image can receive input
                    image.InputTransparent = false;
                    #if GESTURE_RECOGNIZER
                    
                    // Add the tap gesture recognizer to the image, not the cell
                    image.GestureRecognizers.Add(tapGesture);
                    Debug.WriteLine($"Added tap gesture to image at row {i}, column {j}");
                    #else
                    
                    image.Behaviors.Add(new TouchBehavior()
                    {
                        Command = new Command(() =>
                        {
                            Debug.WriteLine($"Image tapped at row {capturedRow}, column {capturedCol}");
                            
                            // Create a new event args instance for each tap
                            var eventArgs = new TileTappedEventArgs(capturedRow, capturedCol, image, false);
                            
                            // Invoke the GameTileTapped event
                            // This event is guaranteed to only fire for actual game tiles
                            GameTileTapped?.Invoke(this, eventArgs);
                            
                            if (eventArgs.DoRemove)
                            {
                                // Remove the tap gesture from the image
                                image.Behaviors.Clear();
                            }
                            
                            Debug.WriteLine($"GameTileTapped event raised for row {capturedRow}, column {capturedCol}");
                        }),
                        LongPressCommand = new Command(() =>
                        {
                            Debug.WriteLine($"Image long pressed at row {capturedRow}, column {capturedCol}");
                            
                            // Create a new event args instance for each tap
                            var eventArgs = new TileTappedEventArgs(capturedRow, capturedCol, image, true);
                            
                            // Invoke the GameTileTapped event
                            // This event is guaranteed to only fire for actual game tiles
                            GameTileTapped?.Invoke(this, eventArgs);
                            
                            Debug.WriteLine($"GameTileTapped event raised for row {capturedRow}, column {capturedCol}");
                        }),
                        LongPressDuration = 500
                        
                    });
                  
                    
                    #endif
                    
                    
                }
                
                // Add the cell to the grid (always add the cell, regardless of whether it's a tile or whitespace)
                _grid.Children.Add(cell);
            }

        // Update the layout
        UpdateLayout();
    }

    /// <summary>
    ///     Called when the GridSize property changes.
    /// </summary>
    /// <param name="bindable">The bindable object whose property changed.</param>
    /// <param name="oldValue">The old property value.</param>
    /// <param name="newValue">The new property value.</param>
    private static void OnGridSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SquareImageGrid grid && newValue is int size)
            // Create a new grid with the specified number of rows and columns
            grid.CreateGrid(size, size);
    }

    /// <summary>
    ///     Called when any of the border properties change.
    /// </summary>
    /// <param name="bindable">The bindable object whose property changed.</param>
    /// <param name="oldValue">The old property value.</param>
    /// <param name="newValue">The new property value.</param>
    private static void OnBorderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SquareImageGrid grid)
            // Update the border appearance
            grid.UpdateBorderAppearance();
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
        _border.IsRecessed = IsRecessed;
        _border.IsVisible = ShowBorder;
    }

    /// <summary>
    ///     Validates that the row and column indices are within bounds.
    /// </summary>
    /// <param name="row">The row index to validate.</param>
    /// <param name="col">The column index to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when row or column is outside valid range.</exception>
    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= Rows)
            throw new ArgumentOutOfRangeException(nameof(row), $"Row index must be between 0 and {Rows - 1}");
        if (col < 0 || col >= Columns)
            throw new ArgumentOutOfRangeException(nameof(col), $"Column index must be between 0 and {Columns - 1}");
    }

    /// <summary>
    ///     Updates the image for a specific cell in the grid.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="image">The new image for the cell.</param>
    private void UpdateCellImage(int row, int col, View image)
    {
        // Find the existing cell in the grid
        var existingCell = GetCellFromGrid(row, col);
        if (existingCell != null)
            // Remove the existing cell
            _grid.Children.Remove(existingCell);

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
            if (_images != null) _images[row, col] = defaultCell;
        }
    }

    /// <summary>
    ///     Gets the cell at the specified row and column from the grid.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="col">The column index.</param>
    /// <returns>The cell at the specified position, or null if not found.</returns>
    private View? GetCellFromGrid(int row, int col)
    {
        // Since we're maintaining our own array of images, we can just return the image from there
        if (row >= 0 && row < Rows && col >= 0 && col < Columns && _images != null) return _images[row, col];

        return null;
    }

    // OnIndividualTileTapped method removed - we're now using buttons for each cell instead of individual tap gesture recognizers
    
    // OnGridCellTapped method removed - we're now using buttons for each cell instead of a grid-wide tap gesture

    /// <summary>
    ///     Determines if a view is a default/blank tile.
    /// </summary>
    /// <param name="view">The view to check.</param>
    /// <returns>True if the view is a default tile; otherwise, false.</returns>
    private bool IsDefaultTile(View view)
    {
        return view is Rectangle rectangle &&
               rectangle.Fill is SolidColorBrush brush &&
               brush.Color == Colors.Transparent;
    }
    
    /// <summary>
    ///     Clears the current cell selection.
    /// </summary>
    public void ClearSelection()
    {
        // Clear any visual selection indicators
    }

    /// <summary>
    ///     Handles the Loaded event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnLoaded(object? sender, EventArgs e)
    {
        // Ensure the grid is properly sized when it's first loaded
        UpdateLayout();
    }

    /// <summary>
    ///     Handles the SizeChanged event.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments.</param>
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        // Update the layout when the size changes
        UpdateLayout();
    }

    /// <summary>
    ///     Updates the layout to ensure the grid is square and centered, with tighter spacing to the border.
    /// </summary>
    private void UpdateLayout()
    {
        // Get the available width and height
        var availableWidth = Width;
        var availableHeight = Height;

        // If we have valid dimensions, update the layout
        if (availableWidth > 0 && availableHeight > 0)
        {
            // Use the minimum dimension to ensure the grid is square
            var size = Math.Min(availableWidth, availableHeight);

            // Adjust the size to be closer to the border (95% of available space)
            // This makes the grid take up more space, bringing it closer to the border
            var adjustedSize = size * 0.95;

            // Center the grid using proportional positioning
            AbsoluteLayout.SetLayoutBounds(_grid, new Rect(0.5, 0.5, adjustedSize, adjustedSize));
            AbsoluteLayout.SetLayoutFlags(_grid, AbsoluteLayoutFlags.PositionProportional);

            // Log the size for debugging
            Debug.WriteLine(
                $"SquareImageGrid: Setting size to {adjustedSize}x{adjustedSize} centered proportionally");
        }
        else
        {
            // Log that we don't have valid dimensions yet
            Debug.WriteLine(
                $"SquareImageGrid: Invalid dimensions - Width: {availableWidth}, Height: {availableHeight}");
        }
    }

    /// <summary>
    ///     Override the OnParentSet method to update the layout when the parent changes.
    /// </summary>
    protected override void OnParentSet()
    {
        base.OnParentSet();

        // Update the layout when the parent changes
        UpdateLayout();
    }
}


