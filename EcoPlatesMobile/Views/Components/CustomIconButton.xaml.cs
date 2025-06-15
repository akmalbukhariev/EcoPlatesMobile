using Microsoft.Maui.Controls.Shapes;

namespace EcoPlatesMobile.Views.Components;

public partial class CustomIconButton : ContentView
{
    public static readonly BindableProperty ButtonWidthProperty =
            BindableProperty.Create(nameof(ButtonWidth), typeof(int), typeof(CustomIconButton), 5, propertyChanged: ButtonWidthChanged);

    public static readonly BindableProperty ButtonHeightProperty =
            BindableProperty.Create(nameof(ButtonHeight), typeof(int), typeof(CustomIconButton), 5, propertyChanged: ButtonHeightChanged);

    public static readonly BindableProperty ButtonIconProperty =
        BindableProperty.Create(nameof(ButtonIcon), typeof(ImageSource), typeof(CustomIconButton), default(ImageSource), propertyChanged: ButtonIconChanged);

    public static readonly BindableProperty ButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(CustomIconButton), Colors.White, propertyChanged: ButtonBackgroundColorChanged);

    public static readonly BindableProperty ButtonBorderColorProperty =
        BindableProperty.Create(nameof(ButtonBorderColor), typeof(Color), typeof(CustomIconButton), Colors.Grey, propertyChanged: ButtonBorderColorChanged);

    public static readonly BindableProperty ButtonCornerRadiusProperty =
       BindableProperty.Create(nameof(ButtonCornerRadius), typeof(double), typeof(CustomEntry), 10.0, propertyChanged: ButtonCornerRadiusChanged);

    public int ButtonWidth
    {
        get => (int)GetValue(ButtonWidthProperty);
        set => SetValue(ButtonWidthProperty, value);
    }

    public int ButtonHeight
    {
        get => (int)GetValue(ButtonHeightProperty);
        set => SetValue(ButtonHeightProperty, value);
    }

    public ImageSource ButtonIcon
    {
        get => (ImageSource)GetValue(ButtonIconProperty);
        set => SetValue(ButtonIconProperty, value);
    }

    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    public Color ButtonBorderColor
    {
        get => (Color)GetValue(ButtonBorderColorProperty);
        set => SetValue(ButtonBorderColorProperty, value);
    }

    public double ButtonCornerRadius
    {
        get => (double)GetValue(ButtonCornerRadiusProperty);
        set => SetValue(ButtonCornerRadiusProperty, value);
    }

    public event Action EventClick;

    public CustomIconButton()
    {
        InitializeComponent();

        BindingContext = this;
    }

    private static void ButtonWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.grdMain.WidthRequest = (int)newValue;
    }

    private static void ButtonHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.grdMain.HeightRequest = (int)newValue;
    }

    private static void ButtonIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.image.Source = (ImageSource)newValue;
    }

    private static void ButtonBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.border.BackgroundColor = (Color)newValue;
    }

    private static void ButtonBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.border.Stroke = (Color)newValue;
    }

    private static void ButtonCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.border.StrokeShape = new RoundRectangle { CornerRadius = (double)newValue };
    }

    private async void MainGrid_Tapped(object sender, TappedEventArgs e)
    {
        await AnimateElementScaleDown(sender as Grid);
        EventClick?.Invoke();
    }
    
    protected Task AnimateElementScaleDown(VisualElement element)
    {
        return Task.Run(async () =>
        {
            await element.ScaleTo(0.9, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        });
    }
}