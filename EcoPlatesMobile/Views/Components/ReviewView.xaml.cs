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
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            await borderButton.ScaleTo(0.95, 100, Easing.CubicOut);
            await borderButton.ScaleTo(1.0, 100, Easing.CubicIn);

            EventReviewClick?.Invoke();
        });
    }

    private async void Close_Tapped(object sender, TappedEventArgs e)
    {
        await ClickGuard.RunAsync((Microsoft.Maui.Controls.VisualElement)sender, async () =>
        {
            this.IsVisible = false;
            EventCloseClick?.Invoke();
        });
    }
}