using System.Windows.Input;

namespace EcoPlatesMobile.Views.Components;

public partial class ListTileView : ContentView
{
    public enum ListTileType
    {
        None,
        ActiveAds,
        PreviousAds,
        Share,
        Location,
        AboutApp,
        Suggestions,
        Message,
        AccountManagement
    }

    public static readonly BindableProperty TileIconProperty =
        BindableProperty.Create(nameof(TileIcon), typeof(ImageSource), typeof(ListTileView), default(ImageSource), propertyChanged: TileIconChanged);

    public static readonly BindableProperty TileTextProperty =
       BindableProperty.Create(nameof(TileText), typeof(string), typeof(ListTileView), default(string), propertyChanged: TileTextChanged);

    public static readonly BindableProperty TileText1Property =
       BindableProperty.Create(nameof(TileText1), typeof(string), typeof(ListTileView), default(string), propertyChanged: TileText1Changed);

    public static readonly BindableProperty TileTextColorProperty =
        BindableProperty.Create(nameof(TileTextColor), typeof(Color), typeof(ListTileView), Colors.Black, propertyChanged: TileTextColorChanged);

    public static readonly BindableProperty TileText1ColorProperty =
        BindableProperty.Create(nameof(TileText1Color), typeof(Color), typeof(ListTileView), Colors.Black, propertyChanged: TileText1ColorChanged);

    public static readonly BindableProperty TileTextSizeProperty =
       BindableProperty.Create(nameof(TileTextSize), typeof(double), typeof(ListTileView), 18.0, propertyChanged: TileTextSizeChanged);

    public static readonly BindableProperty ShowTiletext1Property =
       BindableProperty.Create(nameof(ShowTiletext1), typeof(bool), typeof(ListTileView), false, propertyChanged: ShowTiletext1Changed);

    public event Action<object> EventClickTile;

    public ImageSource TileIcon
    {
        get => (ImageSource)GetValue(TileIconProperty);
        set => SetValue(TileIconProperty, value);
    }

    public string TileText
    {
        get => (string)GetValue(TileTextProperty);
        set => SetValue(TileTextProperty, value);
    }

    public string TileText1
    {
        get => (string)GetValue(TileText1Property);
        set => SetValue(TileText1Property, value);
    }

    public Color TileTextColor
    {
        get => (Color)GetValue(TileTextColorProperty);
        set => SetValue(TileTextColorProperty, value);
    }

    public Color TileText1Color
    {
        get => (Color)GetValue(TileText1ColorProperty);
        set => SetValue(TileText1ColorProperty, value);
    }

    public double TileTextSize
    {
        get => (double)GetValue(TileTextSizeProperty);
        set => SetValue(TileTextSizeProperty, value);
    }

    public bool ShowTiletext1
    {
        get => (bool)GetValue(ShowTiletext1Property);
        set => SetValue(ShowTiletext1Property, value);
    }

    public ListTileType TileType { get; set; } = ListTileType.None;

    public ListTileView()
	{
		InitializeComponent();
	}

    private static void TileIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.image.Source = (ImageSource)newValue;
    }

    private static void TileTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label.Text= (string)newValue;
    }

    private static void TileText1Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label1.Text = (string)newValue;
    }

    private static void TileTextColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label.TextColor = (Color)newValue;
    }

    private static void TileText1ColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label1.TextColor = (Color)newValue;
    }

    private static void TileTextSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label.FontSize = (double)newValue;
    }

    private static void ShowTiletext1Changed(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.borderTileTex1.IsVisible = (bool)newValue;
    }

    private async void Tile_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await grdMain.ScaleTo(0.95, 100, Easing.CubicOut);
            await grdMain.ScaleTo(1.0, 100, Easing.CubicIn);

            EventClickTile?.Invoke(this);
        });
    }
}