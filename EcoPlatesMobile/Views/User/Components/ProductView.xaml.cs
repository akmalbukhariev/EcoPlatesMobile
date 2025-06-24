using System.Windows.Input;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.Views.User.Components;

public partial class ProductView : ContentView
{
    public static readonly BindableProperty ProductImageProperty =
        BindableProperty.Create(nameof(ProductImage), typeof(ImageSource), typeof(ProductView), default(ImageSource), propertyChanged: ProductImageChanged);

    public static readonly BindableProperty ProductCountProperty =
      BindableProperty.Create(nameof(ProductCount), typeof(int), typeof(ProductView), 1, propertyChanged: ProductCountChanged);
     
    public static readonly BindableProperty ProductNameProperty =
       BindableProperty.Create(nameof(ProductName), typeof(string), typeof(ProductView), default(string), propertyChanged: ProductNameChanged);

    public static readonly BindableProperty ProductMakerNameProperty =
       BindableProperty.Create(nameof(ProductMakerName), typeof(string), typeof(ProductView), default(string), propertyChanged: ProductMakerNameChanged);
     
    public static readonly BindableProperty NewPriceProperty =
       BindableProperty.Create(nameof(NewPrice), typeof(string), typeof(ProductView), "0.0", propertyChanged: NewPriceChanged);

    public static readonly BindableProperty OldPriceProperty =
      BindableProperty.Create(nameof(OldPrice), typeof(string), typeof(ProductView), "0.0", propertyChanged: OldPriceChanged);

    public static readonly BindableProperty StarsProperty =
      BindableProperty.Create(nameof(Stars), typeof(string), typeof(ProductView), "0", propertyChanged: StarsChanged);

    public static readonly BindableProperty DistanceProperty =
      BindableProperty.Create(nameof(Distance), typeof(string), typeof(ProductView), "0.0", propertyChanged: DistanceChanged);

    public static readonly BindableProperty LikedProperty =
     BindableProperty.Create(nameof(Liked), typeof(bool), typeof(ProductView), false, propertyChanged: LikedChanged);

    public static readonly BindableProperty LikeCommandProperty =
        BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(ProductView));

    public static readonly BindableProperty ClickCommandProperty =
        BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(ProductView));

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

    public string NewPrice
    {
        get => (string)GetValue(NewPriceProperty);
        set => SetValue(NewPriceProperty, value);
    }

    public string OldPrice
    {
        get => (string)GetValue(OldPriceProperty);
        set => SetValue(OldPriceProperty, value);
    }

    public string Stars
    {
        get => (string)GetValue(StarsProperty);
        set => SetValue(StarsProperty, value);
    }

    public string Distance
    {
        get => (string)GetValue(DistanceProperty);
        set => SetValue(DistanceProperty, value);
    }

    public bool Liked
    {
        get => (bool)GetValue(LikedProperty);
        set => SetValue(LikedProperty, value);
    }

    public ICommand LikeCommand
    {
        get => (ICommand)GetValue(LikeCommandProperty);
        set => SetValue(LikeCommandProperty, value);
    }

    public ICommand ClickCommand
    {
        get => (ICommand)GetValue(ClickCommandProperty);
        set => SetValue(ClickCommandProperty, value);
    }

    public ProductView()
	{
		InitializeComponent();
	}

    private static void ProductImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.productImage.Source = (ImageSource)newValue;
    }

    private static void ProductCountChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.productCount.Text = (string)newValue;
    }

    private static void ProductNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.productName.Text = (string)newValue;
    }

    private static void ProductMakerNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.productMakerName.Text = (string)newValue;
    }

    private static void NewPriceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.newPrice.Text = (string)newValue;
    }

    private static void OldPriceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.oldPrice.Text = (string)newValue;
    }

    private static void StarsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.stars.Text = (string)newValue;
    }

    private static void DistanceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.distance.Text = (string)newValue;
    }

    private static void LikedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        bool liked = (bool)newValue;

        control.imLiked.Source = liked ? "liked.png" : "like.png";
    }

    private async void Like_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        if (BindingContext is ProductModel product && LikeCommand?.CanExecute(product) == true)
        {
            LikeCommand.Execute(product);
        }
    }

    private async void Product_Tapped(object sender, TappedEventArgs e)
    {
        await mainFrame.ScaleTo(0.95, 100, Easing.CubicOut);
        await mainFrame.ScaleTo(1.0, 100, Easing.CubicIn);

        if (BindingContext is ProductModel product && ClickCommand?.CanExecute(product) == true)
        {
            ClickCommand.Execute(product);
        }
    }
}