namespace EcoPlatesMobile.Views.Components;

public partial class CustomLikedView : ContentView
{
    public static readonly BindableProperty ShowLikedProperty =
       BindableProperty.Create(nameof(ShowLiked), typeof(bool), typeof(CustomLikedView), false, propertyChanged: ShowLikedChanged);

    public bool ShowLiked
    {
        get => (bool)GetValue(ShowLikedProperty);
        set => SetValue(ShowLikedProperty, value);
    }

    public CustomLikedView()
    {
        InitializeComponent();
        BindingContext = this;
    }

     private static void ShowLikedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomLikedView)bindable;
        bool show = (bool)newValue;
        control.imLiked.IsVisible = show;
        control.imUnLiked.IsVisible = !show;
    }
}