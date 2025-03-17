using CommunityToolkit.Maui.Layouts;
using Size = Microsoft.Maui.Graphics.Size;

namespace MineSweeper.Views.Controls;

/// <summary>
/// A grid layout that arranges items in a uniform grid with equal-sized cells
/// </summary>
public class UniformGrid : UniformItemsLayout, IGridLayout
{
    private IGridLayout? _gridLayoutImplementation;

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
        set => SetValue(ItemsSourceProperty, value);
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
            // Clear existing items
            Children.Clear();

            // If new value is null, just return
            if (newValue == null)
            {
                _logger.Log("ItemsSource is null");
                return;
            }

            var index = 0;
            // Re-add items with the new template
            foreach (var item in newValue)
            {
                if (ItemTemplate is null)
                {
                    _logger.Log("ItemTemplate is null, using default Label template");
                    ItemTemplate = new DataTemplate(() => new Label());
                }
                
                var view = (View) ItemTemplate.CreateContent();
                view.BindingContext = item;
                
                // Set position in grid
                var row = index / Math.Max(1, Columns);
                var column = index % Math.Max(1, Columns);
                Grid.SetRow(view, row);
                Grid.SetColumn(view, column);
                
                Children.Add(view);
                index++;
            }

            // Update the item size
            UpdateItemSize();
            
            _logger.Log($"ItemsSource changed, added {index} items");
        }
        catch (Exception ex)
        {
            _logger.Log($"Exception in OnItemsSourceChanged: {ex}");
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
            // If new template is null or items source is null, just return
            if (newValue == null || ItemsSource == null)
            {
                _logger.Log("ItemTemplate or ItemsSource is null");
                return;
            }
            
            // Clear existing items
            Children.Clear();
            var index = 0;

            // Re-add items with the new template
            foreach (var item in ItemsSource)
            {
                var view = (View) newValue.CreateContent();
                view.BindingContext = item;
                
                // Set position in grid
                var row = index / Math.Max(1, Columns);
                var column = index % Math.Max(1, Columns);
                Grid.SetRow(view, row);
                Grid.SetColumn(view, column);
                
                Children.Add(view);
                index++;
            }

            // Update the item size
            UpdateItemSize();
            
            _logger.Log($"ItemTemplate changed, added {index} items");
        }
        catch (Exception ex)
        {
            _logger.Log($"Exception in OnItemTemplateChanged: {ex}");
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
        _logger.Log($"Rows changed from {oldValue} to {newValue}");
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
        _logger.Log($"Columns changed from {oldValue} to {newValue}");
        UpdateItemSize();
    }

    /// <summary>
    /// Initializes a new instance of the UniformGrid class
    /// </summary>
    public UniformGrid()
    {
        // Initialize the control
        Initialize();
    }

    #region Private Methods

    /// <summary>
    /// Initializes the control
    /// </summary>
    private void Initialize()
    {
        try
        {
            // Set this instance as the grid layout implementation
            _gridLayoutImplementation = this;
            
            // Set default values
            _availableWidth = 100;
            _availableHeight = 100;
            
            // Log initialization
            _logger.Log("UniformGrid initialized");
        }
        catch (Exception ex)
        {
            _logger.Log($"Exception in Initialize: {ex}");
        }
    }

    // Private fields to store the available width and height
    private double _availableWidth;
    private double _availableHeight;
    
    /// <summary>
    /// Updates the size of each item in the grid
    /// </summary>
    private void UpdateItemSize()
    {
        try
        {
            // Ensure we have valid dimensions
            var rows = Math.Max(1, Rows);
            var columns = Math.Max(1, Columns);
            
            // Ensure we have valid available space
            var width = Math.Max(1, _availableWidth);
            var height = Math.Max(1, _availableHeight);
            
            // Calculate item size
            var itemWidth = width / columns;
            var itemHeight = height / rows;
            ItemSize = new Size(itemWidth, itemHeight);
            
            _logger.Log($"UpdateItemSize: Width={width}, Height={height}, Rows={rows}, Columns={columns}, ItemSize={ItemSize}");
            
            // Request a layout update
            InvalidateMeasure();
        }
        catch (Exception ex)
        {
            _logger.Log($"Exception in UpdateItemSize: {ex}");
        }
    }
    
    /// <summary>
    /// Logger for debugging
    /// </summary>
    private class Logger
    {
        public void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"UniformGrid: {message}");
        }
    }
    
    private readonly Logger _logger = new Logger();

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
        try
        {
            _logger.Log($"MeasureOverride: widthConstraint={widthConstraint}, heightConstraint={heightConstraint}");
            
            // Handle invalid constraints
            if (double.IsInfinity(widthConstraint) || widthConstraint <= 0)
                widthConstraint = 1000;
            if (double.IsInfinity(heightConstraint) || heightConstraint <= 0)
                heightConstraint = 1000;
                
            // Store the available size for later use
            _availableWidth = widthConstraint;
            _availableHeight = heightConstraint;
            
            // Update item size based on new constraints
            UpdateItemSize();
            
            // Measure each child
            foreach (var child in Children)
            {
                if (child is View view)
                {
                    view.Measure(ItemSize.Width, ItemSize.Height);
                }
            }
            
            // Call base implementation
            var size = base.MeasureOverride(widthConstraint, heightConstraint);
            _logger.Log($"MeasureOverride result: {size}");
            
            return size;
        }
        catch (Exception ex)
        {
            _logger.Log($"Exception in MeasureOverride: {ex}");
            return new Size(widthConstraint, heightConstraint);
        }
    }

    /// <summary>
    /// Arranges the control and its children
    /// </summary>
    /// <param name="bounds">The bounds of the control</param>
    /// <returns>The size of the control</returns>
    protected override Size ArrangeOverride(Rect bounds)
    {
        try
        {
            _logger.Log($"ArrangeOverride: bounds={bounds}");
            
            // Update width and height
            _availableWidth = bounds.Width;
            _availableHeight = bounds.Height;
            
            // Update item size
            UpdateItemSize();
            
            // Arrange each child
            foreach (var child in Children)
            {
                if (child is View view)
                {
                    var row = Grid.GetRow(view);
                    var column = Grid.GetColumn(view);
                    
                    // Calculate position
                    var x = column * ItemSize.Width;
                    var y = row * ItemSize.Height;
                    
                    // Create bounds for the child
                    var childBounds = new Rect(x, y, ItemSize.Width, ItemSize.Height);
                    
                    // Arrange the child
                    view.Arrange(childBounds);
                    
                    _logger.Log($"Arranged child at row={row}, column={column}, bounds={childBounds}");
                }
            }
            
            // Call base implementation
            var size = base.ArrangeOverride(bounds);
            _logger.Log($"ArrangeOverride result: {size}");
            
            return size;
        }
        catch (Exception ex)
        {
            _logger.Log($"Exception in ArrangeOverride: {ex}");
            return bounds.Size;
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
        try
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
        catch (Exception ex)
        {
            _logger.Log($"Exception in GetRow: {ex}");
            return 0;
        }
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
        try
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
        catch (Exception ex)
        {
            _logger.Log($"Exception in GetColumn: {ex}");
            return 0;
        }
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
    private Size ItemSize { get; set; } = new Size(1, 1);

    #endregion
}
