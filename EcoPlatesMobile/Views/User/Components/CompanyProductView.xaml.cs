using System.Windows.Input;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.Views.User.Components;

public partial class CompanyProductView : ContentView
{
    public static readonly BindableProperty ProductImageProperty =
        BindableProperty.Create(nameof(ProductImage), typeof(ImageSource), typeof(CompanyProductView), default(ImageSource), propertyChanged: ProductImageChanged);

    public static readonly BindableProperty ProductCountProperty =
      BindableProperty.Create(nameof(ProductCount), typeof(int), typeof(CompanyProductView), 1, propertyChanged: ProductCountChanged);

    public static readonly BindableProperty ProductNameProperty =
       BindableProperty.Create(nameof(ProductName), typeof(string), typeof(CompanyProductView), default(string), propertyChanged: ProductNameChanged);

    public static readonly BindableProperty NewPriceProperty =
      BindableProperty.Create(nameof(NewPrice), typeof(string), typeof(CompanyProductView), "0.0", propertyChanged: NewPriceChanged);

    public static readonly BindableProperty OldPriceProperty =
      BindableProperty.Create(nameof(OldPrice), typeof(string), typeof(CompanyProductView), "0.0", propertyChanged: OldPriceChanged);

    public static readonly BindableProperty StarsProperty =
      BindableProperty.Create(nameof(Stars), typeof(string), typeof(CompanyProductView), "0", propertyChanged: StarsChanged);

    public static readonly BindableProperty ClickCommandProperty =
        BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(CompanyProductView));

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

    public ICommand ClickCommand
    {
        get => (ICommand)GetValue(ClickCommandProperty);
        set => SetValue(ClickCommandProperty, value);
    }

    public CompanyProductView()
	{
		InitializeComponent();
	}

    private static void ProductImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyProductView)bindable;
        control.productImage.Source = (ImageSource)newValue;
    }

    private static void ProductCountChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyProductView)bindable;
        //control.productCount.Text = (string)newValue;
    }

    private static void ProductNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyProductView)bindable;
        control.productName.Text = (string)newValue;
    }

    private static void NewPriceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyProductView)bindable;
        control.newPrice.Text = (string)newValue;
    }

    private static void OldPriceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyProductView)bindable;
        control.oldPrice.Text = (string)newValue;
    }

    private static void StarsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyProductView)bindable;
        control.stars.Text = (string)newValue;
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