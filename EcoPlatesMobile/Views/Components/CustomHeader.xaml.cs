namespace EcoPlatesMobile.Views.Components;

public partial class CustomHeader : ContentView
{
    public static readonly BindableProperty TitleProperty =
      BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomEntry), default(string), propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public CustomHeader()
	{
		InitializeComponent();
	}

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomHeader)bindable;
        control.lbTitle.Text = (string)newValue;
    }
}