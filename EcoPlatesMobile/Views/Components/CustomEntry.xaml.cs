
namespace EcoPlatesMobile.Views.Components;

public partial class CustomEntry : ContentView
{
    public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CustomEntry), Color.FromArgb("#E9E9E9"), propertyChanged: OnBorderColorChanged);

    public static readonly BindableProperty IconSourceProperty =
        BindableProperty.Create(nameof(IconSource), typeof(ImageSource), typeof(CustomEntry), default(ImageSource), propertyChanged: OnIconSourceChanged);

    public static readonly BindableProperty EntryBackgroundColorProperty =
        BindableProperty.Create(nameof(EntryBackgroundColor), typeof(Color), typeof(CustomEntry), Color.FromArgb("#F5F5F5"), propertyChanged: OnEntryBackgroundColorChanged);

    public static readonly BindableProperty EntryPlaceHolderProperty =
       BindableProperty.Create(nameof(EntryPlaceHolder), typeof(string), typeof(CustomEntry), default(string), propertyChanged: OnEntryPlaceHolderChanged);

    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public ImageSource IconSource
    {
        get => (ImageSource)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public Color EntryBackgroundColor
    {
        get => (Color)GetValue(EntryBackgroundColorProperty);
        set => SetValue(EntryBackgroundColorProperty, value);
    }

    public string EntryPlaceHolder
    {
        get => (string)GetValue(EntryPlaceHolderProperty);
        set => SetValue(EntryPlaceHolderProperty, value);
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

    private static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.borderContainer.Stroke = (Color)newValue;
    }

    private static void OnIconSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.iconImage.Source = (ImageSource)newValue;
    }

    private static void OnEntryBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.customEntry.BackgroundColor = (Color)newValue;
    }

    private static void OnEntryPlaceHolderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.customEntry.Placeholder = (string)newValue;
    }
}