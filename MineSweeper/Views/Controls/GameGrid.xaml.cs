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
