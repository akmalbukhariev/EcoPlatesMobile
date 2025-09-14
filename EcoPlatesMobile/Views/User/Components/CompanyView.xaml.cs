
using System.Windows.Input;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.Views.User.Components;

public partial class CompanyView : ContentView
{
    #region Properties
    public static readonly BindableProperty CompanyImageProperty =
       BindableProperty.Create(nameof(CompanyImage), typeof(ImageSource), typeof(CompanyView), default(ImageSource), propertyChanged: CompanyImageChanged);

    public static readonly BindableProperty CompanyNameProperty =
       BindableProperty.Create(nameof(CompanyName), typeof(string), typeof(CompanyView), default(string), propertyChanged: CompanyNameChanged);

    public static readonly BindableProperty WorkingTimeProperty =
       BindableProperty.Create(nameof(WorkingTime), typeof(string), typeof(CompanyView), default(string), propertyChanged: WorkingTimeChanged);

    public static readonly BindableProperty StarsProperty =
      BindableProperty.Create(nameof(Stars), typeof(string), typeof(CompanyView), "0", propertyChanged: StarsChanged);

    public static readonly BindableProperty DistanceProperty =
      BindableProperty.Create(nameof(Distance), typeof(string), typeof(CompanyView), "0.0", propertyChanged: DistanceChanged);

    public static readonly BindableProperty LikedProperty =
     BindableProperty.Create(nameof(Liked), typeof(bool), typeof(CompanyView), false, propertyChanged: LikedChanged);

    public static readonly BindableProperty LikeCommandProperty =
       BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(CompanyView));

    public static readonly BindableProperty ShowLikedProperty =
     BindableProperty.Create(nameof(ShowLiked), typeof(bool), typeof(CompanyView), false, propertyChanged: ShowLikedChanged);

    public static readonly BindableProperty ClickCommandProperty =
        BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(CompanyView));

    public ImageSource CompanyImage
    {
        get => (ImageSource)GetValue(CompanyImageProperty);
        set => SetValue(CompanyImageProperty, value);
    }

    public string CompanyName
    {
        get => (string)GetValue(CompanyNameProperty);
        set => SetValue(CompanyNameProperty, value);
    }

    public string WorkingTime
    {
        get => (string)GetValue(WorkingTimeProperty);
        set => SetValue(WorkingTimeProperty, value);
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

    public bool ShowLiked
    {
        get => (bool)GetValue(ShowLikedProperty);
        set => SetValue(ShowLikedProperty, value);
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

    public CompanyView()
    {
        InitializeComponent();
        Loaded += OnLoadedAnimateOnce;
        BindingContextChanged += OnBindingContextChangedAnimate;

        imLiked.IsVisible = ShowLiked;
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
        if (BindingContext is not CompanyModel m)
            return;
 
        if (m.CompanyId > 0)
        {
            if (!_animatedThisCycle.Add(m.CompanyId))
                return; 
        }
        else
        {
            if (_didAnimateThisInstance) return;
            _didAnimateThisInstance = true;
        }

        int delayMs = (m.CompanyId > 0)
            ? (int)(m.CompanyId % StaggerBuckets) * StaggerStepMs
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

    private static void CompanyImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        control.companyImage.Source = (ImageSource)newValue;
    }

    private static void CompanyNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        control.companyName.Text = (string)newValue;
    }

    private static void WorkingTimeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        control.companyWorkingTime.Text = (string)newValue;
    }

    private static void StarsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        //control.stars.Text = (string)newValue;
    }

    private static void DistanceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        control.distance.Text = (string)newValue;
    }

    private static void LikedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        bool liked = (bool)newValue;

        control.imLiked.Source = liked ? "liked.png" : "like.png";
    }

    private static void ShowLikedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        bool show = (bool)newValue;
        if (control.imLiked != null)
        {
            control.imLiked.IsVisible = show;
        }
    }

    private async void Like_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            if (sender is VisualElement element)
            {
                await element.ScaleTo(1.3, 100, Easing.CubicOut);
                await element.ScaleTo(1.0, 100, Easing.CubicIn);
            }

            if (BindingContext is CompanyModel product && LikeCommand?.CanExecute(product) == true)
            {
                LikeCommand.Execute(product);
            }
        });
    }

    private async void Company_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await mainFrame.ScaleTo(0.95, 100, Easing.CubicOut);
            await mainFrame.ScaleTo(1.0, 100, Easing.CubicIn);

            if (BindingContext is CompanyModel company && ClickCommand?.CanExecute(company) == true)
            {
                ClickCommand.Execute(company);
            }
        });
    }
}