using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using MineSweeper.Models;
using Size = Microsoft.Maui.Graphics.Size;

namespace MineSweeper.Views.Controls;

/// <summary>
/// A grid layout that arranges items in a uniform grid with equal-sized cells.
/// This is modeled after WPF's UniformGrid control.
/// </summary>
public class SquareUniformGrid : Layout, IGridLayout
{
    // Flag to indicate if a batch update is in progress
    private bool _batchUpdateInProgress = false;
    
    // Dictionary to cache views for reuse
    private readonly Dictionary<object, View> _viewCache = new();
    
    // Logger for debugging
    private readonly ILogger? _logger = null;

    /// <summary>
    /// Creates the layout manager for this layout
    /// </summary>
    protected override ILayoutManager CreateLayoutManager()
    {
        _logger?.Log("SquareUniformGrid: Creating layout manager");
        return new GridLayoutManager(this);
    }
    
    /// <summary>
    /// Initializes a new instance of the SquareUniformGrid class
    /// </summary>
    public SquareUniformGrid()
    {
        try
        {
            _logger = new CustomDebugLogger();
            _logger?.Log("SquareUniformGrid: Initialized");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing logger: {ex}");
        }
    }
    
    /// <summary>
    /// Initializes a new instance of the SquareUniformGrid class with a custom logger
    /// </summary>
    public SquareUniformGrid(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.Log("SquareUniformGrid: Initialized with custom logger");
    }

    #region Bindable Properties

