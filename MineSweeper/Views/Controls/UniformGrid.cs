using CommunityToolkit.Maui.Layouts;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using MineSweeper.Models;
using Size = Microsoft.Maui.Graphics.Size;

namespace MineSweeper.Views.Controls;

/// <summary>
/// A grid layout that arranges items in a uniform grid with equal-sized cells
/// </summary>
public class UniformGrid : UniformItemsLayout, IGridLayout
{
    private IGridLayout? _gridLayoutImplementation;
    
    // Flag to indicate if a batch update is in progress
    private bool _batchUpdateInProgress = false;
    
    // Dictionary to cache views for reuse
    private readonly Dictionary<object, View> _viewCache = new();
    
    // Logger for debugging
    private readonly ILogger? _logger;

    /// <summary>
    /// Bindable property for the items source
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty
        = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<object>),
            typeof(UniformGrid),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (UniformGrid) bindable;
                control.OnItemsSourceChanged((IEnumerable<object>?) oldValue, (IEnumerable<object>?) newValue);
            });

    /// <summary>
    /// Gets or sets the items source for the grid
    /// </summary>
    public IEnumerable<object>? ItemsSource
    {
        get => (IEnumerable<object>?) GetValue(ItemsSourceProperty);
        set 
        {
            _logger?.Log($"UniformGrid: Setting ItemsSource, type: {value?.GetType().Name ?? "null"}");
            if (value != null)
            {
                _logger?.Log($"UniformGrid: ItemsSource count: {value.Count()}");
                foreach (var item in value.Take(1))
                {
                    _logger?.Log($"UniformGrid: First item type: {item.GetType().Name}");
                }
            }
            SetValue(ItemsSourceProperty, value);
        }
    }
    
    /// <summary>
    /// Handles changes to the items source
    /// </summary>
    /// <param name="oldValue">The old items source</param>
    /// <param name="newValue">The new items source</param>
    private void OnItemsSourceChanged(IEnumerable<object>? oldValue, IEnumerable<object>? newValue)
    {
        try
        {
            _logger?.Log($"UniformGrid: OnItemsSourceChanged called. New items: {newValue?.Count() ?? 0}");
            _logger?.Log($"UniformGrid: Current dimensions - Width={Width}, Height={Height}, Rows={Rows}, Columns={Columns}");
            _logger?.Log($"UniformGrid: Current ItemSize={ItemSize}");
            
            // Start batch update
            _batchUpdateInProgress = true;
            
            // Clear existing items
            Children.Clear();

            // If new value is null, just return
            if (newValue == null)
            {
                _logger?.Log("UniformGrid: ItemsSource is null, returning");
                _batchUpdateInProgress = false;
                return;
            }

            // Check if ItemTemplate is null
            if (ItemTemplate is null)
            {
                _logger?.Log("UniformGrid: ItemTemplate is null, creating default template");
                
                // Create a default template that shows mines and flags
                ItemTemplate = new DataTemplate(() => 
                {
                    var grid = new Grid
                    {
                        BackgroundColor = Colors.LightGray
                    };
                    
                    // Flag emoji (when flagged)
                    var flagLabel = new Label
                    {
                        Text = "🚩",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 16,
                        TextColor = Colors.Red
                    };
                    flagLabel.SetBinding(Label.IsVisibleProperty, "IsFlagged");
                    
                    // Mine emoji (when is mine)
                    var mineLabel = new Label
                    {
                        Text = "💣",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 16,
                        TextColor = Colors.Black
                    };
                    mineLabel.SetBinding(Label.IsVisibleProperty, "IsMine");
                    
                    // Mine count (when not a mine and count > 0)
                    var countLabel = new Label
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.Blue
                    };
                    countLabel.SetBinding(Label.TextProperty, "MineCount");
                    
                    grid.Add(flagLabel);
                    grid.Add(mineLabel);
                    grid.Add(countLabel);
                    
                    return grid;
                });
            }
            
            // Process items in batches for better performance
            var items = newValue.ToList(); // Materialize the collection to avoid multiple enumeration
            int totalItems = items.Count;
            int batchSize = 50;
            
            for (int batchStart = 0; batchStart < totalItems; batchStart += batchSize)
            {
                int batchEnd = Math.Min(batchStart + batchSize, totalItems);
                
                for (int i = batchStart; i < batchEnd; i++)
                {
                    var item = items[i];
                    
                    // Try to reuse an existing view from the cache
                    View view;
                    if (_viewCache.TryGetValue(item, out var cachedView))
                    {
                        view = cachedView;
                    }
                    else
                    {
                        // Create a new view from the template
                        var content = (View)ItemTemplate.CreateContent();
                        
                        // Set the binding context for the content
                        content.BindingContext = item;
                        
                        // Add a border around the view to make it more visible
                        var border = new Border
                        {
                            BackgroundColor = Colors.LightGray,
                            Stroke = Colors.Gray,
                            StrokeThickness = 1,
                            StrokeShape = new RoundRectangle { CornerRadius = 2 },
                            Padding = 0,
                            Margin = 0,
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalOptions = LayoutOptions.Fill,
                            Content = content
                        };
                        
                        view = border;
                        
                        // Cache the view for future reuse
                        _viewCache[item] = view;
                    }
                    
                    // Set position in grid
                    var row = i / Math.Max(1, Columns);
                    var column = i % Math.Max(1, Columns);
                    Grid.SetRow(view, row);
                    Grid.SetColumn(view, column);
                    
                    Children.Add(view);
                }
            }

            // Update the item size
            UpdateItemSize();
            
            // Force a layout update
            InvalidateMeasure();
        }
        catch (Exception ex)
        {
            _logger?.LogError($"UniformGrid: Exception in OnItemsSourceChanged: {ex}");
        }
        finally
        {
            // End batch update
            _batchUpdateInProgress = false;
        }
    }

    /// <summary>
    /// Bindable property for the item template
    /// </summary>
    public static readonly BindableProperty ItemTemplateProperty
        = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(UniformGrid),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (UniformGrid) bindable;
                control.OnItemTemplateChanged((DataTemplate?) oldValue, (DataTemplate?) newValue);
            });

    /// <summary>
    /// Gets or sets the item template for the grid
    /// </summary>
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?) GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Handles changes to the item template
    /// </summary>
    /// <param name="oldValue">The old item template</param>
    /// <param name="newValue">The new item template</param>
    private void OnItemTemplateChanged(DataTemplate? oldValue, DataTemplate? newValue)
    {
        try
        {
            _logger?.Log($"UniformGrid: OnItemTemplateChanged called. New template is null? {newValue == null}");
            
            // If new template is null or items source is null, just return
            if (newValue == null || ItemsSource == null)
            {
                _logger?.Log("UniformGrid: New template or ItemsSource is null, returning");
                return;
            }
            
            // Start batch update
            _batchUpdateInProgress = true;
            
            // Clear view cache to force recreation with new template
            _viewCache.Clear();
            
            // Clear existing items
            Children.Clear();

            // Process items in batches for better performance
            var items = ItemsSource.ToList(); // Materialize the collection to avoid multiple enumeration
            int totalItems = items.Count;
            int batchSize = 50;
            
            _logger?.Log($"UniformGrid: Processing {totalItems} items with new template");
            
            for (int batchStart = 0; batchStart < totalItems; batchStart += batchSize)
            {
                int batchEnd = Math.Min(batchStart + batchSize, totalItems);
                
                for (int i = batchStart; i < batchEnd; i++)
                {
                    var item = items[i];
                    
                    // Create a new view from the template
                    var content = (View)newValue.CreateContent();
                    
                    // Set the binding context for the content
                    content.BindingContext = item;
                    
                    // Add a border around the view to make it more visible
                    var border = new Border
                    {
                        BackgroundColor = Colors.LightGray,
                        Stroke = Colors.Gray,
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 2 },
                        Padding = 0,
                        Margin = 0,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        Content = content
                    };
                    
                    var view = border;
                    
                    // Cache the view for future reuse
                    _viewCache[item] = view;
                    
                    // Set position in grid
                    var row = i / Math.Max(1, Columns);
                    var column = i % Math.Max(1, Columns);
                    Grid.SetRow(view, row);
                    Grid.SetColumn(view, column);
                    
                    Children.Add(view);
                    
                    if (i < 5) // Log only the first few items to avoid flooding the log
                    {
                        _logger?.Log($"UniformGrid: Created view for item {i}, content type: {content.GetType().Name}");
                    }
                }
            }

            _logger?.Log($"UniformGrid: Added {Children.Count} children to the grid");

            // Update the item size
            UpdateItemSize();
            
            // Force a layout update
            InvalidateMeasure();
        }
        catch (Exception ex)
        {
            _logger?.LogError($"UniformGrid: Exception in OnItemTemplateChanged: {ex}");
        }
        finally
        {
            // End batch update
            _batchUpdateInProgress = false;
        }
    }

    /// <summary>
    /// Bindable property for the number of rows
    /// </summary>
    public static readonly BindableProperty RowsProperty = BindableProperty.Create(
        nameof(Rows),
        typeof(int),
        typeof(UniformGrid),
        defaultValue: 10,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (UniformGrid) bindable;
            control.OnRowsChanged((int) oldValue, (int) newValue);
        });

    /// <summary>
    /// Gets or sets the number of rows in the grid
    /// </summary>
    public int Rows
    {
        get => (int) GetValue(RowsProperty);
        set => SetValue(RowsProperty, Math.Max(1, value)); // Ensure at least 1 row
    }

    /// <summary>
    /// Handles changes to the number of rows
    /// </summary>
    /// <param name="oldValue">The old number of rows</param>
    /// <param name="newValue">The new number of rows</param>
    private void OnRowsChanged(int oldValue, int newValue)
    {
        UpdateItemSize();
    }

    /// <summary>
    /// Bindable property for the number of columns
    /// </summary>
    public static readonly BindableProperty ColumnsProperty
        = BindableProperty.Create(
            nameof(Columns),
            typeof(int),
            typeof(UniformGrid),
            defaultValue: 10,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (UniformGrid) bindable;
                control.OnColumnsChanged((int) oldValue, (int) newValue);
            });

    /// <summary>
    /// Gets or sets the number of columns in the grid
    /// </summary>
    public int Columns
    {
        get => (int) GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, Math.Max(1, value)); // Ensure at least 1 column
    }

    /// <summary>
    /// Handles changes to the number of columns
    /// </summary>
    /// <param name="oldValue">The old number of columns</param>
    /// <param name="newValue">The new number of columns</param>
    private void OnColumnsChanged(int oldValue, int newValue)
    {
        UpdateItemSize();
    }

    /// <summary>
    /// Updates the size of each item in the grid
    /// </summary>
    private void UpdateItemSize()
    {
        if (_isUpdatingLayout || _batchUpdateInProgress)
            return;

        _logger?.Log(
            $"UniformGrid: UpdateItemSize called with _availableWidth={_availableWidth}, _availableHeight={_availableHeight}, Rows={Rows}, Columns={Columns}, Children={Children.Count}");

        // Ensure we have valid dimensions
        var rows = Math.Max(1, Rows);
        var columns = Math.Max(1, Columns);

        // Ensure we have valid available space
        var width = Math.Max(1, _availableWidth);
        var height = Math.Max(1, _availableHeight);

        _logger?.Log($"UniformGrid: Using width={width}, height={height}, rows={rows}, columns={columns}");

        // Calculate item size with precise division to avoid rounding errors
        // Use Math.Floor to ensure we don't exceed the available space
        var itemWidth = Math.Floor(width / columns * 100) / 100;
        var itemHeight = Math.Floor(height / rows * 100) / 100;

        // Calculate border and margin as a proportion of cell size rather than fixed pixels
        // This ensures the borders scale appropriately with the grid size
        var borderThickness = Math.Max(1, Math.Min(itemWidth, itemHeight) * 0.02);
        var margin = borderThickness / 2;

        // Store the calculated item size
        ItemSize = new Size(itemWidth, itemHeight);

        _logger?.Log(
            $"UniformGrid: Setting item size to width={itemWidth}, height={itemHeight}, borderThickness={borderThickness}, margin={margin}");

        // Set a proportional size for each child to improve scaling
        foreach (var child in Children)
        {
            if (child is View view)
            {
                // Calculate the exact cell content size by subtracting the border and margin
                var contentWidth = itemWidth - (borderThickness * 2) - (margin * 2);
                var contentHeight = itemHeight - (borderThickness * 2) - (margin * 2);

                view.WidthRequest = contentWidth;
                view.HeightRequest = contentHeight;

                // If the view is a Border, set its properties with proportional values
                if (view is Border border)
                {
                    border.StrokeThickness = borderThickness;
                    border.Stroke = Colors.Gray;
                    border.BackgroundColor = Colors.White;
                    border.Margin = 0; // Remove margin to avoid throwing off centering
                    border.Padding = 0;

                    // Ensure content also has no margin or padding
                    if (border.Content is Layout layout)
                    {
                        layout.Margin = 0;
                        layout.Padding = 0;
                    }
                }
            }
        }

        foreach (var child in Children)
        {
            if (child is View view)
            {
                // Set exact sizing without fractional pixels
                view.WidthRequest = Math.Floor(itemWidth);
                view.HeightRequest = Math.Floor(itemHeight);

                if (view is Border border)
                {
                    // Ensure border properties don't cause alignment issues
                    border.StrokeThickness = 1; // Fixed 1px border
                    border.Margin = 0;
                    border.Padding = 0;

                    // Set content alignment
                    if (border.Content is View contentView)
                    {
                        contentView.HorizontalOptions = LayoutOptions.Center;
                        contentView.VerticalOptions = LayoutOptions.Center;
                        contentView.Margin = 0;
                        
                        // Only set Padding if the view is a Layout
                        if (contentView is Layout layout)
                        {
                            layout.Padding = 0;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the UniformGrid class
    /// </summary>
    public UniformGrid()
    {
        // Initialize the control
        // Get customDebugLogger from the ServiceCollection
        
        Initialize();
        if (_logger is null)
             _logger = new CustomDebugLogger();
        _logger?.Log("UniformGrid: Initialized");
    }

    /// <summary>
    /// Initializes a new instance of the UniformGrid class with a customDebugLogger
    /// </summary>
    public UniformGrid(ILogger customDebugLogger)
    {
        _logger = customDebugLogger ?? throw new ArgumentNullException(nameof(customDebugLogger));
        
        // Initialize the control
        Initialize();
        
        _logger?.Log("UniformGrid: Initialized");
    }

    #region Private Methods

    /// <summary>
    /// Initializes the control
    /// </summary>
    private void Initialize()
    {
        // Set this instance as the grid layout implementation
        _gridLayoutImplementation = this;
        
        // Set default values
        _availableWidth = 100;
        _availableHeight = 100;
        
        // Add handler for children changed
        ChildAdded += OnChildAdded;
    }
    
    /// <summary>
    /// Handles when a child is added to the grid
    /// </summary>
    private void OnChildAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is View view && !_batchUpdateInProgress)
        {
            // Get the index of the child
            var index = Children.IndexOf(view);
            
            // Calculate row and column based on index
            var row = index / Math.Max(1, Columns);
            var column = index % Math.Max(1, Columns);
            
            // Set the row and column on the child
            Grid.SetRow(view, row);
            Grid.SetColumn(view, column);
        }
    }

    // Private fields to store the available width and height
    private double _availableWidth;
    private double _availableHeight;
    private bool _isUpdatingLayout = false;

    #endregion

    #region Protected Methods, Overrides And Properties

    /// <summary>
    /// Measures the control and its children
    /// </summary>
    /// <param name="widthConstraint">The width constraint</param>
    /// <param name="heightConstraint">The height constraint</param>
    /// <returns>The size of the control</returns>
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        if (_isUpdatingLayout || _batchUpdateInProgress)
            return new Size(widthConstraint, heightConstraint);
        
        _isUpdatingLayout = true;
        
        try
        {
            _logger?.Log($"UniformGrid: MeasureOverride called with width={widthConstraint}, height={heightConstraint}, Children={Children.Count}");
            
            // Handle invalid constraints
            if (double.IsInfinity(widthConstraint) || widthConstraint <= 0)
            {
                _logger?.Log($"UniformGrid: Invalid width constraint: {widthConstraint}, using default of 1000");
                widthConstraint = 1000;
            }
            if (double.IsInfinity(heightConstraint) || heightConstraint <= 0)
            {
                _logger?.Log($"UniformGrid: Invalid height constraint: {heightConstraint}, using default of 1000");
                heightConstraint = 1000;
            }
                    
            // Store the available size for later use
            _availableWidth = widthConstraint;
            _availableHeight = heightConstraint;
            
            // Update item size based on new constraints
            UpdateItemSize();
            
            // Calculate the size based on the number of rows and columns
            var resultWidth = ItemSize.Width * Columns;
            var resultHeight = ItemSize.Height * Rows;
            var result = new Size(resultWidth, resultHeight);
            
            _logger?.Log($"UniformGrid: MeasureOverride returning width={result.Width}, height={result.Height}");
            
            return result;
        }
        finally
        {
            _isUpdatingLayout = false;
        }
    }

    /// <summary>
    /// Arranges the control and its children
    /// </summary>
    /// <param name="bounds">The bounds of the control</param>
    /// <returns>The size of the control</returns>
    protected override Size ArrangeOverride(Rect bounds)
    {
        if (_isUpdatingLayout || _batchUpdateInProgress)
            return bounds.Size;
        
        _isUpdatingLayout = true;
        
        try
        {
            _logger?.Log($"UniformGrid: ArrangeOverride called with bounds={bounds}, Children={Children.Count}");
            
            // Ensure valid bounds
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                _logger?.Log($"UniformGrid: Invalid bounds {bounds}, skipping arrangement");
                return bounds.Size;
            }
            
            // IMPORTANT CHANGE: Use slightly less than available width to ensure there's space on both sides
            // This ensures we don't fill the entire width, enabling proper centering
            double effectiveWidth = Math.Floor(bounds.Width * 0.98); // Use 98% of available width
            double effectiveHeight = Math.Floor(bounds.Height * 0.98); // Use 98% of available height
            
            // Calculate cell size based on the effective dimensions
            double cellWidth = Math.Floor(effectiveWidth / Columns);
            double cellHeight = Math.Floor(effectiveHeight / Rows);
            
            // Use the smallest dimension for square cells
            double squareCellSize = Math.Min(cellWidth, cellHeight);
            
            // Calculate the exact total grid size (integer values)
            double gridWidth = squareCellSize * Columns;
            double gridHeight = squareCellSize * Rows;
            
            // Calculate the exact centering offsets to ensure perfect symmetry
            double offsetX = Math.Floor((bounds.Width - gridWidth) / 2);

            // Adjust for odd widths to ensure perfect centering
            if ((bounds.Width - gridWidth) % 2 == 1) {
                // If there's an odd number of pixels to distribute, add 0.5 to center exactly
                offsetX += 0.5;
            }

            // For safety, ensure a minimum margin
            if (offsetX < 2) {
                offsetX = 2;
            }
            
            double offsetY = Math.Floor((bounds.Height - gridHeight) / 2);
            
            _logger?.Log($"UniformGrid: Bounds width={bounds.Width}, Grid width={gridWidth}");
            _logger?.Log($"UniformGrid: Total offset for width={bounds.Width - gridWidth}");
            _logger?.Log($"UniformGrid: Centering at integer offset=({offsetX},{offsetY})");
            
            // Update item size
            ItemSize = new Size(squareCellSize, squareCellSize);
            
            // Arrange each child
            int totalChildren = Children.Count;
            
            // Track leftmost and rightmost positions for verification
            double leftmostX = double.MaxValue;
            double rightmostX = double.MinValue;
            
            // Only calculate margins if we have children
            if (totalChildren > 0)
            {
                for (int index = 0; index < totalChildren; index++)
                {
                    if (Children[index] is View view)
                    {
                        // Calculate grid position
                        int row = index / Columns;
                        int column = index % Columns;
                        
                        // Calculate exact pixel position with precise centering
                        double x = offsetX + (column * squareCellSize);
                        double y = offsetY + (row * squareCellSize);
                        
                        // Create bounds for the child with exact dimensions
                        var childBounds = new Rect(x, y, squareCellSize, squareCellSize);
                        
                        // Track leftmost and rightmost positions
                        if (column == 0 && x < leftmostX) leftmostX = x;
                        if (column == Columns - 1 && (x + squareCellSize) > rightmostX) rightmostX = x + squareCellSize;
                        
                        view.Arrange(childBounds);
                        
                        // Log all positioned cells in the first row for debugging
                        if (row == 0 || index == totalChildren - 1)
                        {
                            _logger?.Log($"UniformGrid: Cell at ({row},{column}) positioned at x={x}, width={squareCellSize}");
                        }
                    }
                }

                // Log the calculated margins to verify centering
                double leftMargin = leftmostX != double.MaxValue ? leftmostX : offsetX;
                double rightMargin = rightmostX != double.MinValue ? bounds.Width - rightmostX : bounds.Width - (offsetX + gridWidth);
                
                _logger?.Log($"UniformGrid: Left margin={leftMargin:F1}, Right margin={rightMargin:F1}");
                _logger?.Log($"UniformGrid: Margin difference={(leftMargin - rightMargin):F1}px");
            }
            else
            {
                // When there are no children, just log the theoretical margins
                _logger?.Log($"UniformGrid: No children to arrange. Theoretical left margin={offsetX}");
                _logger?.Log($"UniformGrid: Theoretical right margin={bounds.Width - (offsetX + gridWidth)}");
            }
            
            return bounds.Size;
        }
        finally
        {
            _isUpdatingLayout = false;
        }
    }

    #endregion

    #region IGridLayout Implementation

    /// <summary>
    /// Gets the row index of a view
    /// </summary>
    /// <param name="view">The view to get the row index for</param>
    /// <returns>The row index</returns>
    public int GetRow(IView view)
    {
        if (view is View maviView)
        {
            var index = Children.IndexOf(maviView);
            if (index >= 0)
            {
                return index / Math.Max(1, Columns);
            }
        }
        return 0;
    }

    /// <summary>
    /// Gets the row span of a view
    /// </summary>
    /// <param name="view">The view to get the row span for</param>
    /// <returns>The row span</returns>
    public int GetRowSpan(IView view)
    {
        return 1;
    }

    /// <summary>
    /// Gets the column index of a view
    /// </summary>
    /// <param name="view">The view to get the column index for</param>
    /// <returns>The column index</returns>
    public int GetColumn(IView view)
    {
        if (view is View maviView)
        {
            var index = Children.IndexOf(maviView);
            if (index >= 0)
            {
                return index % Math.Max(1, Columns);
            }
        }
        return 0;
    }

    /// <summary>
    /// Gets the column span of a view
    /// </summary>
    /// <param name="view">The view to get the column span for</param>
    /// <returns>The column span</returns>
    public int GetColumnSpan(IView view)
    {
        return 1;
    }

    /// <summary>
    /// Gets the row definitions for the grid
    /// </summary>
    public IReadOnlyList<IGridRowDefinition> RowDefinitions => new List<IGridRowDefinition>();

    /// <summary>
    /// Gets the column definitions for the grid
    /// </summary>
    public IReadOnlyList<IGridColumnDefinition> ColumnDefinitions => new List<IGridColumnDefinition>();

    /// <summary>
    /// Gets the spacing between rows
    /// </summary>
    public double RowSpacing => 0;

    /// <summary>
    /// Gets the spacing between columns
    /// </summary>
    public double ColumnSpacing => 0;

    /// <summary>
    /// Gets or sets the size of each item in the grid
    /// </summary>
    private Size _itemSize = new Size(1, 1);
    private Size ItemSize
    {
        get => _itemSize;
        set
        {
            if (_itemSize != value)
            {
                _logger?.Log($"UniformGrid: ItemSize changed from {_itemSize} to {value}");
                _itemSize = value;
            }
        }
    }

    #endregion
}
