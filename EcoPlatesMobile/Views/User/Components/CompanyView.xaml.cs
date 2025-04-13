
using System.Windows.Input;
using EcoPlatesMobile.Models.User;

namespace EcoPlatesMobile.Views.User.Components;

public partial class CompanyView : ContentView
{
    public static readonly BindableProperty CompanyImageProperty =
       BindableProperty.Create(nameof(CompanyImage), typeof(ImageSource), typeof(CompanyView), default(ImageSource), propertyChanged: CompanyImageChanged);

    public static readonly BindableProperty CompanyNameProperty =
       BindableProperty.Create(nameof(CompanyName), typeof(string), typeof(CompanyView), default(string), propertyChanged: CompanyNameChanged);

    public static readonly BindableProperty WorkingTimeProperty =
       BindableProperty.Create(nameof(WorkingTime), typeof(string), typeof(CompanyView), default(string), propertyChanged: WorkingTimeChanged);

    public static readonly BindableProperty StarsProperty =
      BindableProperty.Create(nameof(Stars), typeof(double), typeof(CompanyView), 0.0, propertyChanged: StarsChanged);

    public static readonly BindableProperty DistanceProperty =
      BindableProperty.Create(nameof(Distance), typeof(double), typeof(CompanyView), 0.0, propertyChanged: DistanceChanged);

    public static readonly BindableProperty LikedProperty =
     BindableProperty.Create(nameof(Liked), typeof(bool), typeof(CompanyView), false, propertyChanged: LikedChanged);

    public static readonly BindableProperty LikeCommandProperty =
       BindableProperty.Create(nameof(LikeCommand), typeof(ICommand), typeof(CompanyView));

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

    public CompanyView()
	{
		InitializeComponent();
	}

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
        control.stars.Text = ((double)newValue).ToString();
    }

    private static void DistanceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
        control.distance.Text = ((double)newValue).ToString();
    }

    private static void LikedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CompanyView)bindable;
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

        if (BindingContext is CompanyModel product && LikeCommand?.CanExecute(product) == true)
        {
            LikeCommand.Execute(product);
        }
    }
}