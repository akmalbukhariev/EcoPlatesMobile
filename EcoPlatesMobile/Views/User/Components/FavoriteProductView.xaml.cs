using System.Windows.Input;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.Views.User.Components;

public partial class FavoriteProductView : ContentView
{
    #region Properties
    public static readonly BindableProperty ProductImageProperty =
        BindableProperty.Create(nameof(ProductImage), typeof(ImageSource), typeof(FavoriteProductView), default(ImageSource), propertyChanged: ProductImageChanged);

    public static readonly BindableProperty ProductCountProperty =
      BindableProperty.Create(nameof(ProductCount), typeof(int), typeof(FavoriteProductView), 1, propertyChanged: ProductCountChanged);

    public static readonly BindableProperty ProductNameProperty =
       BindableProperty.Create(nameof(ProductName), typeof(string), typeof(FavoriteProductView), default(string), propertyChanged: ProductNameChanged);

    public static readonly BindableProperty ProductMakerNameProperty =
       BindableProperty.Create(nameof(ProductMakerName), typeof(string), typeof(FavoriteProductView), default(string), propertyChanged: ProductMakerNameChanged);

    public static readonly BindableProperty NewPriceProperty =
       BindableProperty.Create(nameof(NewPrice), typeof(string), typeof(FavoriteProductView), "0.0", propertyChanged: NewPriceChanged);

    public static readonly BindableProperty OldPriceProperty =
      BindableProperty.Create(nameof(OldPrice), typeof(string), typeof(FavoriteProductView), "0.0", propertyChanged: OldPriceChanged);

    public static readonly BindableProperty StarsProperty =
      BindableProperty.Create(nameof(Stars), typeof(string), typeof(FavoriteProductView), "0", propertyChanged: StarsChanged);

    public static readonly BindableProperty DistanceProperty =
     BindableProperty.Create(nameof(Distance), typeof(string), typeof(FavoriteProductView), "0.0", propertyChanged: DistanceChanged);

    public static readonly BindableProperty ClickCommandProperty =
        BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(CompanyView));

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

    public ICommand ClickCommand
    {
        get => (ICommand)GetValue(ClickCommandProperty);
        set => SetValue(ClickCommandProperty, value);
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

    public FavoriteProductView()
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
        control.newPrice.Text = (string)newValue;
    }

    private static void OldPriceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.oldPrice.Text = (string)newValue;
    }

    private static void StarsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.stars.Text = (string)newValue;
    }

    private static void DistanceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (FavoriteProductView)bindable;
        control.distance.Text = (string)newValue;
    }

    private async void Product_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await mainFrame.ScaleTo(0.95, 100, Easing.CubicOut);
            await mainFrame.ScaleTo(1.0, 100, Easing.CubicIn);

            if (BindingContext is ProductModel product && ClickCommand?.CanExecute(product) == true)
            {
                ClickCommand.Execute(product);
            }
        });
    }
}