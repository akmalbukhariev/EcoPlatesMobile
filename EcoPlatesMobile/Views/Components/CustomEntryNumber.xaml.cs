using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views.Components;

public partial class CustomEntryNumber : ContentView
{
    public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CustomEntry), Color.FromArgb("#E9E9E9"), propertyChanged: OnBorderColorChanged);

    public static readonly BindableProperty EntryBackgroundColorProperty =
        BindableProperty.Create(nameof(EntryBackgroundColor), typeof(Color), typeof(CustomEntry), Color.FromArgb("#e9e9e9"), propertyChanged: OnEntryBackgroundColorChanged);

    public static readonly BindableProperty EntryPlaceHolderProperty =
       BindableProperty.Create(nameof(EntryPlaceHolder), typeof(string), typeof(CustomEntry), default(string), propertyChanged: OnEntryPlaceHolderChanged);

    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
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

    public CustomEntryNumber NextEntry { get; set; }
    public CustomEntryNumber PreviousEntry { get; set; }

    private Color _defaultBorderColor;
    private Color _defaultEntryBackgroundColor;

    public CustomEntryNumber()
	{
		InitializeComponent();

        BindingContext = this;

        _defaultBorderColor = BorderColor;
        _defaultEntryBackgroundColor = EntryBackgroundColor;

        borderContainer.Stroke = BorderColor;
        borderContainer.BackgroundColor = EntryBackgroundColor;
        customEntry.Placeholder = EntryPlaceHolder;

        customEntry.TextChanged += CustomEntry_TextChanged;
        customEntry.Completed += CustomEntry_Completed;
        customEntry.Unfocused += CustomEntry_Unfocused;

        //var backspaceEffect = new EcoPlatesMobile.Effects.BackspaceEffect();
        //backspaceEffect.BackspacePressed += HandleBackspaceKeyPress;
        //customEntry.Effects.Add(backspaceEffect);
    }

    public void UnfocusEntry()
    {
        customEntry?.Unfocus();
    }

    private void HandleBackspaceKeyPress()
    {
        if (!string.IsNullOrEmpty(customEntry.Text))
        {
            // Step 1: Delete only the current digit
            customEntry.Text = "";
        }
        else if (PreviousEntry != null)
        {
            // Step 2: Move focus to the previous entry, but do NOT delete its text automatically
            PreviousEntry.customEntry.Focus();
        }
    }

    private void CustomEntry_TextChanged(object? sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue ?? "";
        string newText = e.NewTextValue ?? "";

        if (newText.Length > 1)
        {
            customEntry.Text = newText.Substring(0, 1);
            return;
        }

        if (!string.IsNullOrEmpty(newText) && NextEntry != null)
        {
            NextEntry.customEntry.Focus();
        }
  
        if (string.IsNullOrEmpty(newText) && !string.IsNullOrEmpty(oldText))
        {
            System.Diagnostics.Debug.WriteLine("Backspace detected!"); // Log for debugging
            HandleBackspace();
        }

        BorderColor = !string.IsNullOrEmpty(customEntry.Text) ? Constants.COLOR_USER : _defaultBorderColor;
        EntryBackgroundColor = !string.IsNullOrEmpty(customEntry.Text) ? Colors.White : _defaultEntryBackgroundColor;
    }
 
    private void CustomEntry_Completed(object? sender, EventArgs e)
    {
        if (NextEntry != null)
        {
            NextEntry.customEntry.Focus();
        }
        else
        {
            if (!string.IsNullOrEmpty(PreviousEntry?.customEntry.Text))
            {
                  
            }
        }
    }

    private void CustomEntry_Unfocused(object? sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(customEntry.Text) && PreviousEntry != null)
        {
            PreviousEntry.customEntry.Focus();
        }
    }

    public void HandleBackspace()
    {
        if (string.IsNullOrEmpty(customEntry.Text) && PreviousEntry != null)
        {
            //PreviousEntry.customEntry.Text = ""; 
            PreviousEntry.customEntry.Focus();
        }
    }
  
    private static void OnBorderColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntryNumber)bindable;
        control.borderContainer.Stroke = (Color)newValue;
    }

    private static void OnEntryBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntryNumber)bindable;
        control.borderContainer.BackgroundColor = (Color)newValue;
    }

    private static void OnEntryPlaceHolderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomEntryNumber)bindable;
        control.customEntry.Placeholder = (string)newValue;
    }
}