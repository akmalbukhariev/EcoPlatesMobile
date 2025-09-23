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
        await Launcher.OpenAsync("https://t.me/SaleTopTicketBot");
    }

	private async void OpenHelpCenter_Clicked(object sender, EventArgs e)
	{
		await AnimateElementScaleDown(sender as Button);
        await Launcher.OpenAsync("http://95.182.118.233/backend/help-center-ru.html");
    }

	private async void BackToLogin_Clicked(object sender, EventArgs e)
	{
		await AnimateElementScaleDown(sender as Button);

        await ClickGuard.RunAsync((VisualElement)sender, async () =>
        {
            await AppNavigatorService.NavigateTo("..");
        });
    }
}