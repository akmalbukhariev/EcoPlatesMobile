using The49.Maui.BottomSheet;

namespace EcoPlatesMobile.Views.User.Pages;

public partial class MapBottomSheet : BottomSheet
{
	public event Action EventShowResultsClicked;
	public event Action<int> EventValueDistanceChanged;

    public MapBottomSheet()
	{
		InitializeComponent();
	}

	private void DistanceSlider_ValueChanged(object sender, ValueChangedEventArgs e)
	{
        int km = (int)Math.Round(e.NewValue);
        distanceLabel.Text = $"{km} km";
		
        EventValueDistanceChanged?.Invoke(km);
    }

	private void ShowResults_Clicked(object sender, EventArgs e)
	{
		EventShowResultsClicked?.Invoke();
	}
}