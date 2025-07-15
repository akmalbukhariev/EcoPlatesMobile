
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

    public static readonly BindableProperty EntryHeightProperty =
       BindableProperty.Create(nameof(EntryHeight), typeof(double), typeof(CustomEntry), 60.0, propertyChanged: EntryHeightChanged);

    public static readonly BindableProperty EntryKeyboardProperty =
       BindableProperty.Create(nameof(EntryKeyboard), typeof(Keyboard), typeof(CustomEntry), default(Keyboard), propertyChanged: EntryKeyboardChanged);

    public static readonly BindableProperty EntryShowPrefixProperty =
       BindableProperty.Create(nameof(EntryShowPrefix), typeof(bool), typeof(CustomEntry), false, propertyChanged: EntryShowPrefixChanged);

    public static readonly BindableProperty EntrySendIconProperty =
        BindableProperty.Create(nameof(EntrySendIcon), typeof(ImageSource), typeof(CustomEntry), default(ImageSource), propertyChanged: EntrySendIconChanged);

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

    public ImageSource EntrySendIcon
    {
        get => (ImageSource)GetValue(EntrySendIconProperty);
        set => SetValue(EntrySendIconProperty, value);
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

    public Color EntryPlaceHolderColor
    {
        get => (Color)GetValue(EntryPlaceHolderColorProperty);
        set => SetValue(EntryPlaceHolderColorProperty, value);
    }

    public double EntryCornerRadius
    {
        get => (double)GetValue(EntryCornerRadiusProperty);
        set => SetValue(EntryCornerRadiusProperty, value);
    }

    public double EntryHeight
    {
        get => (double)GetValue(EntryHeightProperty);
        set => SetValue(EntryHeightProperty, value);
    }
 
    public Keyboard EntryKeyboard
    {
        get => (Keyboard)GetValue(EntryKeyboardProperty);
        set => SetValue(EntryKeyboardProperty, value);
    }

    public bool EntryShowPrefix
    {
        get => (bool)GetValue(EntryShowPrefixProperty);
        set => SetValue(EntryShowPrefixProperty, value);
    }

    public string GetEntryText()
    {
        return customEntry.Text;
    }

    public void SetEntryText(string text)
    {
        customEntry.Text = text;
    }

    public bool IsPhoneNumber{get; set;}
    public bool ShowSendImage { get; set; }

    private const int MaxPhoneLength = 9;

    private Color _defaultBorderColor;
    private Color _defaultEntryBackgroundColor;

    public event Action EventClickSend;

    public CustomEntry()
	{
		InitializeComponent();

        IsPhoneNumber = false;
        ShowSendImage = false;
        
        _defaultBorderColor = EntryBorderColor;
        _defaultEntryBackgroundColor = EntryBackgroundColor;
        
        //borderContainer.Stroke = EntryBorderColor;
        //iconImage.Source = EntryIcon;
        //customEntry.BackgroundColor = EntryBackgroundColor;
        //customEntry.Placeholder = EntryPlaceHolder;

        customEntry.TextChanged += CustomEntry_TextChanged;

        BindingContext = this;
    }

    private void CustomEntry_TextChanged(object? sender, TextChangedEventArgs e)
    {
        string newText = e.NewTextValue ?? "";

        if (newText.Length == 0)
        {
            EntryBorderColor = _defaultBorderColor;
            EntryBackgroundColor = _defaultEntryBackgroundColor;

            if (ShowSendImage)
            {
                sendImage.IsVisible = false;
            }
        }
        else
        {
            if (ShowSendImage)
            {
                sendImage.IsVisible = true;
            }

            EntryBorderColor = Color.FromArgb("#00C300");
            EntryBackgroundColor = Colors.White;
        }

        if(IsPhoneNumber)
        {
            string digitsOnly = new string(newText.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length > MaxPhoneLength)
                digitsOnly = digitsOnly.Substring(0, MaxPhoneLength);

            //if (customEntry.Text != digitsOnly)
            //    customEntry.Text = digitsOnly;

            if (customEntry.Text != digitsOnly)
            {
                customEntry.TextChanged -= CustomEntry_TextChanged;
                customEntry.Text = digitsOnly;
                customEntry.CursorPosition = digitsOnly.Length;
                customEntry.TextChanged += CustomEntry_TextChanged;
            }
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

    private static void EntrySendIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.sendImage.Source = (ImageSource)newValue;
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

    private static void EntryHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.borderContainer.HeightRequest = (double)newValue;
    }

    private static void EntryKeyboardChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.customEntry.Keyboard = (Keyboard)newValue;
    }

    private static void EntryShowPrefixChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntry)bindable;
        control.prefixLabel.IsVisible = (bool)newValue;
    }

    private async void Send_Tapped(object sender, TappedEventArgs e)
    {
        await sendImage.ScaleTo(0.8, 100, Easing.CubicOut);
        await sendImage.ScaleTo(1.0, 100, Easing.CubicIn);

        EventClickSend?.Invoke();
    }
}