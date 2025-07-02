namespace EcoPlatesMobile.Views.Components;

public partial class LoadingView : ContentView
{
	public LoadingView()
	{
		InitializeComponent();
		overlayLoading.IsVisible = false;
	}

	public bool ShowLoading
	{
		set
		{
			loading.IsRunning = value;
			overlayLoading.IsVisible = value;
		}
	}

	public void ChangeColor(Color color)
	{
		loading.Color = color;
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
    }
}