using System.Collections;
using System.ComponentModel;
using MineSweeper.Models;

namespace MineSweeper.Views.Controls;

/// <summary>
/// A game grid that displays items in a square grid layout.
/// This control is completely decoupled from any specific game logic.
/// </summary>
public partial class SquareGameGrid : ContentView
{
    private readonly ILogger _logger;
    #region Bindable Properties

    /// <summary>
    /// Bindable property for the items source
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(SquareGameGrid),
        null);

    /// <summary>
    /// Gets or sets the items source for the grid
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Bindable property for the item template
    /// </summary>
    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(SquareGameGrid),
        null);

    /// <summary>
    /// Gets or sets the item template for the grid
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Bindable property for the number of rows
    /// </summary>
    public static readonly BindableProperty RowsProperty = BindableProperty.Create(
        nameof(Rows),
        typeof(int),
        typeof(SquareGameGrid),
        10);

    /// <summary>
    /// Gets or sets the number of rows in the grid
    /// </summary>
    public int Rows
    {
        get => (int)GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    /// <summary>
    /// Bindable property for the number of columns
    /// </summary>
    public static readonly BindableProperty ColumnsProperty = BindableProperty.Create(
        nameof(Columns),
        typeof(int),
        typeof(SquareGameGrid),
        10);

    /// <summary>
    /// Gets or sets the number of columns in the grid
    /// </summary>
    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the SquareGameGrid class
    /// </summary>
    public SquareGameGrid()
    {
        // Initialize logger first to avoid null reference warning
        _logger = new CustomDebugLogger();
        
        InitializeComponent();
        
        try
        {
            _logger.Log("SquareGameGrid: Initialized");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing logger: {ex}");
        }
        
        // Set up property change handlers
        this.PropertyChanged += OnPropertyChanged;
    }
    
    /// <summary>
    /// Initializes a new instance of the SquareGameGrid class with a custom logger
    /// </summary>
    public SquareGameGrid(ILogger logger)
    {
        // Initialize logger first to avoid null reference warning
        _logger = logger ?? new CustomDebugLogger();
        
        InitializeComponent();
        
        try
        {
            _logger.Log("SquareGameGrid: Initialized with custom logger");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing logger: {ex}");
        }
        
        // Set up property change handlers
        this.PropertyChanged += OnPropertyChanged;
    }
    
    /// <summary>
    /// Handles property changes
    /// </summary>
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            _logger.Log($"SquareGameGrid: Property changed: {e.PropertyName}");
            
            // Update the board properties when our properties change
            if (e.PropertyName == nameof(ItemsSource))
            {
                var count = ItemsSource?.Cast<object>().Count() ?? 0;
                _logger.Log($"SquareGameGrid: Updating ItemsSource, count: {count}");
                board.ItemsSource = ItemsSource;
            }
            else if (e.PropertyName == nameof(ItemTemplate))
            {
                _logger.Log("SquareGameGrid: Updating ItemTemplate");
                board.ItemTemplate = ItemTemplate;
            }
            else if (e.PropertyName == nameof(Rows))
            {
                _logger.Log($"SquareGameGrid: Updating Rows to {Rows}");
                board.Rows = Rows;
            }
            else if (e.PropertyName == nameof(Columns))
            {
                _logger.Log($"SquareGameGrid: Updating Columns to {Columns}");
                board.Columns = Columns;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"SquareGameGrid: Error in OnPropertyChanged: {ex}");
        }
    }
    
    /// <summary>
    /// Called when the element is loaded
    /// </summary>
    protected override void OnHandlerChanged()
    {
        try
        {
            _logger.Log("SquareGameGrid: OnHandlerChanged called");
            
            base.OnHandlerChanged();
            
            // Initialize the board properties
            if (board != null)
            {
                _logger.Log($"SquareGameGrid: Initializing board properties - Rows: {Rows}, Columns: {Columns}");
                board.ItemsSource = ItemsSource;
                board.ItemTemplate = ItemTemplate;
                board.Rows = Rows;
                board.Columns = Columns;
            }
            else
            {
                _logger.LogWarning("SquareGameGrid: board is null in OnHandlerChanged");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"SquareGameGrid: Error in OnHandlerChanged: {ex}");
        }
    }
}
