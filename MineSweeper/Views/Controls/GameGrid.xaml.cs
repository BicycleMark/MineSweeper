using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Views.Controls;

public partial class GameGrid : ContentView
{ 
    
    public static BindableProperty ItemSourceProperty = BindableProperty.Create(
        nameof(ItemSource),
        typeof(IEnumerable<object>),
        typeof(GameGrid),
        defaultValue: new List<object>(),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (GameGrid) bindable;
            control.OnItemsSourceChanged((IEnumerable<object>) oldValue, (IEnumerable<object>) newValue);
        });

    private void OnItemsSourceChanged(IEnumerable<object> oldValue, IEnumerable<object> newValue)
    {
        var items = newValue.ToList();
        var rows = 2; //Rows;
        var columns = 2; //Columns;
        if (items.Count != rows * columns)
        {
            throw new ArgumentException("Invalid Items Source");
        }

        PropertyChanged:
        InvalidateMeasure();
    }
    

     public static BindableProperty ItemTemplateProperty = BindableProperty.Create
        (nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(GameGrid),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (GameGrid) bindable;
            control.OnItemTemplateChanged((DataTemplate) oldValue, (DataTemplate) newValue);
        });

     private DataTemplate defaultItemTemplate = new DataTemplate(() =>
     {
         var label = new Label();
         label.SetBinding(Label.TextProperty, new Binding("."));
         return label;
     });
    private void OnItemTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
    {
        if (oldValue != newValue)
        {
            InvalidateMeasure();
        }
    }

    public static BindableProperty RowsProperty = BindableProperty.Create(
        nameof(Rows),
        typeof(int),
        typeof(GameGrid),
        10,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (GameGrid) bindable;
            control.OnRowsChanged((int) oldValue, (int) newValue);
        });

    private void OnRowsChanged(int oldValue, int newValue)
    {
        if (oldValue != newValue)
        {
           
            InvalidateMeasure();
        }
    }
    

    public  static BindableProperty ColumnsProperty = BindableProperty.Create(
        nameof(Columns),
        typeof(int),
        typeof(GameGrid),
        10,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (GameGrid) bindable;
            control.OnColumnsChanged((int) oldValue, (int) newValue);
        }
    );

    private void OnColumnsChanged(int oldValue, int newValue)
    {
        if (oldValue != newValue)
        {
           
            InvalidateMeasure();
        }
    }

    // Add a Bindable Border Property 
    public static BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(GameGrid),
        Colors.Black,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (GameGrid) bindable;
            control.InvalidateMeasure();
            
            control.mainBorder.Stroke = (Color) newValue;
        });


    public static BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(int),
        typeof(GameGrid),
        1,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (GameGrid) bindable;
            control.InvalidateMeasure();
            control.mainBorder.StrokeThickness = (int) newValue;
        });


    public GameGrid()
    {
        InitializeComponent();
        this.mainBorder.StrokeThickness= BorderThickness;
       
    }

    
    public int Rows
    {
        get => (int) GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    public int Columns
    {
        get => (int) GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public IEnumerable<object> ItemSource
    {
        get => (IEnumerable<object>) GetValue(ItemSourceProperty);
        set => SetValue(ItemSourceProperty, value);
    }

    // ItemTemplate Property
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate) GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

  
    public Color BorderColor
    {
        get => (Color) GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }


    public int BorderThickness
    {
        get => (int) GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }
    
    // Method to Create the Grid and add the Items as Created from the ItemTemplate
    [Obsolete]
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
       {
           
        var itemTemplate = ItemTemplate ?? defaultItemTemplate;
        var items = ItemSource;
        var rows = Rows;
        var columns = Columns;
        var itemIndex = 0;
        var rowHeight = 0.0;
        var columnWidth = 0.0;
        var totalHeight = 0.0;
        var totalWidth = 0.0;
        var rowDefinitions = new RowDefinitionCollection();
        var columnDefinitions = new ColumnDefinitionCollection();
        var grid = new Grid();
        for (var i = 0; i < rows; i++)
        {
            rowDefinitions.Add(new RowDefinition 
                {Height = new GridLength(1, GridUnitType.Star)});
        }

        for (var i = 0; i < columns; i++)
        {
            columnDefinitions.Add(new ColumnDefinition 
                {Width = new GridLength(1, GridUnitType.Star)});
        }

        grid.RowDefinitions = rowDefinitions;
        grid.ColumnDefinitions = columnDefinitions;
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                
                var view = (View) itemTemplate.CreateContent();
                view.BindingContext = items.ElementAt(itemIndex);
                grid.Children.Add(view);
                itemIndex++;
            }
        }

        grid.Measure(widthConstraint, heightConstraint);
        rowHeight = grid.RowDefinitions[0].Height.Value;
        columnWidth = grid.ColumnDefinitions[0].Width.Value;
        totalHeight = rowHeight * rows;
        totalWidth = columnWidth * columns;
        return base.MeasureOverride(totalWidth, totalHeight);
        
        //SetMeasuredDimension(totalWidth, totalHeight);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        foreach (var item in ItemSource)
        {
            if (item is BindableObject bindableObject)
            {
                bindableObject.BindingContext = BindingContext;
            }
        }
    }
    
}
