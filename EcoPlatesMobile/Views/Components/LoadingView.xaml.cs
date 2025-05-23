namespace EcoPlatesMobile.Views.Components;

public partial class LoadingView : ContentView
{
	public LoadingView()
	{
		InitializeComponent();
	}

	public bool ShowLoading
	{
		set
		{
			loading.IsRunning = value;
			overlayLoading.IsVisible = value;
		}
	}
}