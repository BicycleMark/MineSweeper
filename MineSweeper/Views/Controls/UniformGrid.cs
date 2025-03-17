using CommunityToolkit.Maui.Layouts;
using Size = Microsoft.Maui.Graphics.Size;

namespace MineSweeper.Views.Controls;

public class UniformGrid : UniformItemsLayout, IGridLayout
{
    private IGridLayout _gridLayoutImplementation;

    /// <summary>
    /// 
    /// </summary>
    public static readonly BindableProperty ItemsSourceProperty
        = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<object>),
            typeof(UniformGrid),
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (UniformGrid) bindable;
                control.OnItemsSourceChanged((IEnumerable<object>) oldValue, (IEnumerable<object>) newValue);
            });

    public IEnumerable<object> ItemsSource
    {
        get => (IEnumerable<object>) GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    
    private void OnItemsSourceChanged(IEnumerable<object> oldValue, IEnumerable<object> newValue)
    {
        // Clear existing items
        Children.Clear();

        var index = 0;
        // Re-add items with the new template
        foreach (var item in newValue)
        {
            if (ItemTemplate is null)
                ItemTemplate = new DataTemplate(() => new Label());
            var view = (View) ItemTemplate.CreateContent();
            view.BindingContext = item;
            // How to set each item's position in the grid?
            var row = index / Columns;
            var column = index % Columns;
            Grid.SetRow(view, row);
            Grid.SetColumn(view, column);
            Children.Add(view);
            index++;
        }

        // Update the item size
        UpdateItemSize();
    }


    public static readonly BindableProperty ItemTemplateProperty
        = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(UniformGrid),
            null,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (UniformGrid) bindable;
                control.OnItemTemplateChanged((DataTemplate) oldValue, (DataTemplate) newValue);
            });

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate) GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    private void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
    {
        // Clear existing items
        Children.Clear();
        var index = 0;

        // Re-add items with the new template
        foreach (var item in ItemsSource)
        {
            var view = (View) newValue.CreateContent();
            view.BindingContext = item;
            // How to set each item's position in the grid?
            var row = index / Columns;
            var column = index % Columns;
            Grid.SetRow(view, row);
            Grid.SetColumn(view, column);
            Children.Add(view);
            index++;
        }

        // Update the item size
        UpdateItemSize();
    }

    public static readonly BindableProperty RowsProperty = BindableProperty.Create(
        nameof(Rows),
        typeof(int),
        typeof(UniformGrid),
        2,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (UniformGrid) bindable;
            control.OnRowsChanged((int) oldValue, (int) newValue);
        });

    public int Rows
    {
        get => (int) GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    private void OnRowsChanged(int oldValue, int newValue)
    {
        UpdateItemSize();
    }

    public static readonly BindableProperty ColumnsProperty
        = BindableProperty.Create(
            nameof(Columns),
            typeof(int),
            typeof(UniformGrid),
            2,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (UniformGrid) bindable;
                control.OnColumnsChanged((int) oldValue, (int) newValue);
            });

    public int Columns
    {
        get => (int) GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    private void OnColumnsChanged(int oldValue, int newValue)
    {
        UpdateItemSize();
    }


    public UniformGrid()
    {
        // Initialize the control
        Initialize();
    }

    #region Private Methods

    private void Initialize()
    {
        _gridLayoutImplementation = this;
    }

    private void UpdateItemSize()
    {
        if (Rows > 0 && Columns > 0)
        {
            var itemWidth = Width / Columns;
            var itemHeight = Height / Rows;
            ItemSize = new Size(itemWidth, itemHeight);
        }
    }

    #endregion

    #region Protected Methods, Overrides And Properties

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        if (ItemSize.Width == 0 || ItemSize.Height == 0) 
            UpdateItemSize();
        var sz = base.MeasureOverride(widthConstraint, heightConstraint);
        return new Size(widthConstraint, heightConstraint);
    }

    protected override Size ArrangeOverride(Rect bounds)
    {
        if (ItemSize.Width == 0 || ItemSize.Height == 0) 
            UpdateItemSize();
        var sz = base.ArrangeOverride(bounds);
        return new Size(bounds.Width, bounds.Height);
    }

    
   
    
    // IGridLayout implementation for UniformGrid
    public int GetRow(IView view)
    {
        var index = Children.IndexOf((View)view);
        return index / Columns;
    }

    public int GetRowSpan(IView view)
    {
        return 1;
    }

    public int GetColumn(IView view)
    {
        var index = Children.IndexOf((View)view);
        return index % Columns;
    }

    public int GetColumnSpan(IView view)
    {
        return 1;
    }

    public IReadOnlyList<IGridRowDefinition> RowDefinitions => new List<IGridRowDefinition>();

    public IReadOnlyList<IGridColumnDefinition> ColumnDefinitions => new List<IGridColumnDefinition>();

    public double RowSpacing => 0;
    public double ColumnSpacing => 0;
    private Size ItemSize { get; set; }

    #endregion
}
