
namespace EcoPlatesMobile.Views.User.Components;

public partial class NotificationBanner : ContentView
{
    public static readonly BindableProperty ImageProperty =
        BindableProperty.Create(
            nameof(Image),
            typeof(ImageSource),
            typeof(NotificationBanner),
            default(ImageSource),
            propertyChanged: OnImageChanged);

    public ImageSource Image
    {
        get => (ImageSource)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    private static void OnImageChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (NotificationBanner)bindable;
        control.imgNotification.Source = (ImageSource)newValue;
    }

    public static readonly BindableProperty Title1Property =
        BindableProperty.Create(
            nameof(Title1),
            typeof(string),
            typeof(NotificationBanner),
            string.Empty,
            propertyChanged: OnTitle1Changed);

    public string Title1
    {
        get => (string)GetValue(Title1Property);
        set => SetValue(Title1Property, value);
    }

    private static void OnTitle1Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (NotificationBanner)bindable;
        control.lbTitle1.Text = (string)newValue;
    }

    public static readonly BindableProperty Title2Property =
        BindableProperty.Create(
            nameof(Title2),
            typeof(string),
            typeof(NotificationBanner),
            string.Empty,
            propertyChanged: OnTitle2Changed);

    public string Title2
    {
        get => (string)GetValue(Title2Property);
        set => SetValue(Title2Property, value);
    }

    private static void OnTitle2Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (NotificationBanner)bindable;
        control.lbTitle2.Text = (string)newValue;
    }

    public NotificationBanner()
    {
        InitializeComponent();

        lbTitle1.TextColor = Colors.Black;
        lbTitle1.FontSize = 16;
        lbTitle1.FontFamily = "RobotoVar";
        
        lbTitle2.TextColor = Colors.Black;
        lbTitle2.FontSize = 16;
        lbTitle2.FontFamily = "RobotoVar";
    }
}