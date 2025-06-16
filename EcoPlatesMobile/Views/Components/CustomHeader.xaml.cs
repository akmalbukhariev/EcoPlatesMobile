namespace EcoPlatesMobile.Views.Components;

public partial class CustomHeader : ContentView
{
    public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomHeader), default(string), propertyChanged: OnTitleChanged);

    public static readonly BindableProperty TitleColorProperty =
        BindableProperty.Create(nameof(TitleColor), typeof(Color), typeof(CustomHeader), Colors.White, propertyChanged: TitleColorChanged);


    public static readonly BindableProperty ShowBackProperty =
      BindableProperty.Create(nameof(ShowBack), typeof(bool), typeof(CustomHeader), true, propertyChanged: OnShowBackChanged);

    public static readonly BindableProperty HeaderBackgroundColorProperty =
        BindableProperty.Create(nameof(HeaderBackground), typeof(Color), typeof(CustomHeader), Color.FromArgb("#007200"), propertyChanged: HeaderBackgroundChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Color TitleColor
    {
        get => (Color)GetValue(TitleColorProperty);
        set => SetValue(TitleColorProperty, value);
    }

    public bool ShowBack
    {
        get => (bool)GetValue(ShowBackProperty);
        set => SetValue(ShowBackProperty, value);
    }

    public Color HeaderBackground
    {
        get => (Color)GetValue(HeaderBackgroundColorProperty);
        set => SetValue(HeaderBackgroundColorProperty, value);
    }

    public CustomHeader()
	{
		InitializeComponent();
        BindingContext = this;
	}
    
    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomHeader)bindable;
        control.lbTitle.Text = (string)newValue;
    }

    private static void TitleColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomHeader)bindable;
        control.lbTitle.TextColor = (Color)newValue;
    }

    private static void OnShowBackChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomHeader)bindable;
        control.imBack.IsVisible = (bool)newValue;
    }

    private static void HeaderBackgroundChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomHeader)bindable;
        control.mainGrid.BackgroundColor = (Color)newValue;
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement element)
        {
            await element.ScaleTo(1.3, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }

        await AppNavigatorService.NavigateTo("..");
    }
}