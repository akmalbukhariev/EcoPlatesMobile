using EcoPlatesMobile.Services;

namespace EcoPlatesMobile.Views;

public partial class BlockedPage : BasePage
{
	private readonly LanguageService languageService;
	public BlockedPage(LanguageService languageService)
	{
		InitializeComponent();
		this.languageService = languageService;
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

    string url = "http://95.182.118.233/backend/help-center-uz.html"; // fallback

    switch (currentLang.ToLower())
    {
        case "uz":
            url = "http://95.182.118.233/backend/help-center-uz.html";
            break;
        case "ru":
            url = "http://95.182.118.233/backend/help-center-ru.html";
            break;
        case "en":
            url = "http://95.182.118.233/backend/help-center-en.html";
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