namespace EcoPlatesMobile.Views;

public partial class BlockedPage : BasePage
{
	public BlockedPage()
	{
		InitializeComponent();
	}

	private async void ContactSupport_Clicked(object sender, EventArgs e)
	{
		await AnimateElementScaleDown(sender as Button);
    }

	private async void OpenHelpCenter_Clicked(object sender, EventArgs e)
	{
		await AnimateElementScaleDown(sender as Button);
    }

	private async void BackToLogin_Clicked(object sender, EventArgs e)
	{
		await AnimateElementScaleDown(sender as Button);
    }
}