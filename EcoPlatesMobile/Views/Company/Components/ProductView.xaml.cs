using System.Windows.Input;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.Views.Company.Components;

public partial class ProductView : ContentView
{
    #region Properties
    public static readonly BindableProperty ProductImageProperty =
        BindableProperty.Create(nameof(ProductImage), typeof(ImageSource), typeof(ProductView), default(ImageSource), propertyChanged: ProductImageChanged);

    public static readonly BindableProperty ProductCountProperty =
      BindableProperty.Create(nameof(ProductCount), typeof(int), typeof(ProductView), 1, propertyChanged: ProductCountChanged);

    public static readonly BindableProperty ProductNameProperty =
       BindableProperty.Create(nameof(ProductName), typeof(string), typeof(ProductView), default(string), propertyChanged: ProductNameChanged);

    public static readonly BindableProperty NewPriceProperty =
      BindableProperty.Create(nameof(NewPrice), typeof(string), typeof(ProductView), "0.0", propertyChanged: NewPriceChanged);

    public static readonly BindableProperty OldPriceProperty =
      BindableProperty.Create(nameof(OldPrice), typeof(string), typeof(ProductView), "0.0", propertyChanged: OldPriceChanged);

    public static readonly BindableProperty StarsProperty =
     BindableProperty.Create(nameof(Stars), typeof(string), typeof(ProductView), "0", propertyChanged: StarsChanged);

    public static readonly BindableProperty IsNonActiveProductProperty =
       BindableProperty.Create(nameof(IsNonActiveProduct), typeof(bool), typeof(ProductView), true, propertyChanged: IsNonActiveProductChanged);

    public static readonly BindableProperty ShowCheckProductProperty =
       BindableProperty.Create(nameof(ShowCheckProduct), typeof(bool), typeof(ProductView), true, propertyChanged: ShowCheckProductPropertyChanged);

    public static readonly BindableProperty IsCheckedProductProperty =
       BindableProperty.Create(nameof(IsCheckedProduct), typeof(bool), typeof(ProductView), true, propertyChanged: IsCheckedProductChanged);

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

    public bool ShowCheckProduct
    {
        get => (bool)GetValue(ShowCheckProductProperty);
        set => SetValue(ShowCheckProductProperty, value);
    }

    public bool IsCheckedProduct
    {
        get => (bool)GetValue(IsCheckedProductProperty);
        set => SetValue(IsCheckedProductProperty, value);
    }

    public ICommand ClickCommand
    {
        get => (ICommand)GetValue(ClickCommandProperty);
        set => SetValue(ClickCommandProperty, value);
    }

    public bool IsNonActiveProduct
    {
        get => (bool)GetValue(IsNonActiveProductProperty);
        set => SetValue(IsNonActiveProductProperty, value);
    }
    #endregion

    #region Animation
    private static int _animCycle = 0;                         
    private static HashSet<long> _animatedThisCycle = new();     
    private static readonly Random _rand = new Random();       
    private bool _didAnimateThisInstance = false;    
    private const double StartTranslate = -40;
    private const double StartScale = 0.96;
    private const uint DurationMs = 520;
    private const int StaggerBuckets = 4;
    private const int StaggerStepMs = 80;
    #endregion

    public ProductView()
    {
        InitializeComponent();
        Loaded += OnLoadedAnimateOnce;
        BindingContextChanged += OnBindingContextChangedAnimate;
    }

    #region Animation
    public static void BeginNewAnimationCycle()
    {
        _animCycle++;                 
        _animatedThisCycle.Clear();    
    }

    private void OnLoadedAnimateOnce(object? sender, EventArgs e)
    {
        TryAnimateIn();
    }

    private void OnBindingContextChangedAnimate(object? sender, EventArgs e)
    {
        _didAnimateThisInstance = false;
        TryAnimateIn();
    }

    private async void TryAnimateIn()
    {
        if (BindingContext is not ProductModel m)
            return;
 
        if (m.PromotionId > 0)
        {
            if (!_animatedThisCycle.Add(m.PromotionId))
                return; 
        }
        else
        {
            if (_didAnimateThisInstance) return;
            _didAnimateThisInstance = true;
        }

        int delayMs = (m.PromotionId > 0)
            ? (int)(m.PromotionId % StaggerBuckets) * StaggerStepMs
            : _rand.Next(0, StaggerBuckets) * StaggerStepMs;

        if (mainFrame != null)
        {
            mainFrame.TranslationY = StartTranslate;
            mainFrame.Opacity = 0;
            mainFrame.Scale = StartScale;

            await Task.Delay(delayMs);

            await Task.WhenAll(
                mainFrame.TranslateTo(0, 0, DurationMs, Easing.CubicOut),
                mainFrame.FadeTo(1, DurationMs, Easing.CubicOut),
                mainFrame.ScaleTo(1.0, DurationMs, Easing.CubicOut)
            );
        }
        else
        {
            this.TranslationY = StartTranslate;
            this.Opacity = 0;
            this.Scale = StartScale;

            await Task.Delay(delayMs);

            await Task.WhenAll(
                this.TranslateTo(0, 0, DurationMs, Easing.CubicOut),
                this.FadeTo(1, DurationMs, Easing.CubicOut),
                this.ScaleTo(1.0, DurationMs, Easing.CubicOut)
            );
        }
    }
    #endregion

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

    private async void Product_Tapped(object sender, TappedEventArgs e)
    {
        await mainFrame.ScaleTo(0.95, 100, Easing.CubicOut);
        await mainFrame.ScaleTo(1.0, 100, Easing.CubicIn);

        if (BindingContext is ProductModel product && ClickCommand?.CanExecute(product) == true)
        {
            ClickCommand.Execute(product);
        }
    }

    private static void IsNonActiveProductChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.boxViewBack.IsVisible = (bool)newValue;
    }

    private static void ShowCheckProductPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.checkProduct.IsVisible = (bool)newValue;
    }
    
    private static void IsCheckedProductChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        try
        {
            control.suppressCheckEvent = true;
            control.checkProduct.IsChecked = (bool)newValue;
        }
        finally
        {
            control.suppressCheckEvent = false;
        }
    }

    bool suppressCheckEvent; 
    private void CheckProduct_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (suppressCheckEvent) return;     

        Product_Tapped(null, null);
    }
}