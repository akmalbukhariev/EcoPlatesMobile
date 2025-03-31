namespace EcoPlatesMobile.Views.Company.Components;

public partial class ProductView : ContentView
{
	public static readonly BindableProperty IsNonActiveProductProperty =
       BindableProperty.Create(nameof(IsNonActiveProduct), typeof(bool), typeof(ProductView), true, propertyChanged: IsNonActiveProductChanged);

    public bool IsNonActiveProduct
    {
        get => (bool)GetValue(IsNonActiveProductProperty);
        set => SetValue(IsNonActiveProductProperty, value);
    }

	public ProductView()
	{
		InitializeComponent();
		BindingContext = this;
	}

	private static void IsNonActiveProductChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProductView)bindable;
        control.boxViewBack.IsVisible = (bool)newValue;
    }
}