    /// <summary>
    /// Bindable property for the items source
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty
        = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(SquareUniformGrid),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (SquareUniformGrid) bindable;
                control.OnItemsSourceChanged((IEnumerable) oldValue, (IEnumerable) newValue);
            });

    /// <summary>
    /// Gets or sets the items source for the grid
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => (IEnumerable) GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    /// <summary>
    /// Bindable property for the item template
    /// </summary>
    public static readonly BindableProperty ItemTemplateProperty
        = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(SquareUniformGrid),
            defaultValue: null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (SquareUniformGrid) bindable;
                control.OnItemTemplateChanged((DataTemplate) oldValue, (DataTemplate) newValue);
            });

    /// <summary>
    /// Gets or sets the item template for the grid
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate) GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Bindable property for the number of rows
    /// </summary>
    public static readonly BindableProperty RowsProperty = BindableProperty.Create(
        nameof(Rows),
        typeof(int),
        typeof(SquareUniformGrid),
        defaultValue: 1,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (SquareUniformGrid) bindable;
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
    /// Bindable property for the number of columns
    /// </summary>
    public static readonly BindableProperty ColumnsProperty
        = BindableProperty.Create(
            nameof(Columns),
            typeof(int),
            typeof(SquareUniformGrid),
            defaultValue: 1,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (SquareUniformGrid) bindable;
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

    #endregion

    #region Property Change Handlers

    // Track the current items source for collection change events
    private INotifyCollectionChanged? _notifyCollectionChanged;
    
    /// <summary>
    /// Handles changes to the items source
    /// </summary>
    private void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
        try
        {
            _logger?.Log($"SquareUniformGrid: OnItemsSourceChanged called. New items: {newValue?.Cast<object>().Count() ?? 0}");
            _logger?.Log($"SquareUniformGrid: Current dimensions - Width={Width}, Height={Height}, Rows={Rows}, Columns={Columns}");
            
            // Unsubscribe from old collection changed events
            if (_notifyCollectionChanged != null)
            {
                _notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
                _notifyCollectionChanged = null;
            }
            
            // Start batch update
            _batchUpdateInProgress = true;
            
            // Clear existing items
            Children.Clear();
            
            // Clear view cache to force recreation of views
            _viewCache.Clear();

            // If new value is null, just return
            if (newValue == null)
            {
                _logger?.Log("SquareUniformGrid: ItemsSource is null, returning");
                _batchUpdateInProgress = false;
                return;
            }

            // Check if ItemTemplate is null
            if (ItemTemplate is null)
            {
                _logger?.Log("SquareUniformGrid: ItemTemplate is null, creating default template");
                
                // Create a default template that just shows a colored rectangle
                ItemTemplate = new DataTemplate(() => 
                {
                    var border = new Border
                    {
                        BackgroundColor = Colors.LightGray,
                        Stroke = Colors.Gray,
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 2 },
                        Padding = 0,
                        Margin = 0,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    };
                    
                    return border;
                });
            }
            
            // Subscribe to collection changed events if possible
            if (newValue is INotifyCollectionChanged notifyCollectionChanged)
            {
                _logger?.Log("SquareUniformGrid: Subscribing to CollectionChanged events");
                _notifyCollectionChanged = notifyCollectionChanged;
                _notifyCollectionChanged.CollectionChanged += OnCollectionChanged;
            }
            
            // Process items
            var items = new List<object>();
            foreach (var item in newValue)
            {
                items.Add(item);
            }
            
            int totalItems = items.Count;
            _logger?.Log($"SquareUniformGrid: Processing {totalItems} items");
            
            for (int i = 0; i < totalItems; i++)
            {
                var item = items[i];
                AddItemToGrid(item);
            }

            // Force a layout update
            InvalidateMeasure();
            
            _logger?.Log($"SquareUniformGrid: Added {Children.Count} children to the grid");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"SquareUniformGrid: Exception in OnItemsSourceChanged: {ex}");
        }
        finally
        {
            // End batch update
            _batchUpdateInProgress = false;
        }
    }
    
    /// <summary>
    /// Handles collection changed events from the items source
    /// </summary>
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        try
        {
            _logger?.Log($"SquareUniformGrid: Collection changed: {e.Action}");
            
            // Start batch update
            _batchUpdateInProgress = true;
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        _logger?.Log($"SquareUniformGrid: Adding {e.NewItems.Count} items");
                        foreach (var item in e.NewItems)
                        {
                            AddItemToGrid(item);
                        }
                    }
                    break;
                    
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        _logger?.Log($"SquareUniformGrid: Removing {e.OldItems.Count} items");
                        foreach (var item in e.OldItems)
                        {
                            if (item != null && _viewCache.TryGetValue(item, out var view))
                            {
                                Children.Remove(view);
                                _viewCache.Remove(item);
                            }
                        }
                    }
                    break;
                    
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null && e.NewItems != null)
                    {
                        _logger?.Log($"SquareUniformGrid: Replacing {e.OldItems.Count} items");
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            var oldItem = e.OldItems[i];
                            var newItem = e.NewItems[i];
                            
                            if (_viewCache.TryGetValue(oldItem, out var view))
                            {
                                Children.Remove(view);
                                _viewCache.Remove(oldItem);
                            }
                            
                            AddItemToGrid(newItem);
                        }
                    }
                    break;
                    
                case NotifyCollectionChangedAction.Reset:
                    _logger?.Log("SquareUniformGrid: Resetting collection");
                    Children.Clear();
                    _viewCache.Clear();
                    
                    if (ItemsSource != null)
                    {
                        foreach (var item in ItemsSource)
                        {
                            AddItemToGrid(item);
                        }
                    }
                    break;
            }
            
            // Force a layout update
            InvalidateMeasure();
        }
        catch (Exception ex)
        {
            _logger?.LogError($"SquareUniformGrid: Exception in OnCollectionChanged: {ex}");
        }
        finally
        {
            // End batch update
            _batchUpdateInProgress = false;
        }
    }
    
    /// <summary>
    /// Adds an item to the grid
    /// </summary>
    private void AddItemToGrid(object item)
    {
        try
        {
            // Skip null items
            if (item == null)
            {
                _logger?.LogWarning("SquareUniformGrid: Attempted to add null item to grid");
                return;
            }
            
            // Create a new view from the template
            var content = (View)ItemTemplate.CreateContent();
            
            // Set the binding context for the content
            content.BindingContext = item;
            
            // Set layout options directly on the content
            content.HorizontalOptions = LayoutOptions.Fill;
            content.VerticalOptions = LayoutOptions.Fill;
            
            // Cache the view for future reuse
            _viewCache[item] = content;
            
            // Add to children
            Children.Add(content);
            
            _logger?.Log($"SquareUniformGrid: Added item to grid, total children: {Children.Count}");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"SquareUniformGrid: Error creating view for item: {ex}");
        }
    }

    /// <summary>
    /// Handles changes to the item template
    /// </summary>
    private void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
    {
        try
        {
            _logger?.Log($"SquareUniformGrid: OnItemTemplateChanged called. New template is null? {newValue == null}");
            
            // If new template is null, just return
            if (newValue == null)
            {
                _logger?.Log("SquareUniformGrid: New template is null, returning");
                return;
            }
            
            // If we have an ItemsSource, refresh the views with the new template
            if (ItemsSource != null)
            {
                _logger?.Log("SquareUniformGrid: Refreshing views with new template");
                
                // Re-apply the ItemsSource to trigger a refresh with the new template
                OnItemsSourceChanged(ItemsSource, ItemsSource);
            }
            else
            {
                _logger?.Log("SquareUniformGrid: ItemsSource is null, template will be applied when items are set");
            }
            
            _logger?.Log($"SquareUniformGrid: Template change processed, Children count: {Children.Count}");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"SquareUniformGrid: Exception in OnItemTemplateChanged: {ex}");
        }
    }

    /// <summary>
    /// Handles changes to the number of rows
    /// </summary>
    private void OnRowsChanged(int oldValue, int newValue)
    {
        _logger?.Log($"SquareUniformGrid: Rows changed from {oldValue} to {newValue}");
        InvalidateMeasure();
    }

    /// <summary>
    /// Handles changes to the number of columns
    /// </summary>
    private void OnColumnsChanged(int oldValue, int newValue)
    {
        _logger?.Log($"SquareUniformGrid: Columns changed from {oldValue} to {newValue}");
        InvalidateMeasure();
    }

    #endregion

    #region Layout Methods

    // Private fields to store the available width and height
    private double _availableWidth;
    private double _availableHeight;
    private bool _isUpdatingLayout = false;
    private Size _itemSize = new Size(1, 1);

    /// <summary>
    /// Measures the control and its children
    /// </summary>
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        if (_isUpdatingLayout || _batchUpdateInProgress)
            return new Size(widthConstraint, heightConstraint);
        
        _isUpdatingLayout = true;
        
        try
        {
            _logger?.Log($"SquareUniformGrid: MeasureOverride called with width={widthConstraint}, height={heightConstraint}, Children={Children.Count}");
            
            // Handle invalid constraints
            if (double.IsInfinity(widthConstraint) || widthConstraint <= 0)
            {
                _logger?.Log($"SquareUniformGrid: Invalid width constraint: {widthConstraint}, using default of 1000");
                widthConstraint = 1000;
            }
            if (double.IsInfinity(heightConstraint) || heightConstraint <= 0)
            {
                _logger?.Log($"SquareUniformGrid: Invalid height constraint: {heightConstraint}, using default of 1000");
                heightConstraint = 1000;
            }
                    
            // Store the available size for later use
            _availableWidth = widthConstraint;
            _availableHeight = heightConstraint;
            
            // Calculate the size based on the number of rows and columns
            var cellWidth = Math.Floor(widthConstraint / Columns);
            var cellHeight = Math.Floor(heightConstraint / Rows);
            
            // Use the smallest dimension for square cells
            var cellSize = Math.Min(cellWidth, cellHeight);
            
            // Store the calculated item size
            _itemSize = new Size(cellSize, cellSize);
            
            // Calculate the total size
            var resultWidth = cellSize * Columns;
            var resultHeight = cellSize * Rows;
            
            _logger?.Log($"SquareUniformGrid: MeasureOverride returning width={resultWidth}, height={resultHeight}");
            
            return new Size(resultWidth, resultHeight);
        }
        finally
        {
            _isUpdatingLayout = false;
        }
    }

    /// <summary>
    /// Arranges the control and its children
    /// </summary>
    protected override Size ArrangeOverride(Rect bounds)
    {
        if (_isUpdatingLayout || _batchUpdateInProgress || Children.Count == 0)
            return bounds.Size;
        
        _isUpdatingLayout = true;
        
        try
        {
            _logger?.Log($"SquareUniformGrid: ArrangeOverride called with bounds={bounds}, Children={Children.Count}");
            
            // Ensure valid bounds
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                _logger?.Log($"SquareUniformGrid: Invalid bounds {bounds}, skipping arrangement");
                return bounds.Size;
            }
            
            // Calculate cell size
            double cellWidth = Math.Floor(bounds.Width / Columns);
            double cellHeight = Math.Floor(bounds.Height / Rows);
            
            // Use the smallest dimension for square cells
            double cellSize = Math.Min(cellWidth, cellHeight);
            
            // Calculate the total grid size
            double gridWidth = cellSize * Columns;
            double gridHeight = cellSize * Rows;
            
            // Calculate centering offsets
            double offsetX = Math.Floor((bounds.Width - gridWidth) / 2);
            double offsetY = Math.Floor((bounds.Height - gridHeight) / 2);
            
            _logger?.Log($"SquareUniformGrid: Centering at offset=({offsetX},{offsetY})");
            
            // Ensure minimum margins
            if (offsetX < 2) offsetX = 2;
            if (offsetY < 2) offsetY = 2;
            
            // Arrange each child
            int totalChildren = Children.Count;
            int rows = Rows;
            int columns = Columns;
            
            for (int index = 0; index < totalChildren; index++)
            {
                if (index < Children.Count)
                {
                    var view = Children[index];
                    
                    // Calculate grid position
                    int row = index / columns;
                    int column = index % columns;
                    
                    // Calculate pixel position
                    double x = offsetX + (column * cellSize);
                    double y = offsetY + (row * cellSize);
                    
                    // Create bounds for the child
                    var childBounds = new Rect(x, y, cellSize, cellSize);
                    
                    view.Arrange(childBounds);
                }
            }
            
            _logger?.Log($"SquareUniformGrid: Arranged {totalChildren} children");
            
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
    public int GetRow(IView view)
    {
        var index = Children.IndexOf(view);
        if (index >= 0)
        {
            return index / Math.Max(1, Columns);
        }
        return 0;
    }

    /// <summary>
    /// Gets the row span of a view
    /// </summary>
    public int GetRowSpan(IView view)
    {
        return 1;
    }

    /// <summary>
    /// Gets the column index of a view
    /// </summary>
    public int GetColumn(IView view)
    {
        var index = Children.IndexOf(view);
        if (index >= 0)
        {
            return index % Math.Max(1, Columns);
        }
        return 0;
    }

    /// <summary>
    /// Gets the column span of a view
    /// </summary>
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

    #endregion
}
