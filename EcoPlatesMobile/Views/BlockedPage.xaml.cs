using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Views;

public partial class BlockedPage : BasePage
{
	private readonly LanguageService languageService;
    public BlockedPage(LanguageService languageService)
    {
        InitializeComponent();

        this.languageService = languageService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (appControl.ResultCode == ApiResult.DELETE_USER)
        {
            lbUntill.IsVisible = false;
            lbUntillBlock.IsVisible = false;
        }

        lbUntillBlock.Text = appControl.StrBlockUntill;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        appControl.StrBlockUntill = "";
    }

	private async void ContactSupport_Clicked(object sender, EventArgs e)
    {
        await AnimateElementScaleDown(sender as Button);
        await Launcher.OpenAsync("https://t.me/SaleTopTicketBot");
    }

	private async void OpenHelpCenter_Clicked(object sender, EventArgs e)
    {
        await AnimateElementScaleDown(sender as Button);
 
        string currentLang = languageService.GetCurrentLanguage();

        string url = $"http://{Constants.SERVER_DOMAIN}/help-center-uz.html";

        switch (currentLang.ToLower())
        {
            case Constants.UZ:
                url = $"http://{Constants.SERVER_DOMAIN}/help-center-uz.html";
                break;
            case Constants.RU:
                url = $"http://{Constants.SERVER_DOMAIN}/help-center-ru.html";
                break;
            case Constants.EN:
                url = $"http://{Constants.SERVER_DOMAIN}/help-center-en.html";
                break;
        }

        await Launcher.OpenAsync(url);
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