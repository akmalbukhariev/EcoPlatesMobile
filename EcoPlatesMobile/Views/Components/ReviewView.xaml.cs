namespace EcoPlatesMobile.Views.Components;

public partial class ReviewView : ContentView
{
    public event Action EventReviewClick;
    public event Action EventCloseClick;

    public ReviewView()
	{
		InitializeComponent();
	}

    private async void Review_Tapped(object sender, TappedEventArgs e)
    {
        await borderButton.ScaleTo(0.95, 100, Easing.CubicOut);
        await borderButton.ScaleTo(1.0, 100, Easing.CubicIn);

        EventReviewClick?.Invoke();
    }

    private void Close_Tapped(object sender, TappedEventArgs e)
    {
        this.IsVisible = false;
        EventCloseClick?.Invoke();
    }
}