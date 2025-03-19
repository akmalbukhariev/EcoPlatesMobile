using Microsoft.Maui.Controls.Shapes;

namespace EcoPlatesMobile.Views.Components;

public partial class CustomIconButton : ContentView
{
    public static readonly BindableProperty ButtonWidthProperty =
            BindableProperty.Create(nameof(ButtonWidth), typeof(int), typeof(CustomIconButton), 5, propertyChanged: OnButtonWidthChanged);

    public static readonly BindableProperty ButtonHeightProperty =
            BindableProperty.Create(nameof(ButtonHeight), typeof(int), typeof(CustomIconButton), 5, propertyChanged: OnButtonHeightChanged);

    public static readonly BindableProperty ButtonIconProperty =
        BindableProperty.Create(nameof(ButtonIcon), typeof(ImageSource), typeof(CustomIconButton), default(ImageSource), propertyChanged: OnButtonIconChanged);

    public static readonly BindableProperty ButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(CustomIconButton), Colors.White, propertyChanged: OnButtonBackgroundColorChanged);

   public static readonly BindableProperty ButtonCornerRadiusProperty =
      BindableProperty.Create(nameof(ButtonCornerRadius), typeof(double), typeof(CustomEntry), 10.0, propertyChanged: OnButtonCornerRadiusChanged);

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

    public double ButtonCornerRadius
    {
        get => (double)GetValue(ButtonCornerRadiusProperty);
        set => SetValue(ButtonCornerRadiusProperty, value);
    }

    public CustomIconButton()
	{
		InitializeComponent();

        BindingContext = this;
    }

    private static void OnButtonWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.grdMain.WidthRequest = (int)newValue;
    }

    private static void OnButtonHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.grdMain.HeightRequest = (int)newValue;
    }

    private static void OnButtonIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.image.Source = (ImageSource)newValue;
    }

    private static void OnButtonBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.border.BackgroundColor = (Color)newValue;
    }

    private static void OnButtonCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomIconButton)bindable;
        control.border.StrokeShape = new RoundRectangle { CornerRadius = (double)newValue };
    }
}