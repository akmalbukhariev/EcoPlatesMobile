namespace EcoPlatesMobile.Views.Components;

public partial class ListTileView : ContentView
{
    public static readonly BindableProperty TileIconProperty =
        BindableProperty.Create(nameof(TileIcon), typeof(ImageSource), typeof(ListTileView), default(ImageSource), propertyChanged: TileIconChanged);

    public static readonly BindableProperty TileTitleProperty =
       BindableProperty.Create(nameof(TileTitle), typeof(string), typeof(ListTileView), default(string), propertyChanged: TileTitleTextChanged);

    public static readonly BindableProperty TileTitleColorProperty =
        BindableProperty.Create(nameof(TileTitleColor), typeof(Color), typeof(ListTileView), Color.FromArgb("#444444"), propertyChanged: TileTitleColorChanged);
     
    public static readonly BindableProperty TileTitleSizeProperty =
       BindableProperty.Create(nameof(TileTitleSize), typeof(double), typeof(ListTileView), 18.0, propertyChanged: TileTitleSizeChanged);

    public ImageSource TileIcon
    {
        get => (ImageSource)GetValue(TileIconProperty);
        set => SetValue(TileIconProperty, value);
    }

    public string TileTitle
    {
        get => (string)GetValue(TileTitleProperty);
        set => SetValue(TileTitleProperty, value);
    }

    public Color TileTitleColor
    {
        get => (Color)GetValue(TileTitleColorProperty);
        set => SetValue(TileTitleColorProperty, value);
    }

    public double TileTitleSize
    {
        get => (double)GetValue(TileTitleSizeProperty);
        set => SetValue(TileTitleSizeProperty, value);
    }

    public ListTileView()
	{
		InitializeComponent();
	}

    private static void TileIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.image.Source = (ImageSource)newValue;
    }

    private static void TileTitleTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label.Text= (string)newValue;
    }

    private static void TileTitleColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label.TextColor = (Color)newValue;
    }

    private static void TileTitleSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ListTileView)bindable;
        control.label.FontSize = (double)newValue;
    }

    private void Tile_Tapped(object sender, TappedEventArgs e)
    {

    }
}