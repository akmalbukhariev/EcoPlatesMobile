using Microsoft.Maui.Maps;

namespace EcoPlatesMobile.Views.User.Components;

public partial class FavoriteProductView : ContentView
{
    public static readonly BindableProperty ProductImageProperty =
        BindableProperty.Create(nameof(ProductImage), typeof(ImageSource), typeof(FavoriteProductView), default(ImageSource), propertyChanged: ProductImageChanged);

    public static readonly BindableProperty ProductCountProperty =
      BindableProperty.Create(nameof(ProductCount), typeof(int), typeof(FavoriteProductView), 1, propertyChanged: ProductCountChanged);

    public static readonly BindableProperty ProductNameProperty =
       BindableProperty.Create(nameof(ProductName), typeof(string), typeof(FavoriteProductView), default(string), propertyChanged: ProductNameChanged);

    public static readonly BindableProperty ProductMakerNameProperty =
       BindableProperty.Create(nameof(ProductMakerName), typeof(string), typeof(FavoriteProductView), default(string), propertyChanged: ProductMakerNameChanged);

    public static readonly BindableProperty NewPriceProperty =
       BindableProperty.Create(nameof(NewPrice), typeof(double), typeof(FavoriteProductView), 0.0, propertyChanged: NewPriceChanged);

    public static readonly BindableProperty OldPriceProperty =
      BindableProperty.Create(nameof(OldPrice), typeof(double), typeof(FavoriteProductView), 0.0, propertyChanged: OldPriceChanged);

    public static readonly BindableProperty StarsProperty =
      BindableProperty.Create(nameof(Stars), typeof(double), typeof(FavoriteProductView), 0.0, propertyChanged: StarsChanged);

    public static readonly BindableProperty DistanceProperty =
     BindableProperty.Create(nameof(Distance), typeof(double), typeof(FavoriteProductView), 0.0, propertyChanged: DistanceChanged);

    public ImageSource ProductImage
    {
        get => (ImageSource)GetValue(ProductImageProperty);
        set => SetValue(ProductImageProperty, value);
    }

    public int ProductCount
    {
        get => (int)GetValue(ProductCountProperty);
        set => SetValue(ProductCountProperty, value);
    }

    public string ProductName
    {
        get => (string)GetValue(ProductNameProperty);
        set => SetValue(ProductNameProperty, value);
    }

    public string ProductMakerName
    {
        get => (string)GetValue(ProductMakerNameProperty);
        set => SetValue(ProductMakerNameProperty, value);
    }

    public double NewPrice
    {
        get => (double)GetValue(NewPriceProperty);
        set => SetValue(NewPriceProperty, value);
    }

    public double OldPrice
    {
        get => (double)GetValue(OldPriceProperty);
        set => SetValue(OldPriceProperty, value);
    }

    public double Stars
    {
        get => (double)GetValue(StarsProperty);
        set => SetValue(StarsProperty, value);
    }

    public double Distance
    {
        get => (double)GetValue(DistanceProperty);
        set => SetValue(DistanceProperty, value);
    }

    public FavoriteProductView()
	{
		InitializeComponent();
	}

    private static void ProductImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.productImage.Source = (ImageSource)newValue;
    }

    private static void ProductCountChanged(BindableObject bindable, object oldValue, object newValue)
    {
        //var control = (FavoriteProductView)bindable;
        //control.productCount.Text = (string)newValue;
    }

    private static void ProductNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.productName.Text = (string)newValue;
    }

    private static void ProductMakerNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.productMakerName.Text = (string)newValue;
    }

    private static void NewPriceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.newPrice.Text = ((double)newValue).ToString();
    }

    private static void OldPriceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.oldPrice.Text = ((double)newValue).ToString();
    }

    private static void StarsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.stars.Text = ((double)newValue).ToString();
    }

    private static void DistanceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.distance.Text = ((double)newValue).ToString();
    }
}