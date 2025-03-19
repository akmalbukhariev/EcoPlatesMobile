
using Microsoft.Maui.Controls.Shapes;

namespace EcoPlatesMobile.Views.Components;

public partial class CustomEntry : ContentView
{
    public static readonly BindableProperty EntryBorderColorProperty =
        BindableProperty.Create(nameof(EntryBorderColor), typeof(Color), typeof(CustomEntry), Color.FromArgb("#E9E9E9"), propertyChanged: EntryBorderColorChanged);

    public static readonly BindableProperty EntryIconProperty =
        BindableProperty.Create(nameof(EntryIcon), typeof(ImageSource), typeof(CustomEntry), default(ImageSource), propertyChanged: EntryIconChanged);

    public static readonly BindableProperty EntryBackgroundColorProperty =
        BindableProperty.Create(nameof(EntryBackgroundColor), typeof(Color), typeof(CustomEntry), Color.FromArgb("#F5F5F5"), propertyChanged: EntryBackgroundColorChanged);

    public static readonly BindableProperty EntryPlaceHolderProperty =
       BindableProperty.Create(nameof(EntryPlaceHolder), typeof(string), typeof(CustomEntry), default(string), propertyChanged: EntryPlaceHolderChanged);

    public static readonly BindableProperty EntryPlaceHolderColorProperty =
        BindableProperty.Create(nameof(EntryPlaceHolderColor), typeof(Color), typeof(CustomEntry), Color.FromArgb("#F5F5F5"), propertyChanged: EntryPlaceHolderColorChanged);

    public static readonly BindableProperty EntryCornerRadiusProperty =
       BindableProperty.Create(nameof(EntryCornerRadius), typeof(double), typeof(CustomEntry), 35.0, propertyChanged: EntryCornerRadiusChanged);

    public Color EntryBorderColor
    {
        get => (Color)GetValue(EntryBorderColorProperty);
        set => SetValue(EntryBorderColorProperty, value);
    }

    public ImageSource EntryIcon
    {
        get => (ImageSource)GetValue(EntryIconProperty);
        set => SetValue(EntryIconProperty, value);
    }

    public Color EntryBackgroundColor
    {
        get => (Color)GetValue(EntryBackgroundColorProperty);
        set => SetValue(EntryBackgroundColorProperty, value);
    }

    public string EntryPlaceHolder
    {
        get => (string)GetValue(EntryPlaceHolderColorProperty);
        set => SetValue(EntryPlaceHolderColorProperty, value);
    }

    public string EntryPlaceHolderColor
    {
        get => (string)GetValue(EntryPlaceHolderProperty);
        set => SetValue(EntryPlaceHolderProperty, value);
    }

    public double EntryCornerRadius
    {
        get => (double)GetValue(EntryCornerRadiusProperty);
        set => SetValue(EntryCornerRadiusProperty, value);
    }

    private Color _defaultBorderColor;
    private Color _defaultEntryBackgroundColor;

    public CustomEntry()
	{
		InitializeComponent();
        BindingContext = this;

        _defaultBorderColor = BorderColor;
        _defaultEntryBackgroundColor = EntryBackgroundColor;
        
        borderContainer.Stroke = BorderColor;
        iconImage.Source = IconSource;
        customEntry.BackgroundColor = EntryBackgroundColor;
        customEntry.Placeholder = EntryPlaceHolder;

        customEntry.TextChanged += CustomEntry_TextChanged;
    }

    private void CustomEntry_TextChanged(object? sender, TextChangedEventArgs e)
    {
        string newText = e.NewTextValue ?? "";

        if (newText.Length == 0)
        {
            BorderColor = _defaultBorderColor;
            EntryBackgroundColor = _defaultEntryBackgroundColor;
        }
        else if (string.IsNullOrWhiteSpace(newText))
        {
            BorderColor = Color.FromArgb("#DC0000");
            EntryBackgroundColor = _defaultEntryBackgroundColor;
        }
        else
        {
            BorderColor = Color.FromArgb("#00C300");
            EntryBackgroundColor = Colors.White;
        }
    }

    private static void EntryBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.borderContainer.Stroke = (Color)newValue;
    }

    private static void EntryIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.iconImage.Source = (ImageSource)newValue;
    }

    private static void EntryBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.customEntry.BackgroundColor = (Color)newValue;
    }

    private static void EntryPlaceHolderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.customEntry.Placeholder = (string)newValue;
    }

    private static void EntryPlaceHolderColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.customEntry.PlaceholderColor = (Color)newValue;
    }

    private static void EntryCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.borderContainer.StrokeShape = new RoundRectangle { CornerRadius = (double)newValue };
    }
